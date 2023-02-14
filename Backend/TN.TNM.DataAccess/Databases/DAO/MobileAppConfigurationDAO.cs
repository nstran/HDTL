using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Entities;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class MobileAppConfigurationDAO : BaseDAO, IMobileAppConfigurationDataAccess
    {
        private readonly IMapper _mapper;

        public MobileAppConfigurationDAO(
            Databases.TNTN8Context _content,
            IMapper mapper
            )
        {
            this.context = _content;
            _mapper = mapper;
        }

        public CreateOrEditMobileAppConfigurationResult CreateOrEditMobileAppConfiguration(CreateOrEditMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = _mapper.Map<MobileAppConfiguration>(parameter.MobileAppConfigurationEntityModel);
                if (mobileAppConfiguration.Id != null && mobileAppConfiguration.Id != Guid.Empty)
                {
                    mobileAppConfiguration.UpdatedDate = DateTime.Now;
                    mobileAppConfiguration.UpdatedById = parameter.UserId;
                    context.MobileAppConfiguration.Update(mobileAppConfiguration);
                }
                else
                {
                    mobileAppConfiguration.Id = Guid.NewGuid();
                    mobileAppConfiguration.CreatedDate = DateTime.Now;
                    mobileAppConfiguration.CreatedById = parameter.UserId;
                    context.MobileAppConfiguration.Add(mobileAppConfiguration);
                }

                context.SaveChanges();
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfiguration(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                IntroduceColor = x.IntroduceColor,
                                                IntroduceImageOrVideo = x.IntroduceImageOrVideo,
                                                IntroduceSologan = x.IntroduceSologan,
                                                LoginAndRegisterScreenImage = x.LoginAndRegisterScreenImage,
                                                LoginScreenColor = x.LoginScreenColor,
                                                LoginScreenIcon = x.LoginScreenIcon,
                                                PaymentScreenContentTransfer = x.PaymentScreenContentTransfer,
                                                PaymentScreenContentVnpay = x.PaymentScreenContentVnpay,
                                                PaymentScreenIconTransfer = x.PaymentScreenIconTransfer,
                                                IsPaymentScreenIconTransfer = x.IsPaymentScreenIconTransfer,
                                                IsPaymentScreenIconVnpay = x.IsPaymentScreenIconVnpay,
                                                PaymentScreenIconVnpay = x.PaymentScreenIconVnpay,
                                                OrderNotificationImage = x.OrderNotificationImage
                                            }).FirstOrDefault();

                var paymentMethodCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PaymentMethod").CategoryTypeId;
                var listPayMentCategory = context.Category.Where(x => x.CategoryTypeId == paymentMethodCateTypeId)
                        .Select(y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName
                        }).OrderBy(z => z.CategoryName).ToList();
                var listPayMent = context.PaymentMethodConfigure.Select(x => new PaymentMethodConfigureEntityModel
                {
                    Id = x.Id,
                    Content = x.Content,
                    CategoryId = x.CategoryId,
                    Edit = false,
                    CategoryName = listPayMentCategory.FirstOrDefault(item => item.CategoryId == x.CategoryId).CategoryName,
                    CategoryObject = listPayMentCategory.FirstOrDefault(item => item.CategoryId == x.CategoryId),
                }).ToList();

                var listAdvertisementConfiguration = context.AdvertisementConfiguration
                                            .Select(x => new AdvertisementConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                Content = x.Content,
                                                Image = x.Image,
                                                Title = x.Title,
                                                SortOrder = x.SortOrder,
                                                Edit = false,
                                            }).ToList();

                return new TakeMobileAppConfigurationResult
                {
                    ListPayMentCategory = listPayMentCategory,
                    ListPayMent = listPayMent,
                    ListAdvertisementConfigurationEntityModel = listAdvertisementConfiguration.OrderBy(x => x.SortOrder).ToList(),
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationIntro(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                IntroduceColor = x.IntroduceColor,
                                                IntroduceImageOrVideo = x.IntroduceImageOrVideo,
                                                IntroduceSologan = x.IntroduceSologan
                                            }).FirstOrDefault();

                return new TakeMobileAppConfigurationResult
                {
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginScreen(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                LoginScreenColor = x.LoginScreenColor,
                                                LoginScreenIcon = x.LoginScreenIcon,
                                            }).FirstOrDefault();

                return new TakeMobileAppConfigurationResult
                {
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginAndRegister(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                LoginAndRegisterScreenImage = x.LoginAndRegisterScreenImage
                                            }).FirstOrDefault();

                return new TakeMobileAppConfigurationResult
                {
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationPayment(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                PaymentScreenContentTransfer = x.PaymentScreenContentTransfer,
                                                PaymentScreenContentVnpay = x.PaymentScreenContentVnpay,
                                                PaymentScreenIconTransfer = x.PaymentScreenIconTransfer,
                                                IsPaymentScreenIconTransfer = x.IsPaymentScreenIconTransfer,
                                                IsPaymentScreenIconVnpay = x.IsPaymentScreenIconVnpay,
                                                PaymentScreenIconVnpay = x.PaymentScreenIconVnpay 
                                            }).FirstOrDefault();

                return new TakeMobileAppConfigurationResult
                {
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeMobileAppConfigurationResult TakeMobileAppConfigurationNotificationImage(TakeMobileAppConfigurationParameter parameter)
        {
            try
            {
                var mobileAppConfiguration = context.MobileAppConfiguration
                                            .Select(x => new MobileAppConfigurationEntityModel
                                            {
                                                Id = x.Id,
                                                OrderNotificationImage = x.OrderNotificationImage
                                            }).FirstOrDefault();

                return new TakeMobileAppConfigurationResult
                {
                    MobileAppConfigurationEntityModel = mobileAppConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdatePaymentMethodResult CreateOrUpdatePaymentMethod(CreateOrUpdatePaymentMethodParameter parameter)
        {
            try
            {
                var payment = parameter.Payment;
                var listAllPayMent = context.PaymentMethodConfigure.ToList();
                //Tạo
                if (parameter.Payment.Id == null || parameter.Payment.Id == Guid.Empty)
                {
                    var newPayment = _mapper.Map<PaymentMethodConfigure>(parameter.Payment);
                    var checkUsed = listAllPayMent.FirstOrDefault(x => x.CategoryId == newPayment.CategoryId);
                    if (checkUsed != null)
                    {
                        return new CreateOrUpdatePaymentMethodResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Phương thức thanh toán đã được cấu hình. Không thể thêm mới!"
                        };
                    }
                    newPayment.Id = Guid.NewGuid();
                    newPayment.CreatedById = parameter.UserId;
                    newPayment.UpdatedById = parameter.UserId;
                    newPayment.CreatedDate = DateTime.Now;
                    newPayment.UpdatedDate = DateTime.Now;
                    payment.Id = newPayment.Id;
                    context.PaymentMethodConfigure.Add(newPayment);
                }
                //Cập nhật
                else
                {
                    var exist = context.PaymentMethodConfigure.FirstOrDefault(x => x.Id == parameter.Payment.Id);
                    if (exist == null)
                    {
                        return new CreateOrUpdatePaymentMethodResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy cấu hình trên hệ thống!"
                        };
                    }

                    var checkUsed = listAllPayMent.FirstOrDefault(x => x.CategoryId == parameter.Payment.CategoryId.Value && x.Id != exist.Id);
                    if (checkUsed != null)
                    {
                        return new CreateOrUpdatePaymentMethodResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Phương thức thanh toán đã được cấu hình. Không thể thêm mới!"
                        };
                    }
                    exist.CategoryId = parameter.Payment.CategoryId.Value;
                    exist.Content = parameter.Payment.Content;
                    exist.CreatedDate = DateTime.Now;
                    exist.UpdatedDate = DateTime.Now;
                    context.PaymentMethodConfigure.Update(exist);
                }

                context.SaveChanges();


                return new CreateOrUpdatePaymentMethodResult
                {
                    Payment = payment,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdatePaymentMethodResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdatePaymentMethodResult DeletePaymentMethod(CreateOrUpdatePaymentMethodParameter parameter)
        {
            try
            {
                var exist = context.PaymentMethodConfigure.FirstOrDefault(x => x.Id == parameter.Payment.Id);
                if (exist == null)
                {
                    return new CreateOrUpdatePaymentMethodResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy cấu hình trên hệ thống!"
                    };
                }
                //Kiểm tra xem đã có phiếu nào sử dụng chưa
                var checkUsed = context.CustomerOrder.FirstOrDefault(x => x.PaymentMethod == parameter.Payment.Id);
                if (checkUsed != null)
                {
                    return new CreateOrUpdatePaymentMethodResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Phương thức thanh toán đã được sử dụng, không thể xóa!"
                    };
                }
                context.PaymentMethodConfigure.Remove(exist);
                context.SaveChanges();
                return new CreateOrUpdatePaymentMethodResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công!"
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdatePaymentMethodResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrEditMobileAppConfigurationResult CreateOrEditAdvertisementConfiguration(CreateOrEditAdvertisementConfigurationParameter parameter)
        {
            try
            {
      

                var advertisementConfiguration = _mapper.Map<AdvertisementConfiguration>(parameter.AdvertisementConfigurationEntityModel);
                if (advertisementConfiguration.Id != null && advertisementConfiguration.Id != Guid.Empty)
                {
                    advertisementConfiguration.UpdatedDate = DateTime.Now;
                    advertisementConfiguration.UpdatedById = parameter.UserId;
                    context.AdvertisementConfiguration.Update(advertisementConfiguration);
                }
                else
                {
                    var exits = context.AdvertisementConfiguration.FirstOrDefault(x => x.SortOrder == parameter.AdvertisementConfigurationEntityModel.SortOrder);
                    if (exits != null)
                    {
                        return new CreateOrEditMobileAppConfigurationResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Trùng số thứ tự"
                        };
                    }
                    advertisementConfiguration.Id = Guid.NewGuid();
                    advertisementConfiguration.CreatedDate = DateTime.Now;
                    advertisementConfiguration.CreatedById = parameter.UserId;
                    context.AdvertisementConfiguration.Add(advertisementConfiguration);
                }

                context.SaveChanges();
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrEditMobileAppConfigurationResult DeleteAdvertisementConfiguration(DeleteAdvertisementConfigurationParameter parameter)
        {
            try
            {
                var advertisementConfiguration = context.AdvertisementConfiguration.FirstOrDefault(x => x.Id == parameter.Id);
                if (advertisementConfiguration != null)
                {
                    context.AdvertisementConfiguration.Remove(advertisementConfiguration);
                    context.SaveChanges();
                }
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công!"
                };
            }
            catch (Exception e)
            {
                return new CreateOrEditMobileAppConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TakeListAdvertisementConfigurationResult TakeListAdvertisementConfiguration(TakeListAdvertisementConfigurationParameter parameter)
        {
            try
            {
                var listAdvertisementConfiguration = context.AdvertisementConfiguration
                            .Select(x => new AdvertisementConfigurationEntityModel
                            {
                                Id = x.Id,
                                Content = x.Content,
                                Image = x.Image,
                                Title = x.Title,
                                SortOrder = x.SortOrder
                            }).ToList();

                return new TakeListAdvertisementConfigurationResult
                {
                    ListAdvertisementConfigurationEntityModel = listAdvertisementConfiguration,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công!"
                };
            }
            catch (Exception e)
            {
                return new TakeListAdvertisementConfigurationResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
