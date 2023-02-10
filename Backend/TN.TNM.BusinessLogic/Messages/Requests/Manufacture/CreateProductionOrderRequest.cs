using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateProductionOrderRequest : BaseRequest<CreateProductionOrderParameter>
    {
        public ProductionOrderModel ProductionOrder { get; set; }
        public List<ProductionOrderMappingModel> ListProduct { get; set; }
        public List<ProductionOrderMappingModel> ListProductChildren { get; set; }
        public List<ProductionOrderMappingModel> ListProductChildrenChildren { get; set; }
        public override CreateProductionOrderParameter ToParameter()
        {
            var _parameter = new CreateProductionOrderParameter()
            {
                UserId = UserId,
                ProductionOrder = ProductionOrder.ToEntity(),
                ListProduct = new List<ProductionOrderMappingEntityModel>(),
                ListProductChildren = new List<ProductionOrderMappingEntityModel>() ,
                ListProductChildrenChildren = new List<ProductionOrderMappingEntityModel>()
            };

            ListProduct.ForEach(item =>
            {
                var product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(x =>
                {
                    product.ListTechnique.Add(x.ToEntity());
                });

                _parameter.ListProduct.Add(product);
            });

            ListProductChildren.ForEach(item =>
            {
                var product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(x =>
                {
                    product.ListTechnique.Add(x.ToEntity());
                });

                _parameter.ListProductChildren.Add(product);
            });

            ListProductChildrenChildren.ForEach(item =>
            {
                var product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(x =>
                {
                    product.ListTechnique.Add(x.ToEntity());
                });

                _parameter.ListProductChildrenChildren.Add(product);
            });

            return _parameter;
        }
    }
}
