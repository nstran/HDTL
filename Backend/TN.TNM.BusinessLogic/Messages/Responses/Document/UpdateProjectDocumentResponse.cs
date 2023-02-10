using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class UpdateProjectDocumentResponse: BaseResponse
    {
        public List<FolderEntityModel> ListFolders { get; set; }
    }
}
