using System;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class CreateFolderParameter : BaseParameter
    {
        public FolderEntityModel FolderParent { get; set; }
        
        public string FolderName { get; set; }

        public string ProjectCode { get; set; }
        
        public Guid ProjectId { get; set; }
        
    }
}
