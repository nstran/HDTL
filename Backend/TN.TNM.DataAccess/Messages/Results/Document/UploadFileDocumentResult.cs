using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Document
{
    public class UploadFileDocumentResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }

        public List<FolderEntityModel> ListFolders { get; set; }

        public decimal TotalSize { get; set; }
    }
}
