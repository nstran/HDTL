using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;
using TN.TNM.DataAccess.Messages.Results.BankAccount;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BankAccountDAO : BaseDAO, IBankAccountDataAccess
    {
        public BankAccountDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public CreateBankAccountResult CreateBankAccount(CreateBankAccountParameter parameter)
        {
            try
            {
                if (parameter.BankAccount.BankAccountId != null)
                {
                    var bankAccount =
                        context.BankAccount.FirstOrDefault(x => x.BankAccountId == parameter.BankAccount.BankAccountId);

                    bankAccount.ObjectId = parameter.BankAccount.ObjectId;
                    bankAccount.ObjectType = parameter.BankAccount.ObjectType;
                    bankAccount.AccountNumber = parameter.BankAccount.AccountNumber;
                    bankAccount.BankName = parameter.BankAccount.BankName;
                    bankAccount.BankDetail = parameter.BankAccount.BankDetail;
                    bankAccount.BranchName = parameter.BankAccount.BranchName;
                    bankAccount.AccountName = parameter.BankAccount.AccountName;
                    bankAccount.UpdatedDate = DateTime.Now;
                    bankAccount.UpdatedById = parameter.UserId;

                    context.BankAccount.Update(bankAccount);
                    context.SaveChanges();
                }
                else
                {
                    var newBankAccount = new BankAccount();
                    newBankAccount.BankAccountId = Guid.NewGuid();
                    newBankAccount.ObjectId = parameter.BankAccount.ObjectId;
                    newBankAccount.ObjectType = parameter.BankAccount.ObjectType;
                    newBankAccount.AccountNumber = parameter.BankAccount.AccountNumber;
                    newBankAccount.BankName = parameter.BankAccount.BankName;
                    newBankAccount.BankDetail = parameter.BankAccount.BankDetail;
                    newBankAccount.BranchName = parameter.BankAccount.BranchName;
                    newBankAccount.AccountName = parameter.BankAccount.AccountName;
                    newBankAccount.Active = true;
                    newBankAccount.CreatedById = parameter.UserId;
                    newBankAccount.CreatedDate = DateTime.Now;
                    newBankAccount.UpdatedById = null;
                    newBankAccount.UpdatedDate = null;

                    context.BankAccount.Add(newBankAccount);
                    context.SaveChanges();
                }

                #region Lấy thông tin thanh toán theo đối tượng

                var listBankAccount = new List<BankAccount>();
                listBankAccount = context.BankAccount
                    .Where(b => b.ObjectId == parameter.BankAccount.ObjectId &&
                                b.ObjectType == parameter.BankAccount.ObjectType).OrderByDescending(z => z.CreatedDate)
                    .ToList();
                var listBankAccountResult = new List<BankAccountEntityModel>();

                listBankAccount.ForEach(item => {
                        listBankAccountResult.Add(new BankAccountEntityModel(item));
                });

                #endregion

                return new CreateBankAccountResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListBankAccount = listBankAccountResult
                };
            }
            catch (Exception e)
            {
                return new CreateBankAccountResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetBankAccountByIdResult GetBankAccountById(GetBankAccountByIdParameter parameter)
        {
            try
            {
                var bankAccount = new BankAccountEntityModel(context.BankAccount.FirstOrDefault(b => b.BankAccountId == parameter.BankAccountId));
                return new GetBankAccountByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    BankAccount = bankAccount
                };
            }
            catch (Exception e)
            {
                return new GetBankAccountByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public EditBankAccountResult EditBankAccount(EditBankAccountParameter parameter)
        {
            try
            {
                var bankAcount = context.BankAccount.FirstOrDefault(c => c.BankAccountId == parameter.BankAccount.BankAccountId);
                if(bankAcount == null)
                {
                    return new EditBankAccountResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bank Account không tồn tại trong hệ thống"
                    };
                }
                bankAcount.ObjectId = parameter.BankAccount.ObjectId;
                bankAcount.ObjectType = parameter.BankAccount.ObjectType;
                bankAcount.AccountNumber = parameter.BankAccount.AccountName;
                bankAcount.BankName = parameter.BankAccount.BankName;
                bankAcount.BankDetail = parameter.BankAccount.BankDetail;
                bankAcount.BranchName = parameter.BankAccount.BranchName;
                bankAcount.AccountName = parameter.BankAccount.AccountName;
                bankAcount.Active = parameter.BankAccount.Active;
                bankAcount.UpdatedById = parameter.UserId;
                bankAcount.UpdatedDate = DateTime.Now;

                context.BankAccount.Update(bankAcount);
                context.SaveChanges();
                return new EditBankAccountResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.BankAccount.EDIT_BANK_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new EditBankAccountResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteBankAccountByIdResult DeleteBankAccountById(DeleteBankAccountByIdParameter parameter)
        {
            try
            {
                var bankAccount = context.BankAccount.FirstOrDefault(b => b.BankAccountId == parameter.BankAccountId);
                var bankPayableInvoice = context.BankPayableInvoice.FirstOrDefault(b => b.BankPayableInvoiceBankAccountId == bankAccount.BankAccountId);
                var bankReceiptInvoice = context.BankReceiptInvoice.FirstOrDefault(b => b.BankReceiptInvoiceBankAccountId == bankAccount.BankAccountId);
                if (bankPayableInvoice != null)
                {
                    return new DeleteBankAccountByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tài khoản này đang có phiếu Ủy nhiệm chi gắn cùng"
                    };
                }
                else if (bankReceiptInvoice != null)
                {
                    return new DeleteBankAccountByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tài khoản này đang có Báo có gắn cùng"
                    };
                }

                context.BankAccount.Remove(bankAccount);
                context.SaveChanges();

                #region Lấy thông tin thanh toán theo đối tượng

                var listBankAccount = new List<BankAccount>();
                listBankAccount = context.BankAccount
                    .Where(b => b.ObjectId == parameter.ObjectId &&
                                b.ObjectType == parameter.ObjectType).OrderByDescending(z => z.CreatedDate).ToList();

                var listBankAccountResult = new List<BankAccountEntityModel>();
                listBankAccount.ForEach(item =>
                {
                    listBankAccountResult.Add(new BankAccountEntityModel(item));
                });
                #endregion

                return new DeleteBankAccountByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.BankAccount.DELETE_BANK_SUCCESS,
                    ListBankAccount = listBankAccountResult
                };
            }
            catch (Exception e)
            {
                return new DeleteBankAccountByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllBankAccountByObjectResult GetAllBankAccountByObject(GetAllBankAccountByObjectParameter parameter)
        {
            try
            {
                var bankList = context.BankAccount
                .Where(b => b.ObjectId == parameter.ObjectId && b.ObjectType == parameter.ObjectType).Select(y =>
                    new BankAccountEntityModel
                    {
                        BankAccountId = y.BankAccountId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        AccountNumber = y.AccountNumber,
                        BankName = y.BankName,
                        BankDetail = y.BankDetail,
                        BranchName = y.BranchName,
                        AccountName = y.AccountName,
                        LabelShow = y.AccountNumber + " - " + y.AccountName + " - " + y.BankName,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        Active = y.Active
                    }).ToList();
                return new GetAllBankAccountByObjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    BankAccountList = bankList
                };
            }
            catch (Exception e)
            {
                return new GetAllBankAccountByObjectResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
            
        }

        public GetCompanyBankAccountResult GetCompanyBankAccount(GetCompanyBankAccountParameter parameter)
        {
            try
            {
                var bankList = context.BankAccount.Where(b => b.ObjectType == ObjectType.COMPANY).ToList();
                var bankListResult = new List<BankAccountEntityModel>();
                bankList.ForEach(item =>
                {
                    bankListResult.Add(new BankAccountEntityModel(item));
                });
                return new GetCompanyBankAccountResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    BankList = bankListResult
                };
            }
            catch (Exception e)
            {
                return new GetCompanyBankAccountResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataBankPopupResult GetMasterDataBankPopup(GetMasterDataBankPopupParameter parameter)
        {
            try
            {
                var typeBankId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NH")?.CategoryTypeId;
                var listBankAccount = context.Category.Where(c => c.CategoryTypeId == typeBankId && c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();

                return new GetMasterDataBankPopupResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListBank = listBankAccount
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataBankPopupResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }
    }
}
