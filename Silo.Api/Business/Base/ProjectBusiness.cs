using Silo.Application.Contracts;
using Silo.Identity.Server.Utilities;

namespace Silo.Api.Business;

public class ProjectBusiness
{
    protected readonly IDataAccess sqlDataAccess;
    protected readonly HttpContext httpContext;
    protected readonly ILogger<ProjectBusiness> logger;

    public ProjectBusiness(IDataAccess sqlDataAccess
        , ILogger<ProjectBusiness> logger)
    {
        this.sqlDataAccess = sqlDataAccess;
        this.logger = logger;
    }

    public ProjectBusiness(IDataAccess sqlDataAccess
        , ILogger<ProjectBusiness> logger
        , IHttpContextAccessor httpContextAccessor)
    {
        this.sqlDataAccess = sqlDataAccess;
        this.logger = logger;
        httpContext = httpContextAccessor.HttpContext;
    }

    #region Identity

    public string TSLogin(string username, string password)
    {
        logger.LogInformation("TSLogin"
            , new KeyValuePair<string, object>("username", username)
            , new KeyValuePair<string, object>("password", password));

        var command = @"
										SELECT PasswordHash, IsActive,Id FROM tbl_User
										WHERE UserName = @Usrnam";
        var userdata = sqlDataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("Usrnam", username))
            .Select().ToList();
        if (userdata.Count == 0)
        {
            return "0";
        }
        if (!(bool)userdata[0].ItemArray[1])
        {
            return "0";
        }

        if (CryptoTools.ValidatePasswordInSHA256(userdata[0].ItemArray[0].ToString(), password))
        {
            return userdata[0].ItemArray[2].ToString();
        }

        if (CryptoTools.ValidatePasswordInRfc2898Derive(userdata[0].ItemArray[0].ToString(), password))
        {
            return userdata[0].ItemArray[2].ToString();
        }

        return "0";
    }


    public string TSLoginReturnName(string UserId)
    {
        logger.LogInformation("TSLogin"

            , new KeyValuePair<string, object>("Id", UserId));

        var command = @"SELECT Name FROM tbl_User WHERE Id = @Id";
        var userdata = sqlDataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("Id", UserId))
            .Select().ToList();
        if (userdata.Count == 0)
        {
            return "";
        }
        else
        {
            return userdata[0].ItemArray[0].ToString();
        }
    }


    public DataTable GetUserRoles
(
       string userId,
string userToken
)
    {
        logger.LogInformation("GetUserRoles"
            , new KeyValuePair<string, object>("userId", userId)
            , new KeyValuePair<string, object>("userToken", userToken));

        var commandSelect = @"
				select tbl_Role.* 
                from tbl_UserRole
                inner join 
                tbl_Role 
                on
                tbl_Role.Id = tbl_UserRole.RoleId
                where tbl_UserRole.UserId = @UsrId 
                OR tbl_UserRole.UserId = (SELECT tbl_USER.Id from tbl_USER where tbl_USER.[Username] = @UsrId)";

        return sqlDataAccess.SqlDataAdapter(commandSelect, new KeyValuePair<string, object>("UsrId", userId));
    }

    public DataTable GetUserRolesByToken()
    {
        string userId = httpContext.User.GetUserId();

        logger.LogInformation("GetUserRolesByToken");

        var commandSelect = @"
				select tbl_Role.* 
                from tbl_UserRole
                inner join 
                tbl_Role 
                on
                tbl_Role.Id = tbl_UserRole.RoleId
                where tbl_UserRole.UserId = @UsrId 
                OR tbl_UserRole.UserId = (SELECT tbl_USER.Id from tbl_USER where tbl_USER.[Username] = @UsrId)";

        return sqlDataAccess.SqlDataAdapter(commandSelect, new KeyValuePair<string, object>("UsrId", userId));
    }

    public DataTable GetAllUser
