using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;

namespace TN.TNM.BusinessLogic.Interfaces.Document
{
    public interface IDocument
    {
        DownloadDocumentByIdResponse DownloadDocumentById(DownloadDocumentByIdRequest request);

        UpdateProjectDocumentResponse UpdateProjectDocument(UpdateProjectDocumentRequests requests);

        CreateFolderResponse CreateFolder(CreateFolderRequest request);

        UploadFileDocumentResponse UploadFileDocument(UploadFileDocumentRequest request);

        LoadFileByFolderResponses LoadFileByFolder(LoadFileByFolderRequest request);
    }
}
