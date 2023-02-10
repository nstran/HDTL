using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class UpdateStatusTimeSheetRequest : BaseRequest<UpdateStatusTimeSheetParameter>
    {
        public List<Guid> ListTimeSheetId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public TimeSheetEntityModel TimeSheet { get; set; }

        public override UpdateStatusTimeSheetParameter ToParameter()
        {
            return new UpdateStatusTimeSheetParameter
            {
                ListTimeSheetId = this.ListTimeSheetId,
                Type = this.Type,
                Description = this.Description,
                UserId = this.UserId,
                TimeSheet = this.TimeSheet
            };
        }
    }
}
