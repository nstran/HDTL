using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Email;
using TN.TNM.BusinessLogic.Models.Email;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.BusinessLogic.Factories.Admin.EmailConfig
{
    public class EmailConfigFactory : BaseFactory, IEmailConfig
    {
        private IEmailConfigurationDataAccess iEmailConfigurationDataAccess;

        public EmailConfigFactory(IEmailConfigurationDataAccess _iEmailConfigurationDataAccess, ILogger<EmailConfigFactory> _logger)
        {
            this.iEmailConfigurationDataAccess = _iEmailConfigurationDataAccess;
            this.logger = _logger;
        }

        public CreateUpdateEmailTemplateMasterdataResponse CreateUpdateEmailTemplateMasterdata(CreateUpdateEmailTemplateMasterdataRequest request)
        {
            try
            {
                this.logger.LogInformation("CreateUpdateEmailTemplateMasterdata");
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.CreateUpdateEmailTemplateMasterdata(parameter);
                var response = new CreateUpdateEmailTemplateMasterdataResponse
                {
                    ListEmailType = new List<CategoryEntityModel>(),
                    ListEmailStatus = new List<CategoryEntityModel>(),    
                    EmailTemplateModel = result.EmailTemplateModel,
                    ListEmailToCC = result.ListEmailToCC,
                    ListEmailTemplateToken = new List<EmailTemplateTokenEntityModel>(),
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };

                result.ListEmailType.ForEach(e => response.ListEmailType.Add(e));
                result.ListEmailStatus.ForEach(e => response.ListEmailStatus.Add(e));
                result.ListEmailTemplateToken.ForEach(e => response.ListEmailTemplateToken.Add(e));

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateUpdateEmailTemplateMasterdataResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public CreateUpdateEmailTemplateResponse CreateUpdateEmailTemplate(CreateUpdateEmailTemplateRequest request)
        {
            try
            {
                this.logger.LogInformation("CreateUpdateEmailTemplate");
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.CreateUpdateEmailTemplate(parameter);
                var response = new CreateUpdateEmailTemplateResponse
                {                   
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new CreateUpdateEmailTemplateResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchEmailConfigMasterdataResponse SearchEmailConfigMasterdata(SearchEmailConfigMasterdataRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Email Config Masterdata");
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.SearchEmailConfigMasterdata(parameter);
                var response = new SearchEmailConfigMasterdataResponse
                {
                    ListEmailStatus = new List<Models.Category.CategoryModel>(),
                    ListEmailType = new List<Models.Category.CategoryModel>(),
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };

                result.ListEmailType.ForEach(e => response.ListEmailType.Add(new Models.Category.CategoryModel(e)));
                result.ListEmailStatus.ForEach(e => response.ListEmailStatus.Add(new Models.Category.CategoryModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new SearchEmailConfigMasterdataResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchEmailTemplateResponse SearchEmailTemplate(SearchEmailTemplateRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Email Template");
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.SearchEmailTemplate(parameter);
                var response = new SearchEmailTemplateResponse
                {
                    ListEmailTemplateModel = result.ListEmailTemplate,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };
                
                return response;
            }
            catch (Exception e)
            {
                return new SearchEmailTemplateResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailResponse SendEmail(SendEmailRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email");
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.SendEmail(parameter);
                var response = new SendEmailResponse
                {
             
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Status ? "" : result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new SendEmailResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetTokenForEmailTypeIdResponse GetTokenForEmailTypeId(GetTokenForEmailTypeIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmailConfigurationDataAccess.GetTokenForEmailTypeId(parameter);
                var response = new GetTokenForEmailTypeIdResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmailTemplateToken = new List<EmailTemplateTokenEntityModel>()
                };

                result.ListEmailTemplateToken.ForEach(item =>
                {
                    response.ListEmailTemplateToken.Add(item);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetTokenForEmailTypeIdResponse()
                {
                    MessageCode = e.ToString(),
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
