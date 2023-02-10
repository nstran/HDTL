using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.CashBook;
using TN.TNM.DataAccess.Messages.Results.CashBook;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CashBookDAO : BaseDAO, ICashBookDataAccess
    {
        public CashBookDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetDataSearchCashBookResult GetDataSearchCashBook(GetDataSearchCashBookParameter parameter)
        {
            try
            {
                var employees = context.Employee.OrderBy(c => c.EmployeeName).ToList();
                var organizations = context.Organization.Where(c => c.IsFinancialIndependence == true).ToList();
                var ListEmployeeEntityModel = new List<EmployeeEntityModel>();
                var ListOrganizationEntityModel = new List<OrganizationEntityModel>();
                employees.ForEach(item =>
                {
                    ListEmployeeEntityModel.Add(new EmployeeEntityModel(item));
                });
                organizations.ForEach(item =>
                {
                    ListOrganizationEntityModel.Add(new OrganizationEntityModel(item));
                });
                return new GetDataSearchCashBookResult
                {
                    ListEmployee = ListEmployeeEntityModel,
                    ListOrganization = ListOrganizationEntityModel,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception ex)
            {
                return new GetDataSearchCashBookResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetSurplusCashBookPerMonthResult GetSurplusCashBookPerMonth(GetSurplusCashBookPerMonthParameter parameter)
        {
            try
            {
                if (parameter.OrganizationList.Count > 0)
                {
                    decimal SumOpeningBalance = 0;
                    decimal SumClosingBalance = 0;
                    parameter.OrganizationList.ForEach(item =>
                    {
                        decimal itemOpeningBalance = 0;
                        decimal itemClosingBalance = 0;
                        var AmountOpeningCashBookToGet = (from cb in context.CashBook
                                                          where cb.PaidDate == (from mcb in context.CashBook
                                                                                where mcb.PaidDate < parameter.FromDate.Value
                                                                                    && mcb.OrganizationId == item
                                                                                select mcb.PaidDate).Max().GetValueOrDefault()
                                                                 && cb.OrganizationId == item
                                                          select cb.Amount).FirstOrDefault();
                        // Lay ra so du hien tai (tinh den ngay search)
                        var AmountClosingCashBookToGet = (from cb in context.CashBook
                                                          where cb.PaidDate == (from mcb in context.CashBook
                                                                                where mcb.PaidDate <= parameter.ToDate.Value
                                                                                && mcb.PaidDate >= parameter.FromDate.Value
                                                                                && mcb.OrganizationId == item
                                                                                select mcb.PaidDate).Max().GetValueOrDefault()
                                                                 && cb.OrganizationId == item
                                                          select cb.Amount).FirstOrDefault();
                        if (AmountOpeningCashBookToGet.HasValue)
                        {
                            itemOpeningBalance = AmountOpeningCashBookToGet.Value;
                        }
                        if (AmountClosingCashBookToGet.HasValue)
                        {
                            itemClosingBalance = AmountClosingCashBookToGet.Value;
                        }
                        else
                        {
                            itemClosingBalance = itemOpeningBalance;
                        }
                        SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                        SumClosingBalance = SumClosingBalance + itemClosingBalance;
                    });
                    return new GetSurplusCashBookPerMonthResult
                    {
                        OpeningSurplus = SumOpeningBalance,
                        ClosingSurplus = SumClosingBalance,
                        MessageCode = "Success",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    var lst = (from o in context.Organization
                               where o.IsFinancialIndependence.Value == true
                               select o.OrganizationId).ToList();

                    decimal SumOpeningBalance = 0;
                    decimal SumClosingBalance = 0;
                    lst.ForEach(item =>
                    {
                        decimal itemOpeningBalance = 0;
                        decimal itemClosingBalance = 0;
                        var AmountOpeningCashBookToGet = (from cb in context.CashBook
                                                          where cb.PaidDate == (from mcb in context.CashBook
                                                                                where mcb.PaidDate < parameter.FromDate.Value
                                                                                    && mcb.OrganizationId == item
                                                                                select mcb.PaidDate).Max().GetValueOrDefault()
                                                                 && cb.OrganizationId == item
                                                          select cb.Amount).FirstOrDefault();
                        // Lay ra so du hien tai (tinh den ngay search)
                        var AmountClosingCashBookToGet = (from cb in context.CashBook
                                                          where cb.PaidDate == (from mcb in context.CashBook
                                                                                where mcb.PaidDate <= parameter.ToDate.Value
                                                                                && mcb.PaidDate >= parameter.FromDate.Value
                                                                                    && mcb.OrganizationId == item
                                                                                select mcb.PaidDate).Max().GetValueOrDefault()
                                                                 && cb.OrganizationId == item
                                                          select cb.Amount).FirstOrDefault();
                        if (AmountOpeningCashBookToGet.HasValue)
                        {
                            itemOpeningBalance = AmountOpeningCashBookToGet.Value;
                        }
                        if (AmountClosingCashBookToGet.HasValue)
                        {
                            itemClosingBalance = AmountClosingCashBookToGet.Value;
                        }
                        else
                        {
                            itemClosingBalance = itemOpeningBalance;
                        }
                        SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                        SumClosingBalance = SumClosingBalance + itemClosingBalance;
                    });
                    return new GetSurplusCashBookPerMonthResult
                    {
                        OpeningSurplus = SumOpeningBalance,
                        ClosingSurplus = SumClosingBalance,
                        Message = "Success",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
            }
            catch(Exception e)
            {
                return new GetSurplusCashBookPerMonthResult
                {                  
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }        
        }
    }
}
