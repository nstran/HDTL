using System;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderDetailEntityModel
    {
        public Guid OrderDetailId { get; set; }
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Chủng loại
        /// </summary>
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public string ProductColorCode { get; set; }

        /// <summary>
        /// Độ dày
        /// </summary>
        public double? ProductThickness { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }

        /// <summary>
        /// Ghi chú kĩ thuật
        /// </summary>
        public string TechniqueDescription { get; set; }
        public string ProductCode { get; set; }
        public string UnitName { get; set; }
        public double? TotalArea { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductGroupCode { get; set; }

        public double? Grind { get; set; }
        public double? Stt { get; set; }
    }
}
