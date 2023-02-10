namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class DeleteFileForOptionParameter : BaseParameter
    {
        public string Option { get; set; }

        public string FileName { get; set; }

        public string ProjectCodeName { get; set; }
    }
}
