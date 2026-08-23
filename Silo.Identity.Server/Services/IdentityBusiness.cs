using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Contracts;
using Silo.Identity.Server.Dtos;
using Silo.Identity.Server.Utilities;

namespace Silo.Identity.Server.Services;

public class IdentityBusiness
{
    private readonly HttpContext httpContext;
    private readonly IDataAccess sqlDataAccess;
    private readonly ILogger<IdentityBusiness> logger;

    public IdentityBusiness(IHttpContextAccessor httpContextAccessor
        , IDataAccess sqlDataAccess
        , ILogger<IdentityBusiness> logger)
    {
        this.httpContext = httpContextAccessor.HttpContext;
        this.sqlDataAccess = sqlDataAccess;
        this.logger = logger;
    }

    public (string, string) IUserDataForProfileById(string id)
    {
        string command = " SELECT Name,COALESCE(Image,N'-1') FROM tbl_User WHERE tbl_User.Id = @Id";

        var dt = sqlDataAccess.SqlDataAdapter(command
            , new KeyValuePair<string, object>("Id", id)).Select().First();

        string name = dt.ItemArray[0].ToString();

        string image = dt.ItemArray[1].ToString();

        return new(name, image);
    }

    public bool IUpdateProfile(UserProfile profile)
    {
        string userId = httpContext.User.GetUserId();

        logger.LogInformation("IUpdateProfile" + Environment.NewLine
                            + $"UserId:{userId}");

        if (profile.Password.HasValue())
        {
            var command = @"SELECT PasswordHash,Id FROM tbl_User
						WHERE Id = @Id";
            var userdata = sqlDataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("Id", userId))
                .Select().ToList();

            if (CryptoTools.ValidatePasswordInSHA256(userdata[0].ItemArray[0].ToString(), profile.Password))
            {
                UpdatePassword();

                return true;
            }

            if (CryptoTools.ValidatePasswordInRfc2898Derive(userdata[0].ItemArray[0].ToString(), profile.Password))
            {
                UpdatePassword();

                return true;
            }
        }
        else
        {
            string commandUpdate = "UPDATE tbl_User SET Image=@Image WHERE tbl_User.Id = @Id";

            int result = sqlDataAccess.CmdSqlExecuteNonQuery(commandUpdate
               , new KeyValuePair<string, object>("Id", userId)
               , new KeyValuePair<string, object>("Image", profile.Image));

            logger.LogInformation("IUpdateProfile" + Environment.NewLine
                            + $"UserId:{userId}" + Environment.NewLine
                            + $"Update user profile result:{result}");

            return true;
        }

        return false;

        void UpdatePassword()
        {
            string hashed = CryptoTools.GetHashedStringSha256StringBuilder(profile.NewPassword);

            string commandUpdate = "UPDATE tbl_User SET PasswordHash = @Hash WHERE tbl_User.Id = @Id";

            int result = sqlDataAccess.CmdSqlExecuteNonQuery(commandUpdate
               , new KeyValuePair<string, object>("Id", userId)
               , new KeyValuePair<string, object>("Hash", hashed));

            logger.LogInformation("IUpdateProfile" + Environment.NewLine
                            + $"UserId:{userId}" + Environment.NewLine
                            + $"Update user profile result:{result}");
        }
    }

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
		SELECT Id,Username,Name,IsActive,Coalesce(Details,'') as [Details] FROM tbl_User ORDER BY CreateDate
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
                 where tbl_UserRole.UserId = tbl_User.Id ),N'') as [Role] , COALESCE(Image,N'-1') as [Image]
                FROM tbl_User 
                WHERE tbl_User.UserName = @Name ";
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
                INSERT INTO tbl_UserRole (UserId, RoleId) 
                SELECT TOP 1 @UserId, Id 
                FROM tbl_Role 
                WHERE Id = @RoleParam OR Name = @RoleParam
                """;

            if (sqlDataAccess.CmdSqlExecuteNonQuery(commandRole
                , new KeyValuePair<string, object>("UserId", userId)
                , new KeyValuePair<string, object>("RoleParam", role)) > 0)
            {
                return userId;
            }
        }

        return "0";
    }

    public string IGetStationCodeByMac(string mac)
    {
        string command = $"SELECT fld_StationCode FROM tbl_Station WHERE fld_StationMacAddress = N'{mac}'";

        var dt = sqlDataAccess.SqlDataAdapter(command).Select();

        if (dt.Any())
        {
            return dt.First().ItemArray.First().ToString();
        }

        return string.Empty;
    }


}
