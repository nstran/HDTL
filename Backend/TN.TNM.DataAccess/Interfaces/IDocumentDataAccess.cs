using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Messages.Results.Document;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IDocumentDataAccess
    {
        DownloadDocumentByIdResult DownloadDocumentById(DownloadDocumentByIdParameter parameter);

        UpdateProjectDocumentResults UpdateProjectDocument(UpdateProjectDocumentParameter parameter);

        CreateFolderResult CreateFolder(CreateFolderParameter parameter);

        UploadFileDocumentResult UploadFileDocument(UploadFileDocumentParameter parameter);

        LoadFileByFolderResult LoadFileByFolder(LoadFileByFolderParameter parameter);
    }
}