(
string userToken
)
    {
        logger.LogInformation("GetAllUser"
            , " userToken: " + userToken);
        var commandSelect = @"
		SELECT tbl_User.Id,Username,tbl_User.Name,IsActive,Coalesce(Details,'') as [Details] 
		FROM tbl_User INNER JOIN tbl_UserRole ON tbl_User.Id = tbl_UserRole.UserId
		INNER JOIN tbl_Role ON  tbl_Role.Id = tbl_UserRole.RoleId 
		WHERE LOWER(tbl_Role.Name) <> N'shop' AND LOWER(tbl_Role.Name) <> N'install' ORDER BY CreateDate
        ";
        return sqlDataAccess.SqlDataAdapter(commandSelect);
    }

    public DataTable GetUserById
    (
        string userId,
        string userToken
    )
    {
        logger.LogInformation("GetUserById"
            , " userId: " + userId
           , " userToken: " + userToken);

        var commandSelect = @"
		        SELECT Id,Username,Name,IsActive,Coalesce(Details,'') as [Details] FROM tbl_User 
                WHERE Id = @UsrId ";
        return sqlDataAccess.SqlDataAdapter(commandSelect
            , new KeyValuePair<string, object>("UsrId", userId));
    }

    public string GetLatestIdOfIdentityTable
   (
       string tableName
   )
    {
        var sqlCommand = "select IDENT_CURRENT(@TblNam)";

        return sqlDataAccess.SqlDataAdapter(sqlCommand
            , new KeyValuePair<string, object>("TblNam", tableName)).Select().ToList()[0].ItemArray[0].ToString();
    }

    public DataTable GetAllRoles
(
string userToken
)
    {
        logger.LogInformation("GetAllRoles /n",
            " userToken: " + userToken);

        var commandSelect = @"
				SELECT * FROM [dbo].[tbl_Role] ORDER BY Id 
								";
        return sqlDataAccess.SqlDataAdapter(commandSelect);
    }

    public DataTable GetUserClaims(string userId, string userToken)
    {
        logger.LogInformation("GetUserClaims \n" +
            " userId: " + userId +
            " userToken: " + userToken);

        var commands = $"SELECT ClaimType as [Type], ClaimValue as [Value] FROM tbl_UserClaim WHERE UserId = N'{userId}' ";

        return sqlDataAccess.SqlDataAdapter(commands);
    }

    public DataTable GetUserClaimsByToken()
    {
        string userId = httpContext.User.GetUserId();

        logger.LogInformation("GetUserClaimsByToken \n" +
            " userId: " + userId);

        var commands = $"SELECT ClaimType as [Type], ClaimValue as [Value] FROM tbl_UserClaim WHERE UserId = N'{userId}' ";

        return sqlDataAccess.SqlDataAdapter(commands);
    }

    public DataTable GetUserByUsername
    (
        string username,
        string userToken
    )
    {
        username = username.ToLower();

        logger.LogInformation("GetUserClaims \n" +
            " username: " + username +
            " userToken: " + userToken);

        var commandSelect = @"
		         SELECT  tbl_User.Id,tbl_User.UserName,tbl_User.[Name],tbl_User.IsActive,Coalesce(tbl_User.Details,'') as [Details]
                 ,COALESCE((select TOP(1) tbl_role.Id from tbl_role inner join tbl_UserRole on tbl_role.Id = tbl_UserRole.RoleId 
                 where tbl_UserRole.UserId = tbl_User.Id ),N'') as [Role] 
				 ,COALESCE((select TOP(1) tbl_role.Name from tbl_role inner join tbl_UserRole on tbl_role.Id = tbl_UserRole.RoleId 
                 where tbl_UserRole.UserId = tbl_User.Id ),N'') as [RoleName], COALESCE(Image,N'-1') as [Image]
                FROM tbl_User 
                WHERE tbl_User.UserName = @Name
								";
        var result = sqlDataAccess.SqlDataAdapter(commandSelect, new KeyValuePair<string, object>("Name", username));
        return result;
    }

    public string RegisterNewUser
