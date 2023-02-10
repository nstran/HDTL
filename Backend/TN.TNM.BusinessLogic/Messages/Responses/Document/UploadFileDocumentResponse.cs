using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class UploadFileDocumentResponse : BaseResponse
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }

        public List<FolderEntityModel> ListFolders { get; set; }

        public decimal TotalSize { get; set; }
    }
}
