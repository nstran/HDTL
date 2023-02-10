using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteTaskParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
    }
}
