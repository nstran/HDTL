using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;


namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetListDeXuatCongTacResult : BaseResult
    {
        public List<DeXuatCongTac> ListDeXuatCongTac { get; set; }
 
    }
  
}
