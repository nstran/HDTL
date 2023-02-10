using System;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Document;
using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Document
{
    public class DocumentFactory : BaseFactory, IDocument
    {
        private IDocumentDataAccess iDocumentDataAccess;
        public DocumentFactory(IDocumentDataAccess _iDocumentDataAccess, ILogger<DocumentFactory> _logger)
        {
            iDocumentDataAccess = _iDocumentDataAccess;
            logger = _logger;
        }

        public DownloadDocumentByIdResponse DownloadDocumentById(DownloadDocumentByIdRequest request)
        {
            try
            {
                logger.LogInformation("Download Document ById");
                var parameter = request.ToParameter();
                var result = iDocumentDataAccess.DownloadDocumentById(parameter);
                var response = new DownloadDocumentByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ExcelFile=result.ExcelFile,
                    NameFile=result.NameFile
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DownloadDocumentByIdResponse()
                {
                    MessageCode = CommonMessage.Document.DOWNLOAD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateProjectDocumentResponse UpdateProjectDocument(UpdateProjectDocumentRequests requests)
        {
            try
            {
                var parameter = requests.ToParameter();
                var result = iDocumentDataAccess.UpdateProjectDocument(parameter);
                var response = new UpdateProjectDocumentResponse()
                {
                    ListFolders = result.ListFolders,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectDocumentResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateFolderResponse CreateFolder(CreateFolderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iDocumentDataAccess.CreateFolder(parameter);
                var response = new CreateFolderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    Folder = result.Folder,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new CreateFolderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UploadFileDocumentResponse UploadFileDocument(UploadFileDocumentRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iDocumentDataAccess.UploadFileDocument(parameter);
                var response = new UploadFileDocumentResponse()
                {
                    ListFileInFolder = result.ListFileInFolder,
                    ListFolders = result.ListFolders,
                    TotalSize = result.TotalSize,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new UploadFileDocumentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public LoadFileByFolderResponses LoadFileByFolder(LoadFileByFolderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iDocumentDataAccess.LoadFileByFolder(parameter);
                var response = new LoadFileByFolderResponses()
                {
                    ListFileInFolder = result.ListFileInFolder,
                    //ListFolders = result.ListFolders,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new LoadFileByFolderResponses()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
