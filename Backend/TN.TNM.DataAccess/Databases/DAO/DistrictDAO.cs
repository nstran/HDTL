using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.District;
using TN.TNM.DataAccess.Messages.Results.Admin.District;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DistrictDAO : BaseDAO, IDistrictDataAccess
    {
        public DistrictDAO(IAuditTraceDataAccess _iAuditTrace, Databases.TNTN8Context context)
        {
            this.context = context;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllDistrictByProvinceIdResult GetAllDistrictByProvinceId(
            GetAllDistrictByProvinceIdParameter parameter)
        {
            try
            {
                var provinceId = parameter.ProvinceId;
                var listDistrict = context.District.Where(d => d.ProvinceId == provinceId).OrderBy(l => l.DistrictName).ToList();
                var list = new List<DistrictEntityModel>();
                listDistrict.ForEach(item =>
                {
                    var newItem = new DistrictEntityModel(item);
                    list.Add(newItem);
                });

                return new GetAllDistrictByProvinceIdResult()
                {
                    ListDistrict = list,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (System.Exception e)
            {
                return new GetAllDistrictByProvinceIdResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
            
        }
    }
}
