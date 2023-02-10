using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataViewRememberItemDialogRequest : BaseRequest<GetMasterDataViewRememberItemDialogParameter>
    {
        public string ProductionOrderCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }
        public double? ProductThickness { get; set; }

        public override GetMasterDataViewRememberItemDialogParameter ToParameter()
        {
            return new GetMasterDataViewRememberItemDialogParameter()
            {
                UserId = UserId,
                ProductionOrderCode = ProductionOrderCode,
                FromDate = FromDate,
                ToDate = ToDate,
                ProductName = ProductName,
                ProductThickness = ProductThickness
            };
        }
    }
}
