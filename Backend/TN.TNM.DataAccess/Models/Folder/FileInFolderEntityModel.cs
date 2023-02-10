using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Folder
{
    public class FileInFolderEntityModel
    {
        public Guid? FileInFolderId { get; set; }
        public Guid? FolderId { get; set; }
        public string FileName { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public int? ObjectNumber { get; set; }
        public string Size { get; set; }
        public bool? Active { get; set; }
        public string FileExtension { get; set; }
        public Guid? CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string FileUrl { get; set; }
        public string FileFullName { get; set; }
        public string UploadByName { get; set; }
    }
}
