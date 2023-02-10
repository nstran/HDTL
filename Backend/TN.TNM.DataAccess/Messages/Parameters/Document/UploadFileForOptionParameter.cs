using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class UploadFileForOptionParameter : BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
        public string Option { get; set; }

        public string ProjectCodeName { get; set; }
    }
}
