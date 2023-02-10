using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterDataVacanciesDetailParameter : BaseParameter
    {
        public Guid VacanciesId { get; set; }
    }
}
