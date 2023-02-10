using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetMasterDataTimeSheetRequest : BaseRequest<GetMasterDataTimeSheetParameter>
    {
        public Guid? ProjectId { get; set; }
        public override GetMasterDataTimeSheetParameter ToParameter()
        {
            return new GetMasterDataTimeSheetParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
