using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.Admin.BusinessGoals;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.BusinessGoals;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.BusinessGoals;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.BusinessGoals
{
    public class BusinessGoalsFactory : BaseFactory, IBusinessGoals
    {
        private IBusinessGoalsDataAccess iBusinessGoalsDataAccess;

        public BusinessGoalsFactory(IBusinessGoalsDataAccess _iBusinessGoalsDataAccess, ILogger<BusinessGoalsFactory> _logger)
        {
            this.iBusinessGoalsDataAccess = _iBusinessGoalsDataAccess;
            this.logger = _logger;
        }

        public CreateOrUpdateBusinessGoalsResponse CreateOrUpdateBusinessGoals(CreateOrUpdateBusinessGoalsRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iBusinessGoalsDataAccess.CreateOrUpdateBusinessGoals(parameter);
                var response = new CreateOrUpdateBusinessGoalsResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateOrUpdateBusinessGoalsResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataBusinessGoalsResponse GetMasterDataBusinessGoals(GetMasterDataBusinessGoalsRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iBusinessGoalsDataAccess.GetMasterDataBusinessGoals(parameter);
                var response = new GetMasterDataBusinessGoalsResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                    ListOrganization = result.ListOrganization,
                    ListProductCategory = result.ListProductCategory,
                    ListBusinessGoalsRevenueDetail = result.ListBusinessGoalsRevenueDetail,
                    ListBusinessGoalsSalesDetail = result.ListBusinessGoalsSalesDetail,
                    ListBusinessGoalsRevenueDetailChild = result.ListBusinessGoalsRevenueDetailChild,
                    ListBusinessGoalsSalesDetailChild = result.ListBusinessGoalsSalesDetailChild
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetMasterDataBusinessGoalsResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
