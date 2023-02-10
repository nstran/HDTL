
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetDataAssetDetailResult : BaseResult
    {
        public List<AssetEntityModel> ListTaiSanPhanBo { get; set; }
        public List<BaoDuongEntityModel> ListBaoDuong { get; set; }
        public AssetEntityModel AssetDetail { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }

    }
}
