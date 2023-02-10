using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateWorkFlowForProductionOrderRequest : BaseRequest<UpdateWorkFlowForProductionOrderParameter>
    {
        public List<ProductionOrderMappingModel> ListProductItem { get; set; }
        public override UpdateWorkFlowForProductionOrderParameter ToParameter()
        {
            var parameter = new UpdateWorkFlowForProductionOrderParameter()
            {
                ListProductItem = new List<ProductionOrderMappingEntityModel>(),
                UserId = UserId
            };
            ListProductItem.ForEach(item =>
            {
                var product = new ProductionOrderMappingEntityModel();
                product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(technique =>
                {
                    product.ListTechnique.Add(technique.ToEntity());
                });

                parameter.ListProductItem.Add(product);
            });

            return parameter;
        }
    }
}
