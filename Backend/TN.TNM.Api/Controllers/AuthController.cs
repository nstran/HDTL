using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Requests.Users;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Responses.Users;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;

/// <summary>
/// Controller for authentication and authorization
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 16/06/2018
/// </summary>
namespace TN.TNM.Api.Controllers
{
    public class AuthController : Controller
    {
        private IAuth Auth;        
        private IAuthDataAccess AuthDataAccess;
        private IConfiguration Configuration;        

        public AuthController(IConfiguration configuration, IAuth iAuth, IAuthDataAccess iAuthDataAccess)
        {
            this.Configuration = configuration;
            this.Auth = iAuth;
            this.AuthDataAccess = iAuthDataAccess;
        }

        /// <summary>
        /// Get Auth token
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth")]
        [HttpPost]
        [AllowAnonymous]
        public LoginResult GetAuthToken([FromBody]LoginParameter request)
        {
            var response = this.AuthDataAccess.Login(request,
                this.Configuration["secret-key-name"],
                this.Configuration["token-valid-issuer"],
                this.Configuration["token-valid-audience"]);

            return response;
        }

        [Route("api/loginWithDeviceId")]
        [HttpPost]
        [AllowAnonymous]
        public LoginResult GetAuthTokenWithDeviceId([FromBody] LoginParameter request)
        {
            var response = this.AuthDataAccess.LoginWithDeviceId(request,
                this.Configuration["secret-key-name"],
                this.Configuration["token-valid-issuer"],
                this.Configuration["token-valid-audience"]);

            return response;
        }

        /// <summary>
        /// Get Menu by module code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getmenubymodulecode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetMenuByModuleCodeResult GetMenuByModuleCode([FromBody] GetMenuByModuleCodeParameter request)
        {
            var response = this.AuthDataAccess.GetMenuByModuleCode(request);
            return response;
        }

        /// <summary>
        /// Get current user's permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserPermissionResult GetUserPermission([FromBody]GetUserPermissionParameter request)
        {
            var response = this.AuthDataAccess.GetUserPermission(request);
            return response;
        }

        /// <summary>
        /// Create new permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/createPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public CreatePermissionResult CreatePermission([FromBody]CreatePermissionParameter request)
        {
            var response = this.AuthDataAccess.CreatePermission(request);
            return response;
        }

        /// <summary>
        /// Edit a permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editPermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditPermissionByIdResult EditPermissionById([FromBody]EditPermissionByIdParameter request)
        {
            var response = this.AuthDataAccess.EditPermissionById(request);
            return response;
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/deletePermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DeletePermissionByIdResult DeletePermissionById([FromBody]DeletePermissionByIdParameter request)
        {
            var response = this.AuthDataAccess.DeletePermissionById(request);
            return response;
        }

        /// <summary>
        /// Get a permission's info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPermissionByIdResult GetPermissionById([FromBody]GetPermissionByIdParameter request)
        {
            var response = this.AuthDataAccess.GetPermissionById(request);
            return response;
        }

        /// <summary>
        /// Get all Permission in DB
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllPermissionResult GetAllPermission([FromBody]GetAllPermissionParameter request)
        {
            var response = this.AuthDataAccess.GetAllPermission(request);
            return response;
        }

        /// <summary>
        /// Get Permission by PermissionCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPermissionByCode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPermissionByCodeResult GetPermissionByCode([FromBody]GetPermissionByCodeParameter request)
        {
            var response = this.AuthDataAccess.GetPermissionByCode(request);
            return response;
        }

        /// <summary>
        /// Change user's password
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/changePassword")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public ChangePasswordResult ChangePassword([FromBody]ChangePasswordParameter request)
        {
            var response = this.AuthDataAccess.ChangePassword(request);
            return response;
        }

        [Route("api/auth/changePasswordForgot")]
        [HttpPost]
        [AllowAnonymous]
        public ChangePasswordResult ChangePasswordForgot([FromBody] ChangePasswordForgotParameter request)
        {
            var response = this.AuthDataAccess.ChangePasswordForgot(request);
            return response;
        }


        /// <summary>
        /// Get User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileResult GetUserProfile([FromBody]GetUserProfileParameter request)
        {
            var response = this.AuthDataAccess.GetUserProfile(request);
            return response;
        }

        [Route("api/auth/getUserProfileByRoomName")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileByRoomNameResult GetUserProfileByRoomName([FromBody] GetUserProfileByRoomNameParameter request)
        {
            var response = this.AuthDataAccess.GetUserProfileByRoomName(request);
            return response;
        }

        /// <summary>
        /// Get User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserProfileByEmail")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileByEmailResult GetUserProfileByEmail([FromBody]GetUserProfileByEmailParameter request)
        {
            var response = this.AuthDataAccess.GetUserProfileByEmail(request);
            return response;
        }

        /// <summary>
        /// Edit User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditUserProfileResult EditUserProfile([FromBody]EditUserProfileParameter request)
        {
            var response = this.AuthDataAccess.EditUserProfile(request);
            return response;
        }

        /// <summary>
        /// GetModuleByPermissionSetId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getModuleByPermissionSetId")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetModuleByPermissionSetIdResult GetModuleByPermissionSetId([FromBody]GetModuleByPermissionSetIdParameter request)
        {
            var response = this.AuthDataAccess.GetModuleByPermissionSetId(request);
            return response;
        }

        /// <summary>
        /// GetAllPermissionSetNameAndCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllPermissionSetNameAndCode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllPermissionSetNameAndCodeResult GetAllPermissionSetNameAndCode(
            [FromBody]GetAllPermissionSetNameAndCodeParameter request)
        {
            var response = this.AuthDataAccess.GetAllPermissionSetNameAndCode(request);
            return response;
        }
        /// <summary>
        /// Get All User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllUser")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllUserResult GetAllUser(
            [FromBody]GetAllUserParameter request)
        {
            var response = this.AuthDataAccess.GetAllUser(request);
            return response;
        }

