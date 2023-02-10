using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetThongTinGhiChuResponse : BaseResponse
    {
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
