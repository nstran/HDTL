using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.Queue;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class SendQuickEmailRequest : BaseRequest<SendQuickEmailParameter>
    {
        public QueueModel Queue { get; set; }

        public List<FileUploadEntityModel> ListFile { get; set; }

        public string FolderType { get; set; }

        public override SendQuickEmailParameter ToParameter()
        {
            return new SendQuickEmailParameter
            {
                Queue = Queue.ToEntity(),
                ListFile = ListFile,
                FolderType = FolderType,
                UserId = UserId
            };
        }
    }
}
