using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Contracts;
using Silo.Application.Features;
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
            Id = apiContext.MenuLinks.Max(p=> p.Id) +1,
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
                                          .Select(p=>p.UserId)
                                          .ToList();

        return vm;
    }
}
