using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class GetListNoteParameter : BaseParameter
    {
        public string ObjectType { get; set; }
        public Guid? ObjectId { get; set; }
        public int? ObjectNumber { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
