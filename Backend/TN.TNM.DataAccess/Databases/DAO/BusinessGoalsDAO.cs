using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals;
using TN.TNM.DataAccess.Messages.Results.Admin.BusinessGoals;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BusinessGoals;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BusinessGoalsDAO : BaseDAO, IBusinessGoalsDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BusinessGoalsDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateOrUpdateBusinessGoalsResult CreateOrUpdateBusinessGoals(CreateOrUpdateBusinessGoalsParameter parameter)
        {
            try
            {
                var oldBusinesGoals = context.BusinessGoals.FirstOrDefault(c => c.OrganizationId == parameter.BusinessGoals.OrganizationId && c.Year == parameter.BusinessGoals.Year);

                // Tạo mới
                if (oldBusinesGoals == null)
                {
                    var businesGoals = new BusinessGoals
                    {
                        BusinessGoalsId = Guid.NewGuid(),
                        Year = parameter.BusinessGoals.Year,
                        OrganizationId = parameter.BusinessGoals.OrganizationId,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null
                    };

                    // Mục tiêu doanh số
                    var listBusinesGoalsSales = new List<BusinessGoalsDetail>();
                    if (parameter.ListBusinessGoalsDetailSales.Count != 0)
                    {
                        listBusinesGoalsSales = parameter.ListBusinessGoalsDetailSales
                            .Select(item => new BusinessGoalsDetail
                            {
                                BusinessGoalsDetailId = Guid.NewGuid(),
                                BusinessGoalsId = businesGoals.BusinessGoalsId,
                                BusinessGoalsType = "SALES",
                                ProductCategoryId = item.ProductCategoryId,
                                January = item.January,
                                February = item.February,
                                March = item.March,
                                April = item.April,
                                May = item.May,
                                June = item.June,
                                July = item.July,
                                August = item.August,
                                September = item.September,
                                October = item.October,
                                November = item.November,
                                December = item.December,
                                Active = true
                            }).ToList();
                    }

                    var listBusinesGoalsRevenue = new List<BusinessGoalsDetail>();
                    if (parameter.ListBusinessGoalsDetailRevenue.Count != 0)
                    {
                        listBusinesGoalsRevenue = parameter.ListBusinessGoalsDetailRevenue
                            .Select(item => new BusinessGoalsDetail
                            {
                                BusinessGoalsDetailId = Guid.NewGuid(),
                                BusinessGoalsId = businesGoals.BusinessGoalsId,
                                BusinessGoalsType = "REVENUE",
                                ProductCategoryId = item.ProductCategoryId,
                                January = item.January,
                                February = item.February,
                                March = item.March,
                                April = item.April,
                                May = item.May,
                                June = item.June,
                                July = item.July,
                                August = item.August,
                                September = item.September,
                                October = item.October,
                                November = item.November,
                                December = item.December,
                                Active = true
                            }).ToList();
                    }

                    context.BusinessGoals.Add(businesGoals);
                    context.BusinessGoalsDetail.AddRange(listBusinesGoalsSales);
                    context.BusinessGoalsDetail.AddRange(listBusinesGoalsRevenue);
                    context.SaveChanges();
                }
                // Update
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var delListBusinesGoalsDetail = context.BusinessGoalsDetail.Where(c => c.BusinessGoalsId == oldBusinesGoals.BusinessGoalsId).ToList();
                        context.BusinessGoalsDetail.RemoveRange(delListBusinesGoalsDetail);

                        // Mục tiêu doanh số
                        var listBusinesGoalsSales = new List<BusinessGoalsDetail>();
                        if (parameter.ListBusinessGoalsDetailSales.Count != 0)
                        {
                            listBusinesGoalsSales = parameter.ListBusinessGoalsDetailSales
                                .Select(item => new BusinessGoalsDetail
                                {
                                    BusinessGoalsDetailId = Guid.NewGuid(),
                                    BusinessGoalsId = oldBusinesGoals.BusinessGoalsId,
                                    BusinessGoalsType = "SALES",
                                    ProductCategoryId = item.ProductCategoryId,
                                    January = item.January,
                                    February = item.February,
                                    March = item.March,
                                    April = item.April,
                                    May = item.May,
                                    June = item.June,
                                    July = item.July,
                                    August = item.August,
                                    September = item.September,
                                    October = item.October,
                                    November = item.November,
                                    December = item.December,
                                    Active = true
                                }).ToList();
                        }

                        var listBusinesGoalsRevenue = new List<BusinessGoalsDetail>();
                        if (parameter.ListBusinessGoalsDetailRevenue.Count != 0)
                        {
                            listBusinesGoalsRevenue = parameter.ListBusinessGoalsDetailRevenue
                                .Select(item => new BusinessGoalsDetail
                                {
                                    BusinessGoalsDetailId = Guid.NewGuid(),
                                    BusinessGoalsId = oldBusinesGoals.BusinessGoalsId,
                                    BusinessGoalsType = "REVENUE",
                                    ProductCategoryId = item.ProductCategoryId,
                                    January = item.January,
                                    February = item.February,
                                    March = item.March,
                                    April = item.April,
                                    May = item.May,
                                    June = item.June,
                                    July = item.July,
                                    August = item.August,
                                    September = item.September,
                                    October = item.October,
                                    November = item.November,
                                    December = item.December,
                                    Active = true
                                }).ToList();
                        }

                        context.BusinessGoalsDetail.AddRange(listBusinesGoalsSales);
                        context.BusinessGoalsDetail.AddRange(listBusinesGoalsRevenue);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                }

                return new CreateOrUpdateBusinessGoalsResult
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateBusinessGoalsResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataBusinessGoalsResult GetMasterDataBusinessGoals(GetMasterDataBusinessGoalsParameter parameter)
        {
            try
            {
                var listProductCategory = context.ProductCategory.Where(c => c.Active == true && c.ParentId == null)
                    .Select(m => new ProductCategoryEntityModel
                    {
                        ProductCategoryId = m.ProductCategoryId,
                        ProductCategoryCode = m.ProductCategoryCode,
                        ProductCategoryName = m.ProductCategoryName
                    }).ToList();

                var list = context.Organization.Select(o => new OrganizationEntityModel()
                {
                    OrganizationId = o.OrganizationId,
                    OrganizationName = o.OrganizationName,
                    Level = o.Level,
                    ParentId = o.ParentId,
                    IsFinancialIndependence = o.IsFinancialIndependence
                }).ToList();

                List<OrganizationEntityModel> recoreds = list.Where(l => l.ParentId == null).Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    IsFinancialIndependence = l.IsFinancialIndependence,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();

                // Lấy dữ liệu về doanh số và doanh thu
                var listBusinessGoalsSalesDetail = new List<BusinessGoalsDetailEntityModel>();
                var listBusinessGoalsRevenueDetail = new List<BusinessGoalsDetailEntityModel>();
                var listBusinessGoalsDetalSaleChild = new List<BusinessGoalsDetailEntityModel>();
                var listBusinessGoalsDetalRevenueChild = new List<BusinessGoalsDetailEntityModel>();

                var listAllOrganization = context.Organization.ToList();
                var listAllBusinessGoals = context.BusinessGoals.ToList();

                // Load lần đầu sẽ sử dụng root
                if (parameter.Type)
                {
                    parameter.OrganizationId = list.FirstOrDefault(c => c.ParentId == null)?.OrganizationId;
                }

                if (parameter.OrganizationId != null && parameter.OrganizationId != Guid.Empty)
                {
                    var businessGoalsId = listAllBusinessGoals.FirstOrDefault(c => c.OrganizationId == parameter.OrganizationId && c.Year == parameter.Year)?.BusinessGoalsId;

                    #region Danh sách phòng ban con
                    var listChildId = new List<Guid>();
                    var lstOrganizationChildId = listAllOrganization.Where(c => c.ParentId == parameter.OrganizationId).Select(m => m.OrganizationId).ToList();
                    while (lstOrganizationChildId.Count() != 0)
                    {
                        listChildId.AddRange(lstOrganizationChildId);
                        lstOrganizationChildId = listAllOrganization.Where(c => c.ParentId != null && lstOrganizationChildId.Contains(c.ParentId.Value)).Select(m => m.OrganizationId).ToList();
                    }
                    var listBusinessGoalsId = listAllBusinessGoals.Where(c => listChildId.Contains(c.OrganizationId) && c.Year == parameter.Year).Select(m => m.BusinessGoalsId).ToList();
                    #endregion

                    var listAllBusinessSalesDetail = context.BusinessGoalsDetail
                        .Select(m => new BusinessGoalsDetailEntityModel
                        {
                            BusinessGoalsDetailId = m.BusinessGoalsDetailId,
                            BusinessGoalsId = m.BusinessGoalsId,
                            ProductCategoryId = m.ProductCategoryId,
                            BusinessGoalsType = m.BusinessGoalsType,
                            January = m.January,
                            February = m.February,
                            March = m.March,
                            April = m.April,
                            May = m.May,
                            June = m.June,
                            July = m.July,
                            August = m.August,
                            September = m.September,
                            October = m.October,
                            November = m.November,
                            December = m.December,
                        }).ToList();

                    // Mục tiêu doanh số
                    listBusinessGoalsSalesDetail = listAllBusinessSalesDetail.Where(c => c.BusinessGoalsId == businessGoalsId && c.BusinessGoalsType == "SALES").ToList();

                    listBusinessGoalsSalesDetail.ForEach(item =>
                    {
                        var productCategory = listProductCategory.FirstOrDefault(c => c.ProductCategoryId == item.ProductCategoryId);
                        item.ProductCategoryCode = productCategory?.ProductCategoryCode ?? string.Empty;
                        item.ProductCategoryName = productCategory?.ProductCategoryName ?? string.Empty;
                    });

                    listBusinessGoalsDetalSaleChild = listAllBusinessSalesDetail.Where(c => listBusinessGoalsId.Contains(c.BusinessGoalsId) && c.BusinessGoalsType == "SALES").ToList();
                    listBusinessGoalsDetalSaleChild.ForEach(item =>
                    {
                        var productCategory = listProductCategory.FirstOrDefault(c => c.ProductCategoryId == item.ProductCategoryId);
                        item.OrganizationId = listAllBusinessGoals.FirstOrDefault(c => c.BusinessGoalsId == item.BusinessGoalsId)?.OrganizationId;
                        item.OrganizationName = listAllOrganization.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? string.Empty;
                        item.ProductCategoryCode = productCategory?.ProductCategoryCode ?? string.Empty;
                        item.ProductCategoryName = productCategory?.ProductCategoryName ?? string.Empty;
                    });

                    listBusinessGoalsDetalSaleChild = listBusinessGoalsDetalSaleChild.OrderByDescending(c => c.OrganizationName).ToList();

                    // Mục tiêu doanh thu
                    listBusinessGoalsRevenueDetail = listAllBusinessSalesDetail.Where(c => c.BusinessGoalsId == businessGoalsId && c.BusinessGoalsType == "REVENUE").ToList();
                    listBusinessGoalsRevenueDetail.ForEach(item =>
                    {
                        var productCategory = listProductCategory.FirstOrDefault(c => c.ProductCategoryId == item.ProductCategoryId);
                        item.ProductCategoryCode = productCategory?.ProductCategoryCode ?? string.Empty;
                        item.ProductCategoryName = productCategory?.ProductCategoryName ?? string.Empty;
                    });

                    listBusinessGoalsDetalRevenueChild = listAllBusinessSalesDetail.Where(c => listBusinessGoalsId.Contains(c.BusinessGoalsId) && c.BusinessGoalsType == "REVENUE").ToList();
                    listBusinessGoalsDetalRevenueChild.ForEach(item =>
                    {
                        var productCategory = listProductCategory.FirstOrDefault(c => c.ProductCategoryId == item.ProductCategoryId);
                        item.OrganizationId = listAllBusinessGoals.FirstOrDefault(c => c.BusinessGoalsId == item.BusinessGoalsId)?.OrganizationId;
                        item.OrganizationName = listAllOrganization.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? string.Empty;
                        item.ProductCategoryCode = productCategory?.ProductCategoryCode ?? string.Empty;
                        item.ProductCategoryName = productCategory?.ProductCategoryName ?? string.Empty;
                    });

                    listBusinessGoalsDetalRevenueChild = listBusinessGoalsDetalRevenueChild.OrderByDescending(c => c.OrganizationName).ToList();

                }

                return new GetMasterDataBusinessGoalsResult
                {
                    ListProductCategory = listProductCategory,
                    ListOrganization = list,
                    ListBusinessGoalsSalesDetail = listBusinessGoalsSalesDetail,
                    ListBusinessGoalsRevenueDetail = listBusinessGoalsRevenueDetail,
                    ListBusinessGoalsSalesDetailChild = listBusinessGoalsDetalSaleChild,
                    ListBusinessGoalsRevenueDetailChild = listBusinessGoalsDetalRevenueChild,
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataBusinessGoalsResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<OrganizationEntityModel> GetChildren(Guid? id, List<OrganizationEntityModel> list)
        {
            return list.Where(l => l.ParentId == id)
                .Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    IsFinancialIndependence = l.IsFinancialIndependence,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();
        }
    }
}
