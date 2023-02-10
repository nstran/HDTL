using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Document;
using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;
using TN.TNM.BusinessLogic.Models.File;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Messages.Results.Document;

namespace TN.TNM.Api.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocument iDocument;
        private readonly IHostingEnvironment iHostingEnvironment;
        private readonly IDocumentDataAccess iDocumentDataAccess;


        public DocumentController(IDocument _iDocument, IDocumentDataAccess _iDocumentDataAccess, IHostingEnvironment hostingEnvironment)
        {
            iDocument = _iDocument;
            iDocumentDataAccess = _iDocumentDataAccess;
            iHostingEnvironment = hostingEnvironment;
        }


        [Route("api/document/updateProjectDocument")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public UpdateProjectDocumentResults UpdateProjectDocument([FromBody] UpdateProjectDocumentParameter request)
        {
            return this.iDocumentDataAccess.UpdateProjectDocument(request);
        }


        [HttpPost]
        [Route("api/document/createFolder")]
        [Authorize(Policy = "Member")]
        public CreateFolderResult CreateFolder([FromBody] CreateFolderParameter request)
        {
            return this.iDocumentDataAccess.CreateFolder(request);
        }

        [HttpPost]
        [Route("api/document/uploadFileDocument")]
        [Authorize(Policy = "Member")]
        public UploadFileDocumentResult UploadFileDocument([FromForm] UploadFileDocumentParameter request)
        {
            return this.iDocumentDataAccess.UploadFileDocument(request);
        }

        [HttpPost]
        [Route("api/document/loadFileByFolder")]
        [Authorize(Policy = "Member")]
        public LoadFileByFolderResult LoadFileByFolder([FromBody] LoadFileByFolderParameter request)
        {
            return this.iDocumentDataAccess.LoadFileByFolder(request);
        }

        //
        [HttpPost]
        [Route("api/document/uploadFileForOption")]
        [Authorize(Policy = "Member")]
        public UploadFileForOptionResponse UploadFileForOption(UploadFileForOptionRequest request)
        {
            try
            {
                if (request.FileList != null && request.FileList.Count > 0)
                {
                    string folderName = "FileUpload";
                    string webRootPath = iHostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    newPath = Path.Combine(newPath, "Project");
                    newPath = Path.Combine(newPath, request.ProjectCodeName);
                    newPath = Path.Combine(newPath, request.Option);

                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    var listFileNameExists = new List<FileNameExistsModel>();
                    foreach (IFormFile item in request.FileList)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = item.FileName.Trim();
                            string fullPath = Path.Combine(newPath, fileName);

                            string newfullPath = GetUniqueFilePath(fullPath);

                            if (fullPath != newfullPath)
                            {
                                var _file = new FileNameExistsModel();
                                _file.OldFileName = fileName;
                                _file.NewFileName = newfullPath.Substring(newfullPath.LastIndexOf("\\") + 1);
                                listFileNameExists.Add(_file);
                            }

                            using (var stream = new FileStream(newfullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                        }
                    }

                    return new UploadFileForOptionResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                        ListFileNameExists = listFileNameExists
                    };
                }

                return new UploadFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.FileUpload.NO_FILE
                };
            }
            catch (Exception e)
            {
                return new UploadFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //
        [HttpPost]
        [Route("api/document/deleteFileForOption")]
        [Authorize(Policy = "Member")]
        public DeleteFileForOptionResponse DeleteFileForOption(DeleteFileForOptionRequest request)
        {
            try
            {
                string folderName = "FileUpload";
                string webRootPath = iHostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName, "Project", request.ProjectCodeName);
                newPath = Path.Combine(newPath, request.Option, request.FileName);

                if (System.IO.File.Exists(newPath))
                {
                    System.IO.File.Delete(newPath);

                    return new DeleteFileForOptionResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success"
                    };
                }

                return new DeleteFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Không tồn tại file vật lý này trên hệ thống"
                };
            }
            catch (Exception e)
            {
                return new DeleteFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private static string GetUniqueFilePath(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string folderPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);
                int number = 1;

                var regex = Regex.Match(fileName, @"^(.+) \((\d+)\)$");

                if (regex.Success)
                {
                    fileName = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    var newFileName = $"{fileName} ({number}){fileExtension}";
                    filePath = Path.Combine(folderPath, newFileName);
                }
                while (System.IO.File.Exists(filePath));
            }

            return filePath;
        }
    }
}