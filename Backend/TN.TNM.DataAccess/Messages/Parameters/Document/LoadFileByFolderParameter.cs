using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class LoadFileByFolderParameter : BaseParameter
    {
        public Guid FolderId { get; set; }
        
        public string FolderType { get; set; }

        public Guid ObjectId { get; set; }
    }
}
