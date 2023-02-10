using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TaskEntityModel
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal? ActualHour { get; set; }
        public string Description { get; set; }
        public Guid? Status { get; set; }
        public int? Priority { get; set; }
        public Guid? MilestonesId { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public decimal? EstimateHour { get; set; }
        public decimal? EstimateCost { get; set; }
        public bool? IsSendMail { get; set; }
        public string ProjectScopeName { get; set; }
        public decimal? TaskComplate { get; set; }
        public Guid? ProjectScopeId { get; set; }
        public Guid? TaskTypeId { get; set; }
        public string TimeType { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        // sai chính tả =_=. Bên dưới e có viết lại 1 property mới
        public string TaskTyeName { get; set; }
        public string ScopeName { get; set; }
        public string CreateByName { get; set; }
        public string Employee { get; set; }
        public bool IsDelete { get; set; }
        public string TaskTypeCode { get; set; }
        public string TaskTypeName { get; set; }
        public string PriorityName { get; set; }
        public string PlanEndTimeStr { get; set; }
        public string ActualEndTimeStr { get; set; }
        public string ColorActualEndTimeStr { get; set; }
        public string CreateDateStr { get; set; }
        public string ColorPlanEndTimeStr { get; set; }
        public string PersionInChargedName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public List<TaskResourceMappingEntityModel> ListTaskResource { get; set; }
        public bool CanEdit { get; set; }
        public bool IsHavePic { get; set; }
        public Guid? ParentId { get; set; }
        public int NoteNumber { get; set; }
        public int Overdue { get; set; } // 0: Quá hạn; 1: Đúng hạn; 2: Trước hạn
        public bool IsCreate { get; set; }
        public double PVOfTask { get; set; }
        public double EVOfTask { get; set; }
        public  int Order { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string UpdateDateStr { get; set; }
        public int? SoLanMoLai { get; set; }

    }
}



//public class TaskEntityModel
//{
//    public Guid TaskId { get; set; }
//    public Guid? ProjectId { get; set; }
//    public Guid? SourceId { get; set; }
//    public int SourceType { get; set; }
//    public string TaskCode { get; set; }
//    public string TaskName { get; set; }
//    public Guid? Resource { get; set; }
//    public DateTime? PlanStartTime { get; set; }
//    public DateTime? PlanEndTime { get; set; }
//    public decimal EstimateHour { get; set; }
//    public decimal EstimateCost { get; set; }
//    public DateTime? ActualStartTime { get; set; }
//    public DateTime? ActualEndTime { get; set; }
//    public decimal ActualHour { get; set; }
//    public string Description { get; set; }
//    public Guid? TaskType { get; set; }
//    public Guid? Status { get; set; }
//    public int Priority { get; set; }
//    public Guid? MilestonesId { get; set; }
//    public bool? IncludeWeekend { get; set; }
//    public int TaskComplate { get; set; }
//    public Guid? uniqueidentifier { get; set; }
//    public Guid CreateBy { get; set; }
//    public DateTime CreateDate { get; set; }
//    public Guid? UpdateBy { get; set; }
//    public DateTime? UpdateDate { get; set; }
//    public Guid? TenantId { get; set; }
//    public Guid? ProjectScopeId { get; set; }
//    public int? CountNote { get; set; }
//}