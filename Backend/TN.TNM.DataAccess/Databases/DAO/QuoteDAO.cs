using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Messages.Results.Quote;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Cost;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.SaleBidding;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Promotion;
using QuoteEntityModel = TN.TNM.DataAccess.Models.Quote.QuoteEntityModel;
using System.Text.RegularExpressions;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class QuoteDAO : BaseDAO, IQuoteDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public static string WEB_ENDPOINT;
        public static string PrimaryDomain;
        public static int PrimaryPort;
        public static string SecondayDomain;
        public static int SecondaryPort;
        public static string Email;
        public static string Password;
        public static string BannerUrl;
        public static string Ssl;
        public static string Company;
        public static string Domain;
        public void GetConfiguration()
        {
            PrimaryDomain = Configuration["PrimaryDomain"];
            PrimaryPort = int.Parse(Configuration["PrimaryPort"]);
            SecondayDomain = Configuration["SecondayDomain"];
            SecondaryPort = int.Parse(Configuration["SecondaryPort"]);
            Email = Configuration["Email"];
            Password = Configuration["Password"];
            Ssl = Configuration["Ssl"];
            Company = Configuration["Company"];
            BannerUrl = Configuration["BannerUrl"];
            WEB_ENDPOINT = Configuration["WEB_ENDPOINT"];

            var configEntity = context.SystemParameter.ToList();
            Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
        }

        public QuoteDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            this.Configuration = iconfiguration;
        }

        public CreateQuoteResult CreateQuote(CreateQuoteParameter parameter)
        {
            bool isValidParameterNumber = true;
            if (parameter.Quote?.DaysAreOwed < 0 || parameter.Quote?.DiscountValue < 0 ||
                parameter.Quote?.EffectiveQuoteDate <= 0 || parameter.Quote?.MaxDebt < 0)
            {
                isValidParameterNumber = false;
            }

            parameter.QuoteDetail.ForEach(item =>
            {
                if (item?.DiscountValue < 0 || item?.ExchangeRate < 0 || item?.Quantity <= 0 || item?.UnitPrice < 0 ||
                    item?.Vat < 0)
                {
                    isValidParameterNumber = false;
                }
            });

            if (!isValidParameterNumber)
            {
                return new CreateQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Quote.CREATE_FAIL
                };
            }

            //Kiểm tra chiết khấu của đơn hàng
            if (parameter.Quote.DiscountValue == null)
            {
                parameter.Quote.DiscountValue = 0;
            }

            //Kiểm tra chiết khấu của sản phẩm
            if (parameter.QuoteDetail.Count > 0)
            {
                var listProduct = parameter.QuoteDetail.ToList();
                listProduct.ForEach(item =>
                {
                    if (item.DiscountValue == null)
                    {
                        item.DiscountValue = 0;
                    }
                });
            }

            try
            {
                var quoteCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                var listQuoteStatus = context.Category.Where(x => x.CategoryTypeId == quoteCategoryType.CategoryTypeId)
                    .ToList();

                List<QuoteDocument> listQuoteDocuments = new List<QuoteDocument>();

                if (parameter.Quote.QuoteId != null && parameter.Quote.QuoteId != Guid.Empty)
                {
                    var oldQuote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.Quote.QuoteId);

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        #region Delete all item Relation 

                        var List_Delete_QuoteProductDetailProductAttributeValue =
                            new List<QuoteProductDetailProductAttributeValue>();
                        var List_Delete_QuoteDetail = new List<QuoteDetail>();
                        List_Delete_QuoteDetail = context.QuoteDetail
                            .Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                        List_Delete_QuoteDetail.ForEach(item =>
                        {
                            var objectQuoteDetail = new QuoteDetail();
                            if (item.QuoteDetailId != Guid.Empty)
                            {
                                objectQuoteDetail = item;
                                var QuoteProductDetailProductAttributeValueList = context
                                    .QuoteProductDetailProductAttributeValue
                                    .Where(OPDPAV => OPDPAV.QuoteDetailId == item.QuoteDetailId).ToList();
                                List_Delete_QuoteProductDetailProductAttributeValue.AddRange(
                                    QuoteProductDetailProductAttributeValueList);
                            }
                        });

                        var List_QuoteDocument = context.QuoteDocument.Where(w => w.QuoteId == parameter.Quote.QuoteId)
                            .ToList();
                        var List_AdditionalInformation = context.AdditionalInformation
                            .Where(x => x.ObjectId == parameter.Quote.QuoteId && x.ObjectType == "QUOTE").ToList();

                        var List_Delete_QuoteCostDetail = new List<QuoteCostDetail>();
                        List_Delete_QuoteCostDetail = context.QuoteCostDetail
                            .Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                        var listDeleteParticipant = context.QuoteParticipantMapping
                            .Where(x => x.QuoteId == parameter.Quote.QuoteId).ToList();

                        var listDeletePromotion = context.PromotionObjectApply
                            .Where(x => x.ObjectId == parameter.Quote.QuoteId && x.ObjectType == "QUOTE").ToList();

                        var List_QuotePlan = context.QuotePlan.Where(w => w.QuoteId == parameter.Quote.QuoteId)
                            .ToList();
                        var List_QuoteScope = context.QuoteScope.Where(w => w.QuoteId == parameter.Quote.QuoteId)
                          .ToList();

                        var List_Quote_PaymentTerm = context.QuotePaymentTerm.Where(x => x.QuoteId == parameter.Quote.QuoteId).ToList();

                        context.QuoteProductDetailProductAttributeValue.RemoveRange(
                            List_Delete_QuoteProductDetailProductAttributeValue);
                        context.SaveChanges();
                        context.QuoteDetail.RemoveRange(List_Delete_QuoteDetail);
                        context.SaveChanges();
                        context.QuoteCostDetail.RemoveRange(List_Delete_QuoteCostDetail);
                        context.SaveChanges();
                        context.QuoteDocument.RemoveRange(List_QuoteDocument);
                        context.SaveChanges();
                        context.AdditionalInformation.RemoveRange(List_AdditionalInformation);
                        context.SaveChanges();
                        context.QuoteParticipantMapping.RemoveRange(listDeleteParticipant);
                        context.SaveChanges();
                        context.Quote.Remove(oldQuote);
                        context.SaveChanges();
                        context.PromotionObjectApply.RemoveRange(listDeletePromotion);
                        context.SaveChanges();
                        context.QuotePlan.RemoveRange(List_QuotePlan);
                        context.SaveChanges();
                        context.QuotePaymentTerm.RemoveRange(List_Quote_PaymentTerm);
                        context.SaveChanges();

                        #endregion

                        #region Create new Order base on Old Order

                        parameter.QuoteDetail.ForEach(item =>
                        {
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            if (item.QuoteDetailId == null || item.QuoteDetailId == Guid.Empty)
                                item.QuoteDetailId = Guid.NewGuid();
                            if (item.QuoteProductDetailProductAttributeValue != null)
                            {
                                foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                                {
                                    if (itemX.QuoteProductDetailProductAttributeValueId == null ||
                                        itemX.QuoteProductDetailProductAttributeValueId == Guid.Empty)
                                        itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                                }
                            }
                        });

                        parameter.QuoteCostDetail.ForEach(item =>
                        {
                            item.Active = true;
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            if (item.QuoteCostDetailId == null || item.QuoteCostDetailId == Guid.Empty)
                                item.QuoteCostDetailId = Guid.NewGuid();
                        });

                        parameter.QuoteDocument.ForEach(item =>
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            if (item.QuoteDocumentId == null || item.QuoteDocumentId == Guid.Empty)
                                item.QuoteDocumentId = Guid.NewGuid();
                        });

                        List<AdditionalInformation> listAdditionalInformation = new List<AdditionalInformation>();
                        parameter.ListAdditionalInformation.ForEach(item =>
                        {
                            var additionalInformation = new AdditionalInformation();
                            additionalInformation.AdditionalInformationId = Guid.NewGuid();
                            additionalInformation.ObjectId = parameter.Quote.QuoteId;
                            additionalInformation.ObjectType = "QUOTE";
                            additionalInformation.Title = item.Title;
                            additionalInformation.Content = item.Content;
                            additionalInformation.Ordinal = item.Ordinal;
                            additionalInformation.Active = true;
                            additionalInformation.CreatedById = parameter.UserId;
                            additionalInformation.CreatedDate = DateTime.Now;
                            additionalInformation.UpdatedById = null;
                            additionalInformation.UpdatedDate = null;

                            listAdditionalInformation.Add(additionalInformation);
                        });
                        context.AdditionalInformation.AddRange(listAdditionalInformation);
                        context.SaveChanges();

                        //parameter.Quote.QuoteDetail = parameter.QuoteDetail;
                        //parameter.Quote.QuoteCostDetail = parameter.QuoteCostDetail;
                        //parameter.Quote.QuoteDocument = parameter.QuoteDocument;
                        parameter.Quote.QuoteId = oldQuote.QuoteId;
                        parameter.Quote.QuoteCode = oldQuote.QuoteCode;
                        parameter.Quote.QuoteName = parameter.Quote.QuoteName;
                        parameter.Quote.QuoteDate = oldQuote.QuoteDate;
                        parameter.Quote.CreatedById = oldQuote.CreatedById;
                        parameter.Quote.CreatedDate = oldQuote.CreatedDate;
                        parameter.Quote.UpdatedById = parameter.UserId;
                        parameter.Quote.UpdatedDate = DateTime.Now;
                        var quote = parameter.Quote.ToEntity();
                        var listQuoteDetail = new List<QuoteDetail>();
                        parameter.QuoteDetail.ForEach(item =>
                        {
                            var newItem = new QuoteDetail();
                            newItem = item.ToEntity();

                            if (item.QuoteProductDetailProductAttributeValue != null &&
                                item.QuoteProductDetailProductAttributeValue.Count != 0)
                            {
                                item.QuoteProductDetailProductAttributeValue.ForEach(_item =>
                                {
                                    var _newItem = _item.ToEntity();
                                    newItem.QuoteProductDetailProductAttributeValue.Add(_newItem);
                                });
                            }

                            listQuoteDetail.Add(newItem);
                        });
                        quote.QuoteDetail = listQuoteDetail;
                        var listQuoteCostDetail = new List<QuoteCostDetail>();
                        parameter.QuoteCostDetail.ForEach(item =>
                        {
                            listQuoteCostDetail.Add(item.ToEntity());
                        });
                        quote.QuoteCostDetail = listQuoteCostDetail;

                        var listQuoteDocumentPara = new List<QuoteDocument>();
                        parameter.QuoteDocument.ForEach(item =>
                        {
                            listQuoteDocumentPara.Add(item.ToEntity());
                        });
                        quote.QuoteDocument = listQuoteDocumentPara;
                        context.Quote.Add(quote);
                        context.SaveChanges();

                        listQuoteDocuments.AddRange(listQuoteDocumentPara);

                        for (int i = 0; i < listQuoteDocuments.Count; i++)
                        {
                            var isCheck = false;
                            for (int j = 0; j < List_QuoteDocument.Count; j++)
                            {
                                if (listQuoteDocuments[i].QuoteDocumentId == List_QuoteDocument[j].QuoteDocumentId)
                                {
                                    isCheck = true;
                                    break;
                                }
                            }
                            if (!isCheck)
                            {
                                listQuoteDocuments[i].UpdatedDate = DateTime.Now;
                            }
                        }

                        #region Thêm người tham gia

                        List<QuoteParticipantMapping> listParticipantMapping = new List<QuoteParticipantMapping>();
                        parameter.ListParticipant.ForEach(item =>
                        {
                            var quoteParticipantMapping = new QuoteParticipantMapping();
                            quoteParticipantMapping.QuoteParticipantMappingId = Guid.NewGuid();
                            quoteParticipantMapping.EmployeeId = item;
                            quoteParticipantMapping.QuoteId = parameter.Quote.QuoteId;

                            listParticipantMapping.Add(quoteParticipantMapping);
                        });
                        context.QuoteParticipantMapping.AddRange(listParticipantMapping);

                        #endregion

                        #region Thêm quà khuyến mãi

                        var listPromotionObjectApply = new List<PromotionObjectApply>();
                        parameter.ListPromotionObjectApply.ForEach(item =>
                        {
                            var promotionObjectApply = new PromotionObjectApply();
                            promotionObjectApply.PromotionObjectApplyId = Guid.NewGuid();
                            promotionObjectApply.ObjectId = parameter.Quote.QuoteId;
                            promotionObjectApply.ObjectType = "QUOTE";
                            promotionObjectApply.PromotionId = item.PromotionId;
                            promotionObjectApply.ConditionsType = item.ConditionsType;
                            promotionObjectApply.PropertyType = item.PropertyType;
                            promotionObjectApply.NotMultiplition = item.NotMultiplition;
                            promotionObjectApply.PromotionMappingId = item.PromotionMappingId;
                            promotionObjectApply.ProductId = item.ProductId;
                            promotionObjectApply.SoLuongTang = item.SoLuongTang;
                            promotionObjectApply.LoaiGiaTri = item.LoaiGiaTri;
                            promotionObjectApply.GiaTri = item.GiaTri;
                            promotionObjectApply.Amount = item.Amount;
                            promotionObjectApply.SoTienTu = item.SoTienTu;

                            listPromotionObjectApply.Add(promotionObjectApply);
                        });

                        context.PromotionObjectApply.AddRange(listPromotionObjectApply);

                        #endregion

                        #region Thêm plan

                        List<QuotePlan> listQuotePlan = new List<QuotePlan>();
                        parameter.QuotePlans.ForEach(item =>
                        {
                            var quotePlan = new QuotePlan();

                            quotePlan.PlanId = Guid.NewGuid();
                            quotePlan.Tt = item.Tt;
                            quotePlan.Finished = item.Finished;
                            quotePlan.ExecTime = item.ExecTime;
                            quotePlan.SumExecTime = item.SumExecTime;
                            quotePlan.QuoteId = item.QuoteId;
                            quotePlan.TenantId = item.TenantId;
                            listQuotePlan.Add(quotePlan);
                        });
                        context.QuotePlan.AddRange(listQuotePlan);

                        #endregion

                        #region Thêm điều khoản thanh toán

                        List<QuotePaymentTerm> quotePaymentTerms = new List<QuotePaymentTerm>();
                        parameter.ListQuotePaymentTerm.ForEach(item =>
                        {
                            var paymentTerm = new QuotePaymentTerm();

                            paymentTerm.PaymentTermId = Guid.NewGuid();
                            paymentTerm.QuoteId = parameter.Quote.QuoteId;
                            paymentTerm.OrderNumber = item.OrderNumber;
                            paymentTerm.Milestone = item.Milestone;
                            paymentTerm.PaymentPercentage = item.PaymentPercentage;
                            paymentTerm.CreatedDate = DateTime.Now;
                            paymentTerm.CreatedById = parameter.UserId;

                            quotePaymentTerms.Add(paymentTerm);
                        });

                        context.QuotePaymentTerm.AddRange(quotePaymentTerms);
                        context.SaveChanges();

                        #endregion

                        context.SaveChanges();
                        transaction.Commit();

                        #region Log

                        LogHelper.AuditTrace(context, "Update", "Quote", parameter.Quote.QuoteId, parameter.UserId);

                        #endregion

                        #region Gửi thông báo

                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "UPD", new Quote(),
                            quote, true);

                        #endregion

                        #endregion
                    }
                }
                else
                {
                    if (parameter.isClone)
                    {
                        var quoteClone = context.Quote.FirstOrDefault(cl => cl.QuoteId == parameter.QuoteIdClone);
                        quoteClone.CloneCount = quoteClone.CloneCount == null ? 1 : quoteClone.CloneCount + 1;
                        context.Quote.Update(quoteClone);

                        parameter.Quote.CloneCount = null;
                        parameter.Quote.StatusId = listQuoteStatus
                            .FirstOrDefault(c => c.Active == true && c.CategoryCode == "MTA").CategoryId;
                        parameter.Quote.QuoteName = parameter.Quote.QuoteName + "_" + quoteClone.CloneCount;
                    }
                    parameter.Quote.QuoteId = Guid.NewGuid();
                    parameter.Quote.QuoteDate = DateTime.Now;
                    parameter.Quote.CreatedById = parameter.UserId;
                    parameter.Quote.CreatedDate = DateTime.Now;
                    parameter.Quote.UpdatedById = parameter.UserId;
                    parameter.Quote.UpdatedDate = DateTime.Now;

                    #region Create New Order with GenerateorderCode

                    parameter.QuoteDetail.ForEach(item =>
                    {
                        item.QuoteDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;

                        foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                        {
                            itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                        }
                    });

                    parameter.QuoteCostDetail.ForEach(item =>
                    {
                        item.QuoteCostDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.Quote.CreatedById;
                        item.CreatedDate = parameter.Quote.CreatedDate;
                        item.Active = true;
                    });

                    parameter.QuoteDocument.ForEach(item =>
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);
                        item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                        item.QuoteDocumentId = Guid.NewGuid();
                    });

                    parameter.Quote.QuoteCode = GenerateorderCode();
                    //Kiểm tra trùng quote
                    var dublicateQuote = context.Quote.FirstOrDefault(x => x.QuoteCode == parameter.Quote.QuoteCode);
                    if (dublicateQuote != null)
                    {
                        return new CreateQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Báo giá đã tồn tại trên hệ thống",
                            QuoteID = Guid.Empty
                        };
                    }

                    //parameter.Quote.QuoteDetail = parameter.QuoteDetail;
                    //parameter.Quote.QuoteCostDetail = parameter.QuoteCostDetail;
                    //parameter.Quote.QuoteDocument = parameter.QuoteDocument;
                    var quote = parameter.Quote.ToEntity();
                    var listQuoteDetail = new List<QuoteDetail>();
                    parameter.QuoteDetail.ForEach(item =>
                    {
                        var newItem = new QuoteDetail();
                        newItem = item.ToEntity();

                        if (item.QuoteProductDetailProductAttributeValue != null &&
                            item.QuoteProductDetailProductAttributeValue.Count != 0)
                        {
                            item.QuoteProductDetailProductAttributeValue.ForEach(_item =>
                            {
                                var _newItem = _item.ToEntity();
                                newItem.QuoteProductDetailProductAttributeValue.Add(_newItem);
                            });
                        }

                        listQuoteDetail.Add(newItem);
                    });
                    quote.QuoteDetail = listQuoteDetail;
                    var listQuoteCostDetail = new List<QuoteCostDetail>();
                    parameter.QuoteCostDetail.ForEach(item =>
                    {
                        listQuoteCostDetail.Add(item.ToEntity());
                    });
                    quote.QuoteCostDetail = listQuoteCostDetail;

                    var listQuoteDocumentPara = new List<QuoteDocument>();
                    parameter.QuoteDocument.ForEach(item =>
                    {
                        listQuoteDocumentPara.Add(item.ToEntity());
                    });
                    quote.QuoteDocument = listQuoteDocumentPara;
                    context.Quote.Add(quote);
                    context.SaveChanges();

                    listQuoteDocuments.AddRange(listQuoteDocumentPara);

                    #region Thêm 1 phạm vi công việc
                    var quoteScope = new QuoteScope()
                    {
                        ScopeId = Guid.NewGuid(),
                        Tt = "",
                        Category = "Các chức năng của " + parameter.Quote.QuoteName,
                        Description = "",
                        Level = 0,
                        ParentId = null,
                        QuoteId = parameter.Quote.QuoteId
                    };
                    context.QuoteScope.Add(quoteScope);
                    #endregion

                    #region Thêm người tham gia

                    List<QuoteParticipantMapping> listParticipantMapping = new List<QuoteParticipantMapping>();
                    parameter.ListParticipant.ForEach(item =>
                    {
                        var quoteParticipantMapping = new QuoteParticipantMapping();
                        quoteParticipantMapping.QuoteParticipantMappingId = Guid.NewGuid();
                        quoteParticipantMapping.EmployeeId = item;
                        quoteParticipantMapping.QuoteId = parameter.Quote.QuoteId;

                        listParticipantMapping.Add(quoteParticipantMapping);
                    });
                    context.QuoteParticipantMapping.AddRange(listParticipantMapping);

                    #endregion

                    #region Thêm thông tin bổ sung cho báo giá

                    parameter.ListAdditionalInformation.ForEach(item =>
                    {
                        item.AdditionalInformationId = Guid.NewGuid();
                        item.ObjectId = parameter.Quote.QuoteId;
                        item.ObjectType = "QUOTE";
                        item.Active = true;
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = null;
                        item.UpdatedDate = null;
                    });
                    var listAdditionalInformation = new List<AdditionalInformation>();
                    parameter.ListAdditionalInformation.ForEach(additional =>
                    {
                        listAdditionalInformation.Add(additional.ToEntity());
                    });
                    context.AdditionalInformation.AddRange(listAdditionalInformation);

                    #endregion

                    #region Thêm quà khuyến mãi

                    var listPromotionObjectApply = new List<PromotionObjectApply>();
                    parameter.ListPromotionObjectApply.ForEach(item =>
                    {
                        var promotionObjectApply = new PromotionObjectApply();
                        promotionObjectApply.PromotionObjectApplyId = Guid.NewGuid();
                        promotionObjectApply.ObjectId = parameter.Quote.QuoteId;
                        promotionObjectApply.ObjectType = "QUOTE";
                        promotionObjectApply.PromotionId = item.PromotionId;
                        promotionObjectApply.ConditionsType = item.ConditionsType;
                        promotionObjectApply.PropertyType = item.PropertyType;
                        promotionObjectApply.NotMultiplition = item.NotMultiplition;
                        promotionObjectApply.PromotionMappingId = item.PromotionMappingId;
                        promotionObjectApply.ProductId = item.ProductId;
                        promotionObjectApply.SoLuongTang = item.SoLuongTang;
                        promotionObjectApply.LoaiGiaTri = item.LoaiGiaTri;
                        promotionObjectApply.GiaTri = item.GiaTri;
                        promotionObjectApply.Amount = item.Amount;
                        promotionObjectApply.SoTienTu = item.SoTienTu;

                        listPromotionObjectApply.Add(promotionObjectApply);
                    });

                    context.PromotionObjectApply.AddRange(listPromotionObjectApply);

                    #endregion

                    #region Thêm plan

                    List<QuotePlan> listQuotePlan = new List<QuotePlan>();
                    parameter.QuotePlans.ForEach(item =>
                    {
                        var quotePlan = new QuotePlan();

                        quotePlan.PlanId = Guid.NewGuid();
                        quotePlan.Tt = item.Tt;
                        quotePlan.Finished = item.Finished;
                        quotePlan.ExecTime = item.ExecTime;
                        quotePlan.SumExecTime = item.SumExecTime;
                        quotePlan.QuoteId = parameter.Quote.QuoteId;
                        quotePlan.TenantId = item.TenantId;
                        listQuotePlan.Add(quotePlan);
                    });
                    context.QuotePlan.AddRange(listQuotePlan);

                    #endregion

                    //#region Thêm phạm vì triển khai

                    //List<QuoteScope> listQuoteScope = new List<QuoteScope>();
                    //parameter.QuoteScopes.ForEach(item =>
                    //{
                    //    var quoteScope = new QuoteScope();

                    //    quoteScope.ScopeId = Guid.NewGuid();
                    //    quoteScope.Tt = item.Tt;
                    //    quoteScope.Category = item.Category;
                    //    quoteScope.Description = item.Description;
                    //    quoteScope.QuoteId = parameter.Quote.QuoteId;
                    //    quoteScope.TenantId = item.TenantId;
                    //    listQuoteScope.Add(quoteScope);
                    //});
                    //context.QuoteScope.AddRange(listQuoteScope);

                    //#endregion


                    context.SaveChanges();

                    #region Log

                    LogHelper.AuditTrace(context, "Create", "Quote", parameter.Quote.QuoteId, parameter.UserId);

                    #endregion

                    #region Thêm điều khoản thanh toán

                    List<QuotePaymentTerm> quotePaymentTerms = new List<QuotePaymentTerm>();
                    parameter.ListQuotePaymentTerm.ForEach(item =>
                    {
                        var paymentTerm = new QuotePaymentTerm();

                        paymentTerm.PaymentTermId = Guid.NewGuid();
                        paymentTerm.QuoteId = parameter.Quote.QuoteId;
                        paymentTerm.OrderNumber = item.OrderNumber;
                        paymentTerm.Milestone = item.Milestone;
                        paymentTerm.PaymentPercentage = item.PaymentPercentage;
                        paymentTerm.CreatedDate = DateTime.Now;
                        paymentTerm.CreatedById = parameter.UserId;

                        quotePaymentTerms.Add(paymentTerm);
                    });

                    context.QuotePaymentTerm.AddRange(quotePaymentTerms);
                    context.SaveChanges();

                    #endregion

                    #endregion

                    #region Gửi thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.Quote, "CRE", new Quote(),
                        quote, true);

                    #endregion
                }

                List<QuoteDocumentEntityModel> listQuoteDocumentEntityModels = new List<QuoteDocumentEntityModel>();
                for (int i = 0; i < listQuoteDocuments.Count; i++)
                {
                    var quoteEntityModel = new QuoteDocumentEntityModel();
                    quoteEntityModel.QuoteId = listQuoteDocuments[i].QuoteId;
                    quoteEntityModel.QuoteDocumentId = listQuoteDocuments[i].QuoteDocumentId;
                    quoteEntityModel.DocumentName = listQuoteDocuments[i].DocumentName;
                    quoteEntityModel.DocumentSize = listQuoteDocuments[i].DocumentSize;
                    quoteEntityModel.DocumentUrl = listQuoteDocuments[i].DocumentUrl;
                    quoteEntityModel.Active = listQuoteDocuments[i].Active;
                    quoteEntityModel.CreatedById = listQuoteDocuments[i].CreatedById;
                    quoteEntityModel.CreatedDate = listQuoteDocuments[i].CreatedDate;
                    quoteEntityModel.UpdatedById = listQuoteDocuments[i].UpdatedById;
                    quoteEntityModel.UpdatedDate = listQuoteDocuments[i].UpdatedDate;

                    listQuoteDocumentEntityModels.Add(quoteEntityModel);
                }

                return new CreateQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Quote.CREATE_SUCCESS,
                    QuoteID = parameter.Quote.QuoteId,
                    ListQuoteDocument = listQuoteDocumentEntityModels
                };

            }
            catch (Exception ex)
            {
                return new CreateQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Quote.CREATE_FAIL + ex.Message
                };
            }
        }

        public UploadOuoteDocumentResult UploadOuoteDocument(UploadOuoteDocumentParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();

                List<QuoteDocument> listQuoteDocuments = new List<QuoteDocument>();
                var listQuoteParameter = new List<QuoteDocument>();
                parameter.FileList.ForEach(item =>
                {
                    listQuoteParameter.Add(item.ToEntity());
                });
                // Xoa list file hien cu cua quote
                var quoteDocumentList = context.QuoteDocument.Where(x => x.QuoteId == parameter.QuoteId).ToList();
                context.QuoteDocument.RemoveRange(quoteDocumentList);

                listQuoteParameter.ForEach(item =>
                {
                    string folderName = "FileUpload";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                    if (item.QuoteDocumentId == null || item.QuoteDocumentId == Guid.Empty)
                        item.QuoteDocumentId = Guid.NewGuid();
                });

                var quote = context.Quote.FirstOrDefault(x => x.QuoteId == parameter.QuoteId);

                if (quote != null)
                {
                    quote.QuoteDocument = listQuoteParameter;
                    context.Quote.Update(quote);
                    context.SaveChanges();
                }

                listQuoteDocuments.AddRange(listQuoteParameter);

                context.SaveChanges();

                List<QuoteDocumentEntityModel> listQuoteDocumentEntityModels = new List<QuoteDocumentEntityModel>();
                for (int i = 0; i < listQuoteDocuments.Count; i++)
                {
                    var quoteEntityModel = new QuoteDocumentEntityModel();
                    quoteEntityModel.QuoteId = listQuoteDocuments[i].QuoteId;
                    quoteEntityModel.QuoteDocumentId = listQuoteDocuments[i].QuoteDocumentId;
                    quoteEntityModel.DocumentName = listQuoteDocuments[i].DocumentName;
                    quoteEntityModel.DocumentSize = listQuoteDocuments[i].DocumentSize;
                    quoteEntityModel.DocumentUrl = listQuoteDocuments[i].DocumentUrl;
                    quoteEntityModel.Active = listQuoteDocuments[i].Active;
                    quoteEntityModel.CreatedById = listQuoteDocuments[i].CreatedById;
                    quoteEntityModel.CreatedDate = listQuoteDocuments[i].CreatedDate;
                    quoteEntityModel.UpdatedById = listQuoteDocuments[i].UpdatedById;
                    quoteEntityModel.UpdatedDate = listQuoteDocuments[i].UpdatedDate;

                    var user = listAllUser.FirstOrDefault(x => x.UserId == quoteEntityModel.UpdatedById);
                    if (user != null)
                    {
                        quoteEntityModel.UploadByName = user.UserName;
                    }

                    listQuoteDocumentEntityModels.Add(quoteEntityModel);
                }

                return new UploadOuoteDocumentResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tải file thành công",
                    ListQuoteDocument = listQuoteDocumentEntityModels,
                };
            }
            catch (Exception e)
            {
                return new UploadOuoteDocumentResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public ExportPdfQuotePDFResult ExportPdfQuote(ExportPdfQuoteParameter parameter)
        {
            throw new NotImplementedException();
        }

        public GetAllQuoteResult GetAllQuote(GetAllQuoteParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.QUOTE, "GetAllQuote", parameter.UserId);
                var customerOrder = context.CustomerOrder.ToList();
                parameter.QuoteCode = parameter.QuoteCode == null ? parameter.QuoteCode : parameter.QuoteCode.Trim();
                parameter.ProductCode = parameter.ProductCode == null ? parameter.ProductCode : parameter.ProductCode.Trim();

                var quoteList = (from or in context.Quote
                                     //join os in context.Category on or.StatusId equals os.CategoryId -> Comment By Hung
                                 where (or.Active == true) && (parameter.QuoteStatusId.Count == 0 || parameter.QuoteStatusId.Contains(or.StatusId))
                                 && (string.IsNullOrEmpty(parameter.QuoteCode) || or.QuoteCode.Contains(parameter.QuoteCode))
                                 select new QuoteEntityModel
                                 {
                                     QuoteId = or.QuoteId,
                                     QuoteCode = or.QuoteCode,
                                     QuoteDate = or.QuoteDate,
                                     Description = or.Description,
                                     Note = or.Note,
                                     ObjectTypeId = or.ObjectTypeId,
                                     ObjectType = or.ObjectType,
                                     PaymentMethod = or.PaymentMethod,
                                     DaysAreOwed = or.DaysAreOwed,
                                     MaxDebt = or.MaxDebt,
                                     ExpirationDate = or.ExpirationDate,
                                     ReceivedDate = or.ReceivedDate,
                                     ReceivedHour = or.ReceivedHour,
                                     RecipientName = or.RecipientName,
                                     LocationOfShipment = or.LocationOfShipment,
                                     ShippingNote = or.ShippingNote,
                                     RecipientPhone = or.RecipientPhone,
                                     RecipientEmail = or.RecipientEmail,
                                     PlaceOfDelivery = or.PlaceOfDelivery,
                                     Amount = or.Amount,
                                     DiscountValue = or.DiscountValue,
                                     StatusId = or.StatusId,
                                     CreatedById = or.CreatedById,
                                     CreatedDate = or.CreatedDate,
                                     UpdatedById = or.UpdatedById,
                                     UpdatedDate = or.UpdatedDate,
                                     Active = or.Active,
                                     DiscountType = or.DiscountType,
                                     CountQuoteInOrder = CountQuoteInCustomerOrder(or.QuoteId, customerOrder),
                                     //SellerAvatarUrl = c.AvatarUrl,-> Comment By Hung
                                     //SellerFirstName = e.EmployeeName,-> Comment By Hung
                                     //SellerLastName = c.LastName,-> Comment By Hung
                                     QuoteStatusName = "",//os.CategoryName,-> Comment By Hung
                                     //CustomerName = GetCustomerName(or.ObjectType, or.ObjectTypeId)-> Comment By Hung
                                 }).OrderByDescending(or => or.CreatedDate).ToList();

                #region Add by Hung
                if (quoteList != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    quoteList.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    quoteList.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                            item.QuoteStatusName = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value).CategoryName;
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }
                #endregion

                return new GetAllQuoteResult
                {
                    QuoteList = quoteList,
                    MessageCode = CommonMessage.Quote.SEARCH_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetAllQuoteResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }

        }
        public int CountQuoteInCustomerOrder(Guid quoteId, List<CustomerOrder> customerOrder)
        {
            var count = customerOrder.Where(c => c.QuoteId == quoteId).Count();
            return count;
        }
        public GetTop3QuotesOverdueResult GetTop3QuotesOverdue(GetTop3QuotesOverdueParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3QuotesOverdue", parameter.UserId);
                var employeeList = context.Employee.ToList();
                var customerList = context.Customer.ToList();
                var quoteDataList = context.Quote.Where(x => x.Active == true).ToList();
                var categoryList = context.Category.Where(x => x.Active == true).ToList();

                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var quoteList = new List<GetTop3QuotesOverdueModel>();
                List<string> listCategoryCode = new List<string>() { "CHO" };
                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI").CategoryTypeId;
                var listStatusQuote = context.Category.Where(x =>
                        x.Active == true && x.CategoryTypeId == categoryTypeId &&
                        listCategoryCode.Contains(x.CategoryCode)).Select(y => y.CategoryId)
                    .ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = employeeList.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                if (!employee.IsManager)
                {
                    if (appName == "VNS")
                    {
                        quoteList = (from oq in quoteDataList
                                     join oe in employeeList on parameter.PersonInChangeId equals oe.EmployeeId
                                     join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                     join ca in categoryList on oq.StatusId equals ca.CategoryId
                                     where (oq.Seller == employee.EmployeeId &&
                                            oq.UpdatedDate != null &&
                                            oq.UpdatedDate?.AddDays(oq.EffectiveQuoteDate ?? 0) < DateTime.Now.Date &&
                                            (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                             listStatusQuote.Contains(oq.StatusId.Value)))
                                     select new GetTop3QuotesOverdueModel
                                     {
                                         QuoteId = oq.QuoteId,
                                         QuoteCode = oq.QuoteCode,
                                         QuoteName = oq.QuoteName,
                                         QuoteDate = oq.QuoteDate,
                                         SendQuoteDate = oq.SendQuoteDate,
                                         Amount = oq.Amount,
                                         CustomerName = oc.CustomerName,
                                         EmployeeName = oe.EmployeeName,
                                         IntendedQuoteDate = oq.IntendedQuoteDate,
                                         ExpirationDate = oq.ExpirationDate,
                                         UpdatedDate = oq.UpdatedDate,
                                         Status = ca.CategoryName,
                                         EffectiveQuoteDate = oq.EffectiveQuoteDate,
                                         DiscountType = oq.DiscountType,
                                         DiscountValue = oq.DiscountValue,
                                         TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId, oq.DiscountType,
                                             oq.DiscountValue, oq.Vat, listQuoteDetail, listQuoteCostDetail,
                                             listPromotionObjectApply, appName)
                                     }).OrderBy(oq =>
                                     DateTime.Parse(oq.UpdatedDate.ToString())
                                         .AddDays(double.Parse(oq.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                    }
                    else
                    {
                        quoteList = (from oq in quoteDataList
                                     join oe in employeeList on parameter.PersonInChangeId equals oe.EmployeeId
                                     join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                     join ca in categoryList on oq.StatusId equals ca.CategoryId
                                     join no in context.Note on oq.QuoteId equals no.ObjectId
                                     where (oq.Seller == employee.EmployeeId &&
                                            no.Description != null &&
                                            no.Description.StartsWith("Chuyển trạng thái đã duyệt thành công với lý do:") &&
                                            no.CreatedDate.AddDays(oq.EffectiveQuoteDate ?? 0) < DateTime.Now.Date &&
                                            oq.UpdatedDate != null)
                                     select new GetTop3QuotesOverdueModel
                                     {
                                         QuoteId = oq.QuoteId,
                                         QuoteCode = oq.QuoteCode,
                                         QuoteName = oq.QuoteName,
                                         QuoteDate = oq.QuoteDate,
                                         SendQuoteDate = oq.SendQuoteDate,
                                         Amount = oq.Amount,
                                         CustomerName = oc.CustomerName,
                                         EmployeeName = oe.EmployeeName,
                                         IntendedQuoteDate = oq.IntendedQuoteDate,
                                         ExpirationDate = oq.ExpirationDate,
                                         UpdatedDate = no.CreatedDate,
                                         Status = ca.CategoryName,
                                         EffectiveQuoteDate = oq.EffectiveQuoteDate,
                                         DiscountType = oq.DiscountType,
                                         DiscountValue = oq.DiscountValue,
                                         TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId, oq.DiscountType,
                                             oq.DiscountValue, oq.Vat, listQuoteDetail, listQuoteCostDetail,
                                             listPromotionObjectApply, appName)
                                     }).OrderByDescending(row =>
                                     DateTime.Parse(row.UpdatedDate.ToString())
                                         .AddDays(double.Parse(row.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                    }
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    employeeList = employeeList
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    if (appName == "VNS")
                    {
                        quoteList = (from oq in quoteDataList
                                     join oe in employeeList on oq.Seller equals oe.EmployeeId
                                     join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                     join ca in categoryList on oq.StatusId equals ca.CategoryId
                                     where (
                                         oq.UpdatedDate?.AddDays(oq.EffectiveQuoteDate ?? 0) < DateTime.Now.Date &&
                                         oq.UpdatedDate != null &&
                                         (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                          listStatusQuote.Contains(oq.StatusId.Value)))
                                     select new GetTop3QuotesOverdueModel
                                     {
                                         QuoteId = oq.QuoteId,
                                         QuoteCode = oq.QuoteCode,
                                         QuoteName = oq.QuoteName,
                                         QuoteDate = oq.QuoteDate,
                                         SendQuoteDate = oq.SendQuoteDate,
                                         Amount = oq.Amount,
                                         CustomerName = oc.CustomerName,
                                         EmployeeName = oe.EmployeeName,
                                         IntendedQuoteDate = oq.IntendedQuoteDate,
                                         ExpirationDate = oq.ExpirationDate,
                                         Status = ca.CategoryName,
                                         UpdatedDate = oq.UpdatedDate,
                                         EffectiveQuoteDate = oq.EffectiveQuoteDate,
                                         DiscountType = oq.DiscountType,
                                         DiscountValue = oq.DiscountValue,
                                         TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId, oq.DiscountType,
                                             oq.DiscountValue, oq.Vat, listQuoteDetail, listQuoteCostDetail,
                                             listPromotionObjectApply, appName)
                                     }).OrderBy(oq =>
                                     DateTime.Parse(oq.UpdatedDate.ToString())
                                         .AddDays(double.Parse(oq.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                    }
                    else
                    {
                        quoteList = (from oq in quoteDataList
                                     join oe in employeeList on oq.Seller equals oe.EmployeeId
                                     join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                     join ca in categoryList on oq.StatusId equals ca.CategoryId
                                     join no in context.Note on oq.QuoteId equals no.ObjectId
                                     where (no.Description != null &&
                                            no.Description.StartsWith("Chuyển trạng thái đã duyệt thành công với lý do:") &&
                                            no.CreatedDate.AddDays(oq.EffectiveQuoteDate ?? 0) < DateTime.Now.Date &&
                                            oq.UpdatedDate != null)
                                     select new GetTop3QuotesOverdueModel
                                     {
                                         QuoteId = oq.QuoteId,
                                         QuoteCode = oq.QuoteCode,
                                         QuoteName = oq.QuoteName,
                                         QuoteDate = oq.QuoteDate,
                                         SendQuoteDate = oq.SendQuoteDate,
                                         Amount = oq.Amount,
                                         CustomerName = oc.CustomerName,
                                         EmployeeName = oe.EmployeeName,
                                         IntendedQuoteDate = oq.IntendedQuoteDate,
                                         ExpirationDate = oq.ExpirationDate,
                                         Status = ca.CategoryName,
                                         UpdatedDate = no.CreatedDate,
                                         EffectiveQuoteDate = oq.EffectiveQuoteDate,
                                         DiscountType = oq.DiscountType,
                                         DiscountValue = oq.DiscountValue,
                                         TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId, oq.DiscountType,
                                             oq.DiscountValue, oq.Vat, listQuoteDetail, listQuoteCostDetail,
                                             listPromotionObjectApply, appName)
                                     }).OrderByDescending(row =>
                                     DateTime.Parse(row.UpdatedDate.ToString())
                                         .AddDays(double.Parse(row.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                    }
                }

                quoteList.ForEach(x =>
                {
                    x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                        x.TotalAmountAfterVat, listPromotionObjectApply);
                });

                return new GetTop3QuotesOverdueResult
                {
                    QuoteList = quoteList,
                    MessageCode = CommonMessage.Quote.SEARCH_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetTop3QuotesOverdueResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public GetTop3WeekQuotesOverdueResult GetTop3WeekQuotesOverdue(GetTop3WeekQuotesOverdueParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
                DateTime firstDate = FirstDateOfWeek();
                DateTime lastDate = LastDateOfWeek();

                var employeeList = context.Employee.ToList();
                var customerList = context.Customer.ToList();
                var categoryList = context.Category.ToList();
                var quoteDataList = context.Quote.Where(x => x.Active == true).ToList();

                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var quoteList = new List<GetTop3WeekQuotesOverdueModel>();

                List<string> listCategoryCode = new List<string>() { "MTA", "CHO", "DTH", "DLY", "TUCHOI", "HOA" };
                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI").CategoryTypeId;
                var listStatusQuote = context.Category.Where(x =>
                        x.Active == true && x.CategoryTypeId == categoryTypeId &&
                        listCategoryCode.Contains(x.CategoryCode)).Select(y => y.CategoryId)
                    .ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = employeeList.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);


                if (!employee.IsManager)
                {
                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on parameter.PersonInChangeId equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where oq.StatusId != null && (oq.Seller == employee.EmployeeId &&
                                                               firstDate.Date <= DateTime.Parse(oq.IntendedQuoteDate.ToString()).Date &&
                                                               DateTime.Parse(oq.IntendedQuoteDate.ToString()).Date <= lastDate.Date &&
                                                               (listStatusQuote.Count == 0 ||
                                                                listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3WeekQuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = CalculatorDiscount(oq.Amount, oq.DiscountType.Value, oq.DiscountValue.Value),
                                     EmployeeName = oe.EmployeeName,
                                     CustomerName = oc.CustomerName,
                                     QuoteName = oq.QuoteName,
                                     Status = ca.CategoryName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     IntendedQuoteDateWeek = DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7),
                                     DiscountType = oq.DiscountType,
                                     DiscountValue = oq.DiscountValue,
                                     TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId, oq.DiscountType, oq.DiscountValue, oq.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                                 }).OrderBy(or => or.IntendedQuoteDateWeek).Take(5).ToList();
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    employeeList = employeeList
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on oq.Seller equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where (firstDate.Date <= DateTime.Parse(oq.IntendedQuoteDate.ToString()).Date &&
                                     DateTime.Parse(oq.IntendedQuoteDate.ToString()).Date <= lastDate.Date &&
                                     (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                      listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3WeekQuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = CalculatorDiscount(oq.Amount, oq.DiscountType.Value, oq.DiscountValue.Value),
                                     EmployeeName = oe.EmployeeName,
                                     CustomerName = oc.CustomerName,
                                     QuoteName = oq.QuoteName,
                                     Status = ca.CategoryName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     IntendedQuoteDateWeek = DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7),
                                     DiscountType = oq.DiscountType,
                                     DiscountValue = oq.DiscountValue,
                                     TotalAmountAfterVat = CalculateTotalAmountAfterVat(oq.QuoteId,
                                         oq.DiscountType,
                                         oq.DiscountValue,
                                         oq.Vat,
                                         listQuoteDetail,
                                         listQuoteCostDetail,
                                         listPromotionObjectApply,
                                         appName)
                                 }).OrderBy(or => or.IntendedQuoteDateWeek).Take(5).ToList();
                }

                quoteList.ForEach(x =>
                {
                    x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                        x.TotalAmountAfterVat, listPromotionObjectApply);
                });

                return new GetTop3WeekQuotesOverdueResult
                {
                    QuoteList = quoteList,
                    MessageCode = CommonMessage.Quote.SEARCH_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetTop3WeekQuotesOverdueResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }
        }

        private decimal CalculatorDiscount(decimal amount, bool discountType, decimal discountValue)
        {
            decimal result = 0;

            if (discountType)
            {
                //Chiết khấu được tính theo %
                result = amount - (amount * discountValue) / 100;
            }
            else
            {
                //Chiết khấu được tính theo tiền mặt
                result = amount - discountValue;
            }

            return result;
        }

        public GetTop3PotentialCustomersResult GetTop3PotentialCustomers(GetTop3PotentialCustomersParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
                var listLead = new List<GetTop3PotentialCustomersModel>();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listEmp = context.Employee.ToList();
                var statusCustomer = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "THA");
                var statusDD = context.Category.FirstOrDefault(c => c.CategoryTypeId == statusCustomer.CategoryTypeId && c.Active == true && c.CategoryCode == "HDO");

                if (!employee.IsManager)
                {
                    //Lấy list khách hàng tiềm năng do người đang đăng nhập phụ trách
                    //listLead = context.Lead.Where(x =>
                    //        x.Active == true && x.PersonInChargeId == employee.EmployeeId)
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    listLead = context.Customer.Where(x =>
                            x.Active == true && x.PersonInChargeId == employee.EmployeeId && x.StatusId == statusDD.CategoryId)
                        .Select(y => new GetTop3PotentialCustomersModel
                        {
                            LeadId = y.CustomerId,
                            ContactId = Guid.Empty,
                            LeadFirstName = "",
                            LeadLastName = "",
                            Email = "",
                            Phone = "",
                            PersonInChargeId = y.PersonInChargeId.Value,
                            PersonInChargeName = "",
                            CreatedDate = y.CreatedDate
                        }).ToList();

                    //Lấy list khách hàng tiềm năng không có người phụ trách nhưng do người đang đăng nhập tạo ra
                    //var listLead2 = context.Lead
                    //    .Where(x => x.Active == true && x.PersonInChargeId == null &&
                    //                x.CreatedById.ToString().ToLower() == user.UserId.ToString().ToLower())
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    var listLead2 = context.Customer
                       .Where(x => x.Active == true && x.PersonInChargeId == null && x.StatusId == statusDD.CategoryId &&
                                   x.CreatedById.ToString().ToLower() == user.UserId.ToString().ToLower())
                       .Select(y => new GetTop3PotentialCustomersModel
                       {
                           LeadId = y.CustomerId,
                           ContactId = Guid.Empty,
                           LeadFirstName = "",
                           LeadLastName = "",
                           Email = "",
                           Phone = "",
                           PersonInChargeId = y.PersonInChargeId.Value,
                           PersonInChargeName = "",
                           CreatedDate = y.CreatedDate
                       }).ToList();

                    listLead.AddRange(listLead2);
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var employeeList = context.Employee
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    var listEmployeeId = employeeList.Select(x => x.EmployeeId).ToList();

                    //Lấy list khách hàng tiềm năng do người đang đăng nhập, hoặc nhân viên cấp dưới phụ trách
                    //listLead = context.Lead.Where(x =>
                    //        x.Active == true &&
                    //        (listEmployeeId.Contains(x.PersonInChargeId.Value)))
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    listLead = context.Customer.Where(x =>
                           x.Active == true && x.StatusId == statusDD.CategoryId &&
                           (listEmployeeId.Contains(x.PersonInChargeId.Value)))
                       .Select(y => new GetTop3PotentialCustomersModel
                       {
                           LeadId = y.CustomerId,
                           ContactId = Guid.Empty,
                           LeadFirstName = "",
                           LeadLastName = "",
                           Email = "",
                           Phone = "",
                           PersonInChargeId = y.PersonInChargeId.Value,
                           PersonInChargeName = "",
                           CreatedDate = y.CreatedDate
                       }).ToList();

                    /*
                     * Lấy list khách hàng tiềm năng không có người phụ trách nhưng do người đang đăng nhập hoặc
                     * nhân viên cấp dưới tạo ra
                     */

                    //var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value))
                    //    .Select(y => y.UserId.ToString().ToLower()).ToList();
                    var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value))
                       .Select(y => y.UserId).ToList();

                    //var listLead2 = context.Lead.Where(x => x.Active == true && x.PersonInChargeId == null &&
                    //                                        listUserId.Contains(x.CreatedById))
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    var listLead2 = context.Customer.Where(x => x.Active == true && x.PersonInChargeId == null && x.StatusId == statusDD.CategoryId &&
                                                            listUserId.Contains(x.CreatedById))
                        .Select(y => new GetTop3PotentialCustomersModel
                        {
                            LeadId = y.CustomerId,
                            ContactId = Guid.Empty,
                            LeadFirstName = "",
                            LeadLastName = "",
                            Email = "",
                            Phone = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PersonInChargeName = "",
                            CreatedDate = y.CreatedDate
                        }).ToList();

                    listLead.AddRange(listLead2);
                }

                listLead = listLead.OrderByDescending(z => z.CreatedDate).Take(5).ToList();

                #region Lấy thông tin thêm cho listLead

                var listLeadId = listLead.Select(x => x.LeadId).ToList();
                var listContactLead = context.Contact.Where(x => x.ObjectType == "CUS" && (listLeadId.Contains(x.ObjectId)))
                    .ToList();

                listLead.ForEach(item =>
                {
                    var contactLead = listContactLead.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = contactLead != null
                        ? (contactLead.FirstName != null ? (contactLead.FirstName.Trim()) : "")
                        : "";
                    var lastName = contactLead != null
                       ? (contactLead.LastName != null ? (contactLead.LastName.Trim()) : "")
                       : "";
                    var email = contactLead != null
                        ? (contactLead.Email != null ? (contactLead.Email.Trim()) : "")
                        : "";
                    var phone = contactLead != null
                        ? (contactLead.Phone != null ? (contactLead.Phone.Trim()) : "")
                        : "";
                    var personInCharge = listEmp.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                    var personInChargeName = personInCharge != null
                        ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "")
                        : "";

                    item.ContactId = contactLead.ContactId;
                    item.LeadFirstName = firstName;
                    item.LeadLastName = lastName;
                    item.Email = email;
                    item.Phone = phone;
                    item.PersonInChargeName = personInChargeName;
                });

                #endregion

                return new GetTop3PotentialCustomersResult
                {
                    QuoteList = listLead,
                    MessageCode = CommonMessage.Quote.SEARCH_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetTop3PotentialCustomersResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public GetTotalAmountQuoteResult GetTotalAmountQuote(GetTotalAmountQuoteParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
                List<Guid> leadIdList = new List<Guid>();
                var leadList = context.Lead.Where(w => w.PersonInChargeId == parameter.PersonInChangeId).ToList();

                leadList.ForEach(item =>
                {
                    leadIdList.Add(item.LeadId);
                });

                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.PersonInChangeId);
                bool isManager = employee.IsManager;

                var totalAmount = new List<Quote>();
                if (!isManager)
                {
                    totalAmount = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                }
                else
                {
                    var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                    if (organization.ParentId == null)
                    {
                        totalAmount = context.Quote.Where(w => DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                    }
                    else
                    {
                        var parentId = context.Organization.Where(o => o.ParentId == employee.OrganizationId).ToList();
                        List<Guid> organizationId = new List<Guid>();
                        organizationId.Add(organization.OrganizationId);

                        foreach (var item in parentId)
                        {
                            organizationId.Add(item.OrganizationId);
                        }

                        List<Guid> employIdList = new List<Guid>();
                        foreach (var item in organizationId)
                        {
                            var idlist = context.Employee.Where(e => e.OrganizationId == item).ToList();
                            foreach (var eml in idlist)
                            {
                                employIdList.Add(eml.EmployeeId);
                            }
                        }

                        foreach (var perId in employIdList)
                        {
                            var quoteIdList = context.Quote.Where(w => w.Seller == perId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                            foreach (var quote in quoteIdList)
                            {
                                totalAmount.Add(quote);
                            }
                        }
                    }
                }

                var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
                var categoryList = context.Category.Where(w => w.CategoryTypeId == categoryTypeID && (w.CategoryCode != "DTR" && w.CategoryCode != "HUY") && w.Active == true).ToList();
                var listqk = new List<decimal>();

                totalAmount.ForEach(item =>
                {
                    if (categoryList.FirstOrDefault(f => f.CategoryId == item.StatusId) != null)
                        listqk.Add(CalculatorDiscount(item.Amount, item.DiscountType.Value, item.DiscountValue.Value));
                });


                GetTotalAmountQuoteModel quoteList = new GetTotalAmountQuoteModel()
                {
                    TotalAmount = listqk.Sum()
                };

                return new GetTotalAmountQuoteResult
                {
                    ToTalAmount = quoteList,
                    MessageCode = CommonMessage.Quote.SEARCH_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetTotalAmountQuoteResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public UpdateQuoteResult UpdateQuote(UpdateQuoteParameter parameter)
        {
            try
            {
                var oldQuote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.Quote.QuoteId);
                var oldAmount = oldQuote.Amount;
                using (var transaction = context.Database.BeginTransaction())
                {
                    #region Delete all item Relation 
                    var List_Delete_QuoteProductDetailProductAttributeValue = new List<QuoteProductDetailProductAttributeValue>();
                    var List_Delete_QuoteDetail = new List<QuoteDetail>();
                    List_Delete_QuoteDetail = context.QuoteDetail.Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                    List_Delete_QuoteDetail.ForEach(item =>
                    {
                        var objectQuoteDetail = new QuoteDetail();
                        if (item.QuoteDetailId != Guid.Empty)
                        {
                            objectQuoteDetail = item;
                            var QuoteProductDetailProductAttributeValueList = context.QuoteProductDetailProductAttributeValue.Where(OPDPAV => OPDPAV.QuoteDetailId == item.QuoteDetailId).ToList();
                            List_Delete_QuoteProductDetailProductAttributeValue.AddRange(QuoteProductDetailProductAttributeValueList);
                        }
                    });

                    context.QuoteProductDetailProductAttributeValue.RemoveRange(List_Delete_QuoteProductDetailProductAttributeValue);
                    context.SaveChanges();
                    context.QuoteDetail.RemoveRange(List_Delete_QuoteDetail);
                    context.SaveChanges();

                    context.Quote.Remove(oldQuote);
                    context.SaveChanges();
                    #endregion

                    #region Create new Order base on Old Order
                    parameter.QuoteDetail.ForEach(item =>
                    {
                        item.QuoteDetailId = Guid.NewGuid();
                        if (item.QuoteProductDetailProductAttributeValue != null)
                        {
                            foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                            {
                                itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });
                    var quote = parameter.Quote.ToEntity();
                    var listQuoteDetail = new List<QuoteDetail>();
                    parameter.QuoteDetail.ForEach(item =>
                    {
                        listQuoteDetail.Add(item.ToEntity());
                    });
                    quote.QuoteDetail = listQuoteDetail;
                    context.Quote.Add(quote);
                    context.SaveChanges();

                    transaction.Commit();
                    #endregion

                    return new UpdateQuoteResult
                    {
                        MessageCode = CommonMessage.Quote.EDIT_QUOTE_SUCCESS,
                        QuoteID = parameter.Quote.QuoteId,
                        StatusCode = HttpStatusCode.OK,
                    };

                }

            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    MessageCode = CommonMessage.Quote.EDIT_QUOTE_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetQuoteByIDResult GetQuoteByID(GetQuoteByIDParameter parameter)
        {
            var customerOrder = context.CustomerOrder.ToList();

            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                #region Get Quote By ID

                var quoteObject = (from or in context.Quote
                                   where or.QuoteId == parameter.QuoteId
                                   select new QuoteEntityModel
                                   {
                                       QuoteId = or.QuoteId,
                                       BankAccountId = or.BankAccountId,
                                       QuoteCode = or.QuoteCode,
                                       QuoteDate = or.QuoteDate,
                                       SendQuoteDate = or.SendQuoteDate,
                                       EffectiveQuoteDate = or.EffectiveQuoteDate,
                                       IntendedQuoteDate = or.IntendedQuoteDate,
                                       Description = or.Description,
                                       Note = or.Note,
                                       ObjectTypeId = or.ObjectTypeId,
                                       ObjectType = or.ObjectType,
                                       PaymentMethod = or.PaymentMethod,
                                       DaysAreOwed = or.DaysAreOwed,
                                       MaxDebt = or.MaxDebt,
                                       ReceivedDate = or.ReceivedDate,
                                       ReceivedHour = or.ReceivedHour,
                                       RecipientName = or.RecipientName,
                                       LocationOfShipment = or.LocationOfShipment,
                                       ShippingNote = or.ShippingNote,
                                       RecipientPhone = or.RecipientPhone,
                                       RecipientEmail = or.RecipientEmail,
                                       PlaceOfDelivery = or.PlaceOfDelivery,
                                       Amount = or.Amount,
                                       Seller = or.Seller,
                                       CustomerContactId = or.CustomerContactId,
                                       DiscountValue = or.DiscountValue,
                                       StatusId = or.StatusId,
                                       CreatedById = or.CreatedById,
                                       CreatedDate = or.CreatedDate,
                                       UpdatedById = or.UpdatedById,
                                       UpdatedDate = or.UpdatedDate,
                                       Active = or.Active,
                                       DiscountType = or.DiscountType,
                                       SellerAvatarUrl = "",
                                       SellerFirstName = "",
                                       SellerLastName = "",
                                       CountQuoteInOrder = CountQuoteInCustomerOrder(or.QuoteId, customerOrder)
                                   }).FirstOrDefault();

                #endregion

                #region Get QuoteDetail with OrderDetailType == 0 and QuoteId (Sản phẩm dịch vụ)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = appName == "VNS"
                                                    ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType)
                                                    : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            listProductId.Add(item.ProductId.Value);
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            listCategoryId.Add(item.UnitId.Value);
                    });

                    var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                    var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                    var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId)
                                .CategoryName;
                    });
                }

                #endregion

                #region Get QuoteDetail with OrderDetailType == 1 and QuoteId (Chi phí phát sinh khác)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                    cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                        item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                    });
                }

                #endregion

                listQuoteObjectType0.AddRange(listQuoteObjectType1);

                #region Get QuoteDocument with QuoteId

                var listQuoteDocument = (from QD in context.QuoteDocument
                                         where QD.QuoteId == parameter.QuoteId
                                         select new QuoteDocumentEntityModel
                                         {
                                             QuoteDocumentId = QD.QuoteDocumentId,
                                             QuoteId = QD.QuoteId,
                                             DocumentName = QD.DocumentName,
                                             DocumentSize = QD.DocumentSize,
                                             DocumentUrl = QD.DocumentUrl,
                                             CreatedById = QD.CreatedById,
                                             CreatedDate = QD.CreatedDate,
                                             UpdatedById = QD.UpdatedById,
                                             UpdatedDate = QD.UpdatedDate,
                                             Active = QD.Active
                                         }).ToList();

                #endregion

                return new GetQuoteByIDResult
                {
                    QuoteEntityObject = quoteObject,
                    ListQuoteDetail = listQuoteObjectType0,
                    ListQuoteDocument = listQuoteDocument,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetQuoteByIDResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

        }

        public string getNameGEn(Guid QuoteDetailId)
        {
            string Result = string.Empty;
            List<QuoteProductDetailProductAttributeValueEntityModel> listResult = new List<QuoteProductDetailProductAttributeValueEntityModel>();

            var QuoteProductDetailProductAttributeValueEntityModelList = (from OPDPV in context.QuoteProductDetailProductAttributeValue
                                                                          join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                          where OPDPV.QuoteDetailId == QuoteDetailId
                                                                          select (ProductAttributeCategoryV)).ToList();

            QuoteProductDetailProductAttributeValueEntityModelList.ForEach(item => { Result = Result + item.ProductAttributeCategoryValue1 + ";"; });

            return Result;

        }

        public List<QuoteProductDetailProductAttributeValueEntityModel> getListQuoteProductDetailProductAttributeValue(Guid QuoteDetailId)
        {
            List<QuoteProductDetailProductAttributeValueEntityModel> listResult = new List<QuoteProductDetailProductAttributeValueEntityModel>();

            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.QuoteProductDetailProductAttributeValue
                                                                    join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId equals ProductAttributeC.ProductAttributeCategoryId
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.QuoteDetailId == QuoteDetailId
                                                                    select (new QuoteProductDetailProductAttributeValueEntityModel
                                                                    {
                                                                        QuoteDetailId = OPDPV.QuoteDetailId,
                                                                        QuoteProductDetailProductAttributeValueId = OPDPV.QuoteProductDetailProductAttributeValueId,
                                                                        ProductAttributeCategoryId = OPDPV.ProductAttributeCategoryId,
                                                                        ProductId = OPDPV.ProductId,
                                                                        ProductAttributeCategoryValueId = OPDPV.ProductAttributeCategoryValueId,
                                                                        NameProductAttributeCategory = ProductAttributeC.ProductAttributeCategoryName,
                                                                        NameProductAttributeCategoryValue = ProductAttributeCategoryV.ProductAttributeCategoryValue1
                                                                    })).ToList();
            listResult = OrderProductDetailProductAttributeValueModelList;
            return listResult;

        }

        private decimal SumAmountVNS(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            if (Vat != null)
            {
                CaculateVAT = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value - CacuDiscount) * Vat.Value) / 100;
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType, decimal UnitLaborPrice, int UnitLaborNumber)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value + UnitLaborPrice * UnitLaborNumber * ExchangeRate.Value) * DiscountValue.Value / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            if (Vat != null)
            {
                CaculateVAT = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value + UnitLaborPrice * UnitLaborNumber * ExchangeRate.Value - CacuDiscount) * Vat.Value) / 100;
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value + UnitLaborPrice * UnitLaborNumber * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        private string GenerateorderCode()
        {
            var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                .SystemValueString;

            if (appName == "VNS")
            {
                // sửa định dạng gen code thành "BG-yyMMdd + 4 số"
                var listQuoteCodeToDay = context.Quote.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                    .Select(y => y.QuoteCode.Substring(9)).ToList();

                string currentYear = DateTime.Now.Year.ToString();

                var code = "";
                //Nếu đã có báo giá được tạo trong ngày hiện tại
                if (listQuoteCodeToDay.Count > 0)
                {
                    var maxCount = listQuoteCodeToDay.Select(y => Int32.Parse(y)).OrderByDescending(z => z)
                        .FirstOrDefault();

                    code = "BG-" + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") +
                           DateTime.Now.Day.ToString("00") + (maxCount + 1).ToString("D4");
                }
                //Nếu chưa có báo giá nào được tạo ngày hiện tại
                else
                {
                    code = "BG-" + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") +
                           DateTime.Now.Day.ToString("00") + "0001";
                }

                return code;
            }
            else
            {
                // sửa định dạng gen code thành "BG-yyMMdd + 4 số"
                var todayQuotes = context.Quote.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                    .OrderByDescending(w => w.CreatedDate)
                    .ToList();

                var count = todayQuotes.Count() == 0 ? 0 : todayQuotes.Count();
                string currentYear = DateTime.Now.Year.ToString();
                string result = "BG-" + currentYear.Substring(currentYear.Length - 2) +
                                DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") +
                                (count + 1).ToString("D4");
                return result;
            }
        }

        private static string GetCustomerName(string ObjectType, Guid? ObjectTypeId, TNTN8Context context)
        {
            string Result = string.Empty;

            if (!string.IsNullOrEmpty(ObjectType) && ObjectTypeId != null)
            {
                switch (ObjectType.ToUpper())
                {
                    case "LEAD":
                        Result = (from c in context.Contact
                                  join l in context.Lead on c.ObjectId equals l.LeadId
                                  where l.LeadId == ObjectTypeId.Value
                                  select new { FullName = c.FirstName + ' ' + c.LastName }).DefaultIfEmpty(new { FullName = string.Empty }).FirstOrDefault().FullName.ToString();
                        break;
                    case "CUSTOMER":
                        Result = (from cus in context.Customer
                                  where cus.CustomerId == ObjectTypeId
                                  select new { FullName = cus.CustomerName == null ? "" : cus.CustomerName }).DefaultIfEmpty(new { FullName = string.Empty }).FirstOrDefault().FullName.ToString();
                        break;
                    default:
                        Result = string.Empty;
                        break;
                }
            }
            return Result;
        }

        private DateTime FirstDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow;
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow.AddDays(-6);
                    break;
            }
            int hour = dateNow.Hour;
            int minute = dateNow.Minute;
            int second = dateNow.Second;
            dateReturn = dateReturn.AddHours(-hour).AddMinutes(-minute).AddSeconds(-second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return dateReturn;
        }

        private DateTime LastDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow.AddDays(7);
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(5);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(4);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(2);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(1);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow;
                    break;
            }
            int hour = 23 - dateNow.Hour;
            int minute = 59 - dateNow.Minute;
            int second = 59 - dateNow.Second;
            dateReturn = dateReturn.AddHours(hour).AddMinutes(minute).AddSeconds(second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return dateReturn;
        }

        public GetDashBoardQuoteResult GetDashBoardQuote(GetDashBoardQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                .SystemValueString;

                var result = new GetDashBoardQuoteResult();
                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.PersonInChangeId);
                var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
                bool isManager = employee.IsManager;

                var statusNew = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                var statusWait = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                var statusProcess = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                var statusCloseSuccess = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                var statusCloseFailure = context.Category.FirstOrDefault(c => c.CategoryCode == "DON" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                var statusCancel = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;

                // list common quote
                var quoteList = context.Quote.Where(c => c.Seller != null && c.Active == true).ToList();

                // list id bao gia map bao gia nguoi tham gia
                var listQuoteParticipantMapping = context.QuoteParticipantMapping
                    .Where(x => x.EmployeeId == employee.EmployeeId).Select(y => y.QuoteId).ToList();

                var countNew = 0;
                var countInProgress = 0;
                var countWaiting = 0;
                var countDone = 0;
                var countAbort = 0;
                var countClose = 0;
                var countPause = 0;

                if (!isManager)
                {
                    #region Nếu không phải Quản lý

                    if (appName == "VNS")
                    {
                        // Tạo mới
                        countNew = quoteList.Where(w => w.Seller == parameter.PersonInChangeId &&
                                                        w.StatusId == statusNew &&
                                                        DateTime.Parse(w.QuoteDate.ToString()).Month ==
                                                        parameter.MonthQuote &&
                                                        DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote)
                            .Count();

                        // Đang xử lý
                        countInProgress = quoteList.Where(w =>
                            w.Seller == parameter.PersonInChangeId && w.StatusId == statusProcess &&
                            DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Chờ phản hồi
                        countWaiting = quoteList.Where(w =>
                            w.Seller == parameter.PersonInChangeId && w.StatusId == statusWait &&
                            DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Đóng - Trúng Thầu
                        countDone = quoteList.Where(w =>
                            w.Seller == parameter.PersonInChangeId && w.StatusId == statusCloseSuccess &&
                            DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Huỷ
                        countAbort = quoteList.Where(w =>
                            w.Seller == parameter.PersonInChangeId && w.StatusId == statusCancel &&
                            DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Đóng 
                        countClose = quoteList.Where(w =>
                            w.Seller == parameter.PersonInChangeId && w.StatusId == statusCloseFailure &&
                            DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    }
                    else
                    {
                        // Tạo mới
                        countNew = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusNew && (parameter.MonthQuote == 0 ||
                                                        DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Đang xử lý
                        countInProgress = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusProcess &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Chờ phản hồi
                        countWaiting = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusWait && (parameter.MonthQuote == 0 ||
                                                         DateTime.Parse(w.QuoteDate.ToString()).Month ==
                                                         parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Đóng - Trúng Thầu
                        countDone = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusCloseSuccess && (parameter.MonthQuote == 0 ||
                                                                 DateTime.Parse(w.QuoteDate.ToString()).Month ==
                                                                 parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Huỷ
                        countAbort = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusCancel && (parameter.MonthQuote == 0 ||
                                                           DateTime.Parse(w.QuoteDate.ToString()).Month ==
                                                           parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                        // Đóng 
                        countClose = quoteList.Where(w =>
                            (w.Seller == parameter.PersonInChangeId || listQuoteParticipantMapping.Contains(w.QuoteId)) &&
                            w.StatusId == statusCloseFailure && (parameter.MonthQuote == 0 ||
                                                                 DateTime.Parse(w.QuoteDate.ToString()).Month ==
                                                                 parameter.MonthQuote) &&
                            DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    }

                    #endregion
                }
                else
                {
                    #region Comment by Giang: Code cũ của Ms.NgọcPhạm

                    //var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                    //if (organization.ParentId == null)
                    //{
                    //    // Tạo mới
                    //    var categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countNew = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Đang xử lý
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countInProgress = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Chờ phản hồi
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countWaiting = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Đóng - Trúng Thầu
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countDone = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Huỷ
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countAbort = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Đóng - Không Trúng
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTR" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countClose = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //    // Hoãn
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    countPause = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                    //}
                    //else
                    //{
                    //    var parentId = context.Organization.Where(o => o.ParentId == employee.OrganizationId).ToList();
                    //    List<Guid> organizationId = new List<Guid>();
                    //    organizationId.Add(organization.OrganizationId);

                    //    foreach (var item in parentId)
                    //    {
                    //        organizationId.Add(item.OrganizationId);
                    //    }

                    //    List<Guid> employIdList = new List<Guid>();
                    //    foreach (var item in organizationId)
                    //    {
                    //        var idlist = context.Employee.Where(e => e.OrganizationId == item).ToList();
                    //        foreach (var eml in idlist)
                    //        {
                    //            employIdList.Add(eml.EmployeeId);
                    //        }
                    //    }

                    //    // Tạo mới
                    //    var categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == perId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countNew = countNew + countQuote;
                    //    }

                    //    // Đang xử lý
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countInProgress = countInProgress + countQuote;
                    //    }

                    //    // Chờ phản hồi
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countWaiting = countWaiting + countQuote;
                    //    }

                    //    // Đóng - Trúng Thầu
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countDone = countDone + countQuote;
                    //    }

                    //    // Huỷ
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countAbort = countAbort + countQuote;
                    //    }

                    //    // Đóng - Không Trúng
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTR" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countClose = countClose + countQuote;
                    //    }

                    //    // Hoãn
                    //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                    //    foreach (var perId in employIdList)
                    //    {
                    //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                    //        countPause = countPause + countQuote;
                    //    }
                    //}

                    #endregion

                    #region Nếu là quản lý

                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    if (appName == "VNS")
                    {
                        // Mới tạo
                        countNew = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusNew).Count();

                        // Đang xử lý
                        countInProgress = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusProcess).Count();

                        // Chờ phản hồi
                        countWaiting = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusWait).Count();

                        // Đóng - Trúng Thầu
                        countDone = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCloseSuccess).Count();

                        // Huỷ
                        countAbort = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCancel).Count();

                        // Đóng 
                        countClose = quoteList.Where(x =>
                            (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCloseFailure).Count();
                    }
                    else
                    {
                        // Mới tạo
                        countNew = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusNew).Count();

                        // Đang xử lý
                        countInProgress = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusProcess).Count();

                        // Chờ phản hồi
                        countWaiting = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusWait).Count();

                        // Đóng - Trúng Thầu
                        countDone = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCloseSuccess).Count();

                        // Huỷ
                        countAbort = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCancel).Count();

                        // Đóng 
                        countClose = quoteList.Where(x =>
                            (listEmployeeId.Contains(x.Seller.Value) || listQuoteParticipantMapping.Contains(x.QuoteId)) &&
                            (parameter.MonthQuote == 0 ||
                             DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            x.StatusId == statusCloseFailure).Count();
                    }

                    #endregion
                }
                result = new GetDashBoardQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    DashBoardQuote = new GetDashBoardQuoteModel
                    {
                        CountInProgress = countInProgress,
                        CountNew = countNew,
                        CountDone = countDone,
                        CountClose = countClose,
                        CountAbort = countAbort,
                        CountWaiting = countWaiting
                    }
                };
                return result;
            }
            catch (Exception e)
            {
                return new GetDashBoardQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public UpdateActiveQuoteResult UpdateActiveQuote(UpdateActiveQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var quote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.QuoteId);

                if (appName == "VNS")
                {
                    if (quote == null)
                    {
                        return new UpdateActiveQuoteResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Báo giá không tồn tại trên hệ thống"
                        };
                    }

                    var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI")
                        ?.CategoryTypeId;
                    var listQuoteStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeId).ToList();
                    var quoteStatus = listQuoteStatus.FirstOrDefault(x => x.CategoryId == quote.StatusId);
                    var quoteStatusCode = quoteStatus?.CategoryCode;

                    if (quoteStatusCode != "MTA")
                    {
                        return new UpdateActiveQuoteResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không thể xóa báo giá ở trạng thái " + quoteStatus.CategoryName
                        };
                    }

                    #region Kiểm tra các reference của Báo giá

                    //List đơn hàng
                    var countOrder = context.CustomerOrder.Count(x => x.QuoteId == parameter.QuoteId);

                    if (countOrder > 0)
                    {
                        return new UpdateActiveQuoteResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không thể xóa báo giá đã gắn với Đơn hàng"
                        };
                    }

                    //List hợp đồng
                    var countContract = context.Contract.Count(x => x.QuoteId == parameter.QuoteId);

                    if (countContract > 0)
                    {
                        return new UpdateActiveQuoteResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không thể xóa báo giá đã gắn với Hợp đồng"
                        };
                    }

                    #endregion

                    //List Sản phẩm
                    var listQuoteDetail = context.QuoteDetail.Where(x => x.QuoteId == parameter.QuoteId).ToList();

                    context.QuoteDetail.RemoveRange(listQuoteDetail);

                    //List thuộc tính sản phẩm
                    var listQuoteDetailId = listQuoteDetail.Select(y => y.QuoteDetailId).ToList();
                    var listQuoteProductDetailProductAttributeValue = context.QuoteProductDetailProductAttributeValue
                        .Where(x => listQuoteDetailId.Contains(x.QuoteDetailId)).ToList();

                    context.QuoteProductDetailProductAttributeValue.RemoveRange(
                        listQuoteProductDetailProductAttributeValue);

                    //List chi phí báo giá
                    var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.QuoteId == parameter.QuoteId).ToList();

                    context.QuoteCostDetail.RemoveRange(listQuoteCostDetail);

                    //List người tham gia
                    var listQuoteParticipantMapping =
                        context.QuoteParticipantMapping.Where(x => x.QuoteId == parameter.QuoteId).ToList();

                    context.QuoteParticipantMapping.RemoveRange(listQuoteParticipantMapping);

                    //List file
                    var listFile = context.QuoteDocument.Where(x => x.QuoteId == parameter.QuoteId).ToList();

                    context.QuoteDocument.RemoveRange(listFile);

                    context.Quote.Remove(quote);

                    context.SaveChanges();
                }
                else
                {
                    quote.Active = false;
                    quote.UpdatedById = parameter.UserId;
                    quote.UpdatedDate = DateTime.Now;
                    context.Quote.Update(quote);
                    context.SaveChanges();
                }

                #region Log

                LogHelper.AuditTrace(context, "Delete", "Quote", quote.QuoteId, parameter.UserId);

                #endregion

                return new UpdateActiveQuoteResult
                {
                    MessageCode = CommonMessage.Quote.DELETE_QUOTE_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                };

            }
            catch (Exception ex)
            {
                return new UpdateActiveQuoteResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetDataQuoteToPieChartResult GetDataQuoteToPieChart(GetDataQuoteToPieChartParameter parameter)
        {
            try
            {
                List<string> categoriesPieChart = new List<string>();
                List<decimal?> dataPieChart = new List<decimal?>();

                var commonQuote = context.Quote.OrderByDescending(x => x.QuoteDate).ToList();

                var user = context.User.Where(x => x.UserId == parameter.UserId).FirstOrDefault();
                var employee = context.Employee.Where(x => x.EmployeeId == user.EmployeeId).FirstOrDefault();
                List<Guid> listStatus = new List<Guid>();
                var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
                // Báo giá
                var categoryDLY = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                listStatus.Add(categoryDLY);
                // Đóng
                var categoryCHO = context.Category.FirstOrDefault(c => c.CategoryCode == "DON" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                listStatus.Add(categoryCHO);
                // Đóng - Trúng Thầu
                //var categoryDTH = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //listStatus.Add(categoryDTH);
                // Hoãn
                //var categoryHOA = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //listStatus.Add(categoryHOA);

                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();


                // lay ten app
                var applicationName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                // list id bao gia map bao gia nguoi tham gia
                var listQuoteParticipantMapping = context.QuoteParticipantMapping
                    .Where(x => x.EmployeeId == employee.EmployeeId).Select(y => y.QuoteId).ToList();

                // list common bao gia
                var listCommonQuote = context.Quote.Where(x => x.Active == true).ToList();

                if (employee.IsManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    if (applicationName == "VNS")
                    {
                        var listQuote = listCommonQuote.Where(x =>
                                (parameter.MonthQuote == 0 || DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                                (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                                (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)))
                            .Select(y => new
                            {
                                y.QuoteDate,
                                Total = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, applicationName)
                            }).ToList();

                        var listQuoteParticipant = listCommonQuote.Where(x =>
                                (parameter.MonthQuote == 0 || DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                                (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                                (listQuoteParticipantMapping.Count == 0 || listQuoteParticipantMapping.Contains(x.QuoteId)))
                            .Select(y => new
                            {
                                y.QuoteDate,
                                Total = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, applicationName)
                            }).ToList();

                        if (listQuoteParticipant.Count > 0)
                        {
                            listQuoteParticipant.ForEach(x =>
                            {
                                if (!listQuote.Contains(x))
                                {
                                    listQuote.Add(x);
                                }
                            });
                        }

                        listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                        {
                            y.First().QuoteDate,
                            Total = y.Sum(s => s.Total)
                        }).OrderBy(z => z.QuoteDate).ToList();

                        listQuote.ForEach(item =>
                        {
                            categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                            dataPieChart.Add(item.Total);
                        });
                    }
                    else
                    {
                        var listQuoteFilter = commonQuote.Where(x =>
                                (parameter.MonthQuote == 0 || DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                                listStatus.Contains(x.StatusId.Value) &&
                                (listEmployeeId.Contains(x.Seller.Value) ||
                                 listQuoteParticipantMapping.Contains(x.QuoteId))
                            )
                            .Select(y => new QuoteEntityModel
                            {
                                QuoteId = y.QuoteId,
                                DiscountValue = y.DiscountValue,
                                DiscountType = y.DiscountType,
                                QuoteDate = y.QuoteDate,
                                TotalAmountAfterVat = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, applicationName)
                            }).ToList();

                        listQuoteFilter.ForEach(item =>
                        {
                            item.TotalAmount = CalculateTotalAmount(item.QuoteId, item.DiscountType, item.DiscountValue, item.TotalAmountAfterVat, listPromotionObjectApply);
                        });

                        var listQuote = listQuoteFilter.Select(y => new
                        {
                            y.QuoteDate,
                            Total = y.TotalAmount
                        }).ToList();

                        listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                        {
                            y.First().QuoteDate,
                            Total = y.Sum(s => s.Total)
                        }).OrderBy(z => z.QuoteDate).ToList();

                        listQuote.ForEach(item =>
                        {
                            categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                            dataPieChart.Add(item.Total);
                        });
                    }
                }
                else
                {
                    if (applicationName == "VNS")
                    {
                        var listQuote = context.Quote.Where(x =>
                                (parameter.MonthQuote == 0 || DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                                (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                                x.Seller == employee.EmployeeId &&
                                (listQuoteParticipantMapping.Count == 0 || listQuoteParticipantMapping.Contains(x.QuoteId)))
                            .Select(y => new
                            {
                                y.QuoteDate,
                                Total = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, applicationName)
                            }).ToList();

                        listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                        {
                            y.First().QuoteDate,
                            Total = y.Sum(s => s.Total)
                        }).OrderBy(z => z.QuoteDate).ToList();

                        listQuote.ForEach(item =>
                        {
                            categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                            dataPieChart.Add(item.Total);
                        });
                    }
                    else
                    {
                        var listQuoteFilter = commonQuote.Where(x =>
                                (parameter.MonthQuote == 0 || DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote) &&
                                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                                (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                                (x.Seller == employee.EmployeeId || listQuoteParticipantMapping.Contains(x.QuoteId)))
                            .Select(y => new QuoteEntityModel
                            {
                                QuoteId = y.QuoteId,
                                DiscountValue = y.DiscountValue,
                                DiscountType = y.DiscountType,
                                QuoteDate = y.QuoteDate,
                                TotalAmountAfterVat = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, applicationName)
                            }).ToList();

                        listQuoteFilter.ForEach(item =>
                        {
                            item.TotalAmount = CalculateTotalAmount(item.QuoteId, item.DiscountType, item.DiscountValue, item.TotalAmountAfterVat, listPromotionObjectApply);
                        });

                        var listQuote = listQuoteFilter.Select(y => new
                        {
                            y.QuoteDate,
                            Total = y.TotalAmount
                        }).ToList();

                        listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                        {
                            y.First().QuoteDate,
                            Total = y.Sum(s => s.Total)
                        }).OrderBy(z => z.QuoteDate).ToList();

                        listQuote.ForEach(item =>
                        {
                            categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                            dataPieChart.Add(item.Total);
                        });

                        //var listQuote = commonQuote.Where(x =>
                        //        DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                        //        DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                        //        (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                        //        x.Seller == employee.EmployeeId)
                        //    .Select(y => new QuoteEntityModel
                        //    {
                        //        QuoteId = y.QuoteId,
                        //        QuoteDate = y.QuoteDate,
                        //        Amount = y.Amount
                        //    }).ToList();

                        //var _listQuote = commonQuote.Where(x =>
                        //    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                        //    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                        //    (listStatus.Contains(x.StatusId.Value)) &&
                        //    (listQuoteParticipantMapping.Contains(x.QuoteId))).Select(y => new QuoteEntityModel
                        //{
                        //    QuoteId = y.QuoteId,
                        //    QuoteDate = y.QuoteDate,
                        //    Amount = y.Amount
                        //}).ToList();

                        //var listResult = new List<Guid>();
                        //_listQuote.ForEach(item =>
                        //{
                        //    var existsQuote = listQuote.FirstOrDefault(x => x.QuoteId == item.QuoteId);

                        //    if (existsQuote == null)
                        //    {
                        //        listResult.Add(item.QuoteId);
                        //    }
                        //});

                        //var listAddQuote = _listQuote.Where(x => listResult.Contains(x.QuoteId)).ToList();

                        //if (listAddQuote.Count > 0)
                        //{
                        //    listQuote.AddRange(listAddQuote);
                        //}

                        //var __listQuote = listQuote.Select(y => new
                        //{
                        //    y.QuoteDate,
                        //    y.Amount
                        //}).ToList();

                        //__listQuote = __listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                        //{
                        //    y.First().QuoteDate,
                        //    Amount = y.Sum(s => s.Amount)
                        //}).OrderBy(z => z.QuoteDate).ToList();

                        //__listQuote.ForEach(item =>
                        //{
                        //    categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                        //    dataPieChart.Add(item.Amount);
                        //});
                    }
                }

                return new GetDataQuoteToPieChartResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    CategoriesPieChart = categoriesPieChart,
                    DataPieChart = dataPieChart
                };
            }
            catch (Exception e)
            {
                return new GetDataQuoteToPieChartResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchQuoteResult SearchQuote(SearchQuoteParameter parameter)
        {
            try
            {
                var listQuote = new List<QuoteEntityModel>();
                var customerOrder = context.CustomerOrder.ToList();

                #region Lấy list status của báo giá

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                                    new CategoryEntityModel
                                    {
                                        CategoryId = y.CategoryId,
                                        CategoryName = y.CategoryName,
                                        CategoryCode = y.CategoryCode,
                                        CategoryTypeId = Guid.Empty,
                                        CreatedById = Guid.Empty,
                                        CountCategoryById = 0
                                    }).ToList();

                #endregion

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")?
                    .SystemValueString.Trim();

                // category type
                var customerStatusId = context.CategoryType
                    .FirstOrDefault(x => x.Active == true && x.CategoryTypeCode == "THA")?.CategoryTypeId;

                // khách hàng tiềm năng
                var statusKHTN = context.Category.FirstOrDefault(x =>
                    x.Active == true && x.CategoryTypeId == customerStatusId && x.CategoryCode == "MOI")?.CategoryId;

                // khách hàng
                var statusKH = context.Category.FirstOrDefault(x =>
                    x.Active == true && x.CategoryTypeId == customerStatusId && x.CategoryCode == "HDO")?.CategoryId;

                var paramListUserCreateId = new List<Guid>();
                if (parameter.ListEmpCreateId.Count > 0)
                {
                    paramListUserCreateId = context.User.Where(x => parameter.ListEmpCreateId.Contains(x.EmployeeId.Value))
                        .Select(y => y.UserId).ToList();
                }

                parameter.QuoteCode = parameter.QuoteCode == null ? "" : parameter.QuoteCode.Trim();
                parameter.QuoteName = parameter.QuoteName == null ? "" : parameter.QuoteName.Trim();

                #region Lấy list báo giá mà người dùng được tham gia

                var listQuoteId = context.QuoteParticipantMapping.Where(x => x.EmployeeId == employee.EmployeeId)
                    .Select(y => y.QuoteId).ToList();

                #endregion

                listQuote = (from quote in context.Quote
                             join cus in context.Customer on quote.ObjectTypeId equals cus.CustomerId
                             join personInCharge in context.Employee on quote.PersonInChargeId equals personInCharge.EmployeeId
                             //join seller in context.Employee on quote.PersonInChargeId equals seller.EmployeeId
                             where (parameter.QuoteCode == "" || quote.QuoteCode.Contains(parameter.QuoteCode)) &&
                                    (parameter.QuoteName == "" || quote.QuoteName.Contains(parameter.QuoteName)) &&
                                    (parameter.ListStatusQuote.Count == 0 ||
                                     parameter.ListStatusQuote.Contains(quote.StatusId)) &&
                                    (paramListUserCreateId.Count == 0 || paramListUserCreateId.Contains(quote.CreatedById)) &&
                                    quote.Active == true && quote.Seller != null
                             select new QuoteEntityModel
                             {
                                 QuoteId = quote.QuoteId,
                                 QuoteCode = quote.QuoteCode,
                                 QuoteName = quote.QuoteName,
                                 QuoteDate = quote.QuoteDate,
                                 Seller = quote.Seller,
                                 Description = quote.Description,
                                 Note = quote.Note,
                                 ObjectTypeId = quote.ObjectTypeId,
                                 ObjectType = quote.ObjectType,
                                 CustomerStatusId = cus.StatusId,
                                 PaymentMethod = quote.PaymentMethod,
                                 DaysAreOwed = quote.DaysAreOwed,
                                 IntendedQuoteDate = quote.IntendedQuoteDate,
                                 SendQuoteDate = quote.SendQuoteDate,
                                 MaxDebt = quote.MaxDebt,
                                 ExpirationDate = quote.ExpirationDate,
                                 ReceivedDate = quote.ReceivedDate,
                                 ReceivedHour = quote.ReceivedHour,
                                 RecipientName = quote.RecipientName,
                                 LocationOfShipment = quote.LocationOfShipment,
                                 ShippingNote = quote.ShippingNote,
                                 RecipientPhone = quote.RecipientPhone,
                                 RecipientEmail = quote.RecipientEmail,
                                 PlaceOfDelivery = quote.PlaceOfDelivery,
                                 Amount = (quote.DiscountType == true
                                     ? (quote.Amount - (quote.Amount * quote.DiscountValue) / 100)
                                     : (quote.Amount - quote.DiscountValue)),
                                 DiscountValue = quote.DiscountValue,
                                 StatusId = quote.StatusId,
                                 CreatedById = quote.CreatedById,
                                 CreatedDate = quote.CreatedDate,
                                 UpdatedById = quote.UpdatedById,
                                 UpdatedDate = quote.UpdatedDate,
                                 Active = quote.Active,
                                 DiscountType = quote.DiscountType,
                                 PersonInChargeId = quote.PersonInChargeId,
                                 PersonInChargeIdOfCus = cus.PersonInChargeId,
                                 CountQuoteInOrder = CountQuoteInCustomerOrder(quote.QuoteId, customerOrder),
                                 QuoteStatusName = "",
                                 BackgroundColorForStatus = "",
                                 CustomerName = "",
                                 NguoiPhuTrachName = personInCharge.EmployeeName,
                                 //NguoiBanName = seller.EmployeeName,
                                 EffectiveQuoteDate = quote.EffectiveQuoteDate,
                                 TotalAmountAfterVat = CalculateTotalAmountAfterVat(quote.QuoteId, quote.DiscountType,
                                     quote.DiscountValue,
                                     quote.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                             }).OrderByDescending(z => z.UpdatedDate).ToList();

                if (appName == "VNS")
                {
                    listQuote = listQuote.OrderByDescending(z => z.QuoteDate).ToList();

                    var listUserCreateId = listQuote.Select(y => y.CreatedById).Distinct().ToList();
                    var listUserCreate = context.User.Where(x => listUserCreateId.Contains(x.UserId)).ToList();
                    var listEmpCreateId = listUserCreate.Select(y => y.EmployeeId).ToList();
                    var listEmpCreate = context.Employee.Where(x => listEmpCreateId.Contains(x.EmployeeId)).ToList();

                    listQuote.ForEach(item =>
                    {
                        item.TotalAmount = CalculateTotalAmount(item.QuoteId, item.DiscountType, item.DiscountValue,
                            item.TotalAmountAfterVat, listPromotionObjectApply);

                        var empId = listUserCreate.FirstOrDefault(x => x.UserId == item.CreatedById)?.EmployeeId;
                        var empName = listEmpCreate.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;

                        item.CreatedByEmp = empName;
                    });
                }
                else
                {
                    listQuote.ForEach(x =>
                    {
                        x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                            x.TotalAmountAfterVat, listPromotionObjectApply);
                    });
                }

                if (parameter.IsCompleteInWeek)
                {
                    // Báo giá phải hoàn thành trong tuần
                    parameter.StartDate = FirstDateOfWeek();
                    parameter.EndDate = LastDateOfWeek();

                    listQuote = listQuote.Where(x =>
                        (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                        parameter.StartDate <= DateTime.Parse(x.IntendedQuoteDate.ToString()).AddDays(-7).Date) &&
                        (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                         parameter.EndDate >= DateTime.Parse(x.IntendedQuoteDate.ToString()).AddDays(-7).Date) &&
                        x.SendQuoteDate == null).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                                    (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                                     parameter.StartDate <= x.QuoteDate) &&
                                    (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                                     parameter.EndDate >= x.QuoteDate)).ToList();
                }

                if (parameter.IsOutOfDate)
                {
                    var statusDD = listStatus.FirstOrDefault(f => f.CategoryCode == "CHO");

                    listQuote = listQuote.Where(x =>
                            DateTime.Parse(x.UpdatedDate.ToString()).AddDays(x.EffectiveQuoteDate != null
                                ? int.Parse(x.EffectiveQuoteDate.ToString())
                                : 0) < DateTime.Now.Date && x.UpdatedDate != null
                                                         && x.StatusId == statusDD.CategoryId)
                        .ToList();
                }

                List<Guid> listNhanVienCapDuoiID = new List<Guid>();

                //20.04.2022 cho phép người phụ trách lấy toàn bộ các báo giá của KH ( cả của người phụ trách cũ đã nghỉ việc)
                if (employee.IsManager)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);


                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    listNhanVienCapDuoiID.AddRange(listEmployeeId);


                    listQuote = listQuote.Where(x =>
                        listEmployeeId.Contains(x.Seller.Value) ||
                        x.PersonInChargeId == employee.EmployeeId ||
                        listQuoteId.Contains(x.QuoteId)).ToList();
                }
                else
                {
                    //listQuote = listQuote.Where(x =>
                    //    x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId ||
                    //    listQuoteId.Contains(x.QuoteId)).ToList();
                    listNhanVienCapDuoiID.Add(employee.EmployeeId); 
                    listQuote = listQuote.Where(x =>
                        x.Seller == employee.EmployeeId || x.PersonInChargeIdOfCus == employee.EmployeeId 
                        || x.PersonInChargeId == employee.EmployeeId || listQuoteId.Contains(x.QuoteId)).ToList();
                }

                var listNhanVienPhuTrach = context.Employee
                        .Where(x => listNhanVienCapDuoiID.Count == 0 || listNhanVienCapDuoiID.Contains(x.EmployeeId))
                        .Select(y => new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName
                        }).ToList();

                //var listNguoiBan = listQuote.Select(y => new EmployeeEntityModel
                //{
                //    EmployeeId = y.Seller,
                //    EmployeeName = y.NguoiBanName
                //}).ToList();
                //listNguoiBan = listNguoiBan.GroupBy(x => x.EmployeeId).Select(group => group.First()).ToList();


                #region Lấy tên Đối tượng và tên Trạng thái của Báo giá

                if (listQuote != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            var quoteStatus = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value);
                            switch (quoteStatus.CategoryCode)
                            {
                                case "MTA":
                                    item.BackgroundColorForStatus = "#FFC000";
                                    break;
                                case "CHO":
                                    item.BackgroundColorForStatus = " #9C00FF";
                                    break;
                                case "DTH":
                                    item.BackgroundColorForStatus = "#6D98E7";
                                    break;
                                case "DTR":
                                    item.BackgroundColorForStatus = "#FF0000";
                                    break;
                                case "DLY":
                                    item.BackgroundColorForStatus = "#46B678";
                                    break;
                                case "HUY":
                                    item.BackgroundColorForStatus = "#333333";
                                    break;
                                case "HOA":
                                    item.BackgroundColorForStatus = "#666666";
                                    break;
                                case "TUCHOI":
                                    item.BackgroundColorForStatus = "#878d96";
                                    break;
                            }

                            item.QuoteStatusName = quoteStatus.CategoryName;
                        }

                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }

                #endregion

                if (appName == "VNS")
                {
                    #region lọc khách hàng tiềm năng

                    if (parameter.IsPotentialCustomer && !parameter.IsCustomer)
                    {
                        listQuote = listQuote.Where(x => x.CustomerStatusId == statusKHTN).ToList();
                    }

                    #endregion

                    #region lọc khách hàng

                    if (parameter.IsCustomer && !parameter.IsPotentialCustomer)
                    {
                        listQuote = listQuote.Where(x => x.CustomerStatusId == statusKH).ToList();
                    }

                    #endregion
                }

                if (parameter.IsOutOfDate)
                {
                    listQuote = listQuote.Where(x => x.UpdatedDate != null)
                        .OrderBy(z => z.UpdatedDate?.AddDays(z.EffectiveQuoteDate ?? 0)).ToList();
                }
                if (parameter.IsCompleteInWeek)
                {
                    listQuote = listQuote.Where(x => x.IntendedQuoteDate != null)
                        .OrderBy(z => z.IntendedQuoteDate?.AddDays(-7)).ToList();
                }

                if (parameter.PersonInChargeId != null)
                {
                    if (parameter.PersonInChargeId.Count != 0)
                    {
                        listQuote = listQuote.Where(x => parameter.PersonInChargeId.Contains(x.PersonInChargeId)).ToList();
                    }
                }
                //if (parameter.SellerId != null)
                //{
                //    if (parameter.SellerId.Count != 0)
                //    {
                //        listQuote = listQuote.Where(x => parameter.SellerId.Contains(x.Seller)).ToList();
                //    }
                //}
              
                return new SearchQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListQuote = listQuote,
                    ListStatus = listStatus,
                    ListNhanVienCapDuoi = listNhanVienPhuTrach,
                    //ListNguoiBan = listNguoiBan,
                };
            }
            catch (Exception e)
            {
                return new SearchQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //Tổng tiền sau thuế = Tổng GTHH bán ra + Tổng thành tiền nhân công - Tổng chiết khấu - Tổng khuyến mại + Tổng thuế VAT + Tổng chi phí
        //Tổng tiền sau thuế = Tổng GTHH bán ra  + Tổng thuế VAT + Tổng chi phí
        private decimal? CalculateTotalAmountAfterVat(Guid quoteId, bool? discountType, decimal? discountValue, decimal? vat, List<QuoteDetail> listQuoteDetail, List<QuoteCostDetail> listQuoteCostDetail, List<PromotionObjectApply> listPromotionObjectApply, string appName)
        {
            decimal? result = 0;
            decimal? amount = 0;
            decimal? totalSumAmountLabor = 0;
            decimal? totalAmountDiscount = 0;
            decimal? totalAmountPromotion = 0;
            decimal? totalAmountVat = 0;
            decimal? amountPriceCost = 0;
            decimal? koTrongGiaBan = 0;
            bool hasDiscount = false;
            bool hasVat = false;

            if (appName != null)
            {
                if (appName == "VNS")
                {
                    var quoteDetailList = listQuoteDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteDetailList.ForEach(x =>
                    {
                        if (x.DiscountValue > 0)
                        {
                            hasDiscount = true;
                        }

                        if (x.Vat > 0)
                        {
                            hasVat = true;
                        }

                        var price = x.Quantity * x.UnitPrice * x.ExchangeRate;
                        decimal? amountDiscount = 0;
                        var sumAmountLabor = x.UnitLaborPrice * x.Quantity;

                        if (x.DiscountType == true)
                        {
                            amountDiscount = price * x.DiscountValue / 100;
                        }
                        else
                        {
                            amountDiscount = x.DiscountValue;
                        }

                        var amountVAT = (price - amountDiscount + sumAmountLabor) * x.Vat / 100;

                        //Tổng GTHH bán ra
                        amount += x.Quantity * x.UnitPrice * x.ExchangeRate;
                        //Tổng thành tiền nhân công
                        totalSumAmountLabor += sumAmountLabor;
                        //Tổng chiết khấu
                        totalAmountDiscount += amountDiscount;
                        //Tổng thuế VAT
                        totalAmountVat += amountVAT;
                    });

                    // Tổng khuyến mại
                    var promotionObjectApplyList = listPromotionObjectApply.Where(x => x.ObjectId == quoteId && x.ObjectType == "QUOTE").ToList();

                    promotionObjectApplyList.ForEach(x =>
                    {
                        if (x.ProductId == null)
                        {
                            totalAmountPromotion += x.Amount;
                        }
                    });

                    //Tổng chi phí
                    var quoteCostDetailList = listQuoteCostDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteCostDetailList.ForEach(item =>
                    {
                        var price = item.UnitPrice * item.Quantity;
                        amountPriceCost += price;
                    });

                    if (!hasDiscount)
                    {
                        /*Tổng thành tiền chiết khấu*/
                        if (discountType == true)
                        {
                            totalAmountDiscount = amount * discountValue / 100;
                        }
                        else
                        {
                            totalAmountDiscount = discountValue;
                        }
                        /*End*/
                    }

                    if (!hasVat)
                    {
                        totalAmountVat = (amount + totalSumAmountLabor - totalAmountDiscount - totalAmountPromotion) * vat / 100;
                    }

                    result = amount + totalSumAmountLabor - totalAmountDiscount - totalAmountPromotion +
                             totalAmountVat + amountPriceCost;
                }
                else
                {
                    var quoteDetailList = listQuoteDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteDetailList.ForEach(x =>
                    {

                        var price = x.Quantity * x.UnitPrice * x.ExchangeRate;
                        var laborPrice = x.UnitLaborNumber * x.UnitLaborPrice * x.ExchangeRate;
                        decimal? amountDiscount = 0;

                        if (x.DiscountType == true)
                        {
                            amountDiscount = (price + laborPrice) * x.DiscountValue / 100;
                        }
                        else
                        {
                            amountDiscount = x.DiscountValue;
                        }

                        var amountVAT = (price + laborPrice - amountDiscount) * x.Vat / 100;

                        //Tổng GTHH bán ra
                        amount += (price + laborPrice - amountDiscount);
                        //Tổng thuế VAT
                        totalAmountVat += amountVAT;
                    });

                    //decimal SumAmount = Quantity.Value * UnitPrice.Value * ExChangeRate.Value;
                    //decimal calculateUnitLabor = unitLaborNumber * unitLaborPrice * (ExChangeRate ?? 1);

                    //Tổng chi phí
                    var quoteCostDetailList = listQuoteCostDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteCostDetailList.ForEach(item =>
                    {
                        var price = item.UnitPrice * item.Quantity;
                        if (item.IsInclude == false)
                        {
                            koTrongGiaBan += price;
                        }
                        amountPriceCost += price;
                    });


                    result = amount + totalAmountVat + koTrongGiaBan;
                }
            }

            return result;
        }

        //Tổng tiền thanh toán = Tổng tiền sau thuế - Tổng Thành tiền chiết khấu - Tổng Thành tiền của Phiếu giảm giá tại tab Khuyến mãi
        private decimal? CalculateTotalAmount(Guid quoteId, bool? discountType, decimal? discountValue, decimal? totalAmountAfterVat, List<PromotionObjectApply> listPromotionObjectApply)
        {
            decimal? totalAmountPromotion = 0;
            decimal? discountVal = 0;

            var listPromotion = listPromotionObjectApply.Where(x => x.ObjectId == quoteId).ToList();

            listPromotion.ForEach(x =>
            {
                if (x.ProductId == null)
                {
                    if (!x.LoaiGiaTri)
                    {
                        totalAmountPromotion += x.GiaTri * x.SoLuongTang;
                    }
                    else
                    {
                        totalAmountPromotion += (totalAmountAfterVat * x.GiaTri / 100) * x.SoLuongTang;
                    }
                }
            });

            if (discountType == true)
            {
                discountVal = (totalAmountAfterVat * discountValue) / 100;
            }
            else
            {
                discountVal = discountValue;
            }

            return totalAmountAfterVat - discountVal - totalAmountPromotion;
        }

        public GetDataExportExcelQuoteResult GetDataExportExcelQuote(GetDataExportExcelQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var inforExportExcel = new InforExportExcelModel();

                var quote = new QuoteEntityModel();
                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();
                var listQuotePlan = new List<QuotePlanEntityModel>();
                var listQuotePaymentTerm = new List<QuotePaymentTermEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                {
                    QuoteId = y.QuoteId,
                    QuoteCode = y.QuoteCode,
                    QuoteDate = y.QuoteDate,
                    QuoteName = y.QuoteName,
                    SendQuoteDate = y.SendQuoteDate,
                    Seller = y.Seller,
                    EffectiveQuoteDate = y.EffectiveQuoteDate,
                    ExpirationDate = y.ExpirationDate,
                    Description = y.Description,
                    Note = y.Note,
                    ObjectTypeId = y.ObjectTypeId,
                    ObjectType = y.ObjectType,
                    CustomerContactId = y.CustomerContactId,
                    PaymentMethod = y.PaymentMethod,
                    DiscountType = y.DiscountType,
                    BankAccountId = y.BankAccountId,
                    DaysAreOwed = y.DaysAreOwed,
                    MaxDebt = y.MaxDebt,
                    ReceivedDate = y.ReceivedDate,
                    Amount = y.Amount,
                    DiscountValue = y.DiscountValue,
                    IntendedQuoteDate = y.IntendedQuoteDate,
                    StatusId = y.StatusId,
                    CreatedDate = y.CreatedDate,
                    PersonInChargeId = y.PersonInChargeId,
                    SellerName = "",
                    UpdatedDate = y.UpdatedDate
                }).FirstOrDefault();

                quote.SellerName = context.Employee.FirstOrDefault(x => x.EmployeeId == quote.PersonInChargeId)
                                       ?.EmployeeName ?? "";

                #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 0)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = cod.ProductName,
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = appName == "VNS"
                                                    ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType)
                                                    : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice,
                                                OrderNumber = cod.OrderNumber,
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            listProductId.Add(item.ProductId.Value);
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            listCategoryId.Add(item.UnitId.Value);
                    });

                    var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                    var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                    var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                        item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType0);

                #endregion

                #region Lấy thông tin chi phí

                var listCostQuote = context.QuoteCostDetail.Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(y => new QuoteCostDetailEntityModel
                    {
                        QuoteCostDetailId = y.QuoteCostDetailId,
                        CostId = y.CostId,
                        QuoteId = y.QuoteId,
                        Quantity = y.Quantity,
                        UnitPrice = y.UnitPrice,
                        IsInclude = y.IsInclude,
                    }).ToList();

                #endregion

                #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 1)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = cod.Description,
                                                NameProductUnit = cod.IncurredUnit,
                                                NameMoneyUnit = "",
                                                SumAmount = appName == "VNS"
                                                    ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType)
                                                    : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                        cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice,
                                                OrderNumber = cod.OrderNumber,
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType1);

                #endregion

                #region Lấy list thông tin bổ sung của báo giá

                listAdditionalInformation = context.AdditionalInformation
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(y =>
                        new AdditionalInformationEntityModel
                        {
                            AdditionalInformationId = y.AdditionalInformationId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Title = y.Title,
                            Content = y.Content,
                            Ordinal = y.Ordinal
                        }).OrderBy(z => z.Ordinal).ToList();

                #endregion

                #region Lấy thông tin để export excel báo giá

                if (parameter.QuoteId != null)
                {
                    var company = context.CompanyConfiguration.FirstOrDefault();
                    inforExportExcel.CompanyName = company.CompanyName;
                    inforExportExcel.Address = company.CompanyAddress;
                    inforExportExcel.Phone = company.Phone;
                    inforExportExcel.Website = "";
                    inforExportExcel.Email = company.Email;
                    inforExportExcel.Website = company.Website;

                    decimal totalMoney = 0;
                    decimal totalMoneyNotVat = 0;

                    listQuoteDetail.ForEach(item =>
                    {
                        totalMoney += item.SumAmount;
                    });

                    listCostQuote.ForEach(item =>
                    {
                        if (item.IsInclude == false)
                        {
                            totalMoney += ((decimal)item.Quantity * (decimal)item.UnitPrice);
                        }
                    });

                    decimal discountQuoteMoney = 0;
                    switch (quote.DiscountType)
                    {
                        case true:
                            discountQuoteMoney = totalMoney * (decimal)quote.DiscountValue / 100;
                            break;
                        case false:
                            discountQuoteMoney = (decimal)quote.DiscountValue;
                            break;
                        case null:
                            break;
                    }
                    totalMoney -= discountQuoteMoney;

                    inforExportExcel.TextTotalMoney = MoneyHelper.Convert(totalMoney);
                }

                #endregion

                #region Lấy thông tin kế hoạch triển khai - quote Plan

                listQuotePlan = context.QuotePlan
                    .Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(y =>
                        new QuotePlanEntityModel
                        {
                            PlanId = y.PlanId,
                            Tt = y.Tt,
                            Finished = y.Finished,
                            ExecTime = y.ExecTime,
                            SumExecTime = y.SumExecTime,
                            QuoteId = y.QuoteId,
                        }).OrderBy(z => z.Tt).ToList();

                #endregion

                #region Lấy thông tin điều khoản thanh toán

                listQuotePaymentTerm = context.QuotePaymentTerm
                    .Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(y => new QuotePaymentTermEntityModel
                    {
                        PaymentTermId = y.PaymentTermId,
                        QuoteId = y.QuoteId,
                        OrderNumber = y.OrderNumber,
                        Milestone = y.Milestone,
                        PaymentPercentage = y.PaymentPercentage
                    }).OrderBy(z => z.OrderNumber).ToList();

                #endregion

                return new GetDataExportExcelQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail.OrderBy(x => x.OrderNumber).ToList(),
                    ListAdditionalInformation = listAdditionalInformation,
                    InforExportExcel = inforExportExcel,
                    ListQuotePlan = listQuotePlan,
                    ListQuotePaymentTerm = listQuotePaymentTerm,
                    ListQuoteCostDetail = listCostQuote,
                };
            }
            catch (Exception e)
            {
                return new GetDataExportExcelQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateUpdateQuoteResult GetDataCreateUpdateQuote(GetDataCreateUpdateQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var listCustomer = new List<CustomerEntityModel>();
                var customerAssigned = new CustomerEntityModel();
                var listLead = new List<LeadEntityModel>();
                var leadAssigned = new LeadEntityModel();
                var listEmployee = new List<EmployeeEntityModel>();
                var employeeAssigned = new EmployeeEntityModel();
                var listNote = new List<NoteEntityModel>();
                var listSaleBidding = new List<SaleBiddingEntityModel>();
                bool isAproval = false;

                var contactList = context.Contact.Where(c => c.ObjectType == "LEA" && c.Active == true).ToList();
                var listAdditionalInformationTemplates = new List<CategoryEntityModel>();
                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE)
                    .CategoryTypeId;

                #region Nếu là xem chi tiết báo giá thì lấy thêm thông tin của Quote

                var quote = new QuoteEntityModel();
                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteCostDetail = new List<QuoteCostDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var leadDetailList = context.LeadDetail.ToList();
                var costQuoteList = context.CostsQuote.ToList();
                var leadProductDetailProductAttributeValueList =
                    context.LeadProductDetailProductAttributeValue.ToList();
                var saleBiddingDetailProductAttributeList = context.SaleBiddingDetailProductAttribute.ToList();
                var statusLead = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");
                var statusSaleBidding = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");
                var statusLeadXN = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusLead.CategoryTypeId && c.CategoryCode == "APPR");
                var statusSaleBiddingTT = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusSaleBidding.CategoryTypeId && c.CategoryCode == "APPR");
                var categoryList = context.Category.ToList();
                var vendorList = context.Vendor.ToList();
                var productList = context.Product.ToList();
                var customerAllList = context.Customer.Where(c =>
                    c.Active == true && c.PersonInChargeId != null && c.PersonInChargeId != Guid.Empty).ToList();
                var sellerQuote = new EmployeeEntityModel();
                var listParticipantId = new List<Guid>();

                if (parameter.QuoteId != null)
                {
                    quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteDate = y.QuoteDate,
                        QuoteName = y.QuoteName,
                        SendQuoteDate = y.SendQuoteDate,
                        Seller = y.Seller,
                        EffectiveQuoteDate = y.EffectiveQuoteDate,
                        ExpirationDate = y.ExpirationDate,
                        Description = y.Description,
                        Note = y.Note,
                        ObjectTypeId = y.ObjectTypeId,
                        ObjectType = y.ObjectType,
                        CustomerContactId = y.CustomerContactId,
                        PaymentMethod = y.PaymentMethod,
                        DiscountType = y.DiscountType,
                        BankAccountId = y.BankAccountId,
                        DaysAreOwed = y.DaysAreOwed,
                        MaxDebt = y.MaxDebt,
                        ReceivedDate = y.ReceivedDate,
                        Amount = y.Amount,
                        DiscountValue = y.DiscountValue,
                        IntendedQuoteDate = y.IntendedQuoteDate,
                        StatusId = y.StatusId,
                        CreatedDate = y.CreatedDate,
                        PersonInChargeId = y.PersonInChargeId,
                        SellerName = "",
                        IsSendQuote = y.IsSendQuote,
                        LeadId = y.LeadId,
                        SaleBiddingId = y.SaleBiddingId,
                        ApprovalStep = y.ApprovalStep,
                        InvestmentFundId = y.InvestmentFundId
                    }).FirstOrDefault();

                    if (quote.Seller != null && quote.Seller != Guid.Empty)
                    {
                        var empSeller = context.Employee.FirstOrDefault(x => x.EmployeeId == quote.Seller);
                        sellerQuote = new EmployeeEntityModel()
                        {
                            EmployeeId = empSeller.EmployeeId,
                            EmployeeCode = empSeller.EmployeeCode,
                            EmployeeName = empSeller.EmployeeName,
                            IsManager = empSeller.IsManager,
                            PositionId = empSeller.PositionId,
                            OrganizationId = empSeller.OrganizationId,
                            Active = empSeller.Active
                        };
                    }

                    if (quote.PersonInChargeId != null)
                    {
                        employeeAssigned = context.Employee.Where(x => x.EmployeeId == quote.PersonInChargeId)
                            .Select(y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName
                            }).FirstOrDefault();
                    }

                    if (quote.ObjectType == "CUSTOMER")
                    {
                        customerAssigned = context.Customer.Where(x => x.CustomerId == quote.ObjectTypeId).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerCode = y.CustomerCode,
                                CustomerName = y.CustomerName,
                                CustomerEmail = "",
                                CustomerEmailWork = "",
                                CustomerEmailOther = "",
                                CustomerPhone = "",
                                FullAddress = "",
                                CustomerCompany = "",
                                StatusId = y.StatusId,
                                MaximumDebtDays = y.MaximumDebtDays,
                                MaximumDebtValue = y.MaximumDebtValue,
                                PersonInChargeId = y.PersonInChargeId
                            }).FirstOrDefault();
                    }
                    else if (quote.ObjectType == "LEAD")
                    {
                        leadAssigned = context.Lead.Where(x => x.LeadId == quote.ObjectTypeId).Select(y =>
                            new LeadEntityModel
                            {
                                LeadId = y.LeadId,
                                FullName = "",
                                Email = "",
                                Phone = "",
                                FullAddress = ""
                            }).FirstOrDefault();
                    }

                    #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 0)

                    var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                                where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                                select (new QuoteDetailEntityModel
                                                {
                                                    Active = cod.Active,
                                                    CreatedById = cod.CreatedById,
                                                    QuoteId = cod.QuoteId,
                                                    VendorId = cod.VendorId,
                                                    CreatedDate = cod.CreatedDate,
                                                    CurrencyUnit = cod.CurrencyUnit,
                                                    Description = cod.Description,
                                                    DiscountType = cod.DiscountType,
                                                    DiscountValue = cod.DiscountValue,
                                                    ExchangeRate = cod.ExchangeRate,
                                                    QuoteDetailId = cod.QuoteDetailId,
                                                    OrderDetailType = cod.OrderDetailType,
                                                    ProductId = cod.ProductId.Value,
                                                    UpdatedById = cod.UpdatedById,
                                                    Quantity = cod.Quantity,
                                                    UnitId = cod.UnitId,
                                                    IncurredUnit = cod.IncurredUnit,
                                                    UnitPrice = cod.UnitPrice,
                                                    UpdatedDate = cod.UpdatedDate,
                                                    Vat = cod.Vat,
                                                    NameVendor = "",
                                                    NameProduct = "",
                                                    NameProductUnit = "",
                                                    NameMoneyUnit = "",
                                                    IsPriceInitial = cod.IsPriceInitial,
                                                    PriceInitial = cod.PriceInitial,
                                                    ProductName = cod.ProductName,
                                                    SumAmount = appName == "VNS"
                                                        ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                            cod.DiscountValue,
                                                            cod.DiscountType)
                                                        : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                            cod.DiscountValue, cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                    OrderNumber = cod.OrderNumber,
                                                    UnitLaborNumber = cod.UnitLaborNumber,
                                                    UnitLaborPrice = cod.UnitLaborPrice
                                                })).ToList();

                    if (listQuoteObjectType0 != null)
                    {
                        List<Guid> listVendorId = new List<Guid>();
                        List<Guid> listProductId = new List<Guid>();
                        List<Guid> listCategoryId = new List<Guid>();
                        listQuoteObjectType0.ForEach(item =>
                        {
                            if (item.VendorId != null && item.VendorId != Guid.Empty)
                                listVendorId.Add(item.VendorId.Value);
                            if (item.ProductId != null && item.ProductId != Guid.Empty)
                                listProductId.Add(item.ProductId.Value);
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                listCategoryId.Add(item.CurrencyUnit.Value);
                            if (item.UnitId != null && item.UnitId != Guid.Empty)
                                listCategoryId.Add(item.UnitId.Value);
                        });

                        var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                        var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                        var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                        listQuoteObjectType0.ForEach(item =>
                        {
                            if (item.VendorId != null && item.VendorId != Guid.Empty)
                                item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                            if (item.ProductId != null && item.ProductId != Guid.Empty)
                                item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductName;
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                            if (item.UnitId != null && item.UnitId != Guid.Empty)
                                item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                            //item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                            item.NameGene = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductCode;
                            item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                        });
                    }

                    listQuoteDetail.AddRange(listQuoteObjectType0);


                    #endregion

                    #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 1)

                    var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                                where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                                select (new QuoteDetailEntityModel
                                                {
                                                    Active = cod.Active,
                                                    CreatedById = cod.CreatedById,
                                                    QuoteId = cod.QuoteId,
                                                    VendorId = cod.VendorId,
                                                    CreatedDate = cod.CreatedDate,
                                                    CurrencyUnit = cod.CurrencyUnit,
                                                    Description = cod.Description,
                                                    DiscountType = cod.DiscountType,
                                                    DiscountValue = cod.DiscountValue,
                                                    ExchangeRate = cod.ExchangeRate,
                                                    QuoteDetailId = cod.QuoteDetailId,
                                                    OrderDetailType = cod.OrderDetailType,
                                                    ProductId = cod.ProductId.Value,
                                                    UpdatedById = cod.UpdatedById,
                                                    Quantity = cod.Quantity,
                                                    UnitId = cod.UnitId,
                                                    IncurredUnit = cod.IncurredUnit,
                                                    UnitPrice = cod.UnitPrice,
                                                    UpdatedDate = cod.UpdatedDate,
                                                    ProductName = cod.ProductName,
                                                    Vat = cod.Vat,
                                                    NameVendor = "",
                                                    NameProduct = "",
                                                    NameProductUnit = "",
                                                    NameMoneyUnit = "",
                                                    SumAmount = appName == "VNS"
                                                        ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                            cod.DiscountValue,
                                                            cod.DiscountType)
                                                        : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                            cod.DiscountValue, cod.DiscountType, 0, 0),
                                                    OrderNumber = cod.OrderNumber
                                                })).ToList();

                    if (listQuoteObjectType1 != null)
                    {
                        List<Guid> listCategoryId = new List<Guid>();
                        listQuoteObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                listCategoryId.Add(item.CurrencyUnit.Value);
                        });
                        var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                        listQuoteObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                        });
                    }

                    listQuoteDetail.AddRange(listQuoteObjectType1);

                    listQuoteDetail = listQuoteDetail.OrderBy(z => z.OrderNumber).ToList();

                    #endregion

                    #region Lấy list file đính kèm của báo giá

                    listQuoteDocument = (from QD in context.QuoteDocument
                                         where QD.QuoteId == parameter.QuoteId
                                         select new QuoteDocumentEntityModel
                                         {
                                             QuoteDocumentId = QD.QuoteDocumentId,
                                             QuoteId = QD.QuoteId,
                                             DocumentName = QD.DocumentName,
                                             DocumentSize = QD.DocumentSize,
                                             DocumentUrl = QD.DocumentUrl,
                                             CreatedById = QD.CreatedById,
                                             CreatedDate = QD.CreatedDate,
                                             UpdatedById = QD.UpdatedById,
                                             UpdatedDate = QD.UpdatedDate,
                                             Active = QD.Active,
                                         }).ToList();

                    #endregion

                    #region Lấy list thông tin bổ sung của báo giá

                    listAdditionalInformation = context.AdditionalInformation
                        .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                        .Select(y =>
                            new AdditionalInformationEntityModel
                            {
                                AdditionalInformationId = y.AdditionalInformationId,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                Title = y.Title,
                                Content = y.Content,
                                Ordinal = y.Ordinal
                            }).OrderBy(z => z.Ordinal).ToList();

                    #endregion

                    #region Lấy list note(ghi chú)

                    listNote = context.Note
                        .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                        .Select(
                            y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>()
                            }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var _listAllEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }
                        ).ToList();

                        listNote.ForEach(item =>
                        {
                            var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        });

                        // Sắp xếp lại listnote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    #endregion

                    #region Lấy list chi phí của báo giá

                    var quoteCost = context.QuoteCostDetail
                        .Where(c => c.QuoteId == parameter.QuoteId && c.Active == true).ToList();
                    quoteCost.ForEach(item =>
                    {
                        var cost = context.Cost.FirstOrDefault(c => c.CostId == item.CostId);
                        QuoteCostDetailEntityModel obj = new QuoteCostDetailEntityModel();
                        obj.QuoteCostDetailId = item.QuoteCostDetailId;
                        obj.CostId = item.CostId;
                        obj.QuoteId = item.QuoteId;
                        obj.Quantity = item.Quantity;
                        obj.UnitPrice = item.UnitPrice;
                        obj.CostName = cost.CostName;
                        obj.CostCode = cost.CostCode;
                        obj.Active = item.Active;
                        obj.CreatedById = item.CreatedById;
                        obj.CreatedDate = item.CreatedDate;
                        obj.UpdatedById = item.UpdatedById;
                        obj.UpdatedDate = item.UpdatedDate;

                        listQuoteCostDetail.Add(obj);
                    });

                    #endregion

                    #region Kiểm tra điều kiện để được phê duyệt báo giá

                    var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                    // lấy trạng thái chờ phê duyệt báo giá
                    var statusQuote = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI");
                    var statusQuoteDLY = context.Category.FirstOrDefault(c =>
                        c.CategoryTypeId == statusQuote.CategoryTypeId && c.CategoryCode == "DLY");

                    if (quote.ApprovalStep != null && quote.StatusId == statusQuoteDLY.CategoryId)
                    {
                        var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                            ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                        if (workFlowStep == null)
                        {
                            workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                                .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                        }

                        if ((workFlowStep.ApprovebyPosition && workFlowStep.ApproverPositionId == employee.PositionId)
                            || (!workFlowStep.ApprovebyPosition && workFlowStep.ApproverId == employee.EmployeeId))
                        {
                            isAproval = true;
                        }
                    }

                    #endregion

                    #region Lấy người liên quan

                    listParticipantId = context.QuoteParticipantMapping
                        .Where(x => x.QuoteId == parameter.QuoteId && x.EmployeeId != null)
                        .Select(y => y.EmployeeId.Value).ToList();

                    #endregion
                }

                #endregion

                #region Lấy list phương thức thanh toán và list status của báo giá

                //List Payment Method
                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true).CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                            CategoryTypeId = Guid.Empty,
                            CreatedById = Guid.Empty,
                            CountCategoryById = 0
                        }).ToList();

                //List Status
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                            CategoryTypeId = Guid.Empty,
                            CreatedById = Guid.Empty,
                            CountCategoryById = 0
                        }).ToList();

                #endregion

                #region Lấy list Tỉnh, Huyện, Phường-Xã

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                #endregion

                if (employee.IsManager)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listEmployee = context.Employee
                        .Where(x => x.Active == true &&
                                    (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = customerAllList
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId.Value)) &&
                                    x.Active == true)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerEmailWork = "",
                            CustomerEmailOther = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            CustomerCompany = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listLead = context.Lead
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId.Value)) &&
                                    x.StatusId == statusLeadXN.CategoryId &&
                                    x.Active == true)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            Email = "",
                            Phone = "",
                            FullAddress = "",
                            ListLeadDetail = null,
                            LeadCode = y.LeadCode,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listSaleBidding = context.SaleBidding
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId)) &&
                                    x.StatusId == statusSaleBiddingTT.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingName = y.SaleBiddingName,
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            SaleBiddingCode = y.SaleBiddingCode,
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = null
                        }).ToList();
                }
                else
                {
                    listEmployee = context.Employee.Where(x => x.EmployeeId == employee.EmployeeId && x.Active == true)
                        .Select(y =>
                            new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).ToList();

                    listCustomer = customerAllList
                        .Where(x => x.PersonInChargeId == employee.EmployeeId && x.Active == true).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerCode = y.CustomerCode,
                                CustomerName = y.CustomerName,
                                CustomerGroupId = y.CustomerGroupId,
                                CustomerEmail = "",
                                CustomerEmailWork = "",
                                CustomerEmailOther = "",
                                CustomerPhone = "",
                                FullAddress = "",
                                CustomerCompany = "",
                                StatusId = y.StatusId,
                                MaximumDebtDays = y.MaximumDebtDays,
                                MaximumDebtValue = y.MaximumDebtValue,
                                PersonInChargeId = y.PersonInChargeId
                            }).ToList();

                    listLead = context.Lead
                        .Where(x => x.PersonInChargeId == employee.EmployeeId &&
                                    x.StatusId == statusLeadXN.CategoryId &&
                                    x.Active == true)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            Email = "",
                            Phone = "",
                            FullAddress = "",
                            LeadCode = y.LeadCode,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listSaleBidding = context.SaleBidding
                        .Where(x => x.PersonInChargeId == employee.EmployeeId &&
                                    x.StatusId == statusSaleBiddingTT.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingName = y.SaleBiddingName,
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingDetail = null
                        }).ToList();
                }

                if (sellerQuote != null)
                {
                    var obj = listEmployee.Where(e => e.EmployeeId == sellerQuote.EmployeeId).ToList();
                    if (obj.Count() == 0)
                    {
                        listEmployee.Add(sellerQuote);
                        listEmployee.Distinct();
                    }
                }

                #region Thêm dữ liệu Assigned

                /*
                 * Trong trường hợp xem/sửa chi tiết của Báo giá, nếu user đăng nhập không được phân quyền dữ liệu đối
                 * với các dữ liệu được phân quyền dữ liệu như: listEmployee, listCustomer, listLead thì
                 * lấy thông tin của các loại dữ liệu ấy trong Báo giá và thêm vào các dữ liệu ấy
                 */
                if (parameter.QuoteId != null)
                {
                    if (employeeAssigned.EmployeeId != null && employeeAssigned.EmployeeId != Guid.Empty)
                    {
                        var checkAssignedEmp =
                            listEmployee.FirstOrDefault(x => x.EmployeeId == employeeAssigned.EmployeeId);
                        if (checkAssignedEmp == null)
                        {
                            listEmployee.Add(employeeAssigned);
                            listEmployee = listEmployee.OrderBy(x => x.EmployeeName).ToList();
                        }
                    }

                    if (customerAssigned.CustomerId != null && customerAssigned.CustomerId != Guid.Empty)
                    {
                        var checAssignedCus = listCustomer.FirstOrDefault(x => x.CustomerId == customerAssigned.CustomerId);
                        if (checAssignedCus == null)
                        {
                            listCustomer.Add(customerAssigned);
                        }
                    }

                    if (leadAssigned.LeadId != null && leadAssigned.LeadId != Guid.Empty)
                    {
                        var checkAssignedLea = listLead.FirstOrDefault(x => x.LeadId == leadAssigned.LeadId);
                        if (checkAssignedLea == null)
                        {
                            listLead.Add(leadAssigned);
                        }
                    }
                }

                #endregion

                #region Lấy thêm các customer, lead chưa có người phụ trách theo phân quyền dữ liệu

                //Lấy thêm những customer chưa có người phụ trách theo phân quyền dữ liệu
                var listCustomerNoPersonInCharge =
                    context.Customer.Where(x =>
                        x.PersonInChargeId == null && x.CreatedById == user.UserId && x.Active == true
                        ).Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerEmail = "",
                            CustomerEmailWork = "",
                            CustomerEmailOther = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            CustomerCompany = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                listCustomerNoPersonInCharge.ForEach(item =>
                {
                    bool checkDublicate;
                    checkDublicate = !listCustomer.Contains(item);
                    if (checkDublicate)
                    {
                        listCustomer.Add(item);
                    }
                });

                //Lấy thêm những lead chưa có người phụ trách theo phân quyền dữ liệu
                var listLeadNoPersonInCharge = context.Lead
                    .Where(x => x.PersonInChargeId == null &&
                                x.StatusId == statusLeadXN.CategoryId &&
                                x.CreatedById.ToLower() == user.UserId.ToString().ToLower() && x.Active == true)
                    .Select(y => new LeadEntityModel
                    {
                        LeadId = y.LeadId,
                        CustomerId = y.CustomerId,
                        FullName = "",
                        Email = "",
                        Phone = "",
                        FullAddress = ""
                    }).ToList();

                listLeadNoPersonInCharge.ForEach(item =>
                {
                    bool checkDublicate;
                    checkDublicate = !listLead.Contains(item);
                    if (checkDublicate)
                    {
                        listLead.Add(item);
                    }
                });

                listLead.ForEach(item =>
                {
                    item.ListLeadDetail = leadDetailList.Where(ld => ld.LeadId == item.LeadId && ld.Active == true)
                        .Select(ld => new LeadDetailModel
                        {
                            LeadId = ld.LeadId,
                            LeadDetailId = ld.LeadDetailId,
                            VendorId = ld.VendorId,
                            ProductId = ld.ProductId,
                            Quantity = ld.Quantity,
                            UnitPrice = ld.UnitPrice,
                            CurrencyUnit = ld.CurrencyUnit,
                            ExchangeRate = ld.ExchangeRate,
                            Vat = ld.Vat,
                            DiscountType = ld.DiscountType,
                            DiscountValue = ld.DiscountValue,
                            Description = ld.Description,
                            OrderDetailType = ld.OrderDetailType,
                            UnitId = ld.UnitId,
                            IncurredUnit = ld.IncurredUnit,
                            ProductName = ld.ProductName,
                            ProductCode = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductCode,
                            NameMoneyUnit = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit).CategoryName,
                            ProductNameUnit = categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId).CategoryName,
                            NameVendor = vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                ? ""
                                : vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                            LeadProductDetailProductAttributeValue = null
                        }).ToList();
                });

                listLead.ForEach(item =>
                {
                    var contactObj = contactList.FirstOrDefault(c => c.ObjectId == item.LeadId);
                    if (contactObj != null)
                    {
                        item.ContactId = contactObj.ContactId;
                    }
                    item.ListLeadDetail.ForEach(detail =>
                    {
                        detail.LeadProductDetailProductAttributeValue = leadProductDetailProductAttributeValueList
                            .Where(c => c.LeadDetailId == detail.LeadDetailId)
                            .Select(ld => new LeadProductDetailProductAttributeValueModel
                            {
                                LeadProductDetailProductAttributeValue1 = ld.LeadProductDetailProductAttributeValue1,
                                LeadDetailId = ld.LeadDetailId,
                                ProductId = ld.ProductId,
                                ProductAttributeCategoryId = ld.ProductAttributeCategoryId,
                                ProductAttributeCategoryValueId = ld.ProductAttributeCategoryValueId
                            }).ToList();
                    });
                });

                listSaleBidding.ForEach(item =>
                {
                    item.SaleBiddingDetail = costQuoteList
                        .Where(ld => ld.SaleBiddingId == item.SaleBiddingId && ld.CostsQuoteType == 2)
                        .Select(ld => new CostQuoteModel
                        {
                            SaleBiddingId = ld.SaleBiddingId,
                            CostsQuoteId = ld.CostsQuoteId,
                            VendorId = ld.VendorId,
                            ProductId = ld.ProductId,
                            Quantity = ld.Quantity,
                            UnitPrice = ld.UnitPrice,
                            CurrencyUnit = ld.CurrencyUnit,
                            ExchangeRate = ld.ExchangeRate,
                            Vat = ld.Vat,
                            DiscountType = ld.DiscountType,
                            DiscountValue = ld.DiscountValue,
                            Description = ld.Description,
                            OrderDetailType = ld.OrderDetailType,
                            UnitId = ld.UnitId,
                            IncurredUnit = ld.IncurredUnit,
                            ProductCode = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductCode,
                            ProductName = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ld.Description
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductName,
                            NameMoneyUnit = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit).CategoryName,
                            ProductNameUnit = categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId).CategoryName,
                            NameVendor = vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                ? ""
                                : vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                            SaleBiddingDetailProductAttribute = null
                        }).ToList();
                });
                listSaleBidding.ForEach(item =>
                {
                    item.SaleBiddingDetail.ForEach(detail =>
                    {
                        detail.SaleBiddingDetailProductAttribute = saleBiddingDetailProductAttributeList
                            .Where(c => c.SaleBiddingDetailId == detail.CostsQuoteId)
                            .Select(ld => new SaleBiddingDetailProductAttributeEntityModel
                            {
                                SaleBiddingDetailProductAttributeId = ld.SaleBiddingDetailProductAttributeId,
                                SaleBiddingDetailId = ld.SaleBiddingDetailId,
                                ProductId = ld.ProductId,
                                ProductAttributeCategoryId = ld.ProductAttributeCategoryId,
                                ProductAttributeCategoryValueId = ld.ProductAttributeCategoryValueId
                            }).ToList();
                    });
                });

                #endregion

                #region Lấy Email, Phone, Address, Name cho listCustomer và listLead 

                List<Guid> listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                List<Guid> listLeadId = listLead.Select(x => x.LeadId.Value).ToList();

                //danh sach khach hang (tiem nang va dinh danh)
                var listContactCustomer = context.Contact.Where(x =>
                        (listCustomerId.Count == 0 || listCustomerId.Contains(x.ObjectId)) &&
                        (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS"))
                    .ToList();

                //danh sach co hoi
                var listContactLead = context.Contact.Where(x =>
                    (listLeadId.Count == 0 || listLeadId.Contains(x.ObjectId)) && x.ObjectType == "LEA").ToList();

                listCustomer.ForEach(item =>
                {
                    var customerContact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId) ??
                                          new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerEmailOther = customerContact?.OtherEmail?.Trim() ?? "";
                    item.CustomerEmailWork = customerContact?.WorkEmail?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;
                    item.CustomerCompany = customerContact?.CompanyName?.Trim() ?? "";
                });

                listLead.ForEach(item =>
                {
                    var leadContact = listContactLead.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    var address = leadContact?.Address ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == leadContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == leadContact.DistrictId);
                    var districtName = district?.DistrictName?.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == leadContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.Email = leadContact?.Email?.Trim() ?? "";
                    item.Phone = leadContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + provinceName + districtName + wardName;
                    item.FullName = firstName + " " + lastName;
                });

                #endregion

                #region Lấy nguồn tiềm năng

                var investFundList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                listCustomer = listCustomer.OrderBy(x => x.CustomerName).ToList();
                listLead = listLead.OrderBy(x => x.FullName).ToList();

                var categoryTypeTHA =
                    context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryNew = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "MOI" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);
                var categoryHDO = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                var customerDD = listCustomer.Where(d => d.StatusId == categoryHDO.CategoryId).ToList();
                var customerTD = listCustomer.Where(d => d.StatusId == categoryNew.CategoryId).ToList();

                var listContactAllCustomer = context.Contact.Where(x =>
                        ((x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS")))
                    .ToList();

                var customerAll = customerAllList.Select(y =>
                    new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerGroupId = y.CustomerGroupId,
                        CustomerEmail = "",
                        CustomerEmailWork = "",
                        CustomerEmailOther = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        CustomerCompany = "",
                        StatusId = y.StatusId,
                        MaximumDebtDays = y.MaximumDebtDays,
                        MaximumDebtValue = y.MaximumDebtValue,
                        PersonInChargeId = y.PersonInChargeId,
                        StatusName = y.StatusId == categoryHDO.CategoryId ? "DD" : "TD"
                    }).ToList();

                customerAll.ForEach(item =>
                {
                    //add by dungpt
                    var customerContact = listContactAllCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId) ??
                                          new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerEmailOther = customerContact?.OtherEmail?.Trim() ?? "";
                    item.CustomerEmailWork = customerContact?.WorkEmail?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;
                    item.CustomerCompany = customerContact?.CompanyName?.Trim() ?? "";
                });

                #region Lấy list ProductVendorMapping

                var listProductVendorMapping = new List<ProductVendorMappingEntityModel>();
                listProductVendorMapping = context.ProductVendorMapping.Where(x => x.Active == true).Select(y =>
                    new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = y.ProductVendorMappingId,
                        ProductId = y.ProductId,
                        VendorId = y.VendorId,
                        VendorCode = vendorList.FirstOrDefault(z => z.VendorId == y.VendorId).VendorCode,
                        VendorName = vendorList.FirstOrDefault(z => z.VendorId == y.VendorId).VendorName
                    }).ToList();

                #endregion

                #region Lấy List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                return new GetDataCreateUpdateQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    IsAprovalQuote = isAproval,
                    ListCustomerAll = customerAll,
                    ListCustomer = customerDD,
                    ListCustomerNew = customerTD,
                    ListLead = listLead,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee.Distinct().ToList(),
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail,
                    ListQuoteCostDetail = listQuoteCostDetail,
                    ListQuoteDocument = listQuoteDocument,
                    ListAdditionalInformation = listAdditionalInformation,
                    ListAdditionalInformationTemplates = listAdditionalInformationTemplates,
                    ListNote = listNote,
                    ListInvestFund = investFundList?.OrderBy(w => w.CategoryName).ToList() ??
                                     new List<CategoryEntityModel>(),
                    ListSaleBidding = listSaleBidding,
                    ListProductVendorMapping = listProductVendorMapping,
                    ListParticipant = listParticipant,
                    ListParticipantId = listParticipantId
                };
            }
            catch (Exception e)
            {
                return new GetDataCreateUpdateQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public GetEmployeeSaleResult GetEmployeeSale(GetEmployeeSaleParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();
                //Lấy người phụ trách của Khách hàng
                var eployeePerInChange = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);

                //Nếu người phụ trách của khách hàng không phải là Quản lý
                if (!eployeePerInChange.IsManager)
                {
                    listEmployee.Add(new EmployeeEntityModel
                    {
                        EmployeeId = eployeePerInChange.EmployeeId,
                        EmployeeCode = eployeePerInChange.EmployeeCode,
                        EmployeeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName,
                        IsManager = eployeePerInChange.IsManager,
                        PositionId = eployeePerInChange.PositionId,
                        OrganizationId = eployeePerInChange.OrganizationId,
                        EmployeeCodeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName,
                        Active = eployeePerInChange.Active
                    });
                }
                //Nếu người phụ trách của khách hàng là Quản lý
                else
                {
                    // Lấy nhân viên cấp dưới cùng phòng ban
                    listEmployee = parameter.ListEmployeeByAccount
                        .Where(x => x.OrganizationId == eployeePerInChange.OrganizationId.Value).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                                Active = eployeePerInChange.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    #region Giang comment

                    //// Thêm nhân viên quản lý là cùng phòng ban nó
                    //var empList = parameter.ListEmployeeByAccount.Where(e =>
                    //    e.IsManager == true && e.OrganizationId == eployeePerInChange.OrganizationId).ToList();

                    //empList.ForEach(item =>
                    //{
                    //    var emp = listEmployee.Where(e => e.EmployeeId == item.EmployeeId).ToList();
                    //    if (emp.Count() == 0)
                    //    {
                    //        listEmployee.Add(new EmployeeEntityModel
                    //        {
                    //            EmployeeId = item.EmployeeId,
                    //            EmployeeCode = item.EmployeeCode,
                    //            EmployeeName = item.EmployeeCode + " - " + item.EmployeeName,
                    //            IsManager = item.IsManager,
                    //            PositionId = item.PositionId,
                    //            OrganizationId = item.OrganizationId,
                    //            EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName
                    //        });
                    //    }
                    //});

                    #endregion

                    // Lấy nhân viên phòng ban dưới nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(eployeePerInChange.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(eployeePerInChange.OrganizationId.Value, listGetAllChild);

                    // Bỏ phòng ban chính nó
                    listGetAllChild.Remove(eployeePerInChange.OrganizationId.Value);

                    var listEmployeeIsManager = parameter.ListEmployeeByAccount
                       .Where(x => (listGetAllChild.Contains(x.OrganizationId))).Select(
                           y => new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               IsManager = y.IsManager,
                               PositionId = y.PositionId,
                               OrganizationId = y.OrganizationId,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               Active = y.Active
                           }).OrderBy(z => z.EmployeeName).ToList();

                    listEmployeeIsManager.ForEach(item =>
                    {
                        listEmployee.Add(item);
                    });
                }

                //lấy người phụ trách/nhân viên bán hàng cũ nếu bị thay thể hoặc nghỉ việc
                if (parameter.OldEmployeeId != null)
                {
                    var personInCharge = context.Employee.Where(x => x.EmployeeId == parameter.OldEmployeeId)
                     .Select( y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        IsManager = y.IsManager,
                        PositionId = y.PositionId,
                        OrganizationId = y.OrganizationId,
                        EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                        Active = y.Active
                    }).FirstOrDefault();
                    if(personInCharge != null)
                    {
                        var checkExist = listEmployee.FirstOrDefault(x => x.EmployeeId == personInCharge.EmployeeId);
                        if(checkExist == null)
                        {
                            listEmployee.Add(personInCharge);
                        }
                    }   
                }
                return new GetEmployeeSaleResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListEmployee = listEmployee.OrderBy(z => z.EmployeeName).ToList()
                };
            }
            catch (Exception e)
            {
                return new GetEmployeeSaleResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DownloadTemplateProductResult DownloadTemplateProduct(DownloadTemplateProductParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_import_BOM_lines.xls";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateProductResult
                {
                    TemplateExcel = data,
                    FileName = "Template_import_BOM_lines",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = string.Format("Đã dowload file Template_import_BOM_lines"),
                };

            }
            catch (Exception)
            {
                return new DownloadTemplateProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                };
            }
        }
        public GetDataQuoteAddEditProductDialogResult GetDataQuoteAddEditProductDialog(
            GetDataQuoteAddEditProductDialogParameter parameter)
        {
            try
            {
                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI" && x.Active == true);
                var listUnitMoney = context.Category
                    .Where(x => x.CategoryTypeId == categoryType.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName,
                            IsDefault = y.IsDefauld
                        }).ToList();

                var categoryTypeUnitProduct = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH" && x.Active == true);
                var listUintProduct = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeUnitProduct.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                // lấy list loại hình kinh doanh: Chỉ bán ra, chỉ mua vào và cả 2.
                var loaiHinhTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HHKD")?.CategoryTypeId;
                var listLoaiHinh = context.Category.Where(x => x.CategoryTypeId == loaiHinhTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                }).ToList();

                var listProduct = context.Product.Where(x => x.Active == true).Select(x => new ProductEntityModel
                {
                    ProductId = x.ProductId,
                    //ProductCategoryId = x.ProductCategoryId,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    Price1 = x.Price1,
                    Price2 = x.Price2,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    UpdatedById = x.UpdatedById,
                    UpdatedDate = x.UpdatedDate,
                    Active = x.Active,
                    Quantity = x.Quantity,
                    ProductUnitId = x.ProductUnitId,
                    //ProductUnitName = x.ProductUnitName,
                    ProductDescription = x.ProductDescription,
                    Vat = x.Vat,
                    MinimumInventoryQuantity = x.MinimumInventoryQuantity,
                    ProductMoneyUnitId = x.ProductMoneyUnitId,
                    //ProductCategoryName = x.ProductCategoryName,
                    //ListVendorName = x.ListVendorName,
                    Guarantee = x.Guarantee,
                    GuaranteeTime = x.GuaranteeTime,
                    //CountProductInformation = x.GuaranteeTime,
                    ExWarehousePrice = x.ExWarehousePrice,
                    CalculateInventoryPricesId = x.CalculateInventoryPricesId,
                    PropertyId = x.PropertyId,
                    WarehouseAccountId = x.WarehouseAccountId,
                    RevenueAccountId = x.RevenueAccountId,
                    PayableAccountId = x.PayableAccountId,
                    ImportTax = x.ImportTax,
                    CostPriceAccountId = x.CostPriceAccountId,
                    AccountReturnsId = x.AccountReturnsId,
                    FolowInventory = x.FolowInventory,
                    ManagerSerialNumber = x.ManagerSerialNumber,
                    ProductCodeName = x.ProductCode + " - " + x.ProductName,
                    LoaiKinhDoanhCode = listLoaiHinh.FirstOrDefault(y => y.CategoryId == x.LoaiKinhDoanh).CategoryCode,
                }).ToList();

                // chỉ lấy hàng chỉ bán ra
                listProduct = listProduct.Where(x => x.LoaiKinhDoanhCode == "SALEONLY" || x.LoaiKinhDoanhCode == "SALEANDBUY" || x.LoaiKinhDoanhCode == null).ToList();

                var listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName
                }).ToList();

                var date = DateTime.Now.Date.Add(new TimeSpan(23, 59, 59));
                var listPriceProduct = context.PriceProduct.Where(x => x.Active == true && x.EffectiveDate <= date).ToList() ?? new List<PriceProduct>();
                var listPriceEntityModel = new List<PriceProductEntityModel>();

                listPriceProduct.ForEach(item =>
                {
                    var newPriceProduct = new PriceProductEntityModel
                    {
                        PriceProductId = item.PriceProductId,
                        ProductId = item.ProductId,
                        ProductCode = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductCode ?? "",
                        ProductName = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductName ?? "",
                        EffectiveDate = item.EffectiveDate,
                        PriceVnd = item.PriceVnd,
                        MinQuantity = item.MinQuantity,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        TiLeChietKhau = item.TiLeChietKhau,
                        NgayHetHan = item.NgayHetHan
                    };

                    listPriceEntityModel.Add(newPriceProduct);
                });
                return new GetDataQuoteAddEditProductDialogResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListUnitMoney = listUnitMoney,
                    ListUnitProduct = listUintProduct,
                    ListVendor = listVendor,
                    ListProduct = listProduct,
                    ListPriceProduct = listPriceEntityModel
                };
            }
            catch (Exception e)
            {
                return new GetDataQuoteAddEditProductDialogResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter)
        {
            try
            {
                var listVendorId = context.ProductVendorMapping.Where(x => x.ProductId == parameter.ProductId)
                    .Select(y => y.VendorId).ToList();

                var listVendor = new List<VendorEntityModel>();

                if (listVendorId.Count > 0)
                {
                    listVendor = context.Vendor
                        .Where(x => listVendorId.Contains(x.VendorId) && x.Active == true)
                        .Select(y => new VendorEntityModel
                        {
                            VendorId = y.VendorId,
                            VendorCode = y.VendorCode,
                            VendorName = y.VendorName
                        }).ToList();
                }

                #region Lấy list thuộc tính của sản phẩm

                var listObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>();
                var listObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>();

                var listProductAttribute =
                    context.ProductAttribute.Where(x => x.ProductId == parameter.ProductId).ToList();

                List<Guid> listProductAttributeCategoryId = new List<Guid>();
                listProductAttribute.ForEach(item =>
                {
                    listProductAttributeCategoryId.Add(item.ProductAttributeCategoryId);
                });

                if (listProductAttributeCategoryId.Count > 0)
                {
                    listObjectAttributeNameProduct = context.ProductAttributeCategory
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeNameProductModel
                        {
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                            ProductAttributeCategoryName = y.ProductAttributeCategoryName
                        })
                        .ToList();

                    listObjectAttributeValueProduct = context.ProductAttributeCategoryValue
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeValueProductModel
                        {
                            ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                            ProductAttributeCategoryValue = y.ProductAttributeCategoryValue1,
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId
                        })
                        .ToList();
                }

                #endregion

                decimal priceProduct = 0;
                bool isHetHan = false;

                if (parameter.CustomerGroupId != null && parameter.CustomerGroupId != Guid.Empty)
                {
                    var listPrice = context.PriceProduct
                        .Where(x => x.Active &&
                                    x.ProductId == parameter.ProductId &&
                                    x.CustomerGroupCategory == parameter.CustomerGroupId &&
                                    x.MinQuantity <= parameter.SoLuong)
                        .OrderByDescending(z => z.EffectiveDate)
                        .ToList();

                    if (listPrice.Count > 0)
                    {
                        var price = listPrice.FirstOrDefault(x => x.EffectiveDate.Date <= DateTime.Now.Date &&
                                    (x.NgayHetHan == null ||
                                    (x.NgayHetHan != null && x.NgayHetHan.Value.Date >= DateTime.Now.Date)));

                        if (price != null)
                        {
                            priceProduct = price.PriceVnd;
                        }
                        else
                        {
                            //Kiểm tra có đơn giá hết hạn hay không?
                            var exists = listPrice.FirstOrDefault(x => x.NgayHetHan != null &&
                            x.NgayHetHan.Value.Date < DateTime.Now.Date);

                            if (exists != null)
                            {
                                isHetHan = true;
                            }
                        }
                    }
                }

                return new GetVendorByProductIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListVendor = listVendor,
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct,
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct,
                    PriceProduct = priceProduct,
                    IsHetHan = isHetHan
                };
            }
            catch (Exception e)
            {
                return new GetVendorByProductIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }

        private decimal CalculatorAmount(bool DiscountType, decimal DiscountValue, decimal Amount)
        {
            decimal result;
            if (DiscountType)
            {
                result = Amount - (Amount * DiscountValue) / 100;
                return result;
            }

            result = Amount - DiscountValue;
            return result;
        }

        private string GenerateCustomerCode(int maxCode)
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            int MaxNumberCode = 0;
            if (maxCode == 0)
            {
                var customer = context.Customer.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
                if (customer != null)
                {
                    var customerCode = customer.CustomerCode;
                    if (customerCode.Contains(currentYear.ToString()) && customerCode.Contains(currentMonth.ToString()) && customerCode.Contains(currentDate.ToString()))
                    {
                        try
                        {
                            customerCode = customerCode.Substring(customerCode.Length - 4);
                            if (customerCode != "")
                            {
                                MaxNumberCode = Convert.ToInt32(customerCode) + 1;
                            }
                            else
                            {
                                MaxNumberCode = 1;
                            }
                        }
                        catch
                        {
                            MaxNumberCode = 1;
                        }

                    }
                    else
                    {
                        MaxNumberCode = 1;
                    }
                }
                else
                {
                    MaxNumberCode = 1;
                }
            }
            else
            {
                MaxNumberCode = maxCode + 1;
            }
            return string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
        }

        public CreateCostResult CreateCost(CreateCostParameter parameter)
        {
            try
            {
                var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
                var listOrg = context.Organization.ToList();
                var costOld = context.Cost.FirstOrDefault(c => c.Active == true && c.CostCode == parameter.Cost.CostCode);
                if (costOld != null)
                {
                    return new CreateCostResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã chi phí này đã tồn tại",
                    };
                }
                var cost = new Cost()
                {
                    CostId = Guid.NewGuid(),
                    CostCode = parameter.Cost.CostCode,
                    CostName = parameter.Cost.CostName,
                    StatusId = parameter.Cost.StatusId,
                    OrganizationId = parameter.Cost.OrganizationId,
                    DonGia = parameter.Cost.DonGia ?? 0,
                    NgayHetHan = parameter.Cost.NgayHetHan,
                    SoLuongToiThieu = parameter.Cost.SoLuongToiThieu ?? 0,
                    NgayHieuLuc = parameter.Cost.NgayHieuLuc ?? DateTime.Now,
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                };
                context.Cost.Add(cost);
                context.SaveChanges();

                var listCost = context.Cost.Where(c => c.Active == true).OrderByDescending(c => c.NgayHieuLuc).ToList();
                var listCostEntity = new List<CostEntityModel>();

                listCost.ForEach(item =>
                {
                    var costEntity = new CostEntityModel()
                    {
                        CostId = item.CostId,
                        CostCode = item.CostCode,
                        CostName = item.CostName,
                        StatusId = item.StatusId,
                        OrganizationId = item.OrganizationId,
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                        OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                        DonGia = item.DonGia,
                        NgayHetHan = item.NgayHetHan,
                        NgayHieuLuc = item.NgayHieuLuc,
                        SoLuongToiThieu = item.SoLuongToiThieu
                    };
                    listCostEntity.Add(costEntity);
                });
                return new CreateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = CommonMessage.Cost.CREATE_SUCCESS,
                    ListCost = listCostEntity,
                };
            }
            catch (Exception ex)
            {
                return new CreateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }

        }

        public UpdateCostResult UpdateCost(UpdateCostParameter parameter)
        {
            try
            {
                var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
                var listOrg = context.Organization.ToList();
                var commonCost = context.Cost.Where(c => c.Active == true).ToList();
                var cost = context.Cost.FirstOrDefault(c => c.CostId == parameter.Cost.CostId);
                if (cost == null)
                {
                    return new UpdateCostResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại chi phí!"
                    };
                }
                var costOld = commonCost.FirstOrDefault(c => c.CostCode == parameter.Cost.CostCode && c.CostId != parameter.Cost.CostId);
                if (costOld != null)
                {
                    return new UpdateCostResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã chi phí này đã tồn tại",
                    };
                }
                cost.CostName = parameter.Cost.CostName;
                cost.CostCode = parameter.Cost.CostCode;
                cost.OrganizationId = parameter.Cost.OrganizationId;
                cost.StatusId = parameter.Cost.StatusId;
                cost.UpdatedDate = DateTime.Now;
                cost.UpdatedById = parameter.UserId;
                cost.DonGia = parameter.Cost.DonGia ?? 0;
                cost.NgayHetHan = parameter.Cost.NgayHetHan;
                cost.SoLuongToiThieu = parameter.Cost.SoLuongToiThieu ?? 0;
                cost.NgayHieuLuc = parameter.Cost.NgayHieuLuc ?? DateTime.Now;
                context.Cost.Update(cost);
                context.SaveChanges();
                var listCost = context.Cost.Where(c => c.Active == true).OrderByDescending(c => c.NgayHieuLuc).ToList();
                var listCostEntity = new List<CostEntityModel>();

                listCost.ForEach(item =>
                {
                    var costEntity = new CostEntityModel()
                    {
                        CostId = item.CostId,
                        CostCode = item.CostCode,
                        CostName = item.CostName,
                        StatusId = item.StatusId,
                        OrganizationId = item.OrganizationId,
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                        OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                        DonGia = item.DonGia,
                        NgayHetHan = item.NgayHetHan,
                        NgayHieuLuc = item.NgayHieuLuc,
                        SoLuongToiThieu = item.SoLuongToiThieu
                    };
                    listCostEntity.Add(costEntity);
                });

                return new UpdateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Cập nhật chi phí thành công",
                    ListCost = listCostEntity
                };
            }
            catch (Exception ex)
            {
                return new UpdateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }
        public GetMasterDataCreateCostResult GetMasterDataCreateCost(GetMasterDataCreateCostParameter parameter)
        {
            try
            {
                var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
                var listCategoryStatus = new List<CategoryEntityModel>();
                listStatus.ForEach(item =>
                {
                    var newCategoryStatus = new CategoryEntityModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryName = item.CategoryName,
                        CategoryCode = item.CategoryCode,
                    };
                    listCategoryStatus.Add(newCategoryStatus);
                });
                var listOrg = context.Organization.ToList();

                var listCost = context.Cost.Where(c => c.Active == true).OrderByDescending(c => c.NgayHieuLuc).ToList();
                var listCostEntity = new List<CostEntityModel>();

                listCost.ForEach(item =>
                {
                    var costEntity = new CostEntityModel()
                    {
                        CostId = item.CostId,
                        CostCode = item.CostCode,
                        CostName = item.CostName,
                        CostCodeName = item.CostCode + " - " + item.CostName,
                        StatusId = item.StatusId,
                        OrganizationId = item.OrganizationId,
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                        OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                        DonGia = item.DonGia,
                        NgayHetHan = item.NgayHetHan,
                        NgayHieuLuc = item.NgayHieuLuc,
                        SoLuongToiThieu = item.SoLuongToiThieu
                    };
                    listCostEntity.Add(costEntity);
                });

                if (parameter.UserId != null && parameter.UserId != Guid.Empty)
                {
                    var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;

                    var employees = context.Employee.Where(e => e.Active == true).ToList();

                    var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listCostEntity = listCostEntity
                        .Where(c => listGetAllChild.Contains(c.OrganizationId) || c.OrganizationId == null).ToList();
                }

                return new GetMasterDataCreateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListStatus = listCategoryStatus,
                    ListCost = listCostEntity,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateCostResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DeleteCostResult DeleteCost(DeleteCostParameter parameter)
        {
            try
            {
                var cost = context.Cost.FirstOrDefault(c => c.CostId == parameter.CostId);
                var orderCodeDetail = context.OrderCostDetail.FirstOrDefault(c => c.CostId == parameter.CostId);
                if (cost == null)
                {
                    return new DeleteCostResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Chi phí không tồn tại trong hệ thống"
                    };
                }
                if (orderCodeDetail != null)
                {
                    return new DeleteCostResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Chi phí đã được sử dụng trong hệ thống"
                    };
                }
                context.Cost.Remove(cost);
                context.SaveChanges();

                return new DeleteCostResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xoá chi phí thành công",
                };
            }
            catch (Exception e)
            {
                return new DeleteCostResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }


        public UpdateQuoteResult UpdateStatusQuote(GetQuoteByIDParameter parameter)
        {
            try
            {
                var employee = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI");
                var quote = context.Quote.FirstOrDefault(q => q.QuoteId == parameter.QuoteId);
                string message = "";

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "QUOTE";
                note.ObjectId = quote.QuoteId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";

                switch (parameter.ObjectType)
                {
                    case "SEND_APROVAL":
                        var categoryCHO = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DLY");
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;
                        quote.ApprovalStep = 2;
                        quote.StatusId = categoryCHO?.CategoryId;
                        context.Quote.Update(quote);

                        #region Tạo lịch sử báo giá

                        #region báo giá

                        var quoteHistory = new QuoteApproveHistory()
                        {
                            Id = Guid.NewGuid(),
                            QuoteId = quote.QuoteId,
                            QuoteCode = quote.QuoteCode,
                            QuoteName = quote.QuoteName,
                            SendApproveDate = DateTime.Now,
                            SendApproveById = parameter.UserId,
                            Amount = parameter.CustomerOrderAmountAfterDiscount,
                            AmountPriceInitial = parameter.AmountPriceInitial,
                            DiscountType = quote.DiscountType,
                            DiscountValue = parameter.TotalAmountDiscount,
                            AmountPriceProfit = parameter.AmountPriceProfit,
                        };

                        context.QuoteApproveHistory.Add(quoteHistory);
                        context.SaveChanges();

                        #endregion

                        #region Sản phẩm dịch vụ thuộc báo giá

                        var listQuoteApproveDetailHistory = new List<QuoteApproveDetailHistory>();

                        var listQuoteDetail = context.QuoteDetail.Where(x => x.QuoteId == parameter.QuoteId && x.Active == true).ToList();

                        if (listQuoteDetail.Count > 0)
                        {
                            listQuoteDetail.ForEach(item =>
                            {
                                var quoteApproveDetailHistory = new QuoteApproveDetailHistory()
                                {
                                    Id = Guid.NewGuid(),
                                    QuoteId = item.QuoteId,
                                    QuoteApproveHistoryId = quoteHistory.Id,
                                    VendorId = item.VendorId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.UnitPrice,
                                    CurrencyUnit = item.CurrencyUnit,
                                    ExchangeRate = item.ExchangeRate,
                                    Vat = item.Vat,
                                    DiscountType = item.DiscountType,
                                    DiscountValue = item.DiscountValue,
                                    Description = item.Description,
                                    OrderDetailType = item.OrderDetailType,
                                    UnitId = item.UnitId,
                                    IncurredUnit = item.IncurredUnit,
                                    Active = item.Active,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now
                                };

                                listQuoteApproveDetailHistory.Add(quoteApproveDetailHistory);
                            });
                        }

                        context.QuoteApproveDetailHistory.AddRange(listQuoteApproveDetailHistory);
                        context.SaveChanges();

                        #endregion

                        #endregion

                        message = "Gửi phê duyệt thành công";

                        note.Description = "Đã gửi phê duyệt thành công";
                        break;
                    case "CANCEL_QUOTE":
                        var categoryHUY = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "HUY");
                        quote.StatusId = categoryHUY.CategoryId;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        message = "Hủy báo giá thành công";
                        note.Description = "Chuyển trạng thái hủy báo giá thành công";
                        break;
                    case "APPROVAL_QUOTE":

                        #region Nếu Xác nhận báo giá cho khách hàng tiềm năng

                        var customer = context.Customer.FirstOrDefault(x => x.CustomerId == quote.ObjectTypeId);
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == customer.StatusId)
                            ?.CategoryCode;

                        //Nếu là khách hàng tiềm năng thì chuyển trạng thái sang khách hàng
                        if (type == "MOI")
                        {
                            var typeKH = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                            var statusKH = context.Category.FirstOrDefault(x =>
                                x.CategoryTypeId == typeKH.CategoryTypeId && x.CategoryCode == "HDO")?.CategoryId;

                            if (statusKH != null)
                            {
                                customer.PotentialConversionDate = DateTime.Now;
                                customer.StatusId = statusKH.Value;
                                context.Customer.Update(customer);
                            }
                        }

                        #endregion

                        var categoryBG = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DTH");
                        quote.StatusId = categoryBG.CategoryId;
                        quote.SendQuoteDate = DateTime.Now;
                        quote.UpdatedById = parameter.UserId;

                        quote.UpdatedDate = DateTime.Now;

                        if (quote.LeadId != null && quote.LeadId != Guid.Empty)
                        {
                            var quoteByLead = context.Quote.Where(q => q.LeadId == quote.LeadId).ToList();
                            var quoteByLeadClose = quoteByLead.Where(q => q.StatusId == categoryBG.CategoryId).ToList();
                            if (quoteByLead.Count() == quoteByLeadClose.Count())
                            {
                                var categoryTypeLead = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "CHS");
                                var categoryTypeLeadClose = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == categoryTypeLead.CategoryTypeId && ca.CategoryCode == "CLOSE");
                                var lead = context.Lead.FirstOrDefault(l => l.LeadId == quote.LeadId);
                                lead.StatusId = categoryTypeLeadClose.CategoryId;
                                lead.UpdatedDate = DateTime.Now;

                                context.Lead.Update(lead);
                                context.SaveChanges();
                            }
                        }

                        context.Quote.Update(quote);
                        note.Description = "Xác nhận báo giá thành công";
                        message = "Xác nhận báo giá thành công";
                        break;
                    case "NEW_QUOTE":
                        var categoryMTA = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA");
                        quote.StatusId = categoryMTA.CategoryId;
                        quote.IsSendQuote = false;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        note.Description = "Đặt lại về trạng thái Mới thành công";
                        message = "";
                        break;
                    case "CANCEL_APROVAL":
                        quote.StatusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA").CategoryId;
                        quote.IsSendQuote = false;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        note.Description = "Báo giá được hủy gửi phê duyệt bởi nhân viên: " + employee.UserName;
                        message = "Yêu cầu phê duyệt đã được hủy";
                        break;
                }

                context.Note.Add(note);
                context.SaveChanges();

                #region Gửi thông báo

                switch (parameter.ObjectType)
                {
                    case "SEND_APROVAL":
                        //Gửi thông báo khi gửi phê duyệt báo giá
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "SEND_APPROVAL", new Queue(),
                            quote, true);
                        break;
                    case "CANCEL_QUOTE":
                        //Gửi thông báo khi hủy báo giá
                        break;
                    case "APPROVAL_QUOTE":
                        //Gửi thông báo khi xác nhận báo giá
                        break;
                    case "NEW_QUOTE":
                        //Gửi thông báo khi đặt về nháp báo giá
                        break;
                    case "CANCEL_APROVAL":
                        //Gửi thông báo khi Hủy yêu cầu phê duyệt
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "CANCEL_APPROVAL", new Queue(),
                            quote, true);
                        break;
                }

                #endregion

                return new UpdateQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = message
                };
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        private static List<Guid> GetListEmployeeApproved(TNTN8Context context, string WorkflowCode, int? StepNumber, List<Employee> ListAllEmployee)
        {
            var result = new List<Guid>();

            var workflow = context.WorkFlows.FirstOrDefault(x => x.WorkflowCode == WorkflowCode);

            if (workflow != null)
            {
                var workflowSteps = context.WorkFlowSteps.Where(x => x.WorkflowId == workflow.WorkFlowId).ToList();

                if (workflowSteps.Count > 0)
                {
                    //Nếu đối tượng có trạng thái Mới tạo (StepNumber == null) hoặc có trạng thái Từ chối (StepNumber == 0)
                    if (StepNumber == 0 || StepNumber == null)
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == 2);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                    else
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == StepNumber);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity, Guid userId)
        {
            var result = emailContent;

            #region Common Token

            const string UpdatedDate = "[UPDATED_DATE]";

            const string Url_Login = "[URL]";

            const string Logo = "[LOGO]";

            const string QuoteCode = "[QUOTE_CODE]";
            const string QuoteName = "[QUOTE_NAME]";

            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string employeeEmail = "[EMPLOYEE_EMAIL]"; // email nhan vien
            const string employeePhone = "[EMPLOYEE_PHONE]"; // sdt nhan vien


            const string listProduct = "[LIST_PRODUCT]"; // danh sach san pham

            const string customerCode = "[CUSTOMER_CODE]"; // ma khach hang
            const string customerName = "[CUSTOMER_NAME]"; // ten khach hang

            const string amountQuote = "[AMOUNT_QUOTE]"; // Tổng thanh toán của báo giá

            const string companyWebsite = "[COMPANY_WEBSITE]"; // Website công ty
            const string companyAddress = "[COMPANY_ADDRESS]"; // dia chi cong ty
            const string companyEmail = "[COMPANY_EMAIL]"; // email cong ty
            const string companyName = "[COMPANY_NAME]"; // ten cong ty
            const string companyPhone = "[COMPANY_PHONE]"; // sdt cong ty

            #endregion

            var _model = model as Quote;

            #region Replace token

            #region replace logo

            if (result.Contains(Logo))
            {
                var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                if (!String.IsNullOrEmpty(logo))
                {
                    var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                    result = result.Replace(Logo, temp_logo);
                }
                else
                {
                    result = result.Replace(Logo, "");
                }
            }

            #endregion

            #region replace quote name

            if (result.Contains(QuoteName) && _model.QuoteName != null)
            {
                result = result.Replace(QuoteName, _model.QuoteName.Trim());
            }

            #endregion

            #region replace quote code

            if (result.Contains(QuoteCode) && _model.QuoteCode != null)
            {
                result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
            }

            #endregion

            #region replace change employee send mail

            var employeeId = context.User.FirstOrDefault(x => x.UserId == userId)?.EmployeeId;

            if (result.Contains(EmployeeName))
            {
                var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                result = result.Replace(EmployeeName, !string.IsNullOrEmpty(employeeName) ? employeeName : "");
            }

            if (result.Contains(EmployeeCode))
            {
                var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                result = result.Replace(EmployeeCode, !string.IsNullOrEmpty(employeeCode) ? employeeCode : "");
            }

            if (result.Contains(employeeEmail))
            {
                var email = context.Contact.FirstOrDefault(x => x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                result = result.Replace(employeeEmail, !string.IsNullOrEmpty(email) ? email : "");
            }

            if (result.Contains(employeePhone))
            {
                var phone = context.Contact.FirstOrDefault(x => x.ObjectId == employeeId && x.ObjectType == "EMP")?.Phone;

                result = result.Replace(employeePhone, !string.IsNullOrEmpty(phone) ? phone : "");
            }

            #endregion

            #region replace updated date

            if (result.Contains(UpdatedDate))
            {
                result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
            }

            #endregion

            #region replace url 

            if (result.Contains(Url_Login))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url_Login, loginLink);
                }
            }

            #endregion

            #region replace company infor token

            var company = context.CompanyConfiguration.FirstOrDefault();

            if (result.Contains(companyWebsite))
            {
                if (!string.IsNullOrEmpty(company?.Website))
                {
                    result = result.Replace(companyWebsite, company?.Website);
                }
                else
                {
                    result = result.Replace(companyWebsite, "");
                }
            }

            if (result.Contains(companyAddress))
            {
                if (!string.IsNullOrEmpty(company?.CompanyAddress))
                {
                    result = result.Replace(companyAddress, company?.CompanyAddress);
                }
                else
                {
                    result = result.Replace(companyAddress, "");
                }
            }

            if (result.Contains(companyEmail))
            {
                if (!string.IsNullOrEmpty(company?.Email))
                {
                    result = result.Replace(companyEmail, company?.Email);
                }
                else
                {
                    result = result.Replace(companyEmail, "");
                }
            }

            if (result.Contains(companyName))
            {
                if (!string.IsNullOrEmpty(company?.CompanyName))
                {
                    result = result.Replace(companyName, company?.CompanyName);
                }
                else
                {
                    result = result.Replace(companyName, "");
                }
            }

            if (result.Contains(companyPhone))
            {
                if (!string.IsNullOrEmpty(company?.Phone))
                {
                    result = result.Replace(companyPhone, company?.Phone);
                }
                else
                {
                    result = result.Replace(companyPhone, "");
                }
            }

            #endregion

            #region Tổng thanh toán của báo giá

            var discountValue = 0M;
            if (_model.DiscountType == true)
            {
                discountValue = _model.Amount * (decimal)_model.DiscountValue / 100;
            }
            else
            {
                discountValue = (decimal)_model.DiscountValue;
            }

            var total = _model.Amount - discountValue;

            if (result.Contains(amountQuote))
            {
                result = result.Replace(amountQuote, total.ToString("C2", new CultureInfo("vi-VN")));
            }


            #endregion

            #region ma va ten khach hang

            if (result.Contains(customerCode))
            {
                var code = context.Customer.FirstOrDefault(x => x.CustomerId == _model.ObjectTypeId)?.CustomerCode;

                if (!string.IsNullOrEmpty(code))
                {
                    result = result.Replace(customerCode, code);
                }
                else
                {
                    result = result.Replace(customerCode, "");
                }
            }

            if (result.Contains(customerName))
            {
                var name = context.Customer.FirstOrDefault(x => x.CustomerId == _model.ObjectTypeId)?.CustomerName;

                if (!string.IsNullOrEmpty(name))
                {
                    result = result.Replace(customerName, name);
                }
                else
                {
                    result = result.Replace(customerName, "");
                }
            }

            #endregion

            #endregion

            return result;
        }

        private static string FormatDateToString(DateTime? date)
        {
            var result = "";

            if (date != null)
            {
                result = date.Value.Day.ToString("00") + "/" +
                         date.Value.Month.ToString("00") + "/" +
                         date.Value.Year.ToString("0000") + " " +
                         date.Value.Hour.ToString("00") + ":" +
                         date.Value.Minute.ToString("00");
            }

            return result;
        }

        /// <summary>
        ///     Validate and email address
        ///     It must be follow these rules:
        ///     Has only one @ character
        ///     Has at least 3 chars after the @
        ///     Domain portion contains at least one dot
        ///     Dot can't be before or immediately after the @ character
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>True: If valid, False: If not</returns>
        private static bool ValidateEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;

            if (!Regex.IsMatch(emailAddress, "^[-A-Za-z0-9_@.]+$")) return false;

            // Search for the @ char
            var i = emailAddress.IndexOf("@", StringComparison.Ordinal);

            // There must be at least 3 chars after the @
            if (i <= 0 || i >= emailAddress.Length - 3) return false;

            // Ensure there is only one @
            if (emailAddress.IndexOf("@", i + 1, StringComparison.Ordinal) > 0) return false;


            // Check the domain portion contains at least one dot
            var j = emailAddress.LastIndexOf(".", StringComparison.Ordinal);

            // It can't be before or immediately after the @ character
            //if (j < 0 || j <= i + 1) return false;
            var before = emailAddress.Substring(0, i);
            var after = emailAddress.Substring(i + 1);
            if (before.LastIndexOf(".", StringComparison.Ordinal) == before.Length - 1) return false;
            if (after.IndexOf(".", StringComparison.Ordinal) == 0) return false;

            // EmailAddress is validated
            return true;
        }

        public UpdateQuoteResult SendEmailCustomerQuote(SendEmailCustomerQuoteParameter parameter)
        {
            try
            {
                var quote = context.Quote.FirstOrDefault(q => q.QuoteId == parameter.QuoteId);
                quote.IsSendQuote = true;

                List<string> path = new List<string>();
                var folder = context.Folder.FirstOrDefault(x => x.Active == true && x.FolderType == "BG");
                if (folder == null)
                {
                    return new UpdateQuoteResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục    ."
                    };
                }

                if (parameter.ListFormFile != null && parameter.ListFormFile.Count > 0)
                {
                    var folderName = folder.Url + "\\";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    foreach (IFormFile file in parameter.ListFormFile)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = file.FileName.Trim();

                            var fileInFolder = new FileInFolder();
                            fileInFolder.Active = true;
                            fileInFolder.CreatedById = parameter.UserId;
                            fileInFolder.CreatedDate = DateTime.Now;
                            fileInFolder.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                            fileInFolder.FileInFolderId = Guid.NewGuid();
                            fileInFolder.FileName =
                                fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                            fileInFolder.FolderId = folder.FolderId;
                            fileInFolder.ObjectId = parameter.QuoteId;
                            fileInFolder.ObjectType = "BG";
                            fileInFolder.Size = file.Length.ToString();

                            context.FileInFolder.Add(fileInFolder);
                            fileName = fileInFolder.FileName + "." + fileInFolder.FileExtension;
                            string fullPath = Path.Combine(newPath, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                path.Add(fullPath);
                            }
                        }
                    }
                }

                var SmtpEmailAccount =
                    context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Email")?.SystemValueString;

                var empId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId)?.EmployeeId;
                var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == empId && x.ObjectType == "EMP")?.Email;

                var listInvalidEmail = new List<string>();
                var listEmailSendTo = new List<string>();
                var listEmailSendToCC = new List<string>();
                var listEmailSendToBcc = new List<string>();

                if (empEmail != null)
                {
                    parameter.ListEmailCC = parameter.ListEmailCC != null ? parameter.ListEmailCC : new List<string>();
                    parameter.ListEmailBcc = parameter.ListEmailBcc != null ? parameter.ListEmailBcc : new List<string>();

                    parameter.ListEmail.ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (ValidateEmailAddress(item.Trim()))
                            {
                                listEmailSendTo.Add(item.Trim());
                            }
                            else
                            {
                                listInvalidEmail.Add(item.Trim());
                            }
                        }
                    });

                    parameter.ListEmailCC.ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (ValidateEmailAddress(item.Trim()))
                            {
                                listEmailSendToCC.Add(item.Trim());
                            }
                            else
                            {
                                listInvalidEmail.Add(item.Trim());
                            }
                        }
                    });

                    parameter.ListEmailBcc.ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (ValidateEmailAddress(item.Trim()))
                            {
                                listEmailSendToBcc.Add(item.Trim());
                            }
                            else
                            {
                                listInvalidEmail.Add(item.Trim());
                            }
                        }
                    });

                    if (listEmailSendTo.Count > 0)
                    {
                        var configEntity = context.SystemParameter.ToList();

                        // replace token
                        var emailContent = ReplaceTokenForContent(context, quote, parameter.ContentEmail, configEntity, parameter.UserId);
                        var emailTitle = ReplaceTokenForContent(context, quote, parameter.TitleEmail, configEntity, parameter.UserId);

                        Emailer.SendEmailWithAttachments(context, empEmail, listEmailSendTo, listEmailSendToCC, listEmailSendToBcc,
                        emailTitle, emailContent, path);
                    }
                    else
                    {
                        return new UpdateQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Gửi email không thành công. Email nhận khồng hợp lệ",
                            listInvalidEmail = listInvalidEmail,
                        };
                    }

                }
                else
                {
                    return new UpdateQuoteResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đã có lỗi xảy ra khi lấy Email!"
                    };
                }

                context.Quote.Update(quote);

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "QUOTE";
                note.ObjectId = quote.QuoteId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";
                note.Description = "Gửi mail báo giá khách hàng thành công";

                context.Note.Add(note);
                context.SaveChanges();

                return new UpdateQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Gửi mail thành công",
                    listInvalidEmail = listInvalidEmail,
                };
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataCreateQuoteResult GetMasterDataCreateQuote(GetMasterDataCreateQuoteParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                #region List Kênh bán hàng

                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "IVF")?
                    .CategoryTypeId;
                var listInvestFund = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                #region List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                #region List thông tin bổ sung mẫu của báo giá

                var additionalInformationTemplateId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "GCB" && x.Active == true)?.CategoryTypeId;
                var listAdditionalInformationTemplates = context.Category
                    .Where(x => x.CategoryTypeId == additionalInformationTemplateId).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                            CategoryTypeId = Guid.Empty,
                            CreatedById = Guid.Empty,
                            CountCategoryById = 0
                        }).ToList();

                #endregion

                #region List phương thức thanh toán

                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true)?.CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region List trạng thái báo giá

                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true)?.CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region Lấy Data theo phân quyền dữ liệu

                #region Các List Common

                var listCommonEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listCommonCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listCommonContact = context.Contact
                    .Where(x => x.Active == true &&
                                (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS" || x.ObjectType == "LEA"))
                    .ToList();

                //Cơ hội
                var listCommonLead = context.Lead.Where(x => x.Active == true).ToList();
                var listCommonLeadDetail = context.LeadDetail.Where(x => x.Active == true).ToList();
                var listCommonLeadProductDetailProductAttributeValue =
                    context.LeadProductDetailProductAttributeValue.ToList();

                //Hồ sơ thầu
                var listCommonSaleBidding = context.SaleBidding.Where(x => x.Active == true).ToList();
                var listCommonCostQuote = context.CostsQuote.ToList();
                var listCommonSaleBiddingDetailProductAttribute =
                    context.SaleBiddingDetailProductAttribute.ToList();

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                var listCommonCategory = context.Category.Where(x => x.Active == true).ToList();
                var listCommonVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listCommonProduct = context.Product.Where(x => x.Active == true).ToList();

                #endregion

                #region Các list data cần lấy theo phân quyền dữ liệu

                //List nhân viên bán hàng (Người phụ trách)
                var listEmployee = new List<EmployeeEntityModel>();

                //List khách hàng định danh
                var listCustomer = new List<CustomerEntityModel>();

                //List khách hàng tiềm năng
                var listCustomerNew = new List<CustomerEntityModel>();

                //List Cơ hội có trạng thái Xác nhận
                var listAllLead = new List<LeadEntityModel>();

                //List Hồ sơ thầu có trạng thái Đã duyệt
                var listAllSaleBidding = new List<SaleBiddingEntityModel>();

                #endregion

                #region Các trạng thái cần dùng để lọc các list data

                //Lấy TypeId Trạng thái khách hàng
                var customerTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA")
                    ?.CategoryTypeId;

                //Trạng thái Khách hàng Định danh
                var statusIdentityCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Trạng thái Khách hàng Tiềm năng
                var statusNewCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Lấy TypeId Trạng thái Cơ hội
                var leadTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");

                //Trạng thái Xác nhận của Cơ hội
                var statusApprLead = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == leadTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                //Lấy TypeId Trạng thái Hồ sơ thầu
                var saleBiddingTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");

                //Trạng thái Đã duyệt của Hồ sơ thầu
                var statusApprSaleBidding = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == saleBiddingTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                #endregion

                //Nếu là Quản lý
                if (employee?.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = listCommonEmployee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    UnitLaborNumber = ld.UnitLaborNumber,
                                    UnitLaborPrice = ld.UnitLaborPrice,
                                    ProductName = ld.ProductName != null ? ld.ProductName : ld.Description,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    UnitLaborPrice = ld.UnitLaborPrice,
                                    UnitLaborNumber = ld.UnitLaborNumber,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }
                //Nếu là Nhân viên
                else if (employee?.IsManager == false)
                {

                    var listCus = context.Customer.Where(x => x.PersonInChargeId == employee.EmployeeId).Select(x => x.CustomerId).ToList();

                    listEmployee = listCommonEmployee
                        .Where(x => x.EmployeeId == employee.EmployeeId).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => ( x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId))
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName != null ? ld.ProductName : ld.Description,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList() ?? new List<LeadProductDetailProductAttributeValueModel>()
                                }).ToList() ?? new List<LeadDetailModel>()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }

                #region Lấy thông tin FullName của Cơ hội

                listAllLead.ForEach(item =>
                {
                    var leadContact = listCommonContact.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    item.FullName = firstName + " " + lastName;
                    item.LeadCodeName = item.LeadCode?.Trim() + " - " + item.FullName;
                    item.ContactId = leadContact.ContactId;
                });

                #endregion

                #endregion

                #region Lấy các dữ liệu ngoại lệ nếu có

                if (parameter.ObjectId != null && !String.IsNullOrEmpty(parameter.ObjectType))
                {
                    //Báo giá được tạo từ Cơ hội
                    if (parameter.ObjectType == "LEAD")
                    {

                    }
                    //Báo giá được tạo từ Hồ sơ thầu
                    else if (parameter.ObjectType == "SaleBidding")
                    {

                    }
                    //Báo giá được tạo từ Khách hàng (Định danh hoặc Tiềm năng)
                    else if (parameter.ObjectType == "CUSTOMER")
                    {

                    }
                }

                #endregion

                return new GetMasterDataCreateQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListInvestFund = listInvestFund,
                    ListAdditionalInformationTemplates = listAdditionalInformationTemplates,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerNew = listCustomerNew,
                    ListAllLead = listAllLead,
                    ListAllSaleBidding = listAllSaleBidding,
                    ListParticipant = listParticipant
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetEmployeeByPersonInChargeResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetEmployeeByPersonInChargeResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                #region Giang comment: Đổi lại logic lấy dữ liệu nhân viên bán hàng của báo giá

                //var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

                #endregion

                var listEmployee = new List<EmployeeEntityModel>();

                //Nếu người phụ trách là Quản lý
                if (employee.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = context.Employee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();
                }
                //Nếu người phụ trách là Nhân viên
                else
                {
                    listEmployee.Add(new EmployeeEntityModel
                    {
                        EmployeeId = employee.EmployeeId,
                        EmployeeCode = employee.EmployeeCode,
                        EmployeeName = employee.EmployeeName,
                        EmployeeCodeName = employee.EmployeeCode.Trim() + " - " + employee.EmployeeName.Trim(),
                        IsManager = employee.IsManager,
                        PositionId = employee.PositionId,
                        OrganizationId = employee.OrganizationId,
                        Active = employee.Active
                    });
                }

                //lấy người phụ trách/nhân viên bán hàng cũ nếu bị thay thể hoặc nghỉ việc
                if (parameter.OldEmployeeId != null)
                {
                    var personInCharge = context.Employee.Where(x => x.EmployeeId == parameter.OldEmployeeId)
                     .Select(y => new EmployeeEntityModel
                     {
                         EmployeeId = y.EmployeeId,
                         EmployeeCode = y.EmployeeCode,
                         EmployeeName = y.EmployeeName,
                         IsManager = y.IsManager,
                         PositionId = y.PositionId,
                         OrganizationId = y.OrganizationId,
                         EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                         Active = y.Active
                     }).FirstOrDefault();
                    if (personInCharge != null)
                    {
                        var checkExist = listEmployee.FirstOrDefault(x => x.EmployeeId == personInCharge.EmployeeId);
                        if (checkExist == null)
                        {
                            listEmployee.Add(personInCharge);
                        }
                    }
                }

                return new GetEmployeeByPersonInChargeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListEmployee = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetEmployeeByPersonInChargeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public GetMasterDataUpdateQuoteResult GetMasterDataUpdateQuote(GetMasterDataUpdateQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")?
                    .SystemValueString;

                var quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                {
                    QuoteId = y.QuoteId,
                    QuoteCode = y.QuoteCode,
                    QuoteDate = y.QuoteDate,
                    QuoteName = y.QuoteName,
                    SendQuoteDate = y.SendQuoteDate,
                    Seller = y.Seller,
                    EffectiveQuoteDate = y.EffectiveQuoteDate,
                    ExpirationDate = y.ExpirationDate,
                    Description = y.Description,
                    Note = y.Note,
                    ObjectTypeId = y.ObjectTypeId,
                    ObjectType = y.ObjectType,
                    CustomerContactId = y.CustomerContactId,
                    PaymentMethod = y.PaymentMethod,
                    DiscountType = y.DiscountType,
                    BankAccountId = y.BankAccountId,
                    DaysAreOwed = y.DaysAreOwed,
                    MaxDebt = y.MaxDebt,
                    ReceivedDate = y.ReceivedDate,
                    Amount = y.Amount,
                    DiscountValue = y.DiscountValue,
                    IntendedQuoteDate = y.IntendedQuoteDate,
                    StatusId = y.StatusId,
                    CreatedById = y.CreatedById,
                    CreatedDate = y.CreatedDate,
                    PersonInChargeId = y.PersonInChargeId,
                    SellerName = "",
                    IsSendQuote = y.IsSendQuote,
                    LeadId = y.LeadId,
                    SaleBiddingId = y.SaleBiddingId,
                    ApprovalStep = y.ApprovalStep,
                    InvestmentFundId = y.InvestmentFundId,
                    UpdatedById = y.UpdatedById,
                    UpdatedDate = y.UpdatedDate,
                    Vat = y.Vat,
                    PercentAdvance = y.PercentAdvance,
                    PercentAdvanceType = y.PercentAdvanceType,
                    ConstructionTime = y.ConstructionTime,
                }).FirstOrDefault();

                if (quote == null)
                {
                    return new GetMasterDataUpdateQuoteResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Báo giá không tồn tại trên hệ thống"
                    };
                }

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                #region List Kênh bán hàng

                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "IVF")?
                    .CategoryTypeId;
                var listInvestFund = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                #region List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                #region List phương thức thanh toán

                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true)?.CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region List trạng thái báo giá

                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true)?.CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region Lấy Data theo phân quyền dữ liệu

                #region Các List Common

                var listCommonEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listCommonCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listCommonContact = context.Contact
                    .Where(x => x.Active == true &&
                                (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS" || x.ObjectType == "LEA"))
                    .ToList();

                //Cơ hội
                var listCommonLead = context.Lead.Where(x => x.Active == true).ToList();
                var listCommonLeadDetail = context.LeadDetail.Where(x => x.Active == true).ToList();
                var listCommonLeadProductDetailProductAttributeValue =
                    context.LeadProductDetailProductAttributeValue.ToList();

                //Hồ sơ thầu
                var listCommonSaleBidding = context.SaleBidding.Where(x => x.Active == true).ToList();
                var listCommonCostQuote = context.CostsQuote.ToList();
                var listCommonSaleBiddingDetailProductAttribute =
                    context.SaleBiddingDetailProductAttribute.ToList();

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                var listCommonCategory = context.Category.Where(x => x.Active == true).ToList();
                var listCommonVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listCommonProduct = context.Product.Where(x => x.Active == true).ToList();

                #endregion

                #region Các list data cần lấy theo phân quyền dữ liệu

                //List nhân viên bán hàng (Người phụ trách)
                var listEmployee = new List<EmployeeEntityModel>();

                //List khách hàng định danh
                var listCustomer = new List<CustomerEntityModel>();

                //List khách hàng tiềm năng
                var listCustomerNew = new List<CustomerEntityModel>();

                //List Cơ hội có trạng thái Xác nhận
                var listAllLead = new List<LeadEntityModel>();

                //List Hồ sơ thầu có trạng thái Đã duyệt
                var listAllSaleBidding = new List<SaleBiddingEntityModel>();

                #endregion

                #region Các trạng thái cần dùng để lọc các list data

                //Lấy TypeId Trạng thái khách hàng
                var customerTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA")
                    ?.CategoryTypeId;

                //Trạng thái Khách hàng Định danh
                var statusIdentityCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Trạng thái Khách hàng Tiềm năng
                var statusNewCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Lấy TypeId Trạng thái Cơ hội
                var leadTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");

                //Trạng thái Xác nhận của Cơ hội
                var statusApprLead = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == leadTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                //Lấy TypeId Trạng thái Hồ sơ thầu
                var saleBiddingTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");

                //Trạng thái Đã duyệt của Hồ sơ thầu
                var statusApprSaleBidding = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == saleBiddingTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                #endregion

                //Nếu là Quản lý
                if (employee?.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = listCommonEmployee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName != null ? ld.ProductName : ld.Description,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }
                //Nếu là Nhân viên
                else if (employee?.IsManager == false)
                {
                    listEmployee = listCommonEmployee
                        .Where(x => x.EmployeeId == employee.EmployeeId).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Email,
                            CustomerPhone = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Phone,
                            FullAddress = listCommonContact.FirstOrDefault(x => x.ObjectId == y.CustomerId && x.ObjectType == "CUS")?.Address,
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName != null ? ld.ProductName : ld.Description,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();
                    //Lấy KH của người phụ trách (user đang đăng nhập)
                    var listCusOfLoginUser = context.Customer.Where(x => x.PersonInChargeId == employee.EmployeeId).Select(x => x.CustomerId).ToList();


                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    (listEmployeeId.Contains(x.PersonInChargeId) || listCusOfLoginUser.Contains(x.CustomerId)) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }

                #region Nếu khách hàng của Báo giá không thuộc phân quyền dữ liệu của người đăng nhập

                var _customer = listCommonCustomer.Where(x => x.CustomerId == quote.ObjectTypeId).Select(y =>
                    new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerType = y.CustomerType ?? 1,
                        CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                        CustomerGroupId = y.CustomerGroupId,
                        CustomerEmail = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        StatusId = y.StatusId,
                        MaximumDebtDays = y.MaximumDebtDays,
                        MaximumDebtValue = y.MaximumDebtValue,
                        PersonInChargeId = y.PersonInChargeId
                    }).FirstOrDefault();
                var exists_customer = listCustomer.FirstOrDefault(x => x.CustomerId == quote.ObjectTypeId);
                var exists_customerNew = listCustomerNew.FirstOrDefault(x => x.CustomerId == quote.ObjectTypeId);

                //Nếu không thuộc phân quyền dữ liệu thì
                if (exists_customer == null && exists_customerNew == null)
                {
                    //Nếu là khách hàng
                    if (_customer.StatusId == statusIdentityCustomer)
                    {
                        listCustomer.Add(_customer);
                    }
                    //Nếu là khách hàng tiềm năng
                    else if (_customer.StatusId == statusNewCustomer)
                    {
                        listCustomerNew.Add(_customer);
                    }

                    //Lấy thêm các cơ hội của khách hàng này
                    var listLeadForCustomer = listCommonLead.Where(x =>
                        x.CustomerId == _customer.CustomerId && x.PersonInChargeId != null &&
                        x.StatusId == statusApprLead.CategoryId).Select(
                        y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    //Nếu Cơ hội không nằm trong listAllLead thì thêm vào
                    listLeadForCustomer.ForEach(item =>
                    {
                        var existLead = listAllLead.FirstOrDefault(x => x.LeadId == item.LeadId);

                        if (existLead == null)
                        {
                            listAllLead.Add(item);
                        }
                    });

                    //Lấy thêm các Hồ sơ thầu của khách hàng này
                    var listSaleBiddingForCustomer = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    x.CustomerId == _customer.CustomerId &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();

                    //Nếu Hồ sơ thầu không nằm trong listAllSaleBidding thì thêm vào
                    listSaleBiddingForCustomer.ForEach(item =>
                    {
                        var existSaleBidding =
                            listAllSaleBidding.FirstOrDefault(x => x.SaleBiddingId == item.SaleBiddingId);

                        if (existSaleBidding == null)
                        {
                            listAllSaleBidding.Add(item);
                        }
                    });
                }

                #endregion

                #region Lấy thêm cơ hội trạng thái khác xác nhận

                var existsLead = listAllLead.FirstOrDefault(c => c.LeadId == quote.LeadId);
                if (existsLead == null)
                {
                    var leadExtend = context.Lead.Where(x => x.LeadId == quote.LeadId).Select(y => new LeadEntityModel
                    {
                        LeadId = y.LeadId,
                        CustomerId = y.CustomerId,
                        FullName = "",
                        LeadCode = y.LeadCode,
                        LeadCodeName = "",
                        PersonInChargeId = y.PersonInChargeId,
                        ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    ProductCategory = ld.ProductCategoryId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                    }).FirstOrDefault();

                    if (leadExtend != null)
                    {
                        listAllLead.Add(leadExtend);
                    }
                }

                #endregion

                // #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng định danh
                //
                // listCustomer.ForEach(item =>
                // {
                //     var customerContact =
                //         listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                //     var address = customerContact?.Address?.Trim() ?? "";
                //     address = address == "" ? "" : (address + ", ");
                //     var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                //     var provinceName = province?.ProvinceName?.Trim() ?? "";
                //     provinceName = provinceName == "" ? "" : (provinceName + ", ");
                //     var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                //     var districtName = district?.DistrictName.Trim() ?? "";
                //     districtName = districtName == "" ? "" : (districtName + ", ");
                //     var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                //     var wardName = ward?.WardName?.Trim() ?? "";
                //     wardName = wardName == "" ? "" : (wardName + ", ");
                //
                //     item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                //     item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                //     item.FullAddress = (address + wardName + districtName + provinceName).Trim();
                //
                //     var length = item.FullAddress.Length;
                //     var checkLength = item.FullAddress.LastIndexOf(",");
                //     if ((checkLength + 1) == length && length != 0)
                //     {
                //         item.FullAddress = item.FullAddress.Substring(0, length - 1);
                //     }
                // });
                //
                // #endregion

                // #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng tiềm năng
                //
                // listCustomerNew.ForEach(item =>
                // {
                //     var customerContact =
                //         listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                //     var address = customerContact?.Address?.Trim() ?? "";
                //     address = address == "" ? "" : (address + ", ");
                //     var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                //     var provinceName = province?.ProvinceName?.Trim() ?? "";
                //     provinceName = provinceName == "" ? "" : (provinceName + ", ");
                //     var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                //     var districtName = district?.DistrictName.Trim() ?? "";
                //     districtName = districtName == "" ? "" : (districtName + ", ");
                //     var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                //     var wardName = ward?.WardName?.Trim() ?? "";
                //     wardName = wardName == "" ? "" : wardName;
                //
                //     item.CustomerEmail = item.CustomerType == 1
                //         ? customerContact?.WorkEmail?.Trim()
                //         : customerContact?.Email?.Trim();
                //     item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                //     item.FullAddress = address + wardName + districtName + provinceName;
                //
                //     var length = item.FullAddress.Length;
                //     var checkLength = item.FullAddress.LastIndexOf(",");
                //     if ((checkLength + 1) == length && length != 0)
                //     {
                //         item.FullAddress = item.FullAddress.Substring(0, length - 1);
                //     }
                // });
                //
                // #endregion

                #region Lấy thông tin FullName của Cơ hội

                listAllLead.ForEach(item =>
                {
                    var leadContact = listCommonContact.FirstOrDefault(x => x.ObjectId == item.LeadId);

                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    item.FullName = firstName + " " + lastName;
                    item.LeadCodeName = item.LeadCode?.Trim() + " - " + item.FullName;
                    item.ContactId = leadContact.ContactId;
                });

                #endregion

                #endregion

                #region Các thông tin của Báo giá

                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();
                var listNote = new List<NoteEntityModel>();
                var listQuoteCostDetail = new List<QuoteCostDetailEntityModel>();
                bool isApproval = false;
                var listParticipantId = new List<Guid>();

                #endregion

                #region Lấy chi tiết báo giá theo sản phẩm (OrderDetailType = 0)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                IsPriceInitial = cod.IsPriceInitial,
                                                PriceInitial = cod.PriceInitial,
                                                ProductName = cod.ProductName,
                                                SumAmount = appName == "VNS"
                                                    ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                        cod.DiscountValue,
                                                        cod.DiscountType)
                                                    : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                    cod.DiscountValue, cod.DiscountType, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                OrderNumber = cod.OrderNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice,
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                ProductCategoryId = cod.ProductCategoryId,
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            listProductId.Add(item.ProductId.Value);
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            listCategoryId.Add(item.UnitId.Value);
                    });

                    var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                    var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                    var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            item.ProductNameUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId)
                                .CategoryName;

                        item.ProductCode = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductCode;
                        item.QuoteProductDetailProductAttributeValue =
                            getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType0);

                #endregion

                #region Lấy chi tiết báo giá theo dịch vụ (OrderDetailType = 1)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                ProductName = cod.ProductName,
                                                ProductCode = "",
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                ProductNameUnit = "",
                                                SumAmount = appName == "VNS"
                                                    ? SumAmountVNS(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                        cod.DiscountValue,
                                                        cod.DiscountType)
                                                    : SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                    cod.DiscountValue, cod.DiscountType, 0, 0),
                                                OrderNumber = cod.OrderNumber,
                                                UnitLaborPrice = cod.UnitLaborPrice,
                                                UnitLaborNumber = cod.UnitLaborNumber,
                                                IsPriceInitial = cod.IsPriceInitial,
                                                PriceInitial = cod.PriceInitial,
                                                GuaranteeTime = cod.GuaranteeTime,
                                                ProductCategoryId = cod.ProductCategoryId,
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType1);

                listQuoteDetail = listQuoteDetail.OrderBy(z => z.OrderNumber).ToList();

                #endregion

                #region Lấy list file đính kèm của báo giá

                var listEmp = context.User.ToList();

                listQuoteDocument = (from QD in context.QuoteDocument
                                     where QD.QuoteId == parameter.QuoteId
                                     select new QuoteDocumentEntityModel
                                     {
                                         QuoteDocumentId = QD.QuoteDocumentId,
                                         QuoteId = QD.QuoteId,
                                         DocumentName = QD.DocumentName,
                                         DocumentSize = QD.DocumentSize,
                                         DocumentUrl = QD.DocumentUrl,
                                         CreatedById = QD.CreatedById,
                                         CreatedDate = QD.CreatedDate,
                                         UpdatedById = QD.UpdatedById,
                                         UpdatedDate = QD.UpdatedDate,
                                         Active = QD.Active,
                                     }).ToList();

                listQuoteDocument.ForEach(x =>
                {
                    x.UploadByName = listEmp.FirstOrDefault(e => e.UserId == x.CreatedById).UserName;
                });

                #endregion

                #region Lấy list thông tin bổ sung của báo giá

                listAdditionalInformation = context.AdditionalInformation
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(y =>
                        new AdditionalInformationEntityModel
                        {
                            AdditionalInformationId = y.AdditionalInformationId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Title = y.Title,
                            Content = y.Content,
                            Ordinal = y.Ordinal
                        }).OrderBy(z => z.Ordinal).ToList();

                #endregion

                #region Lấy list note (ghi chú)

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(
                        y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                if (listNote.Count > 0)
                {
                    var listNoteId = listNote.Select(x => x.NoteId).ToList();
                    var listUser = context.User.ToList();
                    var _listAllEmployee = context.Employee.ToList();
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                        y => new NoteDocumentEntityModel
                        {
                            DocumentName = y.DocumentName,
                            DocumentSize = y.DocumentSize,
                            DocumentUrl = y.DocumentUrl,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            NoteDocumentId = y.NoteDocumentId,
                            NoteId = y.NoteId
                        }
                    ).ToList();

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        if (_user != null)
                        {
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        }
                    });

                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                #region Lấy list chi phí của báo giá

                var quoteCost = context.QuoteCostDetail
                    .Where(c => c.QuoteId == parameter.QuoteId && c.Active == true).ToList();
                quoteCost.ForEach(item =>
                {
                    var cost = context.Cost.FirstOrDefault(c => c.CostId == item.CostId);
                    QuoteCostDetailEntityModel obj = new QuoteCostDetailEntityModel();
                    obj.QuoteCostDetailId = item.QuoteCostDetailId;
                    obj.CostId = item.CostId;
                    obj.QuoteId = item.QuoteId;
                    obj.Quantity = item.Quantity;
                    obj.UnitPrice = item.UnitPrice;
                    obj.CostName = cost.CostName;
                    obj.CostCode = cost.CostCode;
                    obj.Active = item.Active;
                    obj.CreatedById = item.CreatedById;
                    obj.CreatedDate = item.CreatedDate;
                    obj.UpdatedById = item.UpdatedById;
                    obj.UpdatedDate = item.UpdatedDate;
                    obj.IsInclude = item.IsInclude;

                    listQuoteCostDetail.Add(obj);
                });

                #endregion

                #region Kiểm tra điều kiện để được phê duyệt báo giá

                var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                // lấy trạng thái chờ phê duyệt báo giá
                var statusQuote = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI");
                var statusQuoteDLY = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusQuote.CategoryTypeId && c.CategoryCode == "DLY");

                if (quote.ApprovalStep != null && quote.StatusId == statusQuoteDLY.CategoryId)
                {
                    var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                        ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                    if (workFlowStep == null)
                    {
                        workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                            .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                    }

                    if ((workFlowStep.ApprovebyPosition && workFlowStep.ApproverPositionId == employee.PositionId)
                        || (!workFlowStep.ApprovebyPosition && workFlowStep.ApproverId == employee.EmployeeId))
                    {
                        isApproval = true;
                    }
                }

                #endregion

                #region Lấy người tham gia

                listParticipantId = context.QuoteParticipantMapping
                    .Where(x => x.QuoteId == parameter.QuoteId && x.EmployeeId != null)
                    .Select(y => y.EmployeeId.Value).ToList();

                #endregion

                #region Kiểm tra người đang đăng nhập có phải người tham gia không

                bool isParticipant = false;
                var existsParticipant = listParticipantId.FirstOrDefault(x => x == employee.EmployeeId);
                //Nếu là người tham gia và người tham gia không phải người phụ trách thì
                if (existsParticipant != null && existsParticipant != Guid.Empty &&
                    existsParticipant != quote.PersonInChargeId)
                {
                    isParticipant = true;

                    if (isApproval)
                    {
                        isParticipant = false;
                    }
                }

                #endregion

                #region Lấy list quà khuyến mãi

                var listPromotionObjectApply = context.PromotionObjectApply
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE").Select(y =>
                        new PromotionObjectApplyEntityModel
                        {
                            PromotionObjectApplyId = y.PromotionObjectApplyId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            PromotionId = y.PromotionId,
                            ConditionsType = y.ConditionsType,
                            PropertyType = y.PropertyType,
                            NotMultiplition = y.NotMultiplition,
                            PromotionMappingId = y.PromotionMappingId,
                            ProductId = y.ProductId,
                            SoLuongTang = y.SoLuongTang,
                            LoaiGiaTri = y.LoaiGiaTri,
                            GiaTri = y.GiaTri,
                            Amount = y.Amount,
                            SoTienTu = y.SoTienTu
                        }).ToList();

                if (listPromotionObjectApply.Count > 0)
                {
                    var listPromotionId = listPromotionObjectApply.Select(y => y.PromotionId).Distinct().ToList();
                    var listProductId = listPromotionObjectApply.Where(x => x.ProductId != null)
                        .Select(y => y.ProductId).Distinct().ToList();
                    var productUnitTypeId =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                            .CategoryTypeId;
                    var listProductUnit = context.Category.Where(x => x.CategoryTypeId == productUnitTypeId)
                        .ToList();

                    var listPromotion = context.Promotion.Where(x => listPromotionId.Contains(x.PromotionId)).ToList();
                    var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).ToList();

                    listPromotionObjectApply.ForEach(item =>
                    {
                        var promotion = listPromotion.FirstOrDefault(x => x.PromotionId == item.PromotionId);

                        if (promotion != null)
                        {
                            item.PromotionName = promotion.PromotionName;
                        }

                        var product = listProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                        if (product != null)
                        {
                            item.PromotionProductName = product.ProductName;

                            var unitName = listProductUnit.FirstOrDefault(x => x.CategoryId == product.ProductUnitId);

                            if (unitName != null)
                            {
                                item.ProductUnitName = unitName.CategoryName;
                            }
                        }
                        else
                        {
                            item.PromotionProductName = "Phiếu giảm giá";
                        }
                    });
                }

                #endregion

                #region Lấy list ke hoach
                var listQuotePlan = context.QuotePlan
                   .Where(x => x.QuoteId == parameter.QuoteId).Select(p =>
                       new QuotePlanEntityModel
                       {
                           PlanId = p.PlanId,
                           Tt = p.Tt,
                           Finished = p.Finished,
                           ExecTime = p.ExecTime,
                           SumExecTime = p.SumExecTime,
                           QuoteId = p.QuoteId,
                           TenantId = p.TenantId
                       }).OrderBy(x => x.Tt).ToList();
                #endregion

                #region Lấy danh sách phạm vi triển khai

                var listQuoteScope = new List<QuoteScopeEntityModel>();

                var scopeEntityModels = context.QuoteScope.Where(x => x.QuoteId == parameter.QuoteId).Select(p => new QuoteScopeEntityModel()
                {
                    ScopeId = p.ScopeId,
                    Tt = p.Tt,
                    Category = p.Category,
                    Description = p.Description,
                    QuoteId = p.QuoteId,
                    TenantId = p.TenantId,
                    Level = p.Level,
                    ParentId = p.ParentId
                }).OrderBy(o => o.Tt).ToList();

                //listQuoteScope = SetTTChildren(listQuoteScope);

                var listQuoteScopeOrderNumber = scopeEntityModels.Where(x => x.Tt.Trim() != "").Select(y => y.Tt).ToList();

                //listQuoteScopeOrderNumber.Sort(new StringNumberComparer());

                //List<string> list = new List<string> { "2.2", "1", "1.1.1", "1.1.3", "1.1", "2", "1.1.4", "1.1.5", "1.1.6", "1.1.7", "1.1.2", "2.3", "1.1.8", "1.1.5.1", "1.1.9", "1.1.10", "1.2", "1.1.5.2", "1.3", "2.1", "2.3" };
                listQuoteScopeOrderNumber.Sort((x, y) =>
                {
                    int ret = 0;
                    var xsplit = x.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    var ysplit = y.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    for (int i = 0; i < Math.Max(xsplit.Count, ysplit.Count); i++)
                    {
                        if (xsplit.Count - 1 < i)
                        {
                            ret = -1;
                            return ret;
                        }
                        else if (ysplit.Count - 1 < i)
                        {
                            ret = 1;
                            return ret;
                        }
                        else
                        {
                            ret = xsplit[i] - ysplit[i];
                            if (ret != 0)
                                return ret;
                        }
                    }
                    return ret;
                });

                var firstItem = scopeEntityModels.FirstOrDefault(x => x.Tt.Trim() == "");
                if (firstItem != null)
                {
                    listQuoteScope.Add(firstItem);

                    listQuoteScopeOrderNumber.ForEach(item =>
                    {
                        var quoteScope = new QuoteScopeEntityModel();

                        quoteScope = scopeEntityModels.FirstOrDefault(x => x.Tt == item);

                        if (quoteScope != null)
                        {
                            listQuoteScope.Add(quoteScope);
                        }
                    });
                }

                #endregion

                #region Lay Danh sach dieu khoan thanh toan

                var listQuotePaymentTerm = context.QuotePaymentTerm.Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(x => new QuotePaymentTermEntityModel()
                    {
                        PaymentTermId = x.PaymentTermId,
                        QuoteId = x.QuoteId,
                        OrderNumber = x.OrderNumber,
                        Milestone = x.Milestone,
                        PaymentPercentage = x.PaymentPercentage,
                        CreatedDate = x.CreatedDate,
                        CreatedById = x.CreatedById,
                    }).OrderBy(y => y.OrderNumber).ToList();

                #endregion

                #region Điều kiện hiển thị các button

                //Trạng thái báo giá
                var statusCode = listQuoteStatus.FirstOrDefault(x => x.CategoryId == quote.StatusId)?.CategoryCode;

                //Gửi phê duyệt
                bool isShowGuiPheDuyet = false;
                bool isShowPheDuyet = false;
                bool isShowTuChoi = false;

                //Trạng thái Nháp
                if (statusCode == "MTA" && !isParticipant)
                {
                    isShowGuiPheDuyet = true;
                }

                //Trạng thái Chờ phê duyệt
                if (statusCode == "DLY")
                {
                    var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectId == quote.QuoteId &&
                                                                       x.DoiTuongApDung == 3 &&
                                                                       x.TrangThai == 0)
                        .OrderByDescending(z => z.Stt)
                        .FirstOrDefault();

                    //Nếu là phê duyệt trưởng bộ phận
                    if (buocHienTai?.LoaiPheDuyet == 1)
                    {
                        var listDonViId_NguoiPhuTrach = context.ThanhVienPhongBan
                            .Where(x => x.EmployeeId == quote.Seller)
                            .Select(y => y.OrganizationId).ToList();

                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == employee.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listDonViId_NguoiPhuTrach.Contains(
                                                                                     x.OrganizationId));

                        if (countPheDuyet > 0)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                    //Nếu là phòng ban phê duyệt
                    else if (buocHienTai?.LoaiPheDuyet == 2)
                    {
                        //Lấy list Phòng ban đã phê duyệt ở bước hiện tại
                        var listDonViIdDaPheDuyet = context.PhongBanApDung
                            .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                        x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy list Phòng ban chưa phê duyệt ở bước hiện tại
                        var listDonViId = context.PhongBanTrongCacBuocQuyTrinh
                            .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                        !listDonViIdDaPheDuyet.Contains(x.OrganizationId))
                            .Select(y => y.OrganizationId).ToList();

                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == employee.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listDonViId.Contains(
                                                                                     x.OrganizationId));

                        if (countPheDuyet > 0)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                }

                #endregion

                #region Lấy ra lịch sử báo giá

                var listQuoteApproveHistory = context.QuoteApproveHistory.Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(y => new QuoteApproveHistoryEntityModel()
                    {
                        Id = y.Id,
                        QuoteName = y.QuoteName,
                        QuoteCode = y.QuoteCode,
                        SendApproveDate = y.SendApproveDate,
                        Amount = y.Amount,
                        DiscountType = y.DiscountType,
                        DiscountValue = y.DiscountValue,
                        AmountPriceInitial = y.AmountPriceInitial,
                        AmountPriceProfit = y.AmountPriceProfit,
                        AmountIncreaseDecrease = 0,
                    })
                    .OrderBy(z => z.SendApproveDate)
                    .ToList();

                var listQuoteApproveDetailHistory = context.QuoteApproveDetailHistory.Where(x => x.QuoteId == parameter.QuoteId)
                    .Select(y => new QuoteApproveDetailHistoryEntityModel()
                    {
                        Id = y.Id,
                        QuoteId = y.QuoteId,
                        QuoteApproveHistoryId = y.QuoteApproveHistoryId,
                        ProductId = y.ProductId,
                        DiscountType = y.DiscountType,
                        DiscountValue = y.DiscountValue,
                        Vat = y.Vat,
                        Quantity = y.Quantity,
                        ExchangeRate = y.ExchangeRate,
                        UnitPrice = y.UnitPrice,
                    }).ToList();

                var listproduct = context.Product.ToList();

                listQuoteApproveDetailHistory.ForEach(item =>
                {
                    item.ProductName = listproduct.FirstOrDefault(x => x.ProductId == item.ProductId)?.ProductName;
                    item.ProductCode = listproduct.FirstOrDefault(x => x.ProductId == item.ProductId)?.ProductCode;

                });

                #endregion

                return new GetMasterDataUpdateQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListInvestFund = listInvestFund,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerNew = listCustomerNew,
                    ListAllLead = listAllLead,
                    ListAllSaleBidding = listAllSaleBidding,
                    ListParticipant = listParticipant,
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail,
                    ListQuoteDocument = listQuoteDocument,
                    ListAdditionalInformation = listAdditionalInformation,
                    ListNote = listNote,
                    ListQuoteCostDetail = listQuoteCostDetail,
                    IsApproval = isApproval,
                    ListParticipantId = listParticipantId,
                    IsParticipant = isParticipant,
                    ListPromotionObjectApply = listPromotionObjectApply,
                    ListQuotePlans = listQuotePlan,
                    ListQuoteScopes = listQuoteScope,
                    ListQuotePaymentTerm = listQuotePaymentTerm,
                    IsShowGuiPheDuyet = isShowGuiPheDuyet,
                    IsShowPheDuyet = isShowPheDuyet,
                    IsShowTuChoi = isShowTuChoi,
                    ListQuoteApproveHistory = listQuoteApproveHistory,
                    ListQuoteApproveDetailHistory = listQuoteApproveDetailHistory,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataUpdateQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateQuoteResult ApprovalOrRejectQuote(ApprovalOrRejectQuoteParameter parameter)
        {
            try
            {
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI");
                if (parameter.IsApproval)
                {
                    //đồng ý phê duyệt
                    var category = context.Category.FirstOrDefault(c =>
                        c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "CHO");
                    parameter.ListQuoteId = parameter.ListQuoteId.Distinct().ToList();
                    parameter.ListQuoteId.ForEach(quoteId =>
                    {
                        var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                        var quote = context.Quote.FirstOrDefault(q => q.QuoteId == quoteId);
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                            ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                        if (workFlowStep == null)
                        {
                            workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                                .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                        }

                        quote.ApprovalStep = workFlowStep.NextStepNumber;
                        if (workFlowStep.NextStepNumber == 0)
                        {
                            quote.StatusId = category.CategoryId;
                            Note note = new Note();
                            note.NoteId = Guid.NewGuid();
                            note.ObjectType = "QUOTE";
                            note.ObjectId = quote.QuoteId;
                            note.Type = "ADD";
                            note.Description = "Chuyển trạng thái đã duyệt thành công";

                            if (!string.IsNullOrEmpty(parameter.Description))
                            {
                                note.Description = "Chuyển trạng thái đã duyệt thành công với lý do: " +
                                                   parameter.Description.Trim();
                            }

                            note.Active = true;
                            note.CreatedById = parameter.UserId;
                            note.CreatedDate = DateTime.Now;
                            note.NoteTitle = "Đã thêm ghi chú";

                            context.Note.Add(note);
                        }
                        else
                        {
                            var employeeApproval = StepApprovalQuote(quote.ApprovalStep, employeeId);
                            employeeApproval.ForEach(item =>
                            {
                                var emailApproval = contact.FirstOrDefault(e => e.ObjectId == item && e.ObjectType == "EMP");
                                if (emailApproval.Email != null)
                                {
                                    GetConfiguration();
                                    string webRootPath = _hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                                    var file = Path.Combine(webRootPath, "SendEmailQuoteApprove.html");
                                    string body = string.Empty;
                                    using (StreamReader reader = new StreamReader(file))
                                    {
                                        body = reader.ReadToEnd();
                                    }

                                    //Thay doi cac thuoc tinh can thiet trong htmltemplate
                                    body = body.Replace("[NameApprove]", emailApproval.FirstName + " " + emailApproval.LastName);
                                    body = body.Replace("[QuoteName]", quote.QuoteCode);
                                    body = body.Replace("[TotalAmount]", string.Format("{0:#,0}", quote.Amount));
                                    body = body.Replace("{forgotUrl}", Domain + "/customer/quote-detail;quoteId=" + quote.QuoteId);

                                    Emailer.SendEmail(context, new[] { emailApproval.Email }, new List<string>(),
                                        new List<string>(),
                                        string.Format("Yêu cầu phê duyệt báo giá {0}", quote.QuoteCode), body);
                                }
                            });

                            Note note = new Note();
                            note.NoteId = Guid.NewGuid();
                            note.ObjectType = "QUOTE";
                            note.ObjectId = quote.QuoteId;
                            note.Type = "ADD";
                            note.Description = "Đã duyệt thành công, chuyển sang bước duyệt thứ: " + quote.ApprovalStep;

                            if (!string.IsNullOrEmpty(parameter.Description))
                            {
                                note.Description = "Đã duyệt thành công với lý do: " + parameter.Description.Trim() +
                                                   ", chuyển sang bước duyệt thứ: " +
                                                   quote.ApprovalStep;
                            }

                            note.Active = true;
                            note.CreatedById = parameter.UserId;
                            note.CreatedDate = DateTime.Now;
                            note.NoteTitle = "Đã thêm ghi chú";

                            context.Note.Add(note);
                        }

                        context.Quote.Update(quote);

                        context.SaveChanges();

                        #region Gửi thông báo

                        var _note = new Note();
                        _note.Description = parameter.Description;
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "APPROVAL", new Queue(),
                            quote, true, _note);

                        #endregion

                    });

                    return new UpdateQuoteResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Gửi phê duyệt thành công"
                    };
                }
                else
                {
                    //từ chối phê duyệt
                    var category = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "TUCHOI");
                    var statusCloseNotHit = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DTR");
                    var statusNew = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA");
                    parameter.ListQuoteId.ForEach(quoteId =>
                    {
                        var quote = context.Quote.FirstOrDefault(q => q.QuoteId == quoteId);
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;
                        quote.ApprovalStep = 0;
                        if (parameter.RejectReason.Equals("EMP"))
                        {
                            // Quản lý từ chối, về trạng thái Nháp
                            quote.StatusId = statusNew.CategoryId;
                        }
                        else if (parameter.RejectReason.Equals("CUS"))
                        {
                            // Khách hàng từ chối về trạng thái Đóng-Không trúng
                            quote.StatusId = statusCloseNotHit.CategoryId;
                        }
                        else
                        {
                            quote.StatusId = category.CategoryId;
                        }
                        context.Quote.Update(quote);

                        Note note = new Note();
                        note.NoteId = Guid.NewGuid();
                        note.ObjectType = "QUOTE";
                        note.ObjectId = quote.QuoteId;
                        note.Type = "ADD";
                        note.Description = "Báo giá đã bị từ chối vì: " + parameter.Description;
                        note.Active = true;
                        note.CreatedById = parameter.UserId;
                        note.CreatedDate = DateTime.Now;
                        note.NoteTitle = "Đã thêm ghi chú";

                        context.Note.Add(note);
                        context.SaveChanges();

                        #region Gửi thông báo

                        var _note = new Note();
                        _note.Description = parameter.Description;
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "REJECT", new Queue(),
                            quote, true, _note);

                        #endregion
                    });

                    return new UpdateQuoteResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Từ chối phê duyệt thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchQuoteResult SearchQuoteAprroval(SearchQuoteParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var listQuote = new List<QuoteEntityModel>();
                var customerOrder = context.CustomerOrder.ToList();
                var customerOrderCost = context.QuoteCostDetail.ToList();

                #region Lấy list status của báo giá

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                                    new CategoryEntityModel
                                    {
                                        CategoryId = y.CategoryId,
                                        CategoryName = y.CategoryName,
                                        CategoryCode = y.CategoryCode,
                                        CategoryTypeId = Guid.Empty,
                                        CreatedById = Guid.Empty,
                                        CountCategoryById = 0
                                    }).ToList();

                #endregion

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();

                parameter.QuoteCode = parameter.QuoteCode == null ? "" : parameter.QuoteCode.Trim();

                listQuote = context.Quote.Where(x => (parameter.QuoteCode == "" || x.QuoteCode.Contains(parameter.QuoteCode)) &&
                                    (parameter.ListStatusQuote.Count == 0 || parameter.ListStatusQuote.Contains(x.StatusId)) &&
                                    x.Active == true && x.Seller != null)
                                    .Select(y => new QuoteEntityModel
                                    {
                                        QuoteId = y.QuoteId,
                                        QuoteCode = y.QuoteCode,
                                        QuoteDate = y.QuoteDate,
                                        Seller = y.Seller,
                                        Description = y.Description,
                                        Note = y.Note,
                                        ObjectTypeId = y.ObjectTypeId,
                                        ObjectType = y.ObjectType,
                                        PaymentMethod = y.PaymentMethod,
                                        DaysAreOwed = y.DaysAreOwed,
                                        IntendedQuoteDate = y.IntendedQuoteDate,
                                        SendQuoteDate = y.SendQuoteDate,
                                        MaxDebt = y.MaxDebt,
                                        ExpirationDate = y.ExpirationDate,
                                        ReceivedDate = y.ReceivedDate,
                                        ReceivedHour = y.ReceivedHour,
                                        RecipientName = y.RecipientName,
                                        LocationOfShipment = y.LocationOfShipment,
                                        ShippingNote = y.ShippingNote,
                                        RecipientPhone = y.RecipientPhone,
                                        RecipientEmail = y.RecipientEmail,
                                        PlaceOfDelivery = y.PlaceOfDelivery,
                                        Amount = y.Amount,
                                        DiscountValue = y.DiscountValue,
                                        StatusId = y.StatusId,
                                        CreatedById = y.CreatedById,
                                        CreatedDate = y.CreatedDate,
                                        UpdatedById = y.UpdatedById,
                                        UpdatedDate = y.UpdatedDate,
                                        Active = y.Active,
                                        DiscountType = y.DiscountType,
                                        PersonInChargeId = y.PersonInChargeId,
                                        CountQuoteInOrder = CountQuoteInCustomerOrder(y.QuoteId, customerOrder),
                                        QuoteStatusName = "",
                                        BackgroundColorForStatus = "",
                                        CustomerName = "",
                                        ApprovalStep = y.ApprovalStep,
                                        TotalAmountAfterVat = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                                        //}).OrderByDescending(z => z.QuoteDate).ToList();
                                    }).OrderBy(z => z.UpdatedDate).ToList();

                listQuote.ForEach(x =>
                {
                    x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                        x.TotalAmountAfterVat, listPromotionObjectApply);
                });

                if (parameter.IsCompleteInWeek)
                {
                    // Báo giá phải hoàn thành trong tuần
                    parameter.StartDate = FirstDateOfWeek();
                    parameter.EndDate = LastDateOfWeek();

                    listQuote = listQuote.Where(x =>
                        (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                         parameter.StartDate <= x.IntendedQuoteDate) &&
                        (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                         parameter.EndDate >= x.IntendedQuoteDate) &&
                        x.SendQuoteDate == null).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                                    (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                                     parameter.StartDate <= x.QuoteDate) &&
                                    (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                                     parameter.EndDate >= x.QuoteDate)).ToList();
                }

                if (parameter.IsOutOfDate)
                {
                    // Báo giá hết hiệu lực
                    listQuote = listQuote.Where(x => x.ExpirationDate < DateTime.Now.Date && x.ExpirationDate != null)
                        .OrderBy(z => z.UpdatedDate).ToList();
                }

                if (employee.IsManager)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    listQuote = listQuote.Where(x =>
                        (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) ||
                        x.PersonInChargeId == employee.EmployeeId).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                        x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId).ToList();
                }

                #region Lấy tên Đối tượng và tên Trạng thái của Báo giá

                if (listQuote != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            var quoteStatus = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value);
                            switch (quoteStatus.CategoryCode)
                            {
                                case "MTA":
                                    item.BackgroundColorForStatus = "#FFC000";
                                    break;
                                case "CHO":
                                    item.BackgroundColorForStatus = " #9C00FF";
                                    break;
                                case "DTH":
                                    item.BackgroundColorForStatus = "#6D98E7";
                                    break;
                                case "DTR":
                                    item.BackgroundColorForStatus = "#FF0000";
                                    break;
                                case "DLY":
                                    item.BackgroundColorForStatus = "#46B678";
                                    break;
                                case "HUY":
                                    item.BackgroundColorForStatus = "#333333";
                                    break;
                                case "HOA":
                                    item.BackgroundColorForStatus = "#666666";
                                    break;
                                case "TUCHOI":
                                    item.BackgroundColorForStatus = "#878d96";
                                    break;
                            }

                            item.QuoteStatusName = quoteStatus.CategoryName;
                        }

                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }

                #endregion

                var statusApprove = listStatus.FirstOrDefault(c => c.CategoryCode == "DLY").CategoryId;
                listQuote = listQuote.Where(q => q.StatusId == statusApprove).ToList();


                var listQuoteApproval = new List<QuoteEntityModel>();
                var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                var workFlowStep = context.WorkFlowSteps
                    .Where(ws => ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber > 1).ToList();
                var maxStep = workFlowStep.OrderByDescending(z => z.StepNumber).FirstOrDefault().StepNumber;

                workFlowStep.ForEach(item =>
                {
                    if ((item.ApprovebyPosition && item.ApproverPositionId == employee.PositionId)
                    || (!item.ApprovebyPosition && item.ApproverId == employee.EmployeeId))
                    {
                        var customerStep = listQuote.Where(ca => ca.ApprovalStep == item.StepNumber).ToList();

                        listQuoteApproval.AddRange(customerStep);

                        #region Trong trường hợp quy trình có thay đổi và ApprovalStep của Quote không còn tôn tại trong quy trình mới

                        if (item.StepNumber == maxStep)
                        {
                            var ignoreStep = listQuote.Where(x => x.ApprovalStep > maxStep).ToList();
                            listQuoteApproval.AddRange(ignoreStep);
                        }

                        #endregion
                    }
                });

                // cộng chi phí
                if (appName == "VNS")
                {
                    listQuote.ForEach(item =>
                    {
                        var costDetail = customerOrderCost.Where(d => d.QuoteId == item.QuoteId).Sum(d => (d.UnitPrice * d.Quantity));
                        item.Amount += costDetail;
                    });
                }

                return new SearchQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListQuote = listQuoteApproval,
                    ListStatus = listStatus
                };
            }
            catch (Exception e)
            {
                return new SearchQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }



        public List<Guid> StepApprovalQuote(int? step, Guid? employeeId)
        {
            List<Guid> employeeList = new List<Guid>();
            var employees = context.Employee.Where(e => e.Active == true).ToList();
            var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
            var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws => ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == step);

            if (workFlowStep.ApprovebyPosition)
            {
                var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                employeeList = employees.Where(e => e.PositionId == workFlowStep.ApproverPositionId
                && (listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId))
                ).Select(e => e.EmployeeId).ToList();
            }
            else
            {
                employeeList.Add(Guid.Parse(workFlowStep.ApproverId.ToString()));
            }
            return employeeList;
        }

        public CreateQuoteScopeResult CreateScope(CreateQuoteScopeParameter parameter)
        {
            try
            {
                var parent = context.QuoteScope.FirstOrDefault(o => o.ScopeId == parameter.ParentId);
                var parentLvl = parent?.Level ?? 1;

                var quoteScope = context.QuoteScope.FirstOrDefault(o => o.ScopeId == parameter.ScopeId);

                string tt = "";
                if (string.IsNullOrEmpty(parameter.ParentId.ToString()))
                    tt = "";
                else
                {
                    int countchild = context.QuoteScope.Count(o => o.ParentId == parameter.ParentId) + 1;
                    tt = !string.IsNullOrEmpty(parent.Tt) ? (parent.Tt + "." + countchild) : countchild.ToString();
                }

                if (quoteScope == null)
                {
                    quoteScope = new QuoteScope()
                    {
                        ScopeId = Guid.NewGuid(),
                        Tt = tt,
                        Category = parameter.Category,
                        Description = parameter.Description,
                        Level = string.IsNullOrEmpty(parameter.ParentId.ToString()) ? parameter.Level : parentLvl + 1,
                        ParentId = parameter.ParentId,
                        QuoteId = parameter.QuoteId,
                        CreatedDate = DateTime.Now
                    };
                    context.QuoteScope.Add(quoteScope);
                }
                else
                {
                    quoteScope.Category = parameter.Category;
                    quoteScope.Description = parameter.Description;

                    context.QuoteScope.Update(quoteScope);
                }

                context.SaveChanges();

                //var listQuoteScope = context.QuoteScope.Where(x => x.QuoteId == parameter.QuoteId).Select(p => new QuoteScopeEntityModel()
                //{
                //    ScopeId = p.ScopeId,
                //    Tt = p.Tt,
                //    Category = p.Category,
                //    Description = p.Description,
                //    QuoteId = p.QuoteId,
                //    TenantId = p.TenantId,
                //    Level = p.Level,
                //    ParentId = p.ParentId
                //}).OrderBy(o => o.CreatedDate).ToList();

                //listQuoteScope = SetTTChildren(listQuoteScope);

                var listQuoteScope = new List<QuoteScopeEntityModel>();

                var scopeEntityModels = context.QuoteScope.Where(x => x.QuoteId == parameter.QuoteId).Select(p => new QuoteScopeEntityModel()
                {
                    ScopeId = p.ScopeId,
                    Tt = p.Tt,
                    Category = p.Category,
                    Description = p.Description,
                    QuoteId = p.QuoteId,
                    TenantId = p.TenantId,
                    Level = p.Level,
                    ParentId = p.ParentId
                }).OrderBy(o => o.Tt).ToList();

                var listQuoteScopeOrderNumber = scopeEntityModels.Where(x => x.Tt.Trim() != "").Select(y => y.Tt).ToList();

                listQuoteScopeOrderNumber.Sort((x, y) =>
                {
                    int ret = 0;
                    var xsplit = x.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    var ysplit = y.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    for (int i = 0; i < Math.Max(xsplit.Count, ysplit.Count); i++)
                    {
                        if (xsplit.Count - 1 < i)
                        {
                            ret = -1;
                            return ret;
                        }
                        else if (ysplit.Count - 1 < i)
                        {
                            ret = 1;
                            return ret;
                        }
                        else
                        {
                            ret = xsplit[i] - ysplit[i];
                            if (ret != 0)
                                return ret;
                        }
                    }
                    return ret;
                });

                var firstItem = scopeEntityModels.FirstOrDefault(x => x.Tt.Trim() == "");
                listQuoteScope.Add(firstItem);

                listQuoteScopeOrderNumber.ForEach(item =>
                {
                    var _quoteScope = new QuoteScopeEntityModel();

                    _quoteScope = scopeEntityModels.FirstOrDefault(x => x.Tt == item);

                    if (_quoteScope != null)
                    {
                        listQuoteScope.Add(_quoteScope);
                    }
                });

                return new CreateQuoteScopeResult
                {
                    MessageCode = CommonMessage.Quote.CREATE_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                    ListQuoteScopes = listQuoteScope,
                };
            }
            catch (Exception ex)
            {
                return new CreateQuoteScopeResult
                {
                    MessageCode = CommonMessage.Quote.CREATE_SCOPE_FAIL + ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<QuoteScopeEntityModel> SetTTChildren(List<QuoteScopeEntityModel> listQuoteScope)
        {
            var listnew = new List<QuoteScopeEntityModel>();

            listQuoteScope.Where(l => l.ParentId == null).ToList().ForEach(item =>
            {
                item.Tt = "";
                var list = listQuoteScope.Where(l => l.ParentId == item.ScopeId).OrderBy(o => o.Tt);
                int index = 1;
                foreach (var item1 in list)
                {
                    item1.Tt = index.ToString();
                    listnew.Add(item1);

                    var list2 = listQuoteScope.Where(l => l.ParentId == item1.ScopeId).OrderBy(o => o.Tt);
                    int index2 = 1;
                    foreach (var item2 in list2)
                    {
                        item2.Tt = item1.Tt + "." + index2;
                        listnew.Add(item2);

                        var list3 = listQuoteScope.Where(l => l.ParentId == item2.ScopeId).OrderBy(o => o.Tt);
                        int index3 = 1;
                        foreach (var item3 in list3)
                        {
                            item3.Tt = item2.Tt + "." + index3;
                            listnew.Add(item3);

                            var list4 = listQuoteScope.Where(l => l.ParentId == item3.ScopeId).OrderBy(o => o.Tt);
                            int index4 = 1;
                            foreach (var item4 in list4)
                            {
                                item4.Tt = item3.Tt + "." + index4;
                                listnew.Add(item4);

                                var list5 = listQuoteScope.Where(l => l.ParentId == item4.ScopeId).OrderBy(o => o.Tt);
                                int index5 = 1;
                                foreach (var item5 in list5)
                                {
                                    item5.Tt = item4.Tt + "." + index5;
                                    listnew.Add(item5);

                                    index5++;
                                }
                                index4++;
                            }
                            index3++;
                        }
                        index2++;
                    }
                    index++;
                }

                listnew.Add(item);
            });

            return listnew.OrderBy(o => o.Tt).ToList();
        }

        public DeleteQuoteScopeResult DeleteScope(DeleteQuoteScopeParameter parameter)
        {
            try
            {
                /*  ************** Sắp xếp lại danh sách quoteScope sau khi xóa theo đúng thứ tự **************
                 *  Sử dụng transaction
                    *  B1: Sắp xếp đúng thứ tự TT những quoteScope cùng cha với quotescope sẽ xóa
                    *  B2: Xóa quoteScope đấy
                    *  B3: Update lại tt của quoteScope vừa xóa sang quoteScope phía sau
                    *  B4: Update TT của con thuộc quoteScope ở B2 theo TT cha mới
                       => Lặp lại B3-B4 cho đến hết
                   */

                var quoteScope = context.QuoteScope.FirstOrDefault(o => o.ScopeId == parameter.ScopeId);
                var lstQuoteScopeAll = context.QuoteScope.Where(x => x.QuoteId == quoteScope.QuoteId).ToList();

                var indexQuoteRemove = 0;
                // List danh sách QuoteScope sau khi sắp xếp
                var lstQuoteScope = new List<QuoteScopeEntityModel>();
                var lstQuoteScopeDefault = new List<QuoteScopeEntityModel>();
                #region B1: Sắp xếp đúng thứ tự TT những quoteScope cùng cha với quotescope sẽ xóa
                var scopeEntityModels = context.QuoteScope.Where(x => x.ParentId == quoteScope.ParentId).Select(p => new QuoteScopeEntityModel()
                {
                    ScopeId = p.ScopeId,
                    Tt = p.Tt,
                    Category = p.Category,
                    Description = p.Description,
                    QuoteId = p.QuoteId,
                    TenantId = p.TenantId,
                    Level = p.Level,
                    ParentId = p.ParentId
                }).OrderBy(o => o.Tt).ToList();

                var listQuoteScopeOrderNumber = scopeEntityModels.Where(x => x.Tt.Trim() != "").Select(y => y.Tt).ToList();
                listQuoteScopeOrderNumber.Sort((x, y) =>
                {
                    int ret = 0;
                    var xsplit = x.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    var ysplit = y.Split(".".ToCharArray()).Select(z => int.Parse(z)).ToList();
                    for (int i = 0; i < Math.Max(xsplit.Count, ysplit.Count); i++)
                    {
                        if (xsplit.Count - 1 < i)
                        {
                            ret = -1;
                            return ret;
                        }
                        else if (ysplit.Count - 1 < i)
                        {
                            ret = 1;
                            return ret;
                        }
                        else
                        {
                            ret = xsplit[i] - ysplit[i];
                            if (ret != 0)
                                return ret;
                        }
                    }
                    return ret;
                });

                var firstItem = scopeEntityModels.FirstOrDefault(x => x.Tt.Trim() == "");
                lstQuoteScope.Add(firstItem);

                listQuoteScopeOrderNumber.ForEach(item =>
                {
                    var _quoteScope = new QuoteScopeEntityModel();

                    _quoteScope = scopeEntityModels.FirstOrDefault(x => x.Tt == item);

                    if (_quoteScope != null)
                    {
                        lstQuoteScope.Add(_quoteScope);
                    }
                });

                lstQuoteScope.RemoveAll(x => x == null);
                lstQuoteScopeDefault = lstQuoteScope.ToList();

                indexQuoteRemove = lstQuoteScope.FindIndex(x => x.Tt.Trim() == quoteScope.Tt.Trim());
                var lenLstQuoteScopeBefore = lstQuoteScope.Count();
                #endregion

                #region B2: Xóa QuoteScope đó
                context.QuoteScope.Remove(quoteScope);
                context.SaveChanges();

                lstQuoteScope.RemoveRange(0, indexQuoteRemove + 1);
                #endregion

                #region B3: Đệ quy các quoteScope phía sau để cập nhập lại tt và tt của các con của nó
                if (lstQuoteScope.Count() > 0)
                {
                    var ttQuoteChange = "";
                    for (int i = indexQuoteRemove; i < lenLstQuoteScopeBefore - 1; i++)
                    {
                        ttQuoteChange = quoteScope.Tt.Substring(0, quoteScope.Tt.LastIndexOf('.'));
                        ttQuoteChange = ttQuoteChange.ToString() + "." + (i + 1).ToString();
                        var quoteUpdate = lstQuoteScopeAll.FirstOrDefault(x => x.ScopeId == lstQuoteScopeDefault[i + 1].ScopeId);
                        quoteUpdate.Tt = ttQuoteChange;
                        context.QuoteScope.Update(quoteUpdate);
                        context.SaveChanges();

                        // danh sách các quoteScope con của quoteScope hiện tại
                        var lstNewQuote = new List<Guid>();
                        var lstQuotes = lstQuoteScopeAll.Where(x => x != null && x.ParentId == quoteUpdate.ScopeId).ToList();
                        while (lstQuotes.Count() > 0)
                        {
                            lstNewQuote.AddRange(lstQuotes.Select(x => x.ScopeId).Distinct().ToList());
                            lstQuotes = lstQuoteScopeAll.Where(x => x.ParentId != null && lstQuotes.Select(b => b.ScopeId).ToList().Contains(x.ParentId.Value)).ToList();
                        }
                        // lstQuotes: danh sách scopeid của các con thuộc quoteScope
                        // Update lại Tt của danh sách này - Cắt chuỗi tt cũ và lắp cái mới vào
                        var lst = lstQuoteScopeAll.Where(x => lstNewQuote.Select(b => b).ToList().Contains(x.ScopeId)).ToList();
                        lst.ForEach(item =>
                        {
                            var ff = item.Tt.Substring(ttQuoteChange.Length, item.Tt.Length - ttQuoteChange.Length);
                            item.Tt = ttQuoteChange.ToString() + item.Tt.Substring(ttQuoteChange.Length, item.Tt.Length - ttQuoteChange.Length);
                        });

                        context.QuoteScope.UpdateRange(lst);
                        context.SaveChanges();
                    }
                }
                #endregion
                var listQuoteScope = context.QuoteScope.Where(x => x.QuoteId == quoteScope.QuoteId).Select(p => new QuoteScopeEntityModel()
                {
                    ScopeId = p.ScopeId,
                    Tt = p.Tt,
                    Category = p.Category,
                    Description = p.Description,
                    QuoteId = p.QuoteId,
                    TenantId = p.TenantId,
                    Level = p.Level,
                    ParentId = p.ParentId
                }).ToList();

                listQuoteScope = SetTTChildren(listQuoteScope);

                return new DeleteQuoteScopeResult
                {
                    MessageCode = CommonMessage.Quote.DELETE_QUOTE_SCOPE_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                    ListQuoteScopes = listQuoteScope,
                };
            }
            catch (Exception ex)
            {
                return new DeleteQuoteScopeResult
                {
                    MessageCode = CommonMessage.Quote.DELETE_QUOTE_SCOPE_FAIL + ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataSearchQuoteResult GetMasterDataSearchQuote(GetMasterDataSearchQuoteParameter parameter)
        {
            try
            {
                var listEmp = context.Employee.Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                }).OrderBy(z => z.EmployeeName).ToList();

                return new GetMasterDataSearchQuoteResult()
                {
                    ListEmp = listEmp,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchQuoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private void UpdateTTOfQuoteScope(List<QuoteScopeEntityModel> lstQuoteScope, string ttQuoteChange)
        {
            // lstQuoteScope: Danh sách từ vị trí remove đến cuối cần update
            lstQuoteScope.ForEach(quote =>
            {
                // Update tt đã xóa/thay thế cho quote phía sau
                var quoteUpdate = context.QuoteScope.FirstOrDefault(x => x.ScopeId == quote.ScopeId);
                quoteUpdate.Tt = ttQuoteChange;
                context.QuoteScope.Update(quoteUpdate);
            });

        }

        public GetVendorByCostIdResult GetVendorByCostId(GetVendorByCostIdParameter parameter)
        {
            try
            {
                decimal priceCost = 0;
                bool isHetHan = false;

                var listCost = context.Cost.Where(x => x.Active &&
                                      x.CostId == parameter.CostId &&
                                      x.SoLuongToiThieu <= parameter.SoLuong)
                        .OrderByDescending(z => z.NgayHieuLuc)
                        .ToList();
                if (listCost.Count > 0)
                {
                    var cost = listCost.FirstOrDefault(x => x.NgayHieuLuc.Date <= DateTime.Now.Date &&
                                      (x.NgayHetHan == null ||
                                      (x.NgayHetHan != null && x.NgayHetHan.Value.Date >= DateTime.Now.Date)));
                    if (cost != null)
                    {
                        priceCost = cost.DonGia;
                    }
                    else
                    {
                        //Kiểm tra có đơn giá hết hạn hay không?
                        var exists = listCost.FirstOrDefault(x => x.NgayHetHan != null &&
                        x.NgayHetHan.Value.Date < DateTime.Now.Date);

                        if (exists != null)
                        {
                            isHetHan = true;
                        }
                    }
                }
                return new GetVendorByCostIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    PriceCost = priceCost,
                    IsHetHan = isHetHan
                };
            }
            catch (Exception e)
            {
                return new GetVendorByCostIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

    }
}
