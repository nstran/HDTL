using System;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class SearchNoteRequest : BaseRequest<SearchNoteParameter>
    {
        public string Keyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid LeadId { get; set; }
        public Guid? ProjectId { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string ScreenName { get; set; }

        public override SearchNoteParameter ToParameter()
        {
            return new SearchNoteParameter()
            {
                Keyword = Keyword,
                FromDate = FromDate,
                ToDate = ToDate,
                LeadId = LeadId,
                UserId = UserId,
                ProjectId = ProjectId,
                PageIndex = PageIndex,
                PageSize = PageSize,
                ScreenName = ScreenName
            };
        }
    }
}
