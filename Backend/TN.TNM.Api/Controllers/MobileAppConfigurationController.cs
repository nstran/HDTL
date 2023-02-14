using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Xml;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;

namespace TN.TNM.Api.Controllers
{
    public class MobileAppConfigurationController : Controller
    {
        private readonly IMobileAppConfigurationDataAccess iMobileAppConfigurationDataAccess;

        public MobileAppConfigurationController(IMobileAppConfigurationDataAccess _iMobileAppConfigurationDataAccess)
        {
            this.iMobileAppConfigurationDataAccess = _iMobileAppConfigurationDataAccess;
        }

        /// <summary>
        /// search product
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MobileAppConfiguration/createOrEditMobileAppConfiguration")]
        [Authorize(Policy = "Member")]
        public CreateOrEditMobileAppConfigurationResult CreateOrEditMobileAppConfiguration([FromBody] CreateOrEditMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.CreateOrEditMobileAppConfiguration(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfiguration")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfiguration(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfiguration(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationIntro")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationIntro(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationIntro(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationLoginScreen")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginScreen(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationLoginScreen(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationLoginAndRegister")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginAndRegister(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationLoginAndRegister(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationPayment")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationPayment(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationPayment(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationNotificationImage")]
        [Authorize(Policy = "Member")]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationNotificationImage(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationNotificationImage(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/createOrUpdatePaymentMethod")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdatePaymentMethodResult CreateOrUpdatePaymentMethod([FromBody] CreateOrUpdatePaymentMethodParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.CreateOrUpdatePaymentMethod(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/deletePaymentMethod")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdatePaymentMethodResult DeletePaymentMethod([FromBody] CreateOrUpdatePaymentMethodParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.DeletePaymentMethod(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/createOrEditAdvertisementConfiguration")]
        [Authorize(Policy = "Member")]
        public CreateOrEditMobileAppConfigurationResult CreateOrEditAdvertisementConfiguration([FromBody] CreateOrEditAdvertisementConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.CreateOrEditAdvertisementConfiguration(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/deleteAdvertisementConfiguration")]
        [Authorize(Policy = "Member")]
        public CreateOrEditMobileAppConfigurationResult DeleteAdvertisementConfiguration([FromBody] DeleteAdvertisementConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.DeleteAdvertisementConfiguration(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeListAdvertisementConfiguration")]
        [Authorize(Policy = "Member")]
        public TakeListAdvertisementConfigurationResult TakeListAdvertisementConfiguration([FromBody] TakeListAdvertisementConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeListAdvertisementConfiguration(request);
        }
    }
}
