using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Requests.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Responses.MenuBuild;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;
using TN.TNM.DataAccess.Messages.Results.MenuBuild;

namespace TN.TNM.Api.Controllers
{
    public class MenuBuildController : Controller
    {
        private readonly IMenuBuildDataAccess _iMenuBuildDataAccess;
        public MenuBuildController(IMenuBuildDataAccess iMenuBuildDataAccess)
        {
            this._iMenuBuildDataAccess = iMenuBuildDataAccess;
        }

        [HttpPost]
        [Route("api/menubuild/getMenuBuild")]
        [Authorize(Policy = "Member")]
        public GetMenuBuildResult GetMenuBuild([FromBody]GetMenuBuildParameter request)
        {
            return this._iMenuBuildDataAccess.GetMenuBuild(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getMenuModule")]
        [Authorize(Policy = "Member")]
        public GetMenuModuleResult GetMenuModule([FromBody] GetMenuModuleParameter request)
        {
            return this._iMenuBuildDataAccess.GetMenuModule(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/createMenuBuild")]
        [Authorize(Policy = "Member")]
        public CreateMenuBuildResult CreateMenuBuild([FromBody] CreateMenuBuildParameter request)
        {
            return this._iMenuBuildDataAccess.CreateMenuBuild(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getSubMenuModuleByMenuModuleCode")]
        [Authorize(Policy = "Member")]
        public GetSubMenuModuleByMenuModuleCodeResult GetSubMenuModuleByMenuModuleCode([FromBody] GetSubMenuModuleByMenuModuleCodeParameter request)
        {
            return this._iMenuBuildDataAccess.GetSubMenuModuleByMenuModuleCode(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getMenuPageBySubMenuCode")]
        [Authorize(Policy = "Member")]
        public GetMenuPageBySubMenuCodeResult GetMenuPageBySubMenuCode([FromBody] GetMenuPageBySubMenuCodeParameter request)
        {
            return this._iMenuBuildDataAccess.GetMenuPageBySubMenuCode(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/updateIsPageDetail")]
        [Authorize(Policy = "Member")]
        public UpdateIsPageDetailResult UpdateIsPageDetail([FromBody] UpdateIsPageDetailParameter request)
        {
            return this._iMenuBuildDataAccess.UpdateIsPageDetail(request);
        }

        [HttpPost]
        [Route("/api/menubuild/updateMenuBuild")]
        [Authorize(Policy = "Member")]
        public UpdateMenuBuildResult UpdateMenuBuild([FromBody] UpdateMenuBuildParameter request)
        {
            return this._iMenuBuildDataAccess.UpdateMenuBuild(request);
        }

        [HttpPost]
        [Route("api/menubuild/updateIsShow")]
        [Authorize(Policy = "Member")]
        public UpdateIsShowResult UpdateIsShow([FromBody] UpdateIsShowParameter request)
        {
            return this._iMenuBuildDataAccess.UpdateIsShow(request);
        }
    }
}