using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Country
{
    public class GetAllCountryResult : BaseResult
    {
        public List<CountryEntityModel> ListCountry { get; set; }
    }
}
