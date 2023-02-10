using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Document
{
    public class CreateFolderResponse : BaseResponse
    {
        public FolderEntityModel Folder { get; set; }
    }
}
