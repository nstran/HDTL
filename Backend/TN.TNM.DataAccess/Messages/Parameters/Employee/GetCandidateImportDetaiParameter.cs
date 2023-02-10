using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetCandidateImportDetaiParameter : BaseParameter
    {
        public Guid VacanciesId { get; set; }
    }
}
