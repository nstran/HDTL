using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class SaveCatHaRequest : BaseRequest<SaveCatHaParameter>
    {
        public List<ProductionOrderMappingModel> ListItemInfor { get; set; }
        public override SaveCatHaParameter ToParameter()
        {
            var _listItemInfor = new List<ProductionOrderMappingEntityModel>();
            ListItemInfor.ForEach(item =>
            {
                _listItemInfor.Add(item.ToEntity());
            });

            return new SaveCatHaParameter()
            {
                UserId = UserId,
                ListItemInfor = _listItemInfor
            };
        }
    }
}
