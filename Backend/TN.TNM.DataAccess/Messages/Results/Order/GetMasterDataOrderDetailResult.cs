using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.DAO;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.WareHouse;
using CustomerOrderEntityModel = TN.TNM.DataAccess.Models.Order.CustomerOrderEntityModel;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderDetailResult : BaseResult
    {
        public string EmpNameCreator { get; set; }
        public CustomerOrderEntityModel CustomerOrder { get; set; }
        public List<CustomerOrderDetailEntityModel> ListDetail { get; set; }
        public List<CustomerOrderExtensionEntityModel> ListAtrrPacket { get; set; }
        public List<CustomerOrderExtensionEntityModel> ListAtrrOption { get; set; }
        public List<ServicePacketEntityModel> ListServicePacket { get; set; }
        public List<CustomerOrderDetailExtenEntityModel> ListOptionExten { get; set; }
        public List<CustomerOrderEntityModel> ListAllOrderExten { get; set; }
        public List<CustomerOrderEntityModel> ListCustomerOrderAction { get; set; }
        public Guid? OrderActionId { get; set; }
        public Boolean? IsCurrentStep { get; set; }
        public Boolean? IsSendApproval { get; set; }
        public Boolean? IsSendApprovalExten { get; set; }
        public Boolean? IsShowXacNhan { get; set; }
        public Boolean? IsShowTuChoi { get; set; }
        public Guid? QuanLyGoi_UserId { get; set; }
        public Guid? QuanLyGoi_EmpId { get; set; }
        public string QuanLyGoi_Name { get; set; }
        public bool IsShowButtonDelete { get; set; }
    }
}
