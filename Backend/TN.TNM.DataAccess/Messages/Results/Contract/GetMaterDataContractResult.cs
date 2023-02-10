using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class GetMaterDataContractResult : BaseResult
    {
        // Master data create
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListTypeContract { get; set; }
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<EmployeeEntityModel> ListEmployeeSeller { get; set; }
        public List<CategoryEntityModel> ListContractStatus { get; set; }
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
        public List<QuoteDetailEntityModel> ListQuoteDetail { get; set; }
        public List<QuoteCostDetailEntityModel> ListQuoteCostDetail { get; set; }
        
        // Get Data view
        public ContractEntityModel Contract { get; set; }
        public List<ContractDetailEntityModel> ListContractDetail { get; set; }
        public List<ContractDetailProductAttributeEntityModel> ListContractDetailProductAttribute { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCost { get; set; }

        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderEntityModel> ListFile { get; set; }
        public List<ContractEntityModel> ListContract { get; set; }
        public Boolean IsOutOfDate { get; set; }

        public List<CustomerOrderEntityModel> ListCustomerOrder { get; set; }
        public decimal? ChiPhiHopDongCon { get; set; }
        public List<ContractDetailEntityModel> ListContractDetailHDCon { get; set; }
        public List<ContractCostDetailEntityModel> ListContractCostDetailHDCon { get; set; }
        public List<CategoryEntityModel> ListLyDoHoanThanh { get; set; }
        public List<PositionModel> ListPosition { get; set; }
        public List<TheoDoiThanhToanEntityModel> ListTheoDoiThanhToan { get; set; }
        public decimal SoTienDaThu { get; set; }

    }
}