(
string username,
string password,
string creatorUserId,
bool isActive,
string persianName,
string detailsJson
)
    {
        logger.LogInformation("RegisterNewUser"
, new KeyValuePair<string, object>("username", username)
, new KeyValuePair<string, object>("password", password)
, new KeyValuePair<string, object>("creatorUserId", creatorUserId)
, new KeyValuePair<string, object>("isActive", isActive)
, new KeyValuePair<string, object>("persianName", persianName)
, new KeyValuePair<string, object>("detailsJson", detailsJson)
);
        var command = @"
INSERT [dbo].[tbl_User] ([Id], [LastModifiedDate], [CreateDate], [IsActive], [CreatorIdentityID], [LastModifierIdentityID], [Name],  [Email], [EmailConfirmed],
[PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName],[Details]) 
VALUES (@Id, GETDATE(), GETDATE(), @IsActiv , @CretrUsrId, @CretrUsrId,@PrsiaNam,NULL, 0, @HshPaswrd,@SecStmp, NULL, 0, 0, NULL, 0, 0, @Usrnam,@Dtl)
								";

        var userId = Guid.NewGuid().ToString();
        var parameters = new Dictionary<string, object>();
        parameters.Add("Id", userId);
        parameters.Add("IsActiv", isActive);
        parameters.Add("CretrUsrId", creatorUserId);
        parameters.Add("PrsiaNam", persianName);
        parameters.Add("HshPaswrd", CryptoTools.GetHashedStringSha256StringBuilder(password) /*HashPassword(password)*/);
        parameters.Add("SecStmp", Guid.NewGuid().ToString());
        parameters.Add("Usrnam", username);
        parameters.Add("Dtl", detailsJson);

        if (sqlDataAccess.CmdSqlExecuteNonQuery(command, parameters.ToArray()) > 0)
        {
            string commandRole =
                """
                 INSERT INTO tbl_UserRole (UserId,RoleId) VALUES (@UserId,@RoleId)
                """;

            return userId;
        }
        else
            return "0";
    }

    public string AddNewUserAndRole
(
string username,
string password,
string creatorUserId,
bool isActive,
string persianName,
string role,
string detailsJson
)
    {
        logger.LogInformation("AddNewUserAndRole"
, new KeyValuePair<string, object>("username", username)
, new KeyValuePair<string, object>("password", password)
, new KeyValuePair<string, object>("creatorUserId", creatorUserId)
, new KeyValuePair<string, object>("isActive", isActive)
, new KeyValuePair<string, object>("persianName", persianName)
, new KeyValuePair<string, object>("role", role)
, new KeyValuePair<string, object>("detailsJson", detailsJson)
);
        var command = @"
INSERT [dbo].[tbl_User] ([Id], [LastModifiedDate], [CreateDate], [IsActive], [CreatorIdentityID], [LastModifierIdentityID], [Name],  [Email], [EmailConfirmed],
[PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName],[Details]) 
VALUES (@Id, GETDATE(), GETDATE(), @IsActiv , @CretrUsrId, @CretrUsrId,@PrsiaNam,NULL, 0, @HshPaswrd,@SecStmp, NULL, 0, 0, NULL, 0, 0, @Usrnam,@Dtl)
								";

        var userId = Guid.NewGuid().ToString();
        var parameters = new Dictionary<string, object>();
        parameters.Add("Id", userId);
        parameters.Add("IsActiv", isActive);
        parameters.Add("CretrUsrId", creatorUserId);
        parameters.Add("PrsiaNam", persianName);
        parameters.Add("HshPaswrd", CryptoTools.GetHashedStringSha256StringBuilder(password) /*HashPassword(password)*/);
        parameters.Add("SecStmp", Guid.NewGuid().ToString());
        parameters.Add("Usrnam", username);
        parameters.Add("Dtl", detailsJson);

        if (sqlDataAccess.CmdSqlExecuteNonQuery(command, parameters.ToArray()) > 0)
        {
            string commandRole =
                """
                 INSERT INTO tbl_UserRole (UserId,RoleId) VALUES (@UserId,@RoleId)
                """;

            if (sqlDataAccess.CmdSqlExecuteNonQuery(commandRole
                , new KeyValuePair<string, object>("UserId", userId)
                , new("RoleId", role)) > 0)
            {
                return userId;
            }
        }

        return "0";
    }
    #endregion
}
