using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetDataMCCResult
    {
        public int Stamp { get; set; }

        public int OpStamp { get; set; }

        public int ErrorDelay { get; set; }

        public int Delay { get; set; }

        public string TransTimes { get; set; }

        public int TransInterval { get; set; }

        public long TransFlag { get; set; }

        public int Realtime { get; set; }

        public int TimeZone { get; set; }

        public int ADMSSyncTime { get; set; }
    }
}
