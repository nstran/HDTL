using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Messages.Parameters.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Parameters.Options;
using TN.TNM.DataAccess.Messages.Results.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Results.DataType;
using TN.TNM.DataAccess.Messages.Results.Options;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IOptionDataAccess
    {
        Task<GetListCategoryAttributesResult> GetListCategoryAttributesResult();
        Task<SearchOptionsResult> SearchOption(SearchOptionParameter parameter);
        Task<GetListCategoryAttributesResult> ListAttributeResult();
        Task<DataTypeResult> GetListDataType();
        Task<SearchOptionsResult> GetOptionById(GetOptionByIdParameter parameter);
        Task<CreateOrUpdateAttributeConfigureResult> CreateAttributeConfigure(CreateOrUpdateAttributeConfigureParamenter paramenter);
        Task<CreateOrUpdateOptionResult> CreateOrUpdateOptions(CreateOrUpdateOptionParameter parameter);
        Task<GetOptionCategoryResult> GetOptionCategory();
        Task<DeleteOptionResult> DeleteOption(DeleteOptionParameter parameter);
        Task<GetListCategoryAttributesResult> GetListCategoryAttributesById(GetListCategoryAttributesByIdParameter parameter);
        Task<DeleteAttributeConfigureResult> DeleteAttributeConfigure(DeleteAttributeConfigureParameter parameter);
        Task<SearchOptionsResult> SearchOptionTree(SearchOptionTreeParameter parameter);
        Task<GetOptionCategoryResult> GetOptionCategoryUnit();
        GetMasterDataAddVendorToOptionResult GetMasterDataAddVendorToOption(GetMasterDataAddVendorToOptionParameter parameter);
        AddVendorToOptionResult AddVendorToOption(AddVendorToOptionParameter parameter);
        DeleteVendorMappingOptionResult DeleteVendorMappingOption(DeleteVendorMappingOptionParameter parameter);
    }
}
