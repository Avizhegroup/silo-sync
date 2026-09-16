using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Contracts;
using Silo.Domains.Entities;
using Silo.Domains.Entities.Api;
using Silo.Domains.Services;

namespace Silo.Api.Business;

public class ReportFormatBusiness : ProjectBusiness
{
    private readonly ILogger<ReportFormatBusiness> logger;
    private readonly IDataAccess dataAccess;
    private readonly IConfiguration configuration;
    private readonly WmsApiContext apiContext;
    private readonly IMapper mapper;

    public ReportFormatBusiness(ILogger<ReportFormatBusiness> logger
        , IDataAccess dataAccess
        , IConfiguration configuration
        , WmsApiContext apiContext
        , IMapper mapper
        , IHttpContextAccessor httpContextAccessor) : base(dataAccess, logger, httpContextAccessor)
    {
        this.logger = logger;
        this.dataAccess = dataAccess;
        this.configuration = configuration;
        this.apiContext = apiContext;
        this.mapper = mapper;
    }

    public bool SCreateReportFormat(CreateReportFormatCommand command)
    {
        var format = mapper.Map<ReportFormat>(command);

        format.UserId = httpContext.User.GetUserId();

        apiContext.ReportFormats.Add(format);

        return apiContext.SaveChanges() >= 1;
    }

    public List<GetReportFormatsByPathVm> SGetReportFormatByPath(string path)
    => mapper.Map<List<GetReportFormatsByPathVm>>(apiContext.ReportFormats
                                                            .Where(x => x.Path == path)
                                                            .OrderBy(x => x.Id) 
                                                            .Include(x => x.User));

    public bool SDeleteReportFormat(int id)
    => apiContext.ReportFormats
                 .Where(p => p.Id == id)
                 .ExecuteDelete() > 0;

    public GetReportFormatByIdVm SGetReportFormatById(GetReportFormatByIdQuery query)
    => mapper.Map<GetReportFormatByIdVm>(apiContext.ReportFormats
                                                   .FirstOrDefault(x => x.Id == query.FormatId));

    public bool SSaveLinkForReportFormat(SaveMenuLinkOfDynamicReportCommand request)
    {
        MenuLink newLink = new()
        {
            Id = apiContext.MenuLinks.Max(p =>p.Id) + 1,
            Level = 3,
            Title = request.Title,
            Url = request.Url.Replace('-','/'),
            ParentId = request.SelectedCategoryId,
            IsShown = true,
            IsDedicated = true
        };

        apiContext.MenuLinks.Add(newLink);

        foreach (var userId in request.UserIds)
        {
            UserClaim claim = new()
            {
                UserId = userId,
                ClaimType = ClaimTypes.Authentication,
                ClaimValue = $"/{request.Url.Replace('-', '/')}"
            };

            apiContext.UserClaims.Add(claim);
        }

        return apiContext.SaveChanges() > 0;
    }

    public GetMenuLinkOfDynamicReportVm SGetLinkForReportFormat(GetMenuLinkOfDynamicReportQuery query)
    {
        GetMenuLinkOfDynamicReportVm vm = new();

        var link = apiContext.MenuLinks.FirstOrDefault(p => (bool)p.IsDedicated
                                                                              && p.Level == 3
                                                                              && p.Url == query.FullUrl);

        if (link is not null)
        {
            vm.Title = link.Title;

            vm.CategoryId = link.ParentId;
        }

        vm.UserIds = apiContext.UserClaims.Where(p => p.ClaimType == ClaimTypes.Authentication
                                                           && p.ClaimValue == query.FullUrl)
                                          .Select(p =>p.UserId)
                                          .ToList();

        return vm;
    }

    // --- Public Ai methods ---

    public bool SSaveLinkForAiReportFormat(SaveMenuLinkOfDynamicReportCommand request)
    {
        var url = NormalizeUrl(request.Url);
        var claimValue = "/" + url;

        UpsertDedicatedMenuLink(url, claimValue, request.Title, request.SelectedCategoryId.Value);

        ReplaceAuthenticationClaims(url, claimValue, request.UserIds.Distinct());

        return apiContext.SaveChanges() > 0;
    }

    public GetMenuLinkOfDynamicReportVm SGetLinkForAiReportFormat(GetMenuLinkOfDynamicReportQuery query)
    {
        var url = NormalizeUrl(query.FullUrl);
        var claimValue = "/" + url;

        var link = FindDedicatedMenuLink(url, claimValue);

        var vm = new GetMenuLinkOfDynamicReportVm();

        if (link is not null)
        {
            vm.Title = link.Title;
            vm.CategoryId = link.ParentId;
        }

        vm.UserIds = AuthenticationClaimsFor(url, claimValue)
            .Select(p => p.UserId)
            .ToList();

        return vm;
    }

    public int SSaveAiReport(SaveAiReportCommand command)
    {
        if (command.ReportName.HasNoValue())
            throw new Exception("نام گزارش نمی‌تواند خالی باشد");

        var format = new ReportFormat
        {
            Name = command.ReportName,
            Type = (int)ReportFormatTypes.AiReport,
            Path = "ai-reports",
            UserId = httpContext.User.GetUserId(),
            QueryId = command.QueryId
        };

        apiContext.ReportFormats.Add(format);
        apiContext.SaveChanges();

        return format.Id;
    }

