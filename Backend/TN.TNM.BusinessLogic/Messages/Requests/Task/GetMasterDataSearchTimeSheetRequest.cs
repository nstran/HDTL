using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetMasterDataSearchTimeSheetRequest : BaseRequest<GetMasterDataSearchTimeSheetParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterDataSearchTimeSheetParameter ToParameter()
        {
            return new GetMasterDataSearchTimeSheetParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
