using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Document
{
    public class UpdateProjectDocumentResults : BaseResult
    {
        public List<FolderEntityModel> ListFolders { get; set; }
    }
}
