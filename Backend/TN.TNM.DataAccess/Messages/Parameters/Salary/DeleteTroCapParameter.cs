using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class DeleteTroCapParameter : BaseParameter
    {
        public int TypeId { get; set; }
        public int ObjectId { get; set; }
    }
}
