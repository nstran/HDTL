using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class UploadFileDocumentParameter : BaseParameter
    {
        public string FolderType { get; set; }
        public Guid FolderId { get; set; }
        public Guid ObjectId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
