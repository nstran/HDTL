using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.File;

namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class UploadFileForOptionResponse : BaseResponse
    {
        public List<FileNameExistsModel> ListFileNameExists { get; set; }
    }
}
