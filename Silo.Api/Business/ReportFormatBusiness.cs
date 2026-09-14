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
        var url = request.Url.Replace('-', '/').TrimStart('/');
        var claimValue = "/" + url;

        var existingLink = apiContext.MenuLinks.FirstOrDefault(p =>
            p.IsDedicated == true &&
            p.Level == 3 &&
            (p.Url == url || p.Url == claimValue));

        if (existingLink is not null)
        {
            existingLink.Title = request.Title;
            existingLink.ParentId = request.SelectedCategoryId;
            existingLink.IsShown = true;
        }
        else
        {
            apiContext.MenuLinks.Add(new MenuLink
            {
                Id = apiContext.MenuLinks.Max(p => p.Id) + 1,
                Level = 3,
                Title = request.Title,
                Url = url,
                ParentId = request.SelectedCategoryId,
                IsShown = true,
                IsDedicated = true
            });
        }

        var oldClaims = apiContext.UserClaims
            .Where(p => p.ClaimType == ClaimTypes.Authentication &&
                       (p.ClaimValue == claimValue || p.ClaimValue == url))
            .ToList();

        apiContext.UserClaims.RemoveRange(oldClaims);

        foreach (var userId in request.UserIds.Distinct())
        {
            apiContext.UserClaims.Add(new UserClaim
            {
                UserId = userId,
                ClaimType = ClaimTypes.Authentication,
                ClaimValue = claimValue
            });
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
                                          .Select(p=>p.UserId)
                                          .ToList();

        return vm;
    }

    public int SSaveAiReport(SaveAiReportCommand command)
    {
        var aiQuery = apiContext.AiGeneratedQueries.FirstOrDefault(x => x.Id == command.QueryId);

        if (string.IsNullOrWhiteSpace(command.ReportName))
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
        var aiParentMenu = apiContext.MenuLinks
            .FirstOrDefault(x => x.Level == 2 && x.Title == "گزارشات مربوط به AI");

        if (aiParentMenu is null)
            return false;

        var url = $"ai/reports/{reportFormatId}";
        var claimValue = "/" + url;
        var currentUserId = httpContext.User.GetUserId();

        var existingLink = apiContext.MenuLinks.FirstOrDefault(p =>
            p.IsDedicated == true &&
            p.Level == 3 &&
            (p.Url == url || p.Url == claimValue));

        if (existingLink is not null)
        {
            existingLink.Title = reportName;
            existingLink.ParentId = aiParentMenu.Id;
            existingLink.IsShown = true;
        }
        else
        {
            apiContext.MenuLinks.Add(new MenuLink
            {
                Id = apiContext.MenuLinks.Max(p => p.Id) + 1,
                Level = 3,
                Title = reportName,
                Url = url,
                ParentId = aiParentMenu.Id,
                IsShown = true,
                IsDedicated = true
            });
        }

        var oldClaims = apiContext.UserClaims
            .Where(p => p.ClaimType == ClaimTypes.Authentication &&
                       (p.ClaimValue == claimValue || p.ClaimValue == url))
            .ToList();
        apiContext.UserClaims.RemoveRange(oldClaims);

        var finalUserIds = (userIds ?? new List<string>()).ToList();

        if (!string.IsNullOrEmpty(currentUserId) && !finalUserIds.Contains(currentUserId))
            finalUserIds.Add(currentUserId);

        foreach (var userId in finalUserIds.Distinct())
        {
            apiContext.UserClaims.Add(new UserClaim
            {
                UserId = userId,
                ClaimType = ClaimTypes.Authentication,
                ClaimValue = claimValue
            });
        }

        return apiContext.SaveChanges() > 0;
    }

    public GetAiReportDataVm SGetAiReportData(GetReportFormatByIdQuery query)
    {
        var report = apiContext.ReportFormats.FirstOrDefault(x => x.Id == query.FormatId && x.Type == (int)ReportFormatTypes.AiReport);

        if (report == null)
            return null;

        var result = new GetAiReportDataVm
        {
            Name = report.Name,
            QueryReferenceId = report.QueryId ?? 0
        };

        var aiQuery = apiContext.AiGeneratedQueries.FirstOrDefault(x => x.Id == report.QueryId);

        if (aiQuery == null || string.IsNullOrWhiteSpace(aiQuery.QueryText))
            return result;

        var dataTable = dataAccess.SqlDataAdapter(aiQuery.QueryText);

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
            var allowedUrls = apiContext.UserClaims
                .Where(c => c.UserId == userId
                         && c.ClaimType == ClaimTypes.Authentication
                         && (c.ClaimValue.StartsWith("/ai/reports/") || c.ClaimValue.StartsWith("ai/reports/")))
                .Select(c => c.ClaimValue.TrimStart('/'))
                .ToList();

            var allowedIds = allowedUrls
                .Select(u =>
                {
                    var part = u.Replace("ai/reports/", "");
                    return int.TryParse(part, out var id) ? id : (int?)null;
                })
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

        var urlWithoutSlash = $"ai/reports/{id}";
        var urlWithSlash = $"/{urlWithoutSlash}";

        var menuLink = apiContext.MenuLinks.FirstOrDefault(x => x.IsDedicated == true && (x.Url == urlWithoutSlash || x.Url == urlWithSlash));

        if (menuLink != null)
            apiContext.MenuLinks.Remove(menuLink);

        var claims = apiContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.Authentication &&(x.ClaimValue == urlWithSlash || x.ClaimValue == urlWithoutSlash)).ToList();

        apiContext.UserClaims.RemoveRange(claims);

        apiContext.ReportFormats.Remove(report);

        return apiContext.SaveChanges() > 0;
    }
}
