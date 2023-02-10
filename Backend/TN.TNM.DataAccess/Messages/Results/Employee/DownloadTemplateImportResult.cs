namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class DownloadTemplateImportResult : BaseResult
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
