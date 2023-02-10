using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateItemInProductionRequest : BaseRequest<CreateItemInProductionParameter>
    {
        public ProductionOrderMappingModel ProductItem { get; set; }

        public override CreateItemInProductionParameter ToParameter()
        {
            var parameter = new CreateItemInProductionParameter()
            {
                UserId = UserId,
                ProductItem = new ProductionOrderMappingEntityModel()
            };
            parameter.ProductItem = ProductItem.ToEntity();
            parameter.ProductItem.ListTechnique = new List<TechniqueRequestEntityModel>();
            ProductItem.ListTechnique.ForEach(item =>
            {
                parameter.ProductItem.ListTechnique.Add(item.ToEntity());
            });

            return parameter;
        }

    }
}
