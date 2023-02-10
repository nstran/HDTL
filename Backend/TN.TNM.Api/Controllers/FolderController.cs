using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Folder;
using TN.TNM.BusinessLogic.Messages.Requests.Folder;
using TN.TNM.BusinessLogic.Messages.Responses.Folder;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Messages.Results.Folder;

namespace TN.TNM.Api.Controllers
{
    public class FolderController : Controller
    {
        private readonly IFolderDataAccess _iFolderDataAccess;
        public FolderController(IFolderDataAccess iFolderDataAccess)
        {
            _iFolderDataAccess = iFolderDataAccess;
        }

        [HttpPost]
        [Route("api/folder/getAllFolderDefault")]
        [Authorize(Policy = "Member")]
        public GetAllFolderDefaultResult GetAllFolderDefault([FromBody] GetAllFolderDefaultParameter request)
        {
            return this._iFolderDataAccess.GetAllFolderDefault(request);
        }

        [HttpPost]
        [Route("api/folder/getAllFolderActive")]
        [Authorize(Policy = "Member")]
        public GetAllFolderActiveResult GetAllFolderActive([FromBody] GetAllFolderActiveParameter request)
        {
            return this._iFolderDataAccess.GetAllFolderActive(request);
        }

        [HttpPost]
        [Route("api/folder/addOrUpdateFolder")]
        [Authorize(Policy = "Member")]
        public AddOrUpdateFolderResult AddOrUpdateFolder([FromBody] AddOrUpdateFolderParameter request)
        {
            return this._iFolderDataAccess.AddOrUpdateFolder(request);
        }

        [HttpPost]
        [Route("api/folder/createFolder")]
        [Authorize(Policy = "Member")]
        public CreateFolderResult CreateFolder([FromBody] CreateFolderParameter request)
        {
            return this._iFolderDataAccess.CreateFolder(request);
        }

        [HttpPost]
        [Route("api/folder/deleteFolder")]
        [Authorize(Policy = "Member")]
        public DeleteFolderResult DeleteFolder([FromBody] DeleteFolderParameter request)
        {
            return this._iFolderDataAccess.DeleteFolder(request);
        }

        [HttpPost]
        [Route("api/folder/uploadFile")]
        [Authorize(Policy = "Member")]
        public  UploadFileResult UploadFile([FromForm] UploadFileParameter request)
        {
            return this._iFolderDataAccess.UploadFile(request);
        }

        [HttpPost]
        [Route("api/folder/downloadFile")]
        [Authorize(Policy = "Member")]
        public DownloadFileResult DownloadFile([FromBody] DownloadFileParameter request)
        {
            return this._iFolderDataAccess.DownloadFile(request);
        }

        [HttpPost]
        [Route("api/folder/uploadFileByFolderId")]
        [Authorize(Policy = "Member")]
        public UploadFileByFolderIdResult UploadFileByFolderId([FromForm] UploadFileByFolderIdParameter request)
        {
            return this._iFolderDataAccess.UploadFileByFolderId(request);
        }

        [HttpPost]
        [Route("api/folder/deleteFile")]
        [Authorize(Policy = "Member")]
        public DeleteFileResult DeleteFile([FromBody] DeleteFileParameter request)
        {
            return this._iFolderDataAccess.DeleteFile(request);
        }
    }
}