using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Parameters.Options;
using TN.TNM.DataAccess.Messages.Results.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Results.DataType;
using TN.TNM.DataAccess.Messages.Results.Options;

namespace TN.TNM.Api.Controllers
{
    public class OptionController : Controller
    {
        private readonly IOptionDataAccess _optionDataAccess;
        public OptionController(IOptionDataAccess optionDataAccess)
        {
            _optionDataAccess = optionDataAccess;
        }
        [HttpPost]
        [Route("api/options/searchOptions")]
        [Authorize(Policy = "Member")]
        public Task<SearchOptionsResult> SearchCustomer([FromBody] SearchOptionParameter parameter)
        {
            return this._optionDataAccess.SearchOption(parameter);
        }
        [HttpPost]
        [Route("api/options/deleteOption")]
        [Authorize(Policy = "Member")]
        public Task<DeleteOptionResult> DeleteOption([FromBody] DeleteOptionParameter request)
        {
            return this._optionDataAccess.DeleteOption(request);
        }
        [HttpGet]
        [Route("api/options/getCategoryAttributes")]
        [Authorize(Policy = "Member")]
        public Task<GetListCategoryAttributesResult> GetListCategoryAttributesResult()
        {
            return this._optionDataAccess.GetListCategoryAttributesResult();
        }
        [HttpGet]
        [Route("api/options/getAllAttributes")]
        [Authorize(Policy = "Member")]
        public Task<GetListCategoryAttributesResult> ListAttributeResult()
        {
            return this._optionDataAccess.ListAttributeResult();
        }
        [HttpPost]
        [Route("api/options/getOptionById")]
        [Authorize(Policy = "Member")]
        public Task<SearchOptionsResult> GetOptionById([FromBody] GetOptionByIdParameter parameter)
        {
            return this._optionDataAccess.GetOptionById(parameter);
        }
        [HttpGet]
        [Route("api/options/getDataType")]
        [Authorize(Policy = "Member")]
        public Task<DataTypeResult> GetGiaTriThuocTinh()
        {
            return this._optionDataAccess.GetListDataType();
        }
        [HttpPost]
        [Route("api/options/createAttrConfig")]
        [Authorize(Policy = "Member")]
        public async Task<CreateOrUpdateAttributeConfigureResult> CreateAttributeConfigure([FromBody] CreateOrUpdateAttributeConfigureParamenter parameter)
        {
            return await this._optionDataAccess.CreateAttributeConfigure(parameter);
        }
        [HttpGet]
        [Route("api/options/getOptionCategory")]
        [Authorize(Policy = "Member")]
        public Task<GetOptionCategoryResult> GetOptionCategory()
        {
            return this._optionDataAccess.GetOptionCategory();
        }
        [HttpPost]
        [Route("api/options/createOrUpdateOptions")]
        [Authorize(Policy = "Member")]
        public async Task<CreateOrUpdateOptionResult> CreateOrUpdateOptions([FromBody] CreateOrUpdateOptionParameter parameter)
        {
            return await this._optionDataAccess.CreateOrUpdateOptions(parameter);
        }
        [HttpPost]
        [Route("api/options/getCategoryAttributeById")]
        [Authorize(Policy = "Member")]
        public Task<GetListCategoryAttributesResult> GetListCategoryAttributesById([FromBody] GetListCategoryAttributesByIdParameter parameter)
        {
            return this._optionDataAccess.GetListCategoryAttributesById(parameter);
        }
        [HttpPost]
        [Route("api/options/deleteAttributeConfigure")]
        [Authorize(Policy = "Member")]
        public Task<DeleteAttributeConfigureResult> DeleteAttributeConfigure([FromBody] DeleteAttributeConfigureParameter parameter)
        {
            return this._optionDataAccess.DeleteAttributeConfigure(parameter);
        }
        [HttpPost]
        [Route("api/options/searchOptionsTree")]
        [Authorize(Policy = "Member")]
        public Task<SearchOptionsResult> SearchOptionTree([FromBody] SearchOptionTreeParameter parameter)
        {
            return this._optionDataAccess.SearchOptionTree(parameter);
        }

        [HttpPost]
        [Route("api/options/getMasterDataAddVendorToOption")]
        [Authorize(Policy = "Member")]
        public GetMasterDataAddVendorToOptionResult GetMasterDataAddVendorToOption([FromBody] GetMasterDataAddVendorToOptionParameter parameter)
        {
            return this._optionDataAccess.GetMasterDataAddVendorToOption(parameter);
        }

        [HttpPost]
        [Route("api/options/addVendorToOption")]
        [Authorize(Policy = "Member")]
        public AddVendorToOptionResult AddVendorToOption([FromBody] AddVendorToOptionParameter parameter)
        {
            return this._optionDataAccess.AddVendorToOption(parameter);
        }

        [HttpPost]
        [Route("api/options/deleteVendorMappingOption")]
        [Authorize(Policy = "Member")]
        public DeleteVendorMappingOptionResult DeleteVendorMappingOption([FromBody] DeleteVendorMappingOptionParameter parameter)
        {
            return this._optionDataAccess.DeleteVendorMappingOption(parameter);
        }


    }
}
