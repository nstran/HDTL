using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.AuditTrace;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.AuditTrace;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin
{
    public class AuditTraceFactory : BaseFactory, IAuditTrace
    {

        private IAuditTraceDataAccess iAuditTraceDataAccess;

        public AuditTraceFactory(IAuditTraceDataAccess _iAuditTraceDataAccess)
        {
            this.iAuditTraceDataAccess = _iAuditTraceDataAccess;
        }

        public GetMasterDataTraceResponses GetMasterDataTrace(GetMasterDataTraceRequests requests)
        {
            try
            {
                var parameter = requests.ToParameter();
                var result = iAuditTraceDataAccess.GetMasterDataTrace(parameter);
                var response = new GetMasterDataTraceResponses()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmp = new List<EmployeeModel>(),
                    ListUser = new List<UserModel>(),
                };
                
                result.ListEmp.ForEach(item =>
                {
                    response.ListEmp.Add(new EmployeeModel(item));
                });

                result.ListUser.ForEach(item =>
                {
                    response.ListUser.Add(new UserModel(item));
                });
                
                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataTraceResponses()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchTraceResponses SearchTrace(SearchTraceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iAuditTraceDataAccess.SearchTrace(parameter);
                var response = new SearchTraceResponses()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListAuditTrace = new List<TraceModel>(),
                    ListLoginTrace = new List<LoginTraceModel>(),
                    TotalRecordsLoginTrace = result.TotalRecordsLoginTrace,
                    TotalRecordsAuditTrace = result.TotalRecordsAuditTrace
                };

                result.ListAuditTrace.ForEach(item =>
                {
                    response.ListAuditTrace.Add(new TraceModel(item));
                });

                result.ListLoginTrace.ForEach(item =>
                {
                    response.ListLoginTrace.Add(new LoginTraceModel(item));
                });

                
                return response;
            }
            catch (Exception e)
            {
                return new SearchTraceResponses()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        
    }
}
