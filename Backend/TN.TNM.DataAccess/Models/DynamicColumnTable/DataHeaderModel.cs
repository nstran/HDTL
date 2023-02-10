using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.DynamicColumnTable
{
    public class DataHeaderModel
    {
        public string ColumnKey { get; set; }
        public string ColumnValue { get; set; }

        /* Các trường tùy chọn */
        public string TextAlign { get; set; }
        public string Width { get; set; }
        public int? Rowspan { get; set; }
        public int? Colspan { get; set; }
        public ValueTypeEnum ValueType { get; set; }

        public DataHeaderModel()
        {
            ValueType = ValueTypeEnum.STRING;
        }
    }
}
