using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Document;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class DeleteFileForOptionRequest: BaseRequest<DeleteFileForOptionParameter>
    {
        public string Option { get; set; }
        public string FileName { get; set; }

        public string ProjectCodeName { get; set; }

        public override DeleteFileForOptionParameter ToParameter()
        {
            return new DeleteFileForOptionParameter
            {
                Option = Option,
                FileName = FileName,
                ProjectCodeName = ProjectCodeName,
                UserId = this.UserId,
            };
        }
    }
}
