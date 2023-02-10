using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Document
{
    public class CreateFolderResult : BaseResult
    {
        public FolderEntityModel Folder { get; set; }
    }
}
