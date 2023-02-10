using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class LoadFileByFolderResponses : BaseResponse
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
