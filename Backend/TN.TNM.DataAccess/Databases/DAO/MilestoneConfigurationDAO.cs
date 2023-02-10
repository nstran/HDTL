using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.Options;
using TN.TNM.DataAccess.Models.MilestoneConfiguration;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class MilestoneConfigurationDAO : BaseDAO, IMilestoneConfigurationDataAccess
    {
        private readonly IMapper _mapper;
        public MilestoneConfigurationDAO(Databases.TNTN8Context _content, IMapper mapper)
        {
            this.context = _content;
            _mapper = mapper;
        }

        public async Task<CreateOrUpdateMilestoneConfigurationResult> CreateOrUpdateMilestoneConfiguration(CreateOrUpdateMilestoneConfigurationParameter parameter)
        {
            try
            {
                var milestone = _mapper.Map<MilestoneConfiguration>(parameter.MilestoneConfigurationEntityModel);

                var optionModel = new GetListMilestoneConfigurationByOptionIdParameter();
                optionModel.Id = parameter.MilestoneConfigurationEntityModel.OptionId;
                var i = await GetListMilestoneConfigurationByOptionIdResult(optionModel);
                var index = i.ListMilestoneConfiguration.Count();
                milestone.Id = Guid.NewGuid();
                milestone.CreatedById = parameter.UserId;
                milestone.CreatedDate = DateTime.Now;
                milestone.SortOrder = index + 1;
                context.MilestoneConfiguration.AddRange(milestone);
                context.SaveChanges();
                return new CreateOrUpdateMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }


        public async Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationResult()
        {
            try
            {
                var a = context.MilestoneConfiguration;


                var listMilestoneConfig = await context.MilestoneConfiguration
                                        .Select(x => new ListMilestoneConfiguration
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            SortOrder = x.SortOrder
                                        }).OrderBy(x => x.SortOrder).ToListAsync();
                return new GetListMilestoneConfigurationResult()
                {
                    ListMilestoneConfiguration = listMilestoneConfig,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception e)
            {
                return new GetListMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public Task<MilestoneConfigurationResult> DeleteMilestoneConfiguration(DeleteMilestoneConfigurationParameter request)
        {
            try
            {
                var item = context.MilestoneConfiguration.FirstOrDefault(x => x.Id == request.Id);
                if (item == null)
                {
                    return System.Threading.Tasks.Task.FromResult(new MilestoneConfigurationResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy tùy chọn dịch vụ!",
                    });
                }

                //Lấy list để sắp xếp lại danh sách
                var listMilestone = context.MilestoneConfiguration.Where(x => x.OptionId == item.OptionId && x.Id != request.Id).OrderBy(x => x.SortOrder).ToList();
                var index = 1;
                listMilestone.ForEach(obj =>
                {
                    obj.SortOrder = index;
                    index++;
                });
                context.MilestoneConfiguration.Remove(item);
                context.MilestoneConfiguration.UpdateRange(listMilestone);
                context.SaveChanges();
                return System.Threading.Tasks.Task.FromResult(new MilestoneConfigurationResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Thành công!",
                });
            }
            catch (Exception e)
            {
                return System.Threading.Tasks.Task.FromResult(new MilestoneConfigurationResult()
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình xóa bản ghi",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                });
            }
        }

        public async Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationByOptionIdResult(GetListMilestoneConfigurationByOptionIdParameter parameter)
        {
            try
            {
                var listMilestoneConfig = await context.MilestoneConfiguration
                                        .Where(x=>x.OptionId == parameter.Id)
                                        .Select(x => new ListMilestoneConfiguration
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            SortOrder = x.SortOrder,
                                        }).OrderBy(x => x.SortOrder).ToListAsync();
                return new GetListMilestoneConfigurationResult()
                {
                    ListMilestoneConfiguration = listMilestoneConfig,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception e)
            {
                return new GetListMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<CreateOrUpdateMilestoneConfigurationResult> CreateMilestoneConfigurationByOptionId(CreateOrUpdateMilestoneConfigurationParameter parameter)
        {
            try
            {
                var milestone = _mapper.Map<MilestoneConfiguration>(parameter.MilestoneConfigurationEntityModel);
                milestone.Id = Guid.NewGuid();
                milestone.CreatedById = parameter.UserId;
                milestone.CreatedDate = DateTime.Now;
                context.MilestoneConfiguration.AddRange(milestone);
                context.SaveChanges();
                return new CreateOrUpdateMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<UpdateListMilestoneConfigurationResult> UpdateListMilestoneConfigure(UpdateListMilestoneConfigurationParameter parameter)
        {
            try
            {
                var milestone = _mapper.Map<List<MilestoneConfiguration>>(parameter.ListMilestoneConfigurationEntityModel);
                foreach (var item in milestone)
                {
                    context.MilestoneConfiguration.Update(item);
                }
                context.SaveChanges();
                return new UpdateListMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo thành công"
                };
            }
            catch (Exception e)
            {
                return new UpdateListMilestoneConfigurationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
