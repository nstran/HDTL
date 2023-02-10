using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models
{
    public class PositionModel
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; }
    }
}
