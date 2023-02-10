using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Dashboard;
using TN.TNM.DataAccess.Messages.Results.Admin.Dashboard;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DashboardDAO : BaseDAO, IDashboardDataAccess
    {
        public DashboardDAO(
            Databases.TNTN8Context _content
        )
        {
            this.context = _content;
        }

        private List<CustomerOrderEntityModel> TakeListCustomerOrderEntityModel()
        {
            var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();
            var listCustomerOrder = context.CustomerOrder
                                    .Where(x => x.StatusOrder == 4 || x.StatusOrder == 5)
                                    .Select(x => new CustomerOrderEntityModel
                                    {
                                        OrderId = x.OrderId,
                                        OrderCode = x.OrderCode,
                                        CustomerId = x.CustomerId,
                                        StatusOrder = x.StatusOrder,
                                        CreatedDate = x.CreatedDate,
                                        CreatedById = x.CreatedById,
                                        UpdatedDate = x.UpdatedDate,
                                        DiscountType = x.DiscountType,
                                        DiscountValue = x.DiscountValue,
                                        Vat = x.Vat,
                                        OrderTypeName = listOrderType.FirstOrDefault(y => y.Value == x.OrderType).Name,
                                    }).ToList();

            var listOrderId = listCustomerOrder.Select(x => x.OrderId).ToList();
            var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();
            var listCustomerOrderDetailExten = context.CustomerOrderDetailExten.Where(x => listOrderId.Contains(x.OrderId)).ToList();
            listCustomerOrder.ForEach(item =>
            {
                #region
                //Tổng tiền trước thuế
                decimal? totalCostBeforeTax = 0;
                //Tổng tiền thuế
                decimal? totalCostTax = 0;
                //Tổng tiền chiết khấu
                decimal? totalDiscountCost = 0;
                // Tổng tiền phải trả
                decimal? totalCostPay = 0;

                //Tùy chọn của phiếu yêu cầu
                var listOrderDetailCurrent = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();
                listOrderDetailCurrent.ForEach(currentDetail =>
                {
                    totalCostBeforeTax += currentDetail.Quantity.Value * currentDetail.PriceInitial.Value;
                    totalCostTax += currentDetail.Quantity * currentDetail.PriceInitial / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                });

                //Nếu có phát sinh
                var listOderDetailExtenCurrent = listCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).ToList();
                if (listOderDetailExtenCurrent.Count > 0 && item.StatusOrder != 1)
                {
                    listOderDetailExtenCurrent.ForEach(extenCur =>
                    {
                        if (extenCur.Price != null && extenCur.Status == 2)
                        {
                            totalCostBeforeTax += extenCur.Quantity * extenCur.Price;
                            totalCostTax += extenCur.Quantity * extenCur.Price / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                        }
                    });
                }

                //Chiết khấu theo %
                // false: theo %, true: Theo số tiền
                if (!item.DiscountType.Value)
                {
                    totalDiscountCost = totalCostBeforeTax * item.DiscountValue / 100;
                }
                //Chiết khấu theo số tiền
                else if (item.DiscountType.Value)
                {
                    totalDiscountCost = item.DiscountValue;
                }
                totalCostPay = totalCostBeforeTax + totalCostTax - totalDiscountCost;

                item.Amount = totalCostPay;
                #endregion
            });

            return listCustomerOrder;
        }

        public TakeRevenueStatisticDashboardResult TakeRevenueStatisticDashboard(TakeRevenueStatisticDashboardParameter parameter)
        {
            try
            {
                var listCustomerOrder = TakeListCustomerOrderEntityModel();

                DateTime today = DateTime.Today;
                DateTime monday = DateTime.Today; while (monday.DayOfWeek != DayOfWeek.Monday) monday = monday.AddDays(-1);
                DateTime sunday = DateTime.Today.AddDays(7); while (sunday.DayOfWeek != DayOfWeek.Sunday) sunday = sunday.AddDays(-1);
                DateTime firstDayMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastDayMonth = firstDayMonth.AddMonths(1).AddTicks(-1);
                DateTime firstDayOfQuarter = new DateTime(DateTime.Now.Year, (((DateTime.Now.Month - 1) / 3 + 1) - 1) * 3 + 1, 1);
                DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddTicks(-1);
                DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime lastDayOfYear = new DateTime(DateTime.Now.Year, 12, 31).AddDays(1).AddTicks(-1);

                var revenueStatisticModel = new RevenueStatisticModel();
                revenueStatisticModel.RevenueOfDay = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= today && x.UpdatedDate <= today.AddDays(1).AddTicks(-1)).Sum(x => x.Amount);
                revenueStatisticModel.RevenueOfWeek = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= monday && x.UpdatedDate <= sunday.AddDays(1).AddTicks(-1)).Sum(x => x.Amount);
                revenueStatisticModel.RevenueOfMonth = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayMonth && x.UpdatedDate <= lastDayMonth).Sum(x => x.Amount);
                revenueStatisticModel.RevenueOfQuarter = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayOfQuarter && x.UpdatedDate <= lastDayOfQuarter).Sum(x => x.Amount);
                revenueStatisticModel.RevenueOfYear = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayOfYear && x.UpdatedDate <= lastDayOfYear).Sum(x => x.Amount);
                revenueStatisticModel.RevenueWaitPayment = (decimal)listCustomerOrder.Where(x => x.StatusOrder == 4).Sum(x => x.Amount);

                revenueStatisticModel.RevenueOfYesterday = (decimal)(revenueStatisticModel.RevenueOfDay - listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= today.AddDays(-1) && x.UpdatedDate <= today.AddTicks(-1)).Sum(x => x.Amount));
                revenueStatisticModel.RevenueOfLastWeek = (decimal)(revenueStatisticModel.RevenueOfWeek - listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= monday.AddDays(-7) && x.UpdatedDate <= sunday.AddDays(-7).AddDays(1).AddTicks(-1)).Sum(x => x.Amount));
                revenueStatisticModel.RevenueOfLastMonth = (decimal)(revenueStatisticModel.RevenueOfMonth - listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayMonth.AddMonths(-1) && x.UpdatedDate <= lastDayMonth.AddMonths(-1)).Sum(x => x.Amount));
                revenueStatisticModel.RevenueOfLastQuarter = (decimal)(revenueStatisticModel.RevenueOfQuarter - listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayOfQuarter.AddMonths(-3) && x.UpdatedDate <= lastDayOfQuarter.AddMonths(-3)).Sum(x => x.Amount));
                revenueStatisticModel.RevenueOfLastYear = (decimal)(revenueStatisticModel.RevenueOfYear - listCustomerOrder.Where(x => x.StatusOrder == 5 && x.UpdatedDate >= firstDayOfYear.AddYears(-1) && x.UpdatedDate <= lastDayOfYear.AddYears(-1)).Sum(x => x.Amount));

                return new TakeRevenueStatisticDashboardResult
                {
                    RevenueStatisticModel = revenueStatisticModel,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new TakeRevenueStatisticDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = ex.Message
                };
            }
        }

        public TakeRevenueStatisticWaitPaymentDashboardResult TakeRevenueStatisticWaitPaymentDashboard(TakeRevenueStatisticDashboardParameter parameter)
        {
            try
            {
                var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();
                var listCustomerOrder = (from cu in context.CustomerOrder
                                         join s in context.ServicePacket on cu.ServicePacketId
                                         equals s.Id into sJoined
                                         from s in sJoined.DefaultIfEmpty()
                                         join c in context.ProductCategory on s.ProductCategoryId
                                         equals c.ProductCategoryId into csJoined
                                         from cs in csJoined.DefaultIfEmpty()
                                         where cu.StatusOrder == 4
                                         select new CustomerOrderEntityModel
                                         {
                                             OrderId = cu.OrderId,
                                             StatusOrder = cu.StatusOrder,
                                             CreatedDate = cu.CreatedDate,
                                             UpdatedDate = cu.UpdatedDate,
                                             DiscountType = cu.DiscountType,
                                             DiscountValue = cu.DiscountValue,
                                             Vat = cu.Vat,
                                             ServicePacketId = cu.ServicePacketId,
                                             OrderTypeName = listOrderType.FirstOrDefault(y => y.Value == cu.OrderType).Name,
                                             ProductCategoryName = cs.ProductCategoryName
                                         }).ToList();

                var listOrderId = listCustomerOrder.Select(x => x.OrderId).ToList();
                var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                var listCustomerOrderDetailExten = context.CustomerOrderDetailExten.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                listCustomerOrder.ForEach(item =>
                {
                    #region
                    //Tổng tiền trước thuế
                    decimal? totalCostBeforeTax = 0;
                    //Tổng tiền thuế
                    decimal? totalCostTax = 0;
                    //Tổng tiền chiết khấu
                    decimal? totalDiscountCost = 0;
                    // Tổng tiền phải trả
                    decimal? totalCostPay = 0;

                    //Tùy chọn của phiếu yêu cầu
                    var listOrderDetailCurrent = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();
                    listOrderDetailCurrent.ForEach(currentDetail =>
                    {
                        totalCostBeforeTax += currentDetail.Quantity.Value * currentDetail.PriceInitial.Value;
                        totalCostTax += currentDetail.Quantity * currentDetail.PriceInitial / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                    });

                    //Nếu có phát sinh
                    var listOderDetailExtenCurrent = listCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).ToList();
                    if (listOderDetailExtenCurrent.Count > 0 && item.StatusOrder != 1)
                    {
                        listOderDetailExtenCurrent.ForEach(extenCur =>
                        {
                            if (extenCur.Price != null && extenCur.Status == 2)
                            {
                                totalCostBeforeTax += extenCur.Quantity * extenCur.Price;
                                totalCostTax += extenCur.Quantity * extenCur.Price / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                            }
                        });
                    }

                    //Chiết khấu theo %
                    // false: theo %, true: Theo số tiền
                    if (!item.DiscountType.Value)
                    {
                        totalDiscountCost = totalCostBeforeTax * item.DiscountValue / 100;
                    }
                    //Chiết khấu theo số tiền
                    else if (item.DiscountType.Value)
                    {
                        totalDiscountCost = item.DiscountValue;
                    }
                    totalCostPay = totalCostBeforeTax + totalCostTax - totalDiscountCost;

                    item.Amount = totalCostPay;
                    #endregion
                });

                var listCustomerOrderGroup = listCustomerOrder.GroupBy(x => x.ProductCategoryName).ToList();
                var listRevenueStatisticWaitPaymentModel = new List<RevenueStatisticWaitPaymentModel>();
                foreach (var item in listCustomerOrderGroup)
                {
                    var revenueStatisticWaitPayment = new RevenueStatisticWaitPaymentModel();
                    revenueStatisticWaitPayment.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    revenueStatisticWaitPayment.Amount = listCustomerOrder.Where(x => x.ProductCategoryName == item.Key).Sum(x => x.Amount);
                    revenueStatisticWaitPayment.Percent = (item.Sum(x => x.Amount) / listCustomerOrder.Sum(x => x.Amount) * 100);
                    listRevenueStatisticWaitPaymentModel.Add(revenueStatisticWaitPayment);
                }

                var listServicePacket = context.ServicePacket
                                        .Select(x => new ServicePacketEntityModel
                                        {
                                            Id = x.Id,
                                            Name = x.Name
                                        });
                var listCustomerOrderGroupByServicePacketId = listCustomerOrder.GroupBy(x => x.ServicePacketId).ToList();
                var listRevenueStatisticWaitPaymentByServicePacketId = new List<RevenueStatisticWaitPaymentModel>();
                foreach (var item in listCustomerOrderGroupByServicePacketId)
                {
                    var revenueStatisticWaitPayment = new RevenueStatisticWaitPaymentModel();
                    revenueStatisticWaitPayment.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    revenueStatisticWaitPayment.PercentOfServicePacket = (item.Sum(x => x.Amount) / listRevenueStatisticWaitPaymentModel.Where(x => x.ProductCategoryName == item.FirstOrDefault().ProductCategoryName).FirstOrDefault().Amount) * 100;
                    revenueStatisticWaitPayment.Amount = item.Sum(x => x.Amount);
                    revenueStatisticWaitPayment.ServicePacketName = listServicePacket.Where(x => x.Id == item.FirstOrDefault().ServicePacketId).FirstOrDefault().Name;
                    listRevenueStatisticWaitPaymentByServicePacketId.Add(revenueStatisticWaitPayment);
                }

                foreach (var item in listRevenueStatisticWaitPaymentModel)
                {
                    item.ListRevenueStatisticWaitPaymentModel = listRevenueStatisticWaitPaymentByServicePacketId.Where(x => x.ProductCategoryName == item.ProductCategoryName).ToList();
                }

                return new TakeRevenueStatisticWaitPaymentDashboardResult
                {
                    ListRevenueStatisticWaitPaymentModel = listRevenueStatisticWaitPaymentModel,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeRevenueStatisticWaitPaymentDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

        public TakeStatisticServiceTicketDashboardResult TakeStatisticServiceTicketDashboard(TakeRevenueStatisticDashboardParameter parameter)
        {
            try
            {
                var listCustomerOrder = (from cu in context.OrderProcess
                                         join s in context.ServicePacket on cu.ServicePacketId equals s.Id into sJoined
                                         from s in sJoined.DefaultIfEmpty()
                                         join c in context.ProductCategory on s.ProductCategoryId equals c.ProductCategoryId into csJoined
                                         from cs in csJoined.DefaultIfEmpty()
                                         select new OrderProcessEntityModel
                                         {
                                             ServicePacketId = cu.ServicePacketId,
                                             Status = cu.Status,
                                             ProductCategoryName = cs.ProductCategoryName
                                         }).ToList();

                var listServicePacket = context.ServicePacket
                                        .Select(x => new ServicePacketEntityModel
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                        });
                var listCustomerOrderGroupByServicePacketId = listCustomerOrder.GroupBy(x => x.ServicePacketId).ToList();
                var listStatisticServicePacketNewStatus = new List<NewStatus>();
                var listStatisticServicePacketProgressStatus = new List<ProgressStatus>();
                var listStatisticServicePacketDoneStatus = new List<DoneStatus>();
                var listStatisticServicePacketCancelStatus = new List<CancelStatus>();
                foreach (var item in listCustomerOrderGroupByServicePacketId)
                {
                    var statisticServicePacketNewStatus = new NewStatus();
                    statisticServicePacketNewStatus.ServicePacketName = listServicePacket.Where(x => x.Id == item.Key).FirstOrDefault().Name;
                    statisticServicePacketNewStatus.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    statisticServicePacketNewStatus.Count = listCustomerOrder.Count(x => x.Status == 1 && x.ServicePacketId == item.Key);
                    listStatisticServicePacketNewStatus.Add(statisticServicePacketNewStatus);

                    var statisticServicePacketProgressStatus = new ProgressStatus();
                    statisticServicePacketProgressStatus.ServicePacketName = listServicePacket.Where(x => x.Id == item.Key).FirstOrDefault().Name;
                    statisticServicePacketProgressStatus.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    statisticServicePacketProgressStatus.Count = listCustomerOrder.Count(x => x.Status == 2 && x.ServicePacketId == item.Key);
                    listStatisticServicePacketProgressStatus.Add(statisticServicePacketProgressStatus);

                    var statisticServicePacketDoneStatus = new DoneStatus();
                    statisticServicePacketDoneStatus.ServicePacketName = listServicePacket.Where(x => x.Id == item.Key).FirstOrDefault().Name;
                    statisticServicePacketDoneStatus.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    statisticServicePacketDoneStatus.Count = listCustomerOrder.Count(x => x.Status == 3 && x.ServicePacketId == item.Key);
                    listStatisticServicePacketDoneStatus.Add(statisticServicePacketDoneStatus);

                    var statisticServicePacketCancelStatus= new CancelStatus();
                    statisticServicePacketCancelStatus.ServicePacketName = listServicePacket.Where(x => x.Id == item.Key).FirstOrDefault().Name;
                    statisticServicePacketCancelStatus.ProductCategoryName = item.FirstOrDefault().ProductCategoryName;
                    statisticServicePacketCancelStatus.Count = listCustomerOrder.Count(x => (x.Status == 4 || x.Status == 5 || x.Status == 6) && x.ServicePacketId == item.Key);
                    listStatisticServicePacketCancelStatus.Add(statisticServicePacketCancelStatus);
                }

                var listCustomerOrderGroup = listCustomerOrder.GroupBy(x => x.ProductCategoryName);
                var listNewStatus = new List<NewStatus>();
                var listProgressStatus = new List<ProgressStatus>();
                var listDoneStatus = new List<DoneStatus>();
                var listCancelStatus = new List<CancelStatus>();

                foreach (var item in listCustomerOrderGroup)
                {
                    var newStatus = new NewStatus();
                    newStatus.Count = listCustomerOrder.Count(x => x.Status == 1 && x.ProductCategoryName == item.Key);
                    newStatus.ProductCategoryName = item.Key;
                    newStatus.ListStatisticServicePacketNewStatus = listStatisticServicePacketNewStatus.Where(x => x.ProductCategoryName == item.Key).ToList();
                    listNewStatus.Add(newStatus);

                    var progressStatus = new ProgressStatus();
                    progressStatus.Count = listCustomerOrder.Count(x => x.Status == 2 && x.ProductCategoryName == item.Key);
                    progressStatus.ProductCategoryName = item.Key;
                    progressStatus.ListStatisticServicePacketProgressStatus = listStatisticServicePacketProgressStatus.Where(x => x.ProductCategoryName == item.Key).ToList();
                    listProgressStatus.Add(progressStatus);

                    var doneStatus = new DoneStatus();
                    doneStatus.Count = listCustomerOrder.Count(x => x.Status == 3 && x.ProductCategoryName == item.Key);
                    doneStatus.ProductCategoryName = item.Key;
                    doneStatus.ListStatisticServicePacketDoneStatus = listStatisticServicePacketDoneStatus.Where(x => x.ProductCategoryName == item.Key).ToList();
                    listDoneStatus.Add(doneStatus);

                    var cancelStatus = new CancelStatus();
                    cancelStatus.Count = listCustomerOrder.Count(x => (x.Status == 4 || x.Status == 5 || x.Status == 6) && x.ProductCategoryName == item.Key);
                    cancelStatus.ProductCategoryName = item.Key;
                    cancelStatus.ListStatisticServicePacketCancelStatus = listStatisticServicePacketCancelStatus.Where(x => x.ProductCategoryName == item.Key).ToList();
                    listCancelStatus.Add(cancelStatus);
                }

                return new TakeStatisticServiceTicketDashboardResult
                {
                    ListNewStatus = listNewStatus,
                    ListProgressStatus = listProgressStatus,
                    ListDoneStatus = listDoneStatus,
                    ListCancelStatus = listCancelStatus,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeStatisticServiceTicketDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

        public TakeRevenueStatisticEmployeeDashboardResult TakeRevenueStatisticEmployeeDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter)
        {
            try
            {
                var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();
                var listCustomerOrder = (from cu in context.CustomerOrder
                                         join u in context.User on cu.CreatedById equals u.UserId into uJoind
                                         from u in uJoind.DefaultIfEmpty()
                                         join e in context.Employee on u.EmployeeId equals e.EmployeeId into euJoined
                                         from eu in euJoined.DefaultIfEmpty()
                                         where cu.StatusOrder == 5 && cu.UpdatedDate >= ((DateTime)(parameter.StartDate)).Date && cu.UpdatedDate <= ((DateTime)(parameter.EndDate)).Date.AddDays(1).AddTicks(-1)
                                         select new CustomerOrderEntityModel
                                         {
                                             OrderId = cu.OrderId,
                                             DiscountType = cu.DiscountType,
                                             DiscountValue = cu.DiscountValue,
                                             Vat = cu.Vat,
                                             StatusOrder = cu.StatusOrder,
                                             EmployeeName = eu.EmployeeName,
                                             EmployeeId = eu.EmployeeId
                                         }).ToList();

                var listOrderId = listCustomerOrder.Select(x => x.OrderId).ToList();
                var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                var listCustomerOrderDetailExten = context.CustomerOrderDetailExten.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                listCustomerOrder.ForEach(item =>
                {
                    #region
                    //Tổng tiền trước thuế
                    decimal? totalCostBeforeTax = 0;
                    //Tổng tiền thuế
                    decimal? totalCostTax = 0;
                    //Tổng tiền chiết khấu
                    decimal? totalDiscountCost = 0;
                    // Tổng tiền phải trả
                    decimal? totalCostPay = 0;

                    //Tùy chọn của phiếu yêu cầu
                    var listOrderDetailCurrent = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();
                    listOrderDetailCurrent.ForEach(currentDetail =>
                    {
                        totalCostBeforeTax += currentDetail.Quantity.Value * currentDetail.PriceInitial.Value;
                        totalCostTax += currentDetail.Quantity * currentDetail.PriceInitial / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                    });

                    //Nếu có phát sinh
                    var listOderDetailExtenCurrent = listCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).ToList();
                    if (listOderDetailExtenCurrent.Count > 0 && item.StatusOrder != 1)
                    {
                        listOderDetailExtenCurrent.ForEach(extenCur =>
                        {
                            if (extenCur.Price != null && extenCur.Status == 2)
                            {
                                totalCostBeforeTax += extenCur.Quantity * extenCur.Price;
                                totalCostTax += extenCur.Quantity * extenCur.Price / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                            }
                        });
                    }

                    //Chiết khấu theo %
                    // false: theo %, true: Theo số tiền
                    if (!item.DiscountType.Value)
                    {
                        totalDiscountCost = totalCostBeforeTax * item.DiscountValue / 100;
                    }
                    //Chiết khấu theo số tiền
                    else if (item.DiscountType.Value)
                    {
                        totalDiscountCost = item.DiscountValue;
                    }
                    totalCostPay = totalCostBeforeTax + totalCostTax - totalDiscountCost;

                    item.Amount = totalCostPay;
                    #endregion
                });

                var listRevenueStatisticEmployeeModel = new List<RevenueStatisticEmployeeModel>();
                var listCustomerOrderGroup = listCustomerOrder.GroupBy(x => x.EmployeeId).ToList();
                foreach (var item in listCustomerOrderGroup)
                {
                    if(item.FirstOrDefault().EmployeeId != null)
                    {
                        var revenueStatisticEmployee = new RevenueStatisticEmployeeModel();
                        revenueStatisticEmployee.EmployeeName = item.FirstOrDefault().EmployeeName;
                        revenueStatisticEmployee.Amount = item.Where(x => x.EmployeeId == item.Key).Sum(x => x.Amount);
                        listRevenueStatisticEmployeeModel.Add(revenueStatisticEmployee);
                    }
                }

                return new TakeRevenueStatisticEmployeeDashboardResult
                {
                    ListRevenueStatisticEmployeeModel = listRevenueStatisticEmployeeModel.OrderByDescending(x => x.Amount).Take((parameter.Count == null || (int)parameter.Count == 0) ? 100000 : (int)parameter.Count).ToList(),
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new TakeRevenueStatisticEmployeeDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

        public TakeRevenueStatisticServicePacketDashboardResult TakeRevenueStatisticServicePacketDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter)
        {
            try
            {
                var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();
                var listCustomerOrder = (from cu in context.CustomerOrder
                                         join s in context.ServicePacket on cu.ServicePacketId
                                         equals s.Id into sJoined
                                         from s in sJoined.DefaultIfEmpty()
                                         join c in context.ProductCategory on s.ProductCategoryId
                                         equals c.ProductCategoryId into csJoined
                                         from cs in csJoined.DefaultIfEmpty()
                                         where cu.StatusOrder == 5 && cu.UpdatedDate >= ((DateTime)(parameter.StartDate)).Date && cu.UpdatedDate <= ((DateTime)(parameter.EndDate)).Date.AddDays(1).AddTicks(-1)
                                         select new CustomerOrderEntityModel
                                         {
                                             OrderId = cu.OrderId,
                                             StatusOrder = cu.StatusOrder,
                                             CreatedDate = cu.CreatedDate,
                                             UpdatedDate = cu.UpdatedDate,
                                             DiscountType = cu.DiscountType,
                                             DiscountValue = cu.DiscountValue,
                                             Vat = cu.Vat,
                                             OrderTypeName = listOrderType.FirstOrDefault(y => y.Value == cu.OrderType).Name,
                                             ProductCategoryName = cs.ProductCategoryName,
                                             ServicePacketId = cu.ServicePacketId
                                         }).ToList();

                if(listCustomerOrder != null && listCustomerOrder.Count > 0)
                {
                    var listOrderId = listCustomerOrder.Select(x => x.OrderId).ToList();
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                    var listCustomerOrderDetailExten = context.CustomerOrderDetailExten.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                    listCustomerOrder.ForEach(item =>
                    {
                        #region Tính tiền
                        //Tổng tiền trước thuế
                        decimal? totalCostBeforeTax = 0;
                        //Tổng tiền thuế
                        decimal? totalCostTax = 0;
                        //Tổng tiền chiết khấu
                        decimal? totalDiscountCost = 0;
                        // Tổng tiền phải trả
                        decimal? totalCostPay = 0;

                        //Tùy chọn của phiếu yêu cầu
                        var listOrderDetailCurrent = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();
                        listOrderDetailCurrent.ForEach(currentDetail =>
                        {
                            totalCostBeforeTax += currentDetail.Quantity.Value * currentDetail.PriceInitial.Value;
                            totalCostTax += currentDetail.Quantity * currentDetail.PriceInitial / 100 * ((currentDetail.Vat == -1 || currentDetail.Vat == 0) ? 0 : currentDetail.Vat);
                        });

                        //Nếu có phát sinh
                        var listOderDetailExtenCurrent = listCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).ToList();
                        if (listOderDetailExtenCurrent.Count > 0 && item.StatusOrder != 1)
                        {
                            listOderDetailExtenCurrent.ForEach(extenCur =>
                            {
                                if (extenCur.Price != null && extenCur.Status == 2)
                                {
                                    totalCostBeforeTax += extenCur.Quantity * extenCur.Price;
                                    totalCostTax += extenCur.Quantity * extenCur.Price / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                                }
                            });
                        }

                        //Chiết khấu theo %
                        // false: theo %, true: Theo số tiền
                        if (!item.DiscountType.Value)
                        {
                            totalDiscountCost = totalCostBeforeTax * item.DiscountValue / 100;
                        }
                        //Chiết khấu theo số tiền
                        else if (item.DiscountType.Value)
                        {
                            totalDiscountCost = item.DiscountValue;
                        }
                        totalCostPay = totalCostBeforeTax + totalCostTax - totalDiscountCost;

                        item.Amount = totalCostPay;
                        #endregion
                    });

                    var dates = new List<DateTime>();
                    for (var dt = (DateTime)parameter.StartDate; dt <= ((DateTime)(parameter.EndDate)); dt = dt.AddDays(1))
                    {
                        dates.Add(dt.Date);
                    }

                    var listServicePacket = (from s in context.ServicePacket
                                             join p in context.ProductCategory
                                             on s.ProductCategoryId equals p.ProductCategoryId
                                             into spJoined
                                             from sp in spJoined.DefaultIfEmpty()
                                             select new ServicePacketEntityModel
                                             {
                                                 Id = s.Id,
                                                 Name = s.Name,
                                                 ProductCategoryName = sp.ProductCategoryName
                                             }).ToList();

                    var listRevenueStatisticServicePacketModelByServicePacket = new List<RevenueStatisticServicePacketModel>();
                    foreach (var item in listServicePacket)
                    {
                        var revenueStatisticServicePacketModel = new RevenueStatisticServicePacketModel();
                        var listAmountByDate = new List<decimal?>();
                        foreach (var date in dates)
                        {
                            listAmountByDate.Add(listCustomerOrder.Where(x => x.ServicePacketId == item.Id && ((DateTime)(x.UpdatedDate)).Date == date.Date && ((DateTime)(x.UpdatedDate)).Year == date.Year).Sum(x => x.Amount));
                        }
                        revenueStatisticServicePacketModel.ListAmount = listAmountByDate;
                        revenueStatisticServicePacketModel.ProductCategoryName = item.ProductCategoryName;
                        revenueStatisticServicePacketModel.ServicePacketName = item.Name;
                        listRevenueStatisticServicePacketModelByServicePacket.Add(revenueStatisticServicePacketModel);
                    }

                    var listCustomerOrderGroup = listCustomerOrder.GroupBy(x => x.ProductCategoryName);
                    var listRevenueStatisticServicePacketModel = new List<RevenueStatisticServicePacketModel>();
                    foreach (var item in listCustomerOrderGroup)
                    {
                        var revenueStatisticServicePacketModel = new RevenueStatisticServicePacketModel();
                        var listAmountByDate = new List<decimal?>();
                        foreach (var date in dates)
                        {
                            listAmountByDate.Add(listCustomerOrder.Where(x => x.ProductCategoryName == item.Key && ((DateTime)(x.UpdatedDate)).Date == date.Date && ((DateTime)(x.UpdatedDate)).Year == date.Year).Sum(x => x.Amount));
                        }
                        revenueStatisticServicePacketModel.ListAmount = listAmountByDate;
                        revenueStatisticServicePacketModel.ProductCategoryName = item.Key;
                        listRevenueStatisticServicePacketModel.Add(revenueStatisticServicePacketModel);
                    }

                    return new TakeRevenueStatisticServicePacketDashboardResult
                    {
                        ListRevenueStatisticServicePacketModel = listRevenueStatisticServicePacketModel.OrderByDescending(x => x.ListAmount.Sum()).Take((parameter.Count == null || (int)parameter.Count == 0) ? 100000 : (int)parameter.Count).ToList(),
                        ListRevenueStatisticServicePacketModelByServicePacket = listRevenueStatisticServicePacketModelByServicePacket,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Thành công"
                    };
                }
                else
                {
                    return new TakeRevenueStatisticServicePacketDashboardResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "Thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new TakeRevenueStatisticServicePacketDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

        public TakeRatingStatistictDashboardResult TakeRatingStatisticDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter)
        {
            try
            {
                var listOrderProcess = (from o in context.OrderProcess
                                        join s in context.ServicePacket on o.ServicePacketId
                                        equals s.Id into sJoined
                                        from s in sJoined.DefaultIfEmpty()
                                        join c in context.ProductCategory on s.ProductCategoryId
                                        equals c.ProductCategoryId into csJoined
                                        from cs in csJoined.DefaultIfEmpty()
                                        where o.RateStar > 0 && o.UpdatedDate >= ((DateTime)(parameter.StartDate)).Date && o.UpdatedDate <= (((DateTime)(parameter.EndDate)).Date.AddDays(1).AddTicks(-1))
                                        select new OrderProcessEntityModel
                                        {
                                            RateStar = o.RateStar,
                                            ServicePacketId = o.ServicePacketId,
                                            ServicePacketName = s.Name,
                                            ProductCategoryName = cs.ProductCategoryName,
                                            UpdatedDate = o.UpdatedDate
                                        }).ToList();

                if (listOrderProcess != null && listOrderProcess.Count > 0)
                {
                    var dates = new List<DateTime>();
                    for (var dt = (DateTime)parameter.StartDate; dt <= ((DateTime)(parameter.EndDate)); dt = dt.AddDays(1))
                    {
                        dates.Add(dt.Date);
                    }

                    var listServicePacket = (from s in context.ServicePacket
                                             join p in context.ProductCategory
                                             on s.ProductCategoryId equals p.ProductCategoryId
                                             into spJoined
                                             from sp in spJoined.DefaultIfEmpty()
                                             select new ServicePacketEntityModel
                                             {
                                                 Id = s.Id,
                                                 Name = s.Name,
                                                 ProductCategoryName = sp.ProductCategoryName
                                             }).ToList();

                    var listRatingStatisticServicePacketModelByServicePacket = new List<RatingStatisticServicePacketModel>();
                    foreach (var item in listServicePacket)
                    {

                        var ratingStatisticServicePacketModel = new RatingStatisticServicePacketModel();
                        var listRateByDate = new List<int>();
                        foreach (var date in dates)
                        {
                            listRateByDate.Add(listOrderProcess.Where(x => x.ServicePacketId == item.Id && ((DateTime)(x.UpdatedDate)).Date == date.Date && ((DateTime)(x.UpdatedDate)).Year == date.Year).ToList().Count());
                        }
                        ratingStatisticServicePacketModel.ProductCategoryName = item.ProductCategoryName;
                        ratingStatisticServicePacketModel.ServicePacketName = item.Name;
                        ratingStatisticServicePacketModel.ListRate = listRateByDate;
                        listRatingStatisticServicePacketModelByServicePacket.Add(ratingStatisticServicePacketModel);
                    }

                    var listOrderProcessGroup = listOrderProcess.GroupBy(x => x.ProductCategoryName);
                    var listRatingStatisticServicePacketModel = new List<RatingStatisticServicePacketModel>();
                    foreach (var item in listOrderProcessGroup)
                    {
                        var ratingStatisticServicePacketModel = new RatingStatisticServicePacketModel();
                        var listRateByDate = new List<int>();
                        foreach (var date in dates)
                        {
                            listRateByDate.Add(listOrderProcess.Where(x => x.ProductCategoryName == item.Key && ((DateTime)(x.UpdatedDate)).Date == date.Date && ((DateTime)(x.UpdatedDate)).Year == date.Year).ToList().Count());
                        }
                        ratingStatisticServicePacketModel.ProductCategoryName = item.Key;
                        ratingStatisticServicePacketModel.ListRate = listRateByDate;
                        listRatingStatisticServicePacketModel.Add(ratingStatisticServicePacketModel);
                    }

                    var listRatingStatisticStarServicePacketModel = new List<RatingStatisticStarServicePacketModel>();
                    var listOrderProcessGroupByServicePacketId = listOrderProcess.GroupBy(x => x.ServicePacketId);
                    var listOrderProcessGroupByStar = listOrderProcess.GroupBy(x => x.RateStar);
                    foreach (var item in listOrderProcessGroupByServicePacketId)
                    {
                        foreach (var star in listOrderProcessGroupByStar)
                        {
                            var ratingStatisticStarServicePacketModel = new RatingStatisticStarServicePacketModel();
                            ratingStatisticStarServicePacketModel.ServicePacketName = item.FirstOrDefault().ServicePacketName;
                            ratingStatisticStarServicePacketModel.RateStar = (int)star.Key;
                            ratingStatisticStarServicePacketModel.Count = listOrderProcess.Where(x => x.RateStar == star.Key && x.ServicePacketId == item.Key).Count();
                            listRatingStatisticStarServicePacketModel.Add(ratingStatisticStarServicePacketModel);
                        }
                    }
                    return new TakeRatingStatistictDashboardResult
                    {
                        ListRatingStatisticServicePacketModel = listRatingStatisticServicePacketModel.OrderByDescending(x => x.ListRate.Sum()).Take((parameter.Count == null || (int)parameter.Count == 0) ? 100000 : (int)parameter.Count).ToList(),
                        ListRatingStatisticServicePacketModelByServicePacket = listRatingStatisticServicePacketModelByServicePacket,
                        ListRatingStatisticStarServicePacketModel = listRatingStatisticStarServicePacketModel,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Thành công"
                    };
                }
                else
                {
                    return new TakeRatingStatistictDashboardResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "Thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new TakeRatingStatistictDashboardResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

    }
}
