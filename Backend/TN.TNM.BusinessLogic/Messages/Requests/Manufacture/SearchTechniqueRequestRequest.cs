using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class SearchTechniqueRequestRequest : BaseRequest<SearchTechniqueRequestParameter>
    {
        public string TechniqueName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public override SearchTechniqueRequestParameter ToParameter()
        {
            return new SearchTechniqueRequestParameter()
            {
                UserId = UserId,
                TechniqueName = TechniqueName,
                OrganizationId = OrganizationId,
                ParentId = ParentId,
                IsDefault = IsDefault,
                Description = Description
            };
        }
    }
}
