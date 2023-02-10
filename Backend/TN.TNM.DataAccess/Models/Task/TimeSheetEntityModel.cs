using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TimeSheetEntityModel
    {
        public Guid TimeSheetId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? SpentHour { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? Status { get; set; }
        public Guid? TimeType { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }

        public string PersonInChargeName { get; set; }
        public string TaskCodeName { get; set; }
        public string WeekName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string TimeTypeName { get; set; }
        public string BackgroundColorForStatus { get; set; }

        public string CreateByName { get; set; }
        public string UpdateByName { get; set; }

        public decimal? Monday { get; set; }
        public decimal? Tuesday { get; set; }
        public decimal? Wednesday { get; set; }
        public decimal? Thursday { get; set; }
        public decimal? Friday { get; set; }
        public decimal? Saturday { get; set; }
        public decimal? Sunday { get; set; }

        public decimal? MondayCheck { get; set; }
        public decimal? TuesdayCheck { get; set; }
        public decimal? WednesdayCheck { get; set; }
        public decimal? ThursdayCheck { get; set; }
        public decimal? FridayCheck { get; set; }
        public decimal? SaturdayCheck { get; set; }
        public decimal? SundayCheck { get; set; }

        public Guid? MondayId { get; set; }
        public Guid? TuesdayId { get; set; }
        public Guid? WednesdayId { get; set; }
        public Guid? ThursdayId { get; set; }
        public Guid? FridayId { get; set; }
        public Guid? SaturdayId { get; set; }
        public Guid? SundayId { get; set; }


        public int SortOrderStatus { get; set; }
        public bool IsAdminApproval { get; set; }

        public List<TimeSheetDetailEntityModel> ListTimeSheetDetail { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