        /// <summary>
        /// Get Check User Name
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCheckUserName")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckUserNameResult GetCheckUserName(
            [FromBody]GetCheckUserNameParameter request)
        {
            var response = this.AuthDataAccess.GetCheckUserName(request);
            return response;
        }

        /// <summary>
        /// Get Check Reset Code User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCheckResetCodeUser")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckResetCodeUserResult GetCheckResetCodeUser(
            [FromBody]GetCheckResetCodeUserParameter request)
        {
            var response = this.AuthDataAccess.GetCheckResetCodeUser(request);
            return response;
        }

        /// <summary>
        /// Reset Password for User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/resetPassword")]
        [HttpPost]
        [AllowAnonymous]
        public ResetPasswordResult ResetPassword(
            [FromBody]ResetPasswordParameter request)
        {
            var response = this.AuthDataAccess.ResetPassword(request);
            return response;
        }

        /// <summary>
        /// Get PositionCode By PositionId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPositionCodeByPositionId")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(
            [FromBody]GetPositionCodeByPositionIdParameter request)
        {
            var response = this.AuthDataAccess.GetPositionCodeByPositionId(request);
            return response;
        }

        /// <summary>
        /// Get All Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllRoleResult GetAllRole([FromBody]GetAllRoleParameter request)
        {
            var response = this.AuthDataAccess.GetAllRole(request);
            return response;
        }

        /// <summary>
        /// Get Create Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCreatePermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetCreatePermissionResult GetCreatePermission([FromBody]GetCreatePermissionParameter request)
        {
            var response = this.AuthDataAccess.GetCreatePermission(request);
            return response;
        }

        /// <summary>
        /// Create Role And Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/createRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public CreateRoleAndPermissionResult CreateRoleAndPermission([FromBody]CreateRoleAndPermissionParameter request)
        {
            var response = this.AuthDataAccess.CreateRoleAndPermission(request);
            return response;
        }

        /// <summary>
        /// Get Detail Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getDetailPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetDetailPermissionResult GetDetailPermission([FromBody]GetDetailPermissionParameter request)
        {
            var response = this.AuthDataAccess.GetDetailPermission(request);
            return response;
        }

        /// <summary>
        /// Edit Role And Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditRoleAndPermissionResult EditRoleAndPermission([FromBody]EditRoleAndPermissionParameter request)
        {
            var response = this.AuthDataAccess.EditRoleAndPermission(request);
            return response;
        }

        /// <summary>
        /// Add User Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/addUserRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public AddUserRoleResult AddUserRole([FromBody]AddUserRoleParameter request)
        {
            var response = this.AuthDataAccess.AddUserRole(request);
            return response;
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/deleteRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DeleteRoleResult DeleteRole([FromBody]DeleteRoleParameter request)
        {
            var response = this.AuthDataAccess.DeleteRole(request);
            return response;
        }

        [Route("api/auth/register")]
        [HttpPost]
        [AllowAnonymous]
        public RegisterResult Register([FromBody] RegisterParameter request)
        {
            var response = this.AuthDataAccess.Register(request);
            return response;
        }

        [Route("api/auth/takeListProvince")]
        [HttpPost]
        [AllowAnonymous]
        public GetListProvinceResult TakeListProvince([FromBody] GetListProvinceParameter request)
        {
            var response = this.AuthDataAccess.TakeListProvince(request);
            return response;
        }

        [Route("api/auth/removeDeviceId")]
        [HttpPost]
        [AllowAnonymous]
        public RemoveDeviceIdResult RemoveDeviceId([FromBody] RemoveDeviceIdParameter request)
        {
            var response = this.AuthDataAccess.RemoveDeviceId(request);
            return response;
        }

        [Route("api/auth/updateUserToNotActive")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public UpdateUserToNotActiveResult UpdateUserToNotActive([FromBody] UpdateUserNotActiveParameter request)
        {
            var response = this.AuthDataAccess.UpdateUserToNotActive(request);
            return response;
        }
    }
}
