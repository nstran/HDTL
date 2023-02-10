using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Country;
using TN.TNM.DataAccess.Messages.Results.Admin.Country;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CountryDAO : BaseDAO , ICountryDataAccess
    {
        public CountryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public GetAllCountryResult GetAllCountry(GetAllCountryParameter parameter)
        {
            try
            {
                var _listCountry = context.Country.Where(ct => true).ToList();
                var list = new List<CountryEntityModel>();
                _listCountry.ForEach(item =>
                {
                    var _item = new CountryEntityModel(item);
                    list.Add(_item);
                });

                return new GetAllCountryResult()
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCountry = list
                };
            }
            catch(Exception e)
            {
                return new GetAllCountryResult()
                {
                    MessageCode =e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }
    }
}
