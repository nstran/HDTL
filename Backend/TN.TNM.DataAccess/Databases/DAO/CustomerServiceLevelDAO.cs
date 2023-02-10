using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerServiceLevelDAO : BaseDAO, ICustomerServiceLevelDataAccess
    {
        public CustomerServiceLevelDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetConfigCustomerServiceLevelResult GetConfigCustomerServiceLevel(GetConfigCustomerServiceLevelParameter parameter)
        {
            try
            {
                var listCustomerConfig = context.CustomerServiceLevel.OrderBy(x => x.MinimumSaleValue).ToList();
                var list = new List<CustomerServiceLevelEntityModel>();
                listCustomerConfig.ForEach(item =>
                {
                    var newItem = new CustomerServiceLevelEntityModel(item);
                    list.Add(newItem);
                });

                return new GetConfigCustomerServiceLevelResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    CustomerServiceLevel = list
                };
            }
            catch (Exception)
            {
                return new GetConfigCustomerServiceLevelResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
            
        }

        public AddLevelCustomerResult AddLevelCustomer(AddLevelCustomerParameter parameter)
        {
            try
            {
                var list = new List<CustomerServiceLevel>();
                parameter.CustomerServiceLevel.ForEach(x =>
                {
                    var newItem = x.ToEntity();
                    newItem.CustomerServiceLevelId = Guid.NewGuid();
                    newItem.Active = true;
                    newItem.CreatedById = parameter.UserId;
                    newItem.CreatedDate = DateTime.Now;

                    list.Add(newItem);
                });

                context.CustomerServiceLevel.AddRange(list);
                context.SaveChanges();

                return new AddLevelCustomerResult
                {
                    MessageCode = "Thêm mới thành công",
                    StatusCode = HttpStatusCode.OK,
                    Message = CommonMessage.CustomerServiceLevel.ADD_SUCCESS,
                };
            }
            catch (Exception e)
            {
                return new AddLevelCustomerResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
            
        }

        public UpdateConfigCustomerResults UpdateConfigCustomer(UpdateConfigCustomerParameter parameter)
        {
            try
            {
                var customerConfig =
                context.CustomerServiceLevel.FirstOrDefault(c => c.CustomerServiceLevelId == parameter.CustomerLevelId);

                if (customerConfig == null)
                {
                    return new UpdateConfigCustomerResults()
                    {
                        MessageCode = "Bản ghi không tồn tại trên hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed,
                    };
                }

                context.CustomerServiceLevel.Remove(customerConfig);
                context.SaveChanges();

                return new UpdateConfigCustomerResults
                {
                    MessageCode = "Xóa thành công",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new UpdateConfigCustomerResults()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }
    }
}
