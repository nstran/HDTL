using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Results.Options
{
    public class SearchOptionsResult : BaseResult
    {
        public SearchOptions Options { get; set; }
        public List<SearchOptions> ListOptions { get; set; }
        public OptionsEntityModel OptionsEntityModel { get; set; }
        public List<SearchOptionTree> ListOptionTrees { get; set; }
    }
    public class SearchOptions
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? Price { get; set; }
        public Guid OptionId { get; set; }
        public Guid? ParentId { get; set; }
        public string Description { get; set; }
        public bool HasChild { get; set; }
        public string OptionName { get; set; }
        public decimal? fromPrice { get; set; }
        public decimal? toPrice { get; set; }
        public decimal? VAT { get; set; }
    }
    public class SearchOptionTree
    {
        public string CategoryName { get; set; }
        public string OptionName { get; set; }
        public decimal? Price { get; set; }
        public Guid OptionId { get; set; }
        public Guid? ParentId { get; set; }
        public bool HasChild { get; set; }
        public List<SearchOptionTree> ListOptionTrees { get; set; } = new List<SearchOptionTree>(); 
    }
}
