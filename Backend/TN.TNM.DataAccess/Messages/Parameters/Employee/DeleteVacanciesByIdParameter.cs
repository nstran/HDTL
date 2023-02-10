using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteVacanciesByIdParameter : BaseParameter
    {
        public Guid VacanciesId { get; set; }
    }
}
