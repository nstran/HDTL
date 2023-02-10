using System;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderGetByIdEntityModel
    {
        public Guid OrderId { get; set; }

        // Tên khách hàng
        public string CustomerName { get; set; }

        // Mã khách hàng
        public string CustomerCode { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Ngày sản xuất
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// Ngày duyệt
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Ngày nhận
        /// </summary>
        public DateTime? ReceivedDate { get; set; }

        /// <summary>
        /// Nơi nhận
        /// </summary>
        public string PlaceOfDelivery { get; set; }

        /// <summary>
        /// Ghi chú kĩ thuật
        /// </summary>
        public string NoteTechnique { get; set; }

        /// <summary>
        /// Số đơn hàng của khách hàng
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        public string OrderCode { get; set; }
        public string StatusName { get; set; }
    }
}
