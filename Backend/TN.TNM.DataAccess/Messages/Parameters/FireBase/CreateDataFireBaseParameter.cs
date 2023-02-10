using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.FireBase
{
    public class CreateDataFireBaseParameter : BaseParameter
    {
        public string RoomName { get; set; }

        public Guid? OtherId { get; set; }
    }
}
