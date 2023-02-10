using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateOrUpdateVacanciesResult : BaseResult
    {
        public Guid VacanciesId { get; set; }
        public List<VacancyEntityModel> ListVacancies { get; set; }
    }
}

