using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ModelChart
    {
    }

    public class ChartTaskFollowStatus
    {
        public Guid CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int CountTask { get; set; }
        public string Color { get; set; }

        public string PercentValue { get; set; }
    }

    public class ChartTaskFollowTime
    {
        public int TimeCode { get; set; }
        public string TimeName { get; set; }
        public int CountTask { get; set; }
        public string Color { get; set; }

        public string PercentValue { get; set; }
    }

    public class CharFollowTaskType
    {
        public Guid? CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int CountTask { get; set; }
    }

    public class ChartTaskFollowResource
    {
        public Guid ResouceId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeCodeName { get; set; }
        public int CountTaskComplete { get; set; }
        public int CountTaskNotComplete { get; set; }
        public int Total { get; set; }
    }

    public class ChartTimeFollowResource
    {
        public Guid? EmployeeId { get; set; }
        public string EmployeeCodeName { get; set; }
        public decimal TotalHour { get; set;}
        public decimal HourUsed { get; set; }
        public decimal HourNotUsed { get; set; }
    }

    public class ChartEvn
    {
        public string DateStr { get; set; }
        public decimal? PV { get; set; }
        public decimal? AC { get; set; }
        public decimal? EV { get; set; }
        public DateTime DateOrder { get; set; }
        public int SortOrder { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
    }

    public class PerformanceCost
    {
        public string DateStr { get; set; }
        public decimal? CPI { get; set; }
        public decimal? SPI { get; set; }
        public decimal? MaxYAxis { get; set; }
    }

    public class ChartBudget
    {
        public string BudgetName { get; set; }
        public decimal BudgetValue { get; set; }
    }

    public class ChartProjectFollowResource
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Color { get; set; }
        public int? Allowcation { get; set; }
        public int? Total { get; set; }
    }
}
