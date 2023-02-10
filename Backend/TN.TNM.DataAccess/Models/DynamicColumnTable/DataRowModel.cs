using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.DynamicColumnTable
{
    public class DataRowModel
    {
        public string ColumnKey { get; set; }
        public string ColumnValue { get; set; }

        /* Các trường tùy chọn */
        public ValueTypeEnum ValueType { get; set; }
        public bool IsShow { get; set; }
        public string Width { get; set; }
        public string TextAlign { get; set; }

        public DataRowModel()
        {
            ValueType = ValueTypeEnum.STRING;
            IsShow = true;
        }
    }
}
