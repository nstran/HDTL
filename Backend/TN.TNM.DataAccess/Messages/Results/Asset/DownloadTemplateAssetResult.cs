
namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class DownloadTemplateAssetResult : BaseResult
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}
