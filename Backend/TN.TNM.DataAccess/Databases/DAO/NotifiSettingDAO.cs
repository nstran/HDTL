using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class NotifiSettingDAO : BaseDAO, INotifiSettingDataAccess
    {
        public NotifiSettingDAO(TNTN8Context _content)
        {
            this.context = _content;
        }

        public GetMasterDataNotifiSettingCreateResult GetMasterDataNotifiSettingCreate(
            GetMasterDataNotifiSettingCreateParameter parameter)
        {
            try
            {
                var listScreen = new List<ScreenEntityModel>();
                var listInforScreen = new List<InforScreenEntityModel>();
                var listNotifiAction = new List<NotifiActionEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listNotifiSettingToken = new List<NotifiSettingTokenEntityModel>();

                listScreen = context.Screen.Select(y => new ScreenEntityModel
                {
                    ScreenId = y.ScreenId,
                    ScreenCode = y.ScreenCode,
                    ScreenName = y.ScreenName
                }).OrderBy(z => z.ScreenCode).ToList();

                listNotifiAction = context.NotifiAction.Select(y => new NotifiActionEntityModel
                {
                    NotifiActionId = y.NotifiActionId,
                    NotifiActionCode = y.NotifiActionCode,
                    NotifiActionName = y.NotifiActionName,
                    ScreenId = y.ScreenId
                }).ToList();

                listInforScreen = context.InforScreen.Select(y => new InforScreenEntityModel
                {
                    InforScreenId = y.InforScreenId,
                    InforScreenCode = y.InforScreenCode,
                    InforScreenName = y.InforScreenName,
                    IsDateTime = y.IsDateTime,
                    ScreenId = y.ScreenId
                }).ToList();

                listEmployee = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                listNotifiSettingToken = context.NotifiSettingToken.Select(y => new NotifiSettingTokenEntityModel
                {
                    NotifiSettingTokenId = y.NotifiSettingTokenId,
                    TokenCode = y.TokenCode,
                    TokenLabel = y.TokenLabel,
                    ScreenId = y.ScreenId
                }).ToList();

                return new GetMasterDataNotifiSettingCreateResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListScreen = listScreen,
                    ListInforScreen = listInforScreen,
                    ListNotifiAction = listNotifiAction,
                    ListEmployee = listEmployee,
                    ListNotifiSettingToken = listNotifiSettingToken,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataNotifiSettingCreateResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNotifiSettingResult CreateNotifiSetting(CreateNotifiSettingParameter parameter)
        {
            try
            {
                #region Mỗi màn hình và Sự kiện chỉ có một setting

                var hasNotifiSetting = context.NotifiSetting.FirstOrDefault(x =>
                    x.ScreenId == parameter.NotifiSetting.ScreenId &&
                    x.NotifiActionId == parameter.NotifiSetting.NotifiActionId);

                if (hasNotifiSetting != null)
                {
                    return new CreateNotifiSettingResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đã tồn tại cấu hình cho Màn hình và Sự kiện này",
                    };
                }

                #endregion

                var notifiSetting = parameter.NotifiSetting.ToEntity();
                notifiSetting.NotifiSettingId = Guid.NewGuid();
                notifiSetting.CreatedById = parameter.UserId;
                notifiSetting.CreatedDate = DateTime.Now;

                context.NotifiSetting.Add(notifiSetting);

                var listNotifiSpecial = new List<NotifiSpecial>();
                parameter.ListNotifiSpecial.ForEach(item =>
                {
                    var notifiSpecial = new NotifiSpecial();
                    notifiSpecial.NotifiSpecialId = Guid.NewGuid();
                    notifiSpecial.NotifiSettingId = notifiSetting.NotifiSettingId;
                    notifiSpecial.EmployeeId = item.EmployeeId;

                    listNotifiSpecial.Add(notifiSpecial);
                });

                context.NotifiSpecial.AddRange(listNotifiSpecial);
                context.SaveChanges();

                return new CreateNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    NotifiSettingId = notifiSetting.NotifiSettingId
                };
            }
            catch (Exception e)
            {
                return new CreateNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataNotifiSettingDetailResult GetMasterDataNotifiSettingDetail(
            GetMasterDataNotifiSettingDetailParameter parameter)
        {
            try
            {
                #region Lấy master data

                var listScreen = new List<ScreenEntityModel>();
                var listInforScreen = new List<InforScreenEntityModel>();
                var listNotifiAction = new List<NotifiActionEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listNotifiSettingToken = new List<NotifiSettingTokenEntityModel>();

                listScreen = context.Screen.Select(y => new ScreenEntityModel
                {
                    ScreenId = y.ScreenId,
                    ScreenCode = y.ScreenCode,
                    ScreenName = y.ScreenName
                }).OrderBy(z => z.ScreenCode).ToList();

                listNotifiAction = context.NotifiAction.Select(y => new NotifiActionEntityModel
                {
                    NotifiActionId = y.NotifiActionId,
                    NotifiActionCode = y.NotifiActionCode,
                    NotifiActionName = y.NotifiActionName,
                    ScreenId = y.ScreenId
                }).ToList();

                listInforScreen = context.InforScreen.Select(y => new InforScreenEntityModel
                {
                    InforScreenId = y.InforScreenId,
                    InforScreenCode = y.InforScreenCode,
                    InforScreenName = y.InforScreenName,
                    IsDateTime = y.IsDateTime,
                    ScreenId = y.ScreenId
                }).ToList();

                listEmployee = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                listNotifiSettingToken = context.NotifiSettingToken.Select(y => new NotifiSettingTokenEntityModel
                {
                    NotifiSettingTokenId = y.NotifiSettingTokenId,
                    TokenCode = y.TokenCode,
                    TokenLabel = y.TokenLabel,
                    ScreenId = y.ScreenId
                }).ToList();

                #endregion

                var notifiSetting =
                    context.NotifiSetting.Where(x => x.NotifiSettingId == parameter.NotifiSettingId).Select(y =>
                        new NotifiSettingEntityModel
                        {
                            NotifiSettingId = y.NotifiSettingId,
                            NotifiSettingName = y.NotifiSettingName,
                            ScreenId = y.ScreenId,
                            NotifiActionId = y.NotifiActionId,
                            IsApproved = y.IsApproved,
                            IsParticipant = y.IsParticipant,
                            IsCreated = y.IsCreated,
                            IsPersonIncharge = y.IsPersonIncharge,
                            SendInternal = y.SendInternal,
                            BackHourInternal = y.BackHourInternal,
                            IsSystem = y.IsSystem,
                            SystemTitle = y.SystemTitle,
                            SystemContent = y.SystemContent,
                            IsEmail = y.IsEmail,
                            EmailTitle = y.EmailTitle,
                            EmailContent = y.EmailContent,
                            IsSms = y.IsSms,
                            SmsTitle = y.SmsTitle,
                            SmsContent = y.SmsContent,
                            SendCustomer = y.SendCustomer,
                            BackHourCustomer = y.BackHourCustomer,
                            IsCustomerEmail = y.IsCustomerEmail,
                            CustomerEmailTitle = y.CustomerEmailTitle,
                            CustomerEmailContent = y.CustomerEmailContent,
                            IsCustomerSms = y.IsCustomerSms,
                            CustomerSmsTitle = y.CustomerSmsTitle,
                            CustomerSmsContent = y.CustomerSmsContent,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ObjectBackHourInternal = y.ObjectBackHourInternal,
                            ObjectBackHourCustomer = y.ObjectBackHourCustomer
                        }).FirstOrDefault();

                if (notifiSetting == null)
                {
                    return new GetMasterDataNotifiSettingDetailResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo không tồn tại trên hệ thống"
                    };
                }

                var listNotifiSpecial = new List<NotifiSpecialEntityModel>();

                listNotifiSpecial = context.NotifiSpecial.Where(x => x.NotifiSettingId == parameter.NotifiSettingId)
                    .Select(y => new NotifiSpecialEntityModel
                    {
                        NotifiSpecialId = y.NotifiSpecialId,
                        NotifiSettingId = y.NotifiSettingId,
                        EmployeeId = y.EmployeeId
                    }).ToList();

                return new GetMasterDataNotifiSettingDetailResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListScreen = listScreen,
                    ListInforScreen = listInforScreen,
                    ListNotifiAction = listNotifiAction,
                    ListEmployee = listEmployee,
                    NotifiSetting = notifiSetting,
                    ListNotifiSpecial = listNotifiSpecial,
                    ListNotifiSettingToken = listNotifiSettingToken
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataNotifiSettingDetailResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public UpdateNotifiSettingResult UpdateNotifiSetting(UpdateNotifiSettingParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x =>
                        x.NotifiSettingId == parameter.NotifiSetting.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new UpdateNotifiSettingResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo không tồn tại trên hệ thống"
                    };
                }

                notifiSetting.Active = parameter.NotifiSetting.Active;
                notifiSetting.NotifiSettingName = parameter.NotifiSetting.NotifiSettingName;
                notifiSetting.ScreenId = parameter.NotifiSetting.ScreenId;
                notifiSetting.NotifiActionId = parameter.NotifiSetting.NotifiActionId;
                notifiSetting.IsApproved = parameter.NotifiSetting.IsApproved;
                notifiSetting.IsParticipant = parameter.NotifiSetting.IsParticipant;
                notifiSetting.IsCreated = parameter.NotifiSetting.IsCreated;
                notifiSetting.IsPersonIncharge = parameter.NotifiSetting.IsPersonIncharge;
                notifiSetting.SendInternal = parameter.NotifiSetting.SendInternal;
                notifiSetting.ObjectBackHourInternal = parameter.NotifiSetting.ObjectBackHourInternal;
                notifiSetting.BackHourInternal = parameter.NotifiSetting.BackHourInternal;
                notifiSetting.IsEmail = parameter.NotifiSetting.IsEmail;
                notifiSetting.EmailTitle = parameter.NotifiSetting.EmailTitle;
                notifiSetting.EmailContent = parameter.NotifiSetting.EmailContent;
                notifiSetting.UpdatedById = parameter.UserId;
                notifiSetting.UpdatedDate = DateTime.Now;

                context.NotifiSetting.Update(notifiSetting);

                var listOldNotifiSpecial = context.NotifiSpecial
                    .Where(x => x.NotifiSettingId == parameter.NotifiSetting.NotifiSettingId).ToList();
                context.NotifiSpecial.RemoveRange(listOldNotifiSpecial);

                var listNewNotifiSpecial = new List<NotifiSpecial>();
                parameter.ListNotifiSpecial.ForEach(item =>
                {
                    var notifiSpecial = new NotifiSpecial();
                    notifiSpecial.NotifiSpecialId = Guid.NewGuid();
                    notifiSpecial.NotifiSettingId = parameter.NotifiSetting.NotifiSettingId;
                    notifiSpecial.EmployeeId = item.EmployeeId;

                    listNewNotifiSpecial.Add(notifiSpecial);
                });

                context.NotifiSpecial.AddRange(listNewNotifiSpecial);
                context.SaveChanges();

                return new UpdateNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new UpdateNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchNotifiSettingResult GetMasterDataSearchNotifiSetting(
            GetMasterDataSearchNotifiSettingParameter parameter)
        {
            try
            {
                var listScreen = new List<ScreenEntityModel>();
                listScreen = context.Screen.Select(y => new ScreenEntityModel
                {
                    ScreenId = y.ScreenId,
                    ScreenCode = y.ScreenCode,
                    ScreenName = y.ScreenName
                }).ToList();

                return new GetMasterDataSearchNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListScreen = listScreen
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public SearchNotifiSettingResult SearchNotifiSetting(SearchNotifiSettingParameter parameter)
        {
            try
            {
                var listNotifiSetting = new List<NotifiSettingEntityModel>();

                var listScreenId = parameter.ListScreen.Select(y => y.ScreenId).ToList();

                listNotifiSetting = context.NotifiSetting
                    .Where(x => (parameter.NotifiSettingName == null || parameter.NotifiSettingName == "" ||
                                 x.NotifiSettingName.Contains(parameter.NotifiSettingName)) &&
                                (listScreenId.Count == 0 || listScreenId.Contains(x.ScreenId.Value))).Select(y =>
                        new NotifiSettingEntityModel
                        {
                            NotifiSettingId = y.NotifiSettingId,
                            NotifiSettingName = y.NotifiSettingName,
                            Active = y.Active,
                            SendInternal = y.SendInternal,
                            IsSystem = y.IsSystem,
                            IsEmail = y.IsEmail,
                            IsSms = y.IsSms,
                            BackHourInternal = y.BackHourInternal,
                            ObjectBackHourInternal = y.ObjectBackHourInternal,
                            ObjectBackHourInternalName = ""
                        }).OrderBy(z => z.NotifiSettingName).ToList();

                var listAllInforScreen = context.InforScreen.ToList();

                listNotifiSetting.ForEach(item =>
                {
                    var inforScreen =
                        listAllInforScreen.FirstOrDefault(x => x.InforScreenId == item.ObjectBackHourInternal);

                    item.ObjectBackHourInternalName = inforScreen?.InforScreenName;
                });

                return new SearchNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListNotifiSetting = listNotifiSetting
                };
            }
            catch (Exception e)
            {
                return new SearchNotifiSettingResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeBackHourInternalResult ChangeBackHourInternal(ChangeBackHourInternalParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeBackHourInternalResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.BackHourInternal = parameter.BackHourInternal;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeBackHourInternalResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeBackHourInternalResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeActiveResult ChangeActive(ChangeActiveParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeActiveResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.Active = parameter.Active;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeActiveResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeActiveResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeSendInternalResult ChangeSendInternal(ChangeSendInternalParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeSendInternalResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.SendInternal = parameter.SendInternal;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeSendInternalResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeSendInternalResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeIsSystemResult ChangeIsSystem(ChangeIsSystemParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeIsSystemResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.IsSystem = parameter.IsSystem;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeIsSystemResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeIsSystemResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeIsEmailResult ChangeIsEmail(ChangeIsEmailParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeIsEmailResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.IsEmail = parameter.IsEmail;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeIsEmailResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeIsEmailResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeIsSmsResult ChangeIsSms(ChangeIsSmsParameter parameter)
        {
            try
            {
                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.NotifiSettingId == parameter.NotifiSettingId);

                if (notifiSetting == null)
                {
                    return new ChangeIsSmsResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình thông báo này không tồn tại trên hệ thống",
                    };
                }

                notifiSetting.IsSms = parameter.IsSms;
                notifiSetting.UpdatedDate = DateTime.Now;
                notifiSetting.UpdatedById = parameter.UserId;

                context.NotifiSetting.Update(notifiSetting);
                context.SaveChanges();

                return new ChangeIsSmsResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeIsSmsResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
    }
}
