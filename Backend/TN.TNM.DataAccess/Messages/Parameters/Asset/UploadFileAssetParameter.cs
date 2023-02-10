
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class UploadFileAssetParameter : BaseParameter
    {
        public string FolderType { get; set; }
        public int ObjectNumber { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
