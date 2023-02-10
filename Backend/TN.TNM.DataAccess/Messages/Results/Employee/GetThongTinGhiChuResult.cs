using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinGhiChuResult : BaseResult
    {
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