    public bool SSaveAiReportLink(int reportFormatId, string reportName, List<string> userIds)
    {
        var aiParentMenu = apiContext.MenuLinks.FirstOrDefault(x => x.Level == 2 && x.Title == "گزارشات مربوط به AI");

        if (aiParentMenu is null)
            return false;

        var url = NormalizeUrl($"ai/reports/{reportFormatId}");
        var claimValue = "/" + url;

        var currentUserId = httpContext.User.GetUserId();

        UpsertDedicatedMenuLink(url, claimValue, reportName, aiParentMenu.Id);

        var finalUserIds = (userIds ?? new List<string>()).ToList();

        if (currentUserId.HasValue() && !finalUserIds.Contains(currentUserId))
            finalUserIds.Add(currentUserId);

        ReplaceAuthenticationClaims(url, claimValue, finalUserIds);

        return apiContext.SaveChanges() > 0;
    }

    public GetAiReportDataVm SGetAiReportData(GetReportFormatByIdQuery query)
    {
        var report = apiContext.ReportFormats
            .Include(x => x.AiGeneratedQuery)
            .FirstOrDefault(x =>
                x.Id == query.FormatId &&
                x.Type == (int)ReportFormatTypes.AiReport);

        if (report == null)
            return null;

        var result = new GetAiReportDataVm
        {
            Name = report.Name,
            QueryId = report.QueryId ?? 0
        };

        if (report.AiGeneratedQuery == null || report.AiGeneratedQuery.QueryText.HasNoValue())
            return result;

        var dataTable = dataAccess.SqlDataAdapter(report.AiGeneratedQuery.QueryText);
        result.Data.Add(DataTableTools.DataTableToObjects(dataTable));

        return result;
    }

    public List<GetReportFormatsByPathVm> SGetAiReportFormats()
    {
        var userId = httpContext.User.GetUserId();
        var isAdmin = httpContext.User.IsInRole("Admin");

        IQueryable<ReportFormat> query = apiContext.ReportFormats
            .Where(x => x.Type == (int)ReportFormatTypes.AiReport);

        if (!isAdmin)
        {
            var allowedIds = apiContext.UserClaims
                .Where(c => c.UserId == userId
                         && c.ClaimType == ClaimTypes.Authentication
                         && (c.ClaimValue.StartsWith("/ai/reports/") || c.ClaimValue.StartsWith("ai/reports/")))
                .Select(c => c.ClaimValue.TrimStart('/').Replace("ai/reports/", ""))
                .ToList()
                .Select(part => int.TryParse(part, out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            query = query.Where(x => x.UserId == userId || allowedIds.Contains(x.Id));
        }

        var reports = query
            .OrderByDescending(x => x.Id)
            .Include(x => x.User)
            .ToList();

        return mapper.Map<List<GetReportFormatsByPathVm>>(reports);
    }

    public bool SDeleteAiReport(int id)
    {
        var report = apiContext.ReportFormats.FirstOrDefault(x => x.Id == id && x.Type == (int)ReportFormatTypes.AiReport);

        if (report == null)
            return false;

        var url = NormalizeUrl($"ai/reports/{id}");
        var claimValue = "/" + url;

        var menuLink = FindDedicatedMenuLink(url, claimValue);
        if (menuLink != null)
            apiContext.MenuLinks.Remove(menuLink);

        apiContext.UserClaims.RemoveRange(AuthenticationClaimsFor(url, claimValue).ToList());
        apiContext.ReportFormats.Remove(report);

        return apiContext.SaveChanges() > 0;
    }

    // --- Shared Ai helpers  ---

    private string NormalizeUrl(string rawUrl)
    {
        return rawUrl.Replace('-', '/').TrimStart('/');
    }

    private MenuLink FindDedicatedMenuLink(string url, string claimValue)
    {
        return apiContext.MenuLinks.FirstOrDefault(p =>
            p.IsDedicated == true &&
            p.Level == 3 &&
            (p.Url == url || p.Url == claimValue));
    }

    private void UpsertDedicatedMenuLink(string url, string claimValue, string title, int parentId)
    {
        var link = FindDedicatedMenuLink(url, claimValue);

        if (link is not null)
        {
            link.Title = title;
            link.ParentId = parentId;
            link.IsShown = true;
        }
        else
        {
            apiContext.MenuLinks.Add(new MenuLink
            {
                Id = apiContext.MenuLinks.Max(p => p.Id) + 1,
                Level = 3,
                Title = title,
                Url = url,
                ParentId = parentId,
                IsShown = true,
                IsDedicated = true
            });
        }
    }

    private IQueryable<UserClaim> AuthenticationClaimsFor(string url, string claimValue)
    {
        return apiContext.UserClaims.Where(p =>
            p.ClaimType == ClaimTypes.Authentication &&
            (p.ClaimValue == claimValue || p.ClaimValue == url));
    }

    private void ReplaceAuthenticationClaims(string url, string claimValue, IEnumerable<string> userIds)
    {
        apiContext.UserClaims.RemoveRange(AuthenticationClaimsFor(url, claimValue).ToList());

        foreach (var userId in userIds.Distinct())
        {
            apiContext.UserClaims.Add(new UserClaim
            {
                UserId = userId,
                ClaimType = ClaimTypes.Authentication,
                ClaimValue = claimValue
            });
        }
    }
}
