using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Xml;
using System.IO;
using System.Net;
using System;
using TN.TNM.BusinessLogic.Interfaces.Admin.Product;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.BusinessLogic.Messages.Responses.File;
using TN.TNM.BusinessLogic.Messages.Requests.File;
using Microsoft.AspNetCore.Hosting;

namespace TN.TNM.Api.Controllers
{
    public class MobileAppConfigurationController : Controller
    {
        private readonly IMobileAppConfigurationDataAccess iMobileAppConfigurationDataAccess;
        private IHostingEnvironment _hostingEnvironment;

        public MobileAppConfigurationController(IMobileAppConfigurationDataAccess _iMobileAppConfigurationDataAccess, IHostingEnvironment hostingEnvironment)
        {
            this.iMobileAppConfigurationDataAccess = _iMobileAppConfigurationDataAccess;
            _hostingEnvironment = hostingEnvironment;
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
        [AllowAnonymous]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationIntro(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationIntro(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationLoginScreen")]
        [AllowAnonymous]
        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginScreen(TakeMobileAppConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeMobileAppConfigurationLoginScreen(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/takeMobileAppConfigurationLoginAndRegister")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public TakeListAdvertisementConfigurationResult TakeListAdvertisementConfiguration([FromBody] TakeListAdvertisementConfigurationParameter request)
        {
            return this.iMobileAppConfigurationDataAccess.TakeListAdvertisementConfiguration(request);
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/uploadMobileAppConfigurationImage")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadMobileAppConfigurationImage(UploadFileRequest request)
        {
            try
            {
                if (request.FileList != null && request.FileList.Count > 0)
                {
                    string folderName = "MobileAppConfigurationImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    foreach (IFormFile item in request.FileList)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = item.FileName.Trim();
                            string fullPath = Path.Combine(newPath, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                        }
                    }

                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                    };
                }
                else
                {
                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.FileUpload.NO_FILE
                    };
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        [HttpPost]
        [Route("api/MobileAppConfiguration/uploadAdvertisementConfigurationImage")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadAdvertisementConfigurationImage(UploadFileRequest request)
        {
            try
            {
                if (request.FileList != null && request.FileList.Count > 0)
                {
                    string folderName = "AdvertisementConfigurationImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    foreach (IFormFile item in request.FileList)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = item.FileName.Trim();
                            string fullPath = Path.Combine(newPath, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                        }
                    }

                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                    };
                }
                else
                {
                    return new UploadFileResponse()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.FileUpload.NO_FILE
                    };
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
    }
}
