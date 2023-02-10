using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class UploadFileDocumentRequest : BaseRequest<UploadFileDocumentParameter>
    {
        public string FolderType { get; set; }
        public Guid FolderId { get; set; }
        public Guid ObjectId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
        

        public override UploadFileDocumentParameter ToParameter()
        {
            return new UploadFileDocumentParameter
            {
                FolderId = FolderId,
                FolderType = FolderType,
                ObjectId = ObjectId,
                ListFile = ListFile,
                UserId = UserId
            };
        }
    }
}
