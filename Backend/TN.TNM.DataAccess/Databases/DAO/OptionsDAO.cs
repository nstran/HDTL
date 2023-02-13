using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Syncfusion.Compression.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TN.TNM.Common;
using TN.TNM.DataAccess.Consts.Product;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Parameters.Options;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.AttributeConfiguration;
using TN.TNM.DataAccess.Messages.Results.Customer;
using TN.TNM.DataAccess.Messages.Results.DataType;
using TN.TNM.DataAccess.Messages.Results.Folder;
using TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.Options;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Category;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Options;
using TN.TNM.DataAccess.Models.Vendor;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class OptionsDAO : BaseDAO, IOptionDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public IConfiguration Configuration { get; }
        public TenantContext tenantContext;
        private readonly IMapper _mapper;


        public OptionsDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration, TenantContext _tenantContext, IMapper mapper)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
            this.tenantContext = _tenantContext;
            _mapper = mapper;
        }
        #region Delete Option
        public Task<DeleteOptionResult> DeleteOption(DeleteOptionParameter parameter)
        {
            try
            {
                var item = context.Options.FirstOrDefault(x => x.Id == parameter.Id);

                if (item == null)
                {
                    return System.Threading.Tasks.Task.FromResult(new DeleteOptionResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy tùy chọn dịch vụ!",
                    });
                }

                //các thuộc tính của option
                //var listAttribute = context.AttributeConfiguration.Where(x => x.Id)
                context.Options.Remove(item);
                context.SaveChanges();
                return System.Threading.Tasks.Task.FromResult(new DeleteOptionResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Thành công!",
                });
            }
            catch (Exception)
            {
                return System.Threading.Tasks.Task.FromResult(new DeleteOptionResult()
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình xóa bản ghi",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                });
            }
        }
        #endregion
        #region Get Attribute Category List
        public async Task<GetListCategoryAttributesResult> GetListCategoryAttributesResult()
        {
            try
            {
                var listDataType = GeneralList.GetGiaTriThuocTinh();
                var result = await (from c in context.Category
                                    join a in context.AttributeConfiguration
                                    on c.CategoryId equals a.CategoryId
                                    where a.ObjectType == 1
                                    orderby a.UpdatedDate descending
                                    select new ListCategoryAttributes
                                    {
                                        Id = a.Id,
                                        CategoryName = c.CategoryName,
                                        DataType = listDataType.FirstOrDefault(x => x.Value == a.DataType).Name,
                                        CategoryId = c.CategoryId,
                                    }).ToListAsync();


                return new GetListCategoryAttributesResult()
                {
                    ListCategoryAttributes = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetListCategoryAttributesResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        #endregion
        #region Search Options
        public async Task<SearchOptionsResult> SearchOption(SearchOptionParameter parameter)
        {
            try
            {
                var result = await (from o in context.Options
                                    join c in context.Category
                                    on o.CategoryId equals c.CategoryId
                                    join a in context.CategoryType
                                    on c.CategoryTypeId equals a.CategoryTypeId
                                    where a.CategoryTypeCode == ProductConsts.CategoryTypeCodeService
                                    && (parameter.ListCategoryId == null ||
                                    parameter.ListCategoryId.Count == 0 ||
                                    parameter.ListCategoryId.Contains(o.CategoryId))
                                    orderby o.UpdatedDate descending
                                    select new SearchOptions
                                    {
                                        CategoryId = c.CategoryId,
                                        CategoryName = c.CategoryName,
                                        OptionName = o.Name,
                                        Price = o.Price,
                                        OptionId = o.Id,
                                        Description = o.Description,
                                        ParentId = o.ParentId,
                                        VAT = o.Vat
                                    }).ToListAsync();

                return new SearchOptionsResult()
                {
                    ListOptions = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new SearchOptionsResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }


        }
        #endregion
        #region 
        public async Task<GetListCategoryAttributesResult> ListAttributeResult()
        {
            try
            {
                var listDataType = GeneralList.GetGiaTriThuocTinh();
                var result = await (from c in context.Category
                                    join ct in context.CategoryType
                                    on c.CategoryTypeId equals ct.CategoryTypeId
                                    where ct.CategoryTypeCode == ProductConsts.CategoryTypeCodeATR
                                    select new ListCategoryAttributes
                                    {
                                        CategoryName = c.CategoryName,
                                        CategoryId = c.CategoryId,
                                    }).ToListAsync();


                return new GetListCategoryAttributesResult()
                {
                    ListCategoryAttributes = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetListCategoryAttributesResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        #endregion
        #region Get DataType
        public async Task<DataTypeResult> GetListDataType()
        {
            try
            {
                var listDataType = GeneralList.GetGiaTriThuocTinh();
                return new DataTypeResult()
                {
                    GetGiaTriThuocTinh = listDataType,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new DataTypeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        #endregion
        #region Get Option By Id
        public async Task<SearchOptionsResult> GetOptionById(GetOptionByIdParameter parameter)
        {
            try
            {
                var result = await (from o in context.Options
                                    join c in context.Category
                                    on o.CategoryId equals c.CategoryId
                                    join ct in context.CategoryType
                                    on c.CategoryTypeId equals ct.CategoryTypeId
                                    where ct.CategoryTypeCode == ProductConsts.CategoryTypeCodeService
                                    && o.Id == parameter.Id
                                    orderby o.UpdatedDate descending
                                    select new OptionsEntityModel
                                    {
                                        Id = o.Id,
                                        CategoryId = c.CategoryId,
                                        CategoryUnitId = o.CategoryUnitId,
                                        Name = o.Name,
                                        Description = o.Description,
                                        Price = o.Price,
                                        ParentId = o.ParentId,
                                        VAT = o.Vat
                                    }).FirstOrDefaultAsync();

                return new SearchOptionsResult()
                {
                    OptionsEntityModel = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new SearchOptionsResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        #endregion
        #region Create new attribute config
        public async Task<CreateOrUpdateAttributeConfigureResult> CreateAttributeConfigure(CreateOrUpdateAttributeConfigureParamenter paramenter)
        {
            try
            {
                //var options = _mapper.Map<Options>(paramenter.OptionsEntityModel);
                var mess = "Thêm thuộc tính bổ sung thành công!";
                if (paramenter.AttributeConfigurationModel.Id != null && paramenter.AttributeConfigurationModel.Id != Guid.Empty)
                {
                    var attrConfig = context.AttributeConfiguration.FirstOrDefault(x => x.Id == paramenter.AttributeConfigurationModel.Id);
                    if(attrConfig == null)
                    {
                        return new CreateOrUpdateAttributeConfigureResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            MessageCode = "Thuộc tính không tồn tại trên hệ thống!"
                        };
                    }
                    mess = "Cập nhật thuộc tính bổ sung thành công!";
                    attrConfig.CategoryId = paramenter.AttributeConfigurationModel.CategoryId.Value;
                    attrConfig.DataType = paramenter.AttributeConfigurationModel.DataType.Value;
                    attrConfig.UpdatedById = paramenter.UserId;
                    attrConfig.UpdatedDate = DateTime.Now;
                    context.AttributeConfiguration.Update(attrConfig);
                }
                else
                {
                    var attrConfig = _mapper.Map<AttributeConfiguration>(paramenter.AttributeConfigurationModel);
                    attrConfig.Id = Guid.NewGuid();
                    attrConfig.ObjectType = 1;
                    attrConfig.CreatedById = paramenter.UserId;
                    attrConfig.CreatedDate = DateTime.Now;
                    context.AttributeConfiguration.Add(attrConfig);
                }
                context.SaveChanges();
                return new CreateOrUpdateAttributeConfigureResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateAttributeConfigureResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        #endregion
        public async Task<CreateOrUpdateOptionResult> CreateOrUpdateOptions(CreateOrUpdateOptionParameter parameter)
        {
            try
            {
                var options = _mapper.Map<Options>(parameter.OptionsEntityModel);
                var milestoneConfig = _mapper.Map<MilestoneConfiguration>(parameter.MilestoneConfigurationEntityModel);
                if (options.Id != null & options.Id != Guid.Empty)
                {
                    options.UpdatedDate = DateTime.Now;
                    options.UpdatedById = parameter.UserId;
                    context.Options.Update(options);
                }
                else
                {
                    options.Id = Guid.NewGuid();
                    options.UpdatedDate = DateTime.Now;
                    options.UpdatedById = parameter.UserId; 
                    context.Options.AddRange(options);
                }

                context.SaveChanges();
                return new CreateOrUpdateOptionResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateOptionResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<GetOptionCategoryResult> GetOptionCategoryUnit()
        {
            try
            {
                var listOptionCategory = await (from c in context.Category
                                                join ct in context.CategoryType
                                                on c.CategoryTypeId equals ct.CategoryTypeId
                                                where ct.CategoryTypeCode == ProductConsts.CategoryTypeCodeUnit
                                                select new OptionCategory
                                                {
                                                    CategoryName = c.CategoryName,
                                                    CategoryId = c.CategoryId
                                                }).ToListAsync();
                return new GetOptionCategoryResult()
                {
                    OptionCategory = listOptionCategory,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception e)
            {
                return new GetOptionCategoryResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<GetOptionCategoryResult> GetOptionCategory()
        {
            try
            {
                var listOptionCategory = await (from c in context.Category
                                                join ct in context.CategoryType
                                                on c.CategoryTypeId equals ct.CategoryTypeId
                                                where ct.CategoryTypeCode == ProductConsts.CategoryTypeCodeService
                                                select new OptionCategory
                                                {
                                                    CategoryName = c.CategoryName,
                                                    CategoryId = c.CategoryId
                                                }).ToListAsync();
                return new GetOptionCategoryResult()
                {
                    OptionCategory = listOptionCategory,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception e)
            {
                return new GetOptionCategoryResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<GetListCategoryAttributesResult> GetListCategoryAttributesById(GetListCategoryAttributesByIdParameter parameter)
        {
            try
            {
                var listDataType = GeneralList.GetTrangThais("DataTypeAttr").ToList();
                var result = await (from c in context.Category
                                    join a in context.AttributeConfiguration
                                    on c.CategoryId equals a.CategoryId
                                    where a.ObjectType == 1 && a.ObjectId == parameter.Id
                                    orderby a.UpdatedDate descending
                                    select new ListCategoryAttributes
                                    {
                                        Id = a.Id,
                                        CategoryName = c.CategoryName,
                                        DataType = listDataType.FirstOrDefault(x => x.Value == a.DataType).Name,
                                        DataTypeValue = a.DataType,
                                        CategoryId = c.CategoryId,
                                        CreatedDate = a.CreatedDate
                                    }).OrderBy(x => x.CreatedDate).ToListAsync();

                var attrCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeATR).CategoryTypeId;
                var listAttr = context.Category.Where(x => x.CategoryTypeId == attrCateTypeId ).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();


                return new GetListCategoryAttributesResult()
                {
                    ListCategoryAttributes = result,
                    ListAttr = listAttr,
                    ListDataType = listDataType,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetListCategoryAttributesResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<DeleteAttributeConfigureResult> DeleteAttributeConfigure(DeleteAttributeConfigureParameter parameter)
        {
            try
            {
                var item = await context.AttributeConfiguration.FirstOrDefaultAsync(x => x.Id == parameter.Id);
                if (item == null)
                {
                    return new DeleteAttributeConfigureResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy tùy chọn dịch vụ!",
                    };
                }
                else
                {
                    context.AttributeConfiguration.Remove(item);
                    context.SaveChanges();
                    return new DeleteAttributeConfigureResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = "Xóa thành công!",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteAttributeConfigureResult()
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình xóa bản ghi",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        //public async Task<SearchOptionsResult> SearchOptionTree(SearchOptionParameter parameter)
        //{
        //    try
        //    {
        //        var result = await (from o in context.Options
        //                            join c in context.Category
        //                            on o.CategoryId equals c.CategoryId
        //                            join a in context.CategoryType
        //                            on c.CategoryTypeId equals a.CategoryTypeId
        //                            where a.CategoryTypeCode == OptionsConsts.ProductCodeType
        //                            select new SearchOptionTree
        //                            {
        //                                CategoryName = c.CategoryName,
        //                                OptionName = o.Name,
        //                                Price = o.Price,
        //                                OptionId = o.Id,
        //                            }).ToListAsync();
        //        return new SearchOptionsResult()
        //        {
        //            ListOptions = result,
        //            StatusCode = HttpStatusCode.OK
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return new SearchOptionsResult()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        public async Task<SearchOptionsResult> SearchOptionTree(SearchOptionTreeParameter parameter)
        {
            try
            {
                var listOPtionId = new List<Guid?>();
                listOPtionId.Add(parameter.OptionId);
                listOPtionId = getOptionChildrenId(parameter.OptionId, listOPtionId);

                var result = await (from o in context.Options
                                    join c in context.Category
                                    on o.CategoryId equals c.CategoryId
                                    join a in context.CategoryType
                                    on c.CategoryTypeId equals a.CategoryTypeId
                                    where a.CategoryTypeCode == ProductConsts.CategoryTypeCodeService && listOPtionId.Contains(o.Id)
                                    orderby o.UpdatedDate descending
                                    select new SearchOptions
                                    {
                                        CategoryName = c.CategoryName,
                                        OptionName = o.Name,
                                        Price = o.Price,
                                        OptionId = o.Id,
                                        ParentId = o.ParentId,
                                        Description = o.Description
                                    }).ToListAsync();
                result.ForEach(x => x.HasChild = result.FirstOrDefault(t => t.ParentId == x.OptionId) != null);

                return new SearchOptionsResult()
                {
                    ListOptions = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new SearchOptionsResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
            //try
            //{

            //    var listOption = await (from o in context.Options
            //                            join c in context.Category
            //                            on o.CategoryId equals c.CategoryId
            //                            join a in context.CategoryType
            //                             on c.CategoryTypeId equals a.CategoryTypeId
            //                            where a.CategoryTypeCode == OptionsConsts.ProductCodeType
            //                            select new SearchOptionTree
            //                            {
            //                                CategoryName = c.CategoryName,
            //                                OptionName = o.Name,
            //                                Price = o.Price,
            //                                OptionId = o.Id,
            //                                ParentId = o.ParentId,
            //                                ListOptionTrees = new List<SearchOptionTree>()
            //                            }).ToListAsync();
            //    listOption.ForEach(x =>
            //    {
            //        var listChild = listOption.Where(y => y.ParentId == x.OptionId).Select(
            //            z => new SearchOptionTree
            //            {
            //                CategoryName = z.CategoryName,
            //                OptionName = z.OptionName,
            //                Price = z.Price,
            //                OptionId = z.OptionId,
            //                ParentId = z.ParentId,
            //                ListOptionTrees = new List<SearchOptionTree>()
            //            }
            //            ).ToList();
            //        x.HasChild = listOption.FirstOrDefault(t => t.ParentId == x.OptionId) != null;
            //        x.ListOptionTrees = listChild;

            //    });
            //    listOption.ForEach(item =>
            //{
            //    var listFileInFolder = listFile.Where(x => x.FolderId == item.FolderId).Select(y => new FileInFolderEntityModel()
            //    {
            //        Active = y.Active,
            //        FileInFolderId = y.FileInFolderId,
            //        FileName = y.FileName,
            //        FolderId = y.FolderId,
            //        ObjectId = y.ObjectId,
            //        ObjectType = y.ObjectType,
            //        Size = y.Size,
            //        FileExtension = y.FileExtension
            //    }).ToList();
            //    item.HasChild = listFolderResult.FirstOrDefault(z => z.ParentId == item.FolderId) != null;
            //    item.ListFile = listFileInFolder;
            //});

            //return new SearchOptionsResult()
            //{
            //    ListOptionTrees = listOption,
            //    StatusCode = HttpStatusCode.OK
            //};
            //}
            //var listFolderId = listFolderResult.Select(x => x.FolderId).ToList();
            //var listFile = context.FileInFolder.Where(x => listFolderId.Contains(x.FolderId)).ToList();



            //    catch (Exception e)
            //    {
            //        return new SearchOptionsResult()
            //{
            //    StatusCode = HttpStatusCode.ExpectationFailed,
            //            MessageCode = e.Message
            //        };
            //}
        }

        public GetMasterDataAddVendorToOptionResult GetMasterDataAddVendorToOption(GetMasterDataAddVendorToOptionParameter parameter)
        {
            var commonCategoryType = context.CategoryType.Where(x => x.CategoryTypeCode == "NCA" || x.CategoryTypeCode == "NH").ToList();
            var listCommonCategoryTypeId = commonCategoryType.Select(x => x.CategoryTypeId).ToList();
            var commonCategory = context.Category.Where(x => listCommonCategoryTypeId.Contains(x.CategoryTypeId)).ToList();

            //Nhóm nhà cung cấp
            var vendorGroupCateTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NCA").CategoryTypeId;
            var listVendorGroup = commonCategory.Where(x => x.CategoryTypeId == vendorGroupCateTypeId).Select(x => new CategoryEntityModel { 
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            }).ToList();

            //danh sashc ngân hàng
            var bankCateTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NH").CategoryTypeId;
            var listBank = commonCategory.Where(x => x.CategoryTypeId == bankCateTypeId).Select(x => new CategoryEntityModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            }).ToList();

            var commonContact = context.Contact.Where(w => w.Active == true && w.ObjectType == ObjectType.VENDOR).ToList();

            var vendorList = context.Vendor.Where(w => w.Active == true)
                            .Select(v => new VendorEntityModel
                            {
                                VendorId = v.VendorId,
                                ContactId = commonContact.FirstOrDefault(c => c.ObjectId == v.VendorId && c.ObjectType == ObjectType.VENDOR).ContactId,
                                VendorName = v.VendorName,
                                VendorGroupId = v.VendorGroupId,
                                VendorGroupName = listVendorGroup.FirstOrDefault(c => c.CategoryId == v.VendorGroupId).CategoryName,
                                VendorCode = v.VendorCode,
                                Address = v.Address,
                                Email = v.Email,
                                PhoneNumber = v.PhoneNumber,
                                Active = v.Active,
                            }).OrderByDescending(v => v.CreatedDate).ToList();

            var listVendorIdMappingOption = context.VendorMappingOption.Where(x => x.OptionId == parameter.OptionId).Select(x => x.VendorId).ToList();
            var listVendorMappingOption = vendorList.Where(x => listVendorIdMappingOption.Contains(x.VendorId)).ToList();
            vendorList = vendorList.Where(x => !listVendorIdMappingOption.Contains(x.VendorId)).ToList();

            var listVendorId = listVendorMappingOption.Select(x => x.VendorId).ToList();
            var listContact = context.Contact.Where(x => listVendorId.Contains(x.ObjectId)).ToList();
            listVendorMappingOption.ForEach(item =>
            {
                var contact = listContact.FirstOrDefault(x => x.ObjectId == item.VendorId);
                item.Address = contact?.Address;
                item.Email = contact?.Email;
                item.PhoneNumber = contact?.Phone;
            });

            return new GetMasterDataAddVendorToOptionResult()
            {
                StatusCode = HttpStatusCode.OK,
                VendorList = vendorList,
                ListVendorGroup = listVendorGroup,
                ListBank = listBank,
                ListVendorMappingOption = listVendorMappingOption
            };
        }

        private List<Guid?> getOptionChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Options.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.Id);
                getOptionChildrenId(item.Id, list);
            });
            return list;
        }

        public AddVendorToOptionResult AddVendorToOption(AddVendorToOptionParameter parameter)
        {
            try
            {
                var newObj = new VendorMappingOption();
                newObj.Id = Guid.NewGuid();
                newObj.VendorId = parameter.VendorId;
                newObj.OptionId = parameter.OptionId;
                newObj.CreatedById = parameter.UserId;
                newObj.CreatedDate = DateTime.Now;
                context.VendorMappingOption.Add(newObj);
                context.SaveChanges();
                return new AddVendorToOptionResult()
                {
                    MessageCode = "Thêm thành công!",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new AddVendorToOptionResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public DeleteVendorMappingOptionResult DeleteVendorMappingOption(DeleteVendorMappingOptionParameter parameter)
        {
            try
            {
                var vendor = context.VendorMappingOption.FirstOrDefault(x => x.VendorId == parameter.VendorId && x.OptionId == parameter.OptionId);
                if(vendor == null)
                {
                    return new DeleteVendorMappingOptionResult()
                    {
                        MessageCode = "Không tìm thấy nhà cung cấp trong dịch vụ!",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }
                context.VendorMappingOption.Remove(vendor);
                context.SaveChanges();
                return new DeleteVendorMappingOptionResult()
                {
                    MessageCode = "Xóa nhà cung cấp thành công!",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new DeleteVendorMappingOptionResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
    }
}
