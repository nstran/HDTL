using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Options
{
    public class GetListCategoryAttributesResult : BaseResult
    {
        public List<ListCategoryAttributes> ListCategoryAttributes { get; set; }
        public List<CategoryEntityModel> ListAttr { get; set; }
        public List<TrangThaiGeneral> ListDataType { get; set; }
    }


    public class ListCategoryAttributes
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string DataType { get; set; }
        public int DataTypeValue { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
