using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Document;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class LoadFileByFolderRequest : BaseRequest<LoadFileByFolderParameter>
    {
        public Guid FolderId { get; set; }

        public string FolderType { get; set; }

        public Guid ObjectId { get; set; }


        public override LoadFileByFolderParameter ToParameter()
        {
            return new LoadFileByFolderParameter()
            {
                FolderId = FolderId,
                FolderType = FolderType,
                ObjectId = ObjectId,
                UserId = UserId
            };
        }
    }
}
