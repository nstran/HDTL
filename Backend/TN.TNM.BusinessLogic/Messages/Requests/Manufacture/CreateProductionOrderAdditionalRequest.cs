using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateProductionOrderAdditionalRequest : BaseRequest<CreateProductionOrderAdditionalParameter>
    {
        public List<ProductionOrderMappingModel> ListProduct { get; set; }

        public override CreateProductionOrderAdditionalParameter ToParameter()
        {
            var parameter = new CreateProductionOrderAdditionalParameter()
            {
                UserId = UserId,
                ListProduct = new List<ProductionOrderMappingEntityModel>()
            };
            ListProduct.ForEach(item =>
            {
                parameter.ListProduct.Add(item.ToEntity());
            });

            return parameter;
        }
    }
}
