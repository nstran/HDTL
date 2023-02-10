using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateAllBTPRequest : BaseRequest<CreateAllBTPParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public ProductionOrderMappingModel BTP { get; set; }
        public List<ProductionOrderMappingModel> ListBTP1 { get; set; }
        public List<ProductionOrderMappingModel> ListBTP2 { get; set; }

        public override CreateAllBTPParameter ToParameter()
        {
            var parameter = new CreateAllBTPParameter()
            {
                ListBTP1 = new List<ProductionOrderMappingEntityModel>(),
                ListBTP2 = new List<ProductionOrderMappingEntityModel>(),
                BTP = new ProductionOrderMappingEntityModel(),
                ProductionOrderId = ProductionOrderId,
                UserId = UserId
            };

            ListBTP1.ForEach(item =>
            {
                var product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(tech =>
                {
                    product.ListTechnique.Add(tech.ToEntity());
                });

                parameter.ListBTP1.Add(product);
            });

            ListBTP2.ForEach(item =>
            {
                var product = item.ToEntity();
                product.ListTechnique = new List<TechniqueRequestEntityModel>();
                item.ListTechnique.ForEach(tech =>
                {
                    product.ListTechnique.Add(tech.ToEntity());
                });

                parameter.ListBTP2.Add(product);
            });

            parameter.BTP = BTP.ToEntity();
            parameter.BTP.ListTechnique = new List<TechniqueRequestEntityModel>();
            BTP.ListTechnique.ForEach(item =>
            {
                parameter.BTP.ListTechnique.Add(item.ToEntity());
            });

            return parameter;
        }
    }
}
