using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductionOrderHistory
    {
        public Guid ProductionOrderHistoryId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public Guid ProductionOrderMappingId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public bool? CalculatorType { get; set; }
        public bool? IsError { get; set; }
        public string Description { get; set; }
        public double? QuantityUnitErr { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public bool? ParentType { get; set; }
        public Guid? NoteQc { get; set; }
        public Guid? ErrorType { get; set; }
        public bool? Thin { get; set; }
        public bool? Thick { get; set; }
        public bool? EspeciallyThin { get; set; }
        public bool? EspeciallyThick { get; set; }
        public bool? BoreholeThin { get; set; }
        public bool? BoreholeThick { get; set; }
        public bool? OriginalThin { get; set; }
        public bool? OriginalThick { get; set; }
        public bool? IsAdd { get; set; }
        public bool? IsErrorPre { get; set; }
        public Guid? OriginalId { get; set; }
        public bool? IsParent { get; set; }
        public bool? IsSubParent { get; set; }
        public Guid? IdChildrent { get; set; }
        public Guid? ParentPartId { get; set; }
        public Guid? ParentExtendId { get; set; }
        public bool? IsChildren { get; set; }
        public bool? IsAddPart { get; set; }
        public bool? IsPresent { get; set; }
        public bool? Present { get; set; }
    }
}
