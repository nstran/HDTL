using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TN.TNM.DataAccess.Models
{

    public class Noti
    {
        public string Key { get; }

        public T Object { get; }
    }

    public class Notification
    {
        public string Content { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string Date { get; set; }
        public List<Guid> ListEmpId { get; set; }
        public string ReportId { get; set; }
        public string ObjectType { get; set; }
    }
}
