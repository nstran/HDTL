using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterProjectDocumentRequest : BaseRequest<GetMasterProjectDocumentParameter>
    {
        public Guid ProjectId { get; set; }

        public override GetMasterProjectDocumentParameter ToParameter()
        {
            return new GetMasterProjectDocumentParameter()
            {
                ProjectId = ProjectId,
                UserId = this.UserId,
            };
        }
    }
}
