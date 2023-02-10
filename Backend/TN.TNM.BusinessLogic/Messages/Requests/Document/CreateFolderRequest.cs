using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class CreateFolderRequest : BaseRequest<CreateFolderParameter>
    {
        public FolderEntityModel FolderParent { get; set; }
        public string FolderName { get; set; }
        
        public string ProjectCode { get; set; }


        public override CreateFolderParameter ToParameter()
        {
            return new CreateFolderParameter()
            {
                FolderName = FolderName,
                ProjectCode = ProjectCode,
                FolderParent = FolderParent,
                UserId = UserId
            };
        }
    }
}
