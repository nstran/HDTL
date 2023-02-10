using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Document
{
    public class LoadFileByFolderResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
