using System;
using System.Linq;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeAllowanceDAO : BaseDAO, IEmployeeAllowanceDataAccess
    {
        public EmployeeAllowanceDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public GetEmployeeAllowanceByEmpIdResult GetEmployeeAllowanceByEmpId(GetEmployeeAllowanceByEmpIdParameter parameter)
        {
            try
            {
                var empAllowance = context.EmployeeAllowance.Where(empalw => empalw.EmployeeId == parameter.EmployeeId).OrderByDescending(t => t.EffectiveDate.Value.Date).FirstOrDefault();
                if (empAllowance != null)
                {
                    return new GetEmployeeAllowanceByEmpIdResult()
                    {
                        EmployeeAllowance = new EmployeeAllowanceEntityModel(empAllowance),
                        MessageCode = "Success",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new GetEmployeeAllowanceByEmpIdResult()
                    {
                        EmployeeAllowance = new EmployeeAllowanceEntityModel(),
                        MessageCode = "Chua co tro cap",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
            }
            catch(Exception ex)
            {
                return new GetEmployeeAllowanceByEmpIdResult()
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
        public EditEmployeeAllowanceResult EditEmployeeAllowance(EditEmployeeAllowanceParameter parameter)
        {
            try
            {
                if (parameter.EmployeeAllowance?.LunchAllowance < 0 || parameter.EmployeeAllowance?.MaternityAllowance < 0 || parameter.EmployeeAllowance?.FuelAllowance < 0 || parameter.EmployeeAllowance?.PhoneAllowance < 0 || parameter.EmployeeAllowance?.OtherAllownce < 0)
                {
                    return new EditEmployeeAllowanceResult()
                    {
                        MessageCode = "Failed",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                    };
                }

                parameter.EmployeeAllowance.EffectiveDate = DateTime.Now;
                // Tim tro cap cua nhan vien
                var empAllowance = context.EmployeeAllowance.FirstOrDefault(_empAllowance => _empAllowance.EmployeeId == parameter.EmployeeAllowance.EmployeeId
                                                                                            && _empAllowance.EffectiveDate.Value.Date == parameter.EmployeeAllowance.EffectiveDate.Value.Date);
                if (empAllowance != null)
                {
                    empAllowance.EffectiveDate = parameter.EmployeeAllowance.EffectiveDate;
                    empAllowance.FuelAllowance = parameter.EmployeeAllowance.FuelAllowance;
                    empAllowance.LunchAllowance = parameter.EmployeeAllowance.LunchAllowance;
                    empAllowance.MaternityAllowance = parameter.EmployeeAllowance.MaternityAllowance;
                    empAllowance.PhoneAllowance = parameter.EmployeeAllowance.PhoneAllowance;
                    empAllowance.OtherAllownce = parameter.EmployeeAllowance.OtherAllownce;
                    empAllowance.FreeTimeUnlimited = parameter.EmployeeAllowance.FreeTimeUnlimited;
                    empAllowance.UpdateById = parameter.UserId;
                    empAllowance.UpdateDate = DateTime.Now;
                    context.EmployeeAllowance.Update(empAllowance);
                }
                else
                {
                    parameter.EmployeeAllowance.CreateById = parameter.UserId;
                    parameter.EmployeeAllowance.CreateDate = DateTime.Now;
                    context.EmployeeAllowance.Add(parameter.EmployeeAllowance.ToEntity());
                }
                context.SaveChanges();
                return new EditEmployeeAllowanceResult()
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new EditEmployeeAllowanceResult()
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
    }

}
