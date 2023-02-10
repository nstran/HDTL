using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Task;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class TaskDAO : BaseDAO, ITaskDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public TaskDAO(TNTN8Context _content, IHostingEnvironment hostingEnvironment)
        {
            context = _content;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateOrUpdateTaskResult CreateOrUpdateTask(CreateOrUpdateTaskParameter parameter)
        {

            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new CreateOrUpdateTaskResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Thư mục upload không tồn tại"
                };
            }

            var listFileDelete = new List<string>();

            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new CreateOrUpdateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed,
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new CreateOrUpdateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại tong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                // Tạo mới
                if (parameter.Task.TaskId == Guid.Empty || parameter.Task.TaskId == null)
                {
                    //#region Kiểm tra nhân viên tạo task có trong thời gian phân bổ dự án không

                    //var isInTime = true;
                    //var message = "";

                    //var resources = context.ProjectResource.Where(x => x.ObjectId == employee.EmployeeId && x.ProjectId == parameter.Task.ProjectId)
                    //    .OrderBy(y => y.CreateDate)
                    //    .ToList();

                    //DateTime taskStart = parameter.Task.PlanStartTime.Value.Date;
                    //DateTime taskEnd = parameter.Task.PlanEndTime.Value.Date;

                    //foreach (var resource in resources)
                    //{
                    //    var startTime = resource.StartTime.Value.Date;
                    //    var endTime = resource.EndTime.Value.Date;

                    //    if (startTime <= endTime && (startTime <= taskStart && taskStart <= endTime)
                    //                             && (startTime <= taskEnd && taskEnd <= endTime))
                    //    {
                    //        isInTime = true;
                    //        message = "";
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        isInTime = false;
                    //        message = $"Nhân viên không thể tạo task ngoài khoảng thời gian phân bổ từ {startTime.Date:dd/MM/yyyy} đến {endTime.Date:dd/MM/yyyy}";
                    //    }
                    //}

                    //if (!isInTime)
                    //{
                    //    return new CreateOrUpdateTaskResult()
                    //    {
                    //        Message = message,
                    //        Status = false,
                    //    };
                    //}

                    //#endregion


                    var projectTask = new Task
                    {
                        TaskId = Guid.NewGuid(),
                        ProjectId = parameter.Task.ProjectId,
                        ProjectScopeId = parameter.Task.ProjectScopeId,
                        TaskCode = GenerateTaskCode(parameter.Task.ProjectId),
                        TaskName = parameter.Task.TaskName,
                        PlanStartTime = parameter.Task.PlanStartTime,
                        PlanEndTime = parameter.Task.PlanEndTime,
                        EstimateHour = parameter.Task.EstimateHour,
                        EstimateCost = parameter.Task.EstimateCost,
                        ActualStartTime = parameter.Task.ActualStartTime,
                        ActualEndTime = parameter.Task.ActualEndTime,
                        ActualHour = parameter.Task.ActualHour,
                        Description = parameter.Task.Description,
                        Status = parameter.Task.Status,
                        Priority = parameter.Task.Priority,
                        MilestonesId = parameter.Task.MilestonesId,
                        IncludeWeekend = parameter.Task.IncludeWeekend,
                        IsSendMail = parameter.Task.IsSendMail,
                        TaskComplate = parameter.Task.TaskComplate,
                        TaskTypeId = parameter.Task.TaskTypeId,
                        TimeType = parameter.Task.TimeType,
                        CreateBy = parameter.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = parameter.UserId,
                        UpdateDate = DateTime.Now
                    };

                    var status = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Task.Status);
                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        ObjectType = "TAS",
                        ObjectId = projectTask.TaskId,
                        Type = "SYS",
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "Đã thêm ghi chú"
                    };

                    if (parameter.Task.TaskComplate != 0 && status.CategoryCode == "NEW")
                    {
                        projectTask.TaskComplate = parameter.Task.TaskComplate;
                        // NgocTB edit Nếu phần trăm hoàn thành là 100 => chuyển sang hoàn thành
                        projectTask.Status = projectTask.TaskComplate == 100
                            ? listStatus.FirstOrDefault(c => c.CategoryCode == "HT").CategoryId
                            : listStatus.FirstOrDefault(c => c.CategoryCode == "DL").CategoryId;
                        projectTask.ActualStartTime = parameter.Task.ActualStartTime != null
                            ? parameter.Task.ActualStartTime
                            : DateTime.Now;
                        note.Description = projectTask.TaskComplate == 100
                            ? "<p>- <strong>" + user.UserName + "</strong>" + " đã hoàn thành công việc"
                            : $"{user.UserName} đã bắt đầu công việc" + "</p>";
                        context.Note.Add(note);
                    }
                    else if (projectTask.ActualEndTime != null && status.CategoryCode != "NEW" && projectTask.ActualEndTime.Value.Date < DateTime.Now.Date)
                    {
                        projectTask.ActualEndTime = parameter.Task.ActualEndTime;
                        projectTask.Status = listStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE").CategoryId;
                        note.Description = "<p>- <strong>" + user.UserName + "</strong>" + " đã Đóng công việc!" + "</p>";
                        context.Note.Add(note);
                    }
                    else if (parameter.Task.ActualStartTime != null && status.CategoryCode == "NEW" && parameter.Task.ActualStartTime.Value.Date <= DateTime.Now.Date)
                    {
                        projectTask.ActualStartTime = parameter.Task.ActualStartTime;
                        projectTask.Status = listStatus.FirstOrDefault(c => c.CategoryCode == "DL").CategoryId;
                        note.Description = "<p>- <strong>" + user.UserName + "</strong>" + " đã Bắt đầu công việc!" + "</p>";
                        context.Note.Add(note);
                    }
                    else
                    {
                        switch (status.CategoryCode)
                        {
                            case "NEW":
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" + " đã tạo công việc" +
                                                   "</p>";
                                context.Note.Add(note);
                                break;
                            case "DL":
                                projectTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                    ? parameter.Task.ActualStartTime
                                    : DateTime.Now;
                                projectTask.Status = parameter.Task.Status;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" +
                                                   " đã Bắt đầu công việc" + "</p>";
                                context.Note.Add(note);
                                break;
                            case "HT":
                                projectTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                    ? parameter.Task.ActualStartTime
                                    : DateTime.Now;
                                projectTask.Status = parameter.Task.Status;
                                projectTask.CompleteDate = DateTime.Now;
                                projectTask.TaskComplate = 100;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" +
                                                   " đã Hoàn thành công việc" + "</p>";
                                context.Note.Add(note);
                                break;
                            case "CLOSE":
                                projectTask.Status = parameter.Task.Status;
                                projectTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                    ? parameter.Task.ActualStartTime
                                    : DateTime.Now;
                                projectTask.ActualEndTime = parameter.Task.ActualEndTime != null
                                    ? parameter.Task.ActualEndTime
                                    : DateTime.Now;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" +
                                                   " đã Đóng công việc" + "</p>";
                                context.Note.Add(note);
                                break;
                        }
                    }
                    context.Task.Add(projectTask);

                    #region cap nhat ngay bat dau thuc te va trang thai cua du an

                    var today = DateTime.Now;
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == projectTask.ProjectId);
                    var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT")?.CategoryTypeId;
                    var categoryTypeIdTask = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                    var statusMoi =
                        context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "MOI")?.CategoryId;
                    var statusDtk = context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DTK")?.CategoryId;
                    var sttStart = context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeIdTask && x.CategoryCode == "DL")?.CategoryId;

                    if (project != null)
                    {
                        if (project.ProjectStatus == statusMoi)
                        {
                            // update trang thai du an
                            if (projectTask.Status == sttStart)
                            {
                                project.ProjectStatus = statusDtk;
                            }

                            // update ngay bat dau thuc te
                            if (project.ActualStartDate == null || project.ActualStartDate > today)
                            {
                                project.ActualStartDate = projectTask.ActualStartTime;
                            }
                        }
                        context.Update(project);
                        context.SaveChanges();
                    }

                    #endregion

                    if (parameter.ListProjectResource != null)
                    {
                        if(parameter.ListProjectResource.Count > 0)
                        {
                            //Kiểm tra xem nhân sự được giao việc đã được Allocate trong khoảng thời gian "Bắt đầu" và "Kết thúc" dự kiến của công việc hay chưa?
                            //Nếu chưa thì trả về thông báo: Nguồn lực sử dụng cho công việc chưa được phân bổ trong khoảng thời gian dự kiến.

                            //Lấy thời gian người phụ trách được phân bổ trong dự án
                            var listAllPhanBoProject = context.ProjectResource.Where(x => x.ProjectId == parameter.Task.ProjectId).ToList();

                            var listTaskResource = new List<TaskResourceMapping>();
                            var isTrongThoiGianPhanBo = false;
                            var listStatusPhanBo = new List<bool?>();
                            parameter.ListProjectResource?.ForEach(item =>
                            {
                                isTrongThoiGianPhanBo = false;
                                var nguoiLucInFor = listAllPhanBoProject.FirstOrDefault(x => x.ProjectResourceId == item.ProjectResourceId);
                                if (nguoiLucInFor != null)
                                {
                                    //Nếu tìm thấy nguồn lực => lấy toàn bộ các khoảng thời gian nguồn lực được phân bổ
                                    var listThoiGianPhanBo = listAllPhanBoProject.Where(x => x.ObjectId == nguoiLucInFor.ObjectId).ToList();
                                    if (listThoiGianPhanBo != null)
                                    {
                                        listThoiGianPhanBo.ForEach(nguonLuc =>
                                        {
                                            //Thời gian của công việc phải trong thời gian phân bổ
                                            if ((nguonLuc.StartTime.Value.Date <= parameter.Task.PlanStartTime.Value.Date) &&
                                                (parameter.Task.PlanEndTime.Value.Date <= nguonLuc.EndTime.Value.Date))
                                            {
                                                isTrongThoiGianPhanBo = true;
                                            }
                                            //parameter.Task.PlanStartTime;
                                            //parameter.Task.PlanEndTime;
                                        });
                                    }
                                    var taskResourceMapping = new TaskResourceMapping
                                    {
                                        TaskResourceMappingId = Guid.NewGuid(),
                                        TaskId = projectTask.TaskId,
                                        ResourceId = item.ProjectResourceId,
                                        Hours = item.Hours,
                                        IsChecker = item.IsCheck,
                                        IsPersonInCharge = item.IsPic
                                    };
                                    listTaskResource.Add(taskResourceMapping);
                                }
                                else
                                {
                                    //Nguồn lực đang k tìm thấy trên hệ thống
                                }
                                listStatusPhanBo.Add(isTrongThoiGianPhanBo);
                            });
                            //Kiểm tra xem list nhân viên được phân công cho cviec có ai năm ngoài thời gian phân bổ k
                            var ngoaiThoiGianPhanBo = listStatusPhanBo.FirstOrDefault(x => x == false);
                            if (ngoaiThoiGianPhanBo != null)//Có người ít nhất 1 người năm ngoài thời gian phân bổ => trả về thông báo
                            {
                                return new CreateOrUpdateTaskResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Nguồn lực sử dụng cho công việc chưa được phân bổ trong khoảng thời gian dự kiến."
                                };
                            }
                            context.TaskResourceMapping.AddRange(listTaskResource);
                        }
                    };

                    if (parameter.ListTaskDocument?.Count > 0)
                    {

                        var isSave = true;
                        parameter.ListTaskDocument?.ForEach(item =>
                        {
                            if (folder == null)
                            {
                                isSave = false;
                            }

                            var folderName = ConvertFolderUrl(folder.Url);
                            var webRootPath = _hostingEnvironment.WebRootPath;
                            var newPath = Path.Combine(webRootPath, folderName);

                            if (!Directory.Exists(newPath))
                            {
                                isSave = false;
                            }

                            if (isSave)
                            {
                                var file = new FileInFolder()
                                {
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedById = parameter.UserId,
                                    UpdatedDate = DateTime.Now,
                                    FileInFolderId = Guid.NewGuid(),
                                    FileName = $"{item.FileInFolder.FileName}_{Guid.NewGuid()}",
                                    FolderId = folder.FolderId,
                                    ObjectId = projectTask.TaskId,
                                    ObjectType = item.FileInFolder.ObjectType,
                                    Size = item.FileInFolder.Size,
                                    FileExtension = item.FileSave.FileName.Substring(
                                        item.FileSave.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1),
                                };

                                context.FileInFolder.Add(file);

                                var fileName = $"{file.FileName}.{file.FileExtension}";

                                if (!isSave) return;
                                var fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    item.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        });

                        if (!isSave)
                        {
                            listFileDelete.ForEach(File.Delete);
                            return new CreateOrUpdateTaskResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Bạn phải cấu hình thư mục để lưu"
                            };
                        }

                        // var projectCodeName = $"{project.ProjectCode} - {project.ProjectName}";
                        // var listTaskDocument = new List<TaskDocument>();
                        // string folderName = "FileUpload";
                        // string webRootPath = _hostingEnvironment.WebRootPath;
                        // string newPath = Path.Combine(webRootPath, folderName);
                        // newPath = Path.Combine(newPath, "Project");
                        // newPath = Path.Combine(newPath, projectCodeName);
                        // newPath = Path.Combine(newPath, "Task");

                        // parameter.ListTaskDocument.ForEach(item =>
                        // {
                        //     item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                        //
                        //     var taskDocument = new TaskDocument
                        //     {
                        //         TaskDocumentId = Guid.NewGuid(),
                        //         TaskId = projectTask.TaskId,
                        //         Active = true,
                        //         DocumentName = item.DocumentName,
                        //         DocumentSize = item.DocumentSize,
                        //         DocumentUrl = item.DocumentUrl,
                        //         CreatedById = item.CreatedById,
                        //         CreatedDate = DateTime.Now,
                        //         UpdatedById = null,
                        //         UpdatedDate = null
                        //     };
                        //     listTaskDocument.Add(taskDocument);
                        // });
                        // context.TaskDocument.AddRange(listTaskDocument);
                    }

                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    context.SaveChanges();

                    if (projectTask.IsSendMail == false)
                    {
                        // Send Mail
                        #region Thông báo

                        NotificationHelper.AccessNotification(context, TypeModel.ProjectTask, "CRE", new Task(),
                            projectTask, true);

                        #endregion
                    }

                    //Lưu công việc liên quan của task
                    if(parameter.ListTaskRelate != null)
                    {
                        if(parameter.ListTaskRelate.Count > 0)
                        {
                            var listRelateTask = new List<RelateTaskMapping>();
                            parameter.ListTaskRelate.ForEach(item =>
                            {
                                var mapping = new RelateTaskMapping();
                                mapping.RelateTaskMappingId = Guid.NewGuid();
                                mapping.RelateTaskId = item.RelateTaskId;
                                mapping.TaskId = projectTask.TaskId;
                                mapping.ProjectId = item.ProjectId;
                                mapping.CreatedDate = DateTime.Now;
                                mapping.CreatedById = parameter.UserId;
                                listRelateTask.Add(mapping);
                            });
                            context.RelateTaskMapping.AddRange(listRelateTask);
                            context.SaveChanges();
                        }
                    }
                    return new CreateOrUpdateTaskResult
                    {
                        MessageCode = "Success",
                        StatusCode = HttpStatusCode.OK,
                        TaskId = projectTask.TaskId
                    };
                }
                // Cập nhật
                else
                {
                    var oldTask = context.Task.FirstOrDefault(c => c.TaskId == parameter.Task.TaskId);
                    if (oldTask == null)
                    {
                        return new CreateOrUpdateTaskResult
                        {
                            MessageCode = "Công việc không tồn tại trong hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed,
                        };
                    }

                    //Kiểm tra số lần mợ lại task
                    var taskStatusTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                    var listStatusTask = context.Category.Where(x => x.CategoryTypeId == taskStatusTypeId).ToList();
                    var workingStatus = listStatusTask.FirstOrDefault(x => x.CategoryCode == "DL").CategoryId;
                    var completeStatus = listStatusTask.FirstOrDefault(x => x.CategoryCode == "HT").CategoryId;
                    if (oldTask.Status == completeStatus && parameter.Task.Status == workingStatus)
                    {
                        oldTask.SoLanMoLai = (oldTask.SoLanMoLai ?? 0) + 1;
                    }


                    if (oldTask.PlanStartTime.Value.Date != parameter.Task.PlanStartTime.Value.Date ||
                        oldTask.PlanEndTime.Value.Date != parameter.Task.PlanEndTime.Value.Date || parameter.ListProjectResource?.Count != 0)
                    {

                        #region Kiểm tra nhân viên tạo task có trong thời gian phân bổ dự án không

                        //var isInTime = true;
                        //var message = "";

                        //var resources = context.ProjectResource.Where(x => x.ObjectId == employee.EmployeeId && x.ProjectId == parameter.Task.ProjectId)
                        //    .OrderBy(y => y.CreateDate)
                        //    .ToList();

                        //DateTime taskStart = parameter.Task.PlanStartTime.Value.Date;
                        //DateTime taskEnd = parameter.Task.PlanEndTime.Value.Date;

                        //foreach (var resource in resources)
                        //{
                        //    var startTime = resource.StartTime.Value.Date;
                        //    var endTime = resource.EndTime.Value.Date;

                        //    if (startTime <= endTime && (startTime <= taskStart && taskStart <= endTime)
                        //                             && (startTime <= taskEnd && taskEnd <= endTime))
                        //    {
                        //        isInTime = true;
                        //        message = "";
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        isInTime = false;
                        //        message = $"Nhân viên không thể cập nhật task ngoài khoảng thời gian phân bổ từ {startTime.Date:dd/MM/yyyy} đến {endTime.Date:dd/MM/yyyy}";
                        //    }
                        //}

                        //if (!isInTime)
                        //{
                        //    return new CreateOrUpdateTaskResult()
                        //    {
                        //        Message = message,
                        //        Status = false,
                        //    };
                        //}

                        #endregion
                    }

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "TAS",
                            ObjectId = oldTask.TaskId,
                            Type = "SYS",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "Đã thêm ghi chú"
                        };

                        var presentStatus = context.Category.FirstOrDefault(c => c.CategoryId == oldTask.Status);
                        var updateStatus = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Task.Status);

                        #region Thêm ghi chú nếu thay đổi trạng thái
                        if (parameter.Task.TaskComplate != 0 && presentStatus.CategoryCode == "NEW" && updateStatus.CategoryCode == "NEW")
                        {
                            oldTask.TaskComplate = parameter.Task.TaskComplate;

                            var status = parameter.Task.TaskComplate == 100
                                ? listStatus.FirstOrDefault(c => c.CategoryCode == "HT")
                                : listStatus.FirstOrDefault(c => c.CategoryCode == "DL");
                            oldTask.Status = status.CategoryId;
                            if (status.CategoryCode == "HT")
                            {
                                oldTask.CompleteDate = DateTime.Now;
                            }
                            oldTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                ? parameter.Task.ActualStartTime
                                : DateTime.Now;
                            string noteDes = "";
                            noteDes += "<p>- <strong>" + user.UserName +
                                       "</strong> đã thay đổi % hoàn thành công việc " + oldTask.TaskCode
                                       + " từ <strong>" + oldTask.TaskComplate + "</strong>" + "% sang "
                                       + "<strong>" + parameter.Task.TaskComplate + "%</strong></p>";

                            if (parameter.Task.TaskComplate == 100)
                            {                            
                                noteDes += "<p>- <strong>" + user.UserName + "</strong>" +
                                           $" đã thay đổi trạng thái công việc sang {status.CategoryName}" + "</p>";

                            }
                            note.Description = noteDes;
                            context.Note.Add(note);
                        }
                        else if ((presentStatus.CategoryCode == "CLOSE" || presentStatus.CategoryCode == "HT") && 
                                 updateStatus.CategoryCode == "DL" && oldTask.ActualEndTime != null)
                        {
                            if (oldTask.ActualEndTime.Value.Date > DateTime.Now.Date)
                            {
                                oldTask.Status = updateStatus.CategoryId;
                                oldTask.TaskComplate = parameter.Task.TaskComplate;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" +
                                                   $" đã thay đổi trạng thái công việc sang {updateStatus.CategoryName}" +
                                                   "</p>";
                            }
                            else
                            {
                                oldTask.Status = updateStatus.CategoryId;
                                oldTask.ActualEndTime = null;
                                string noteDes = "";
                                if (parameter.Task.TaskComplate != oldTask.TaskComplate)
                                {
                                    noteDes += "<p>- <strong>" + user.UserName +
                                               "</strong> đã thay đổi % hoàn thành công việc " + oldTask.TaskCode
                                               + " từ <strong>" + oldTask.TaskComplate + "</strong>" + "% sang "
                                               + "<strong>" + parameter.Task.TaskComplate + "%</strong></p>";
                                }
                                oldTask.TaskComplate = parameter.Task.TaskComplate;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>" +
                                                   $" đã thay đổi trạng thái công việc sang {updateStatus.CategoryName}" +
                                                   "</p>" + noteDes;
                            }
                            context.Note.Add(note);
                        }
                        else if (parameter.Task.ActualEndTime != null && updateStatus.CategoryCode != "CLOSE" && 
                                 updateStatus.CategoryCode != "HT")
                        {
                            /*Khi công việc đang ở trạng thái “Đang thực hiện”, “ Hoàn thành” người dùng vào cập nhật ngày kết thúc thực tế.
                             * Nếu chọn ngày kết thúc thực tế trong tương lai thì trạng thái vẫn là “ Đang thực hiện” “Hoàn thành”.
                             * Nếu = ngày hiện tại thì trạng thái của công việc sang “Hoàn thành”*/
                            string noteDes = "";
                            if (parameter.Task.ActualEndTime.Value.Date > DateTime.Now.Date)
                            {
                                oldTask.TaskComplate = updateStatus.CategoryCode == "HT" ? 100 : parameter.Task.TaskComplate;
                                if (oldTask.ActualEndTime == null)
                                {
                                    noteDes += "<p>- <strong>" + user.UserName + "</strong>"
                                               + $" cập nhật ngày kết thúc thực tế của công việc thành {parameter.Task.ActualEndTime.Value:dd/MM/yyyy}" +
                                               "</p>";
                                }
                                else if (oldTask.ActualStartTime != null && oldTask.ActualEndTime.Value.Date != parameter.Task.ActualEndTime.Value.Date)
                                {
                                    noteDes += "<p>- <strong>" + user.UserName + "</strong>"
                                               + $" đã thay đổi ngày kết thúc thực tế từ ngày {oldTask.ActualEndTime.Value:dd/MM/yyyy} sang ngày {parameter.Task.ActualEndTime.Value:dd/MM/yyyy}" +
                                               "</p>";
                                }

                                oldTask.ActualStartTime = parameter.Task.ActualStartTime;
                                oldTask.ActualEndTime = parameter.Task.ActualEndTime;
                                oldTask.Status = updateStatus.CategoryId;
                            }
                            else
                            {
                                var status = listStatus.FirstOrDefault(c => c.CategoryCode == "HT");
                                oldTask.Status = status.CategoryId;
                                oldTask.CompleteDate = DateTime.Now;
                                if (oldTask.ActualEndTime == null)
                                {
                                    noteDes += "<p>- <strong>" + user.UserName + "</strong>"
                                               + $" đã cập nhật ngày kết thúc thực tế thành {parameter.Task.ActualEndTime.Value:dd/MM/yyyy}" +
                                               "</p>";
                                }
                                else if (oldTask.ActualStartTime != null && oldTask.ActualEndTime.Value.Date != parameter.Task.ActualEndTime.Value.Date)
                                {
                                    noteDes += "<p>- <strong>" + user.UserName + "</strong>"
                                               + $" đã thay đổi ngày kết thúc thực tê từ ngày : {oldTask.ActualEndTime.Value:dd/MM/yyyy} sang ngày {parameter.Task.ActualEndTime.Value:dd/MM/yyyy}. Trạng thái công việc đã thay đổi sang {status.CategoryName}" +
                                               "</p>";
                                }
                                oldTask.ActualStartTime = parameter.Task.ActualStartTime ?? DateTime.Now;
                                oldTask.ActualEndTime = parameter.Task.ActualEndTime;
                                oldTask.TaskComplate = 100;
                            }

                            if (noteDes != null && noteDes != string.Empty)
                            {
                                note.Description = noteDes;
                                context.Note.Add(note);
                            }
                        }
                        else if (parameter.Task.ActualStartTime != null && presentStatus.CategoryCode == "NEW" && parameter.Task.ActualStartTime == null)
                        {
                            // Khi trạng thái của công việc đang là “Mới” mà người dùng vào cập nhật ngày bắt đầu thực tế thì tùy vào ngày thực tế vừa cập nhật là trong tương lai, hiện tại hay quá khứ.
                            // Nếu chọn ngày thực tế trong tương lai thì trạng thái vẫn là Mới
                            // Nếu <= today thì chuyển trạng thái của công việc sang Đang thực hiện.
                            if (parameter.Task.ActualStartTime.Value.Date <= DateTime.Now.Date)
                            {
                                var status = listStatus.FirstOrDefault(c => c.CategoryCode == "DL");
                                oldTask.Status = status.CategoryId;
                                oldTask.ActualStartTime = parameter.Task.ActualStartTime;

                                note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                                   + $" cập nhật ngày bắt đầu thực tế của công việc thành {parameter.Task.ActualStartTime.Value:dd/MM/yyyy}.";
                                //  $" Trạng thái công việc đổi sang {status.CategoryName}." + "</p>";
                                oldTask.TaskComplate = parameter.Task.TaskComplate;
                            }
                            else
                            {
                                if (oldTask.ActualStartTime == null || (oldTask.ActualStartTime != null && oldTask.ActualStartTime.Value.Date != parameter.Task.ActualStartTime.Value.Date))
                                {
                                    if (oldTask.ActualStartTime == null)
                                    {
                                        note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                                           + $" thay đổi ngày bắt đầu thực tế công việc sang ngày {parameter.Task.ActualStartTime.Value:dd/MM/yyyy}" +
                                                           "</p>";

                                    }
                                    else
                                        note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                                           + $" thay đổi ngày bắt đầu thực tế công việc từ ngày {oldTask.ActualStartTime.Value:dd/MM/yyyy} sang ngày {parameter.Task.ActualStartTime.Value:dd/MM/yyyy}" +
                                                           "</p>";

                                    context.Note.Add(note);
                                }
                                oldTask.ActualStartTime = parameter.Task.ActualStartTime;
                                oldTask.TaskComplate = parameter.Task.TaskComplate;
                            }

                            if (note.Description != null && note.Description != string.Empty)
                            {
                                context.Note.Add(note);
                            }
                        }
                        else if (oldTask.Status != parameter.Task.Status)
                        {
                            switch (updateStatus.CategoryCode)
                            {
                                case "NEW":
                                    break;
                                case "DL":
                                    oldTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                        ? parameter.Task.ActualStartTime
                                        : DateTime.Now;
                                    oldTask.Status = parameter.Task.Status;
                                    oldTask.TaskComplate = parameter.Task.TaskComplate;
                                    note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                    + $" đã thay đổi trạng thái công việc sang {updateStatus.CategoryName}" + "</p>";
                                    context.Note.Add(note);
                                    break;
                                case "HT":
                                    oldTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                        ? parameter.Task.ActualStartTime
                                        : DateTime.Now;
                                    oldTask.ActualEndTime = parameter.Task.ActualEndTime != null
                                        ? parameter.Task.ActualEndTime
                                        : DateTime.Now;
                                    oldTask.Status = parameter.Task.Status;
                                    oldTask.CompleteDate = DateTime.Now;
                                    oldTask.TaskComplate = 100;
                                    note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                    + $" đã thay đổi trạng thái công việc sang {updateStatus.CategoryName}" + "</p>";
                                    context.Note.Add(note);
                                    break;
                                case "CLOSE":
                                    oldTask.Status = parameter.Task.Status;
                                    oldTask.ActualStartTime = parameter.Task.ActualStartTime != null
                                        ? parameter.Task.ActualStartTime
                                        : DateTime.Now;
                                    oldTask.ActualEndTime = parameter.Task.ActualEndTime != null
                                        ? parameter.Task.ActualEndTime
                                        : DateTime.Now;

                                    // NgocTB edit % hoàn thành là 100%
                                    // Đổi lại % hoàn thành ăn theo % hoàn thành người dùng đặt - 06/05/22 - longHDH
                                    //oldTask.TaskComplate = parameter.Task.TaskComplate;
                                    oldTask.TaskComplate = parameter.Task.TaskComplate;
                                    note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                                       + $" đã thay đổi trạng thái công việc sang {updateStatus.CategoryName}" +
                                                       "</p>";
                                    context.Note.Add(note);
                                    break;
                            }
                        }
                        else
                        {
                            if (parameter.Task.TaskComplate == 100 && updateStatus.CategoryCode != "CLOSE" && 
                                updateStatus.CategoryCode != "HT")
                            {
                                var status = listStatus.FirstOrDefault(c => c.CategoryCode == "HT");
                                oldTask.Status = status.CategoryId;
                                oldTask.CompleteDate = DateTime.Now;
                                note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                    + $" đã thay đổi trạng thái công việc sang {status.CategoryName}" + "</p>";
                                context.Note.Add(note);
                            }
                            if (parameter.Task.TaskComplate != oldTask.TaskComplate)
                            {
                                string noteDes = "";
                                noteDes += "<p>- <strong>" + user.UserName +
                                           "</strong> đã thay đổi % hoàn thành công việc " + oldTask.TaskCode
                                           + " từ <strong>" + oldTask.TaskComplate + "</strong>" + "% sang "
                                           + "<strong>" + parameter.Task.TaskComplate + "%</strong></p>";
                                note.Description = noteDes;
                                context.Note.Add(note);
                            }
                            oldTask.TaskComplate = parameter.Task.TaskComplate;
                        }
                        #endregion

                        string noteDescription = "";
                        Note nt = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "TAS",
                            ObjectId = oldTask.TaskId,
                            Type = "ADD",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "Đã thêm ghi chú"
                        };
                        if (oldTask.PlanStartTime.Value.Date != parameter.Task.PlanStartTime.Value.Date)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" +
                                               $" thay đổi ngày bắt đầu dự kiến từ {oldTask.PlanStartTime.Value:dd/MM/yyyy} sang ngày {parameter.Task.PlanStartTime.Value:dd/MM/yyyy}" +
                                               "</p>";
                            if (oldTask.PlanEndTime.Value.Date == parameter.Task.PlanEndTime.Value.Date)
                            {
                                nt.Description = noteDescription;
                                context.Note.Add(nt);
                            }
                        }

                        if (oldTask.PlanEndTime.Value.Date != parameter.Task.PlanEndTime.Value.Date)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" +
                                               $" thay đổi ngày kết thúc dự kiến từ ngày {oldTask.PlanEndTime.Value:dd/MM/yyyy} sang ngày {parameter.Task.PlanEndTime.Value:dd/MM/yyyy}" +
                                               "</p>";
                            nt.Description = noteDescription;
                            context.Note.Add(nt);
                        }

                       

                        oldTask.TaskName = parameter.Task.TaskName;
                        oldTask.ProjectScopeId = parameter.Task.ProjectScopeId;
                        oldTask.PlanStartTime = parameter.Task.PlanStartTime;
                        oldTask.PlanEndTime = parameter.Task.PlanEndTime;
                        oldTask.EstimateHour = parameter.Task.EstimateHour;
                        oldTask.EstimateCost = parameter.Task.EstimateCost;
                        oldTask.ActualHour = parameter.Task.ActualHour;
                        oldTask.Description = parameter.Task.Description;
                        oldTask.Priority = parameter.Task.Priority;
                        oldTask.MilestonesId = parameter.Task.MilestonesId;
                        oldTask.IncludeWeekend = parameter.Task.IncludeWeekend;
                        oldTask.IsSendMail = parameter.Task.IsSendMail;
                        oldTask.TaskTypeId = parameter.Task.TaskTypeId;
                        oldTask.TimeType = parameter.Task.TimeType;
                        oldTask.UpdateBy = parameter.UserId;
                        oldTask.UpdateDate = DateTime.Now;

                        context.Task.Update(oldTask);

                        #region cap nhat ngay bat dau thuc te va trang thai cua du an

                        var today = DateTime.Now;
                        var project = context.Project.FirstOrDefault(x => x.ProjectId == oldTask.ProjectId);
                        var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT")?.CategoryTypeId;
                        var categoryTypeIdTask = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                        var statusMoi =
                            context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "MOI")?.CategoryId;
                        var statusDtk = context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DTK")?.CategoryId;
                        var sttStart = context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryTypeIdTask && x.CategoryCode == "DL")?.CategoryId;

                        if (project != null)
                        {
                            if (project.ProjectStatus == statusMoi)
                            {
                                // update trang thai du an
                                if (oldTask.Status == sttStart)
                                {
                                    project.ProjectStatus = statusDtk;
                                }

                                // update ngay bat dau thuc te
                                if (project.ActualStartDate == null || project.ActualStartDate > today)
                                {
                                    project.ActualStartDate = oldTask.ActualStartTime;
                                }
                                context.Update(project);
                                context.SaveChanges();
                            }
                        }

                        #endregion

                        #region Nguồn lực
                        var deleteListTaskResourceMapping = context.TaskResourceMapping.Where(c => c.TaskId == oldTask.TaskId).ToList();
                        context.TaskResourceMapping.RemoveRange(deleteListTaskResourceMapping);
                        //if (parameter.ListProjectResource?.Count != 0)
                        //{
                        //    var listTaskResource = new List<TaskResourceMapping>();
                        //    parameter.ListProjectResource?.ForEach(item =>
                        //    {
                        //        var taskResourceMapping = new TaskResourceMapping
                        //        {
                        //            TaskResourceMappingId = Guid.NewGuid(),
                        //            TaskId = oldTask.TaskId,
                        //            ResourceId = item.ProjectResourceId,
                        //            Hours = item.Hours,
                        //            IsChecker = item.IsCheck,
                        //            IsPersonInCharge = item.IsPic
                        //        };
                        //        listTaskResource.Add(taskResourceMapping);
                        //    });

                        //    context.TaskResourceMapping.AddRange(listTaskResource);
                        //};
                        if (parameter.ListProjectResource != null)
                        {
                            if (parameter.ListProjectResource.Count > 0)
                            {
                                //Kiểm tra xem nhân sự được giao việc đã được Allocate trong khoảng thời gian "Bắt đầu" và "Kết thúc" dự kiến của công việc hay chưa?
                                //Nếu chưa thì trả về thông báo: Nguồn lực sử dụng cho công việc chưa được phân bổ trong khoảng thời gian dự kiến.

                                //Lấy thời gian người phụ trách được phân bổ trong dự án
                                var listAllPhanBoProject = context.ProjectResource.Where(x => x.ProjectId == parameter.Task.ProjectId).ToList();

                                var listTaskResource = new List<TaskResourceMapping>();
                                var isTrongThoiGianPhanBo = false;
                                var listStatusPhanBo = new List<bool?>();
                                parameter.ListProjectResource?.ForEach(item =>
                                {
                                    isTrongThoiGianPhanBo = false;
                                    var nguoiLucInFor = listAllPhanBoProject.FirstOrDefault(x => x.ProjectResourceId == item.ProjectResourceId);
                                    if (nguoiLucInFor != null)
                                    {
                                        //Nếu tìm thấy nguồn lực => lấy toàn bộ các khoảng thời gian nguồn lực được phân bổ
                                        var listThoiGianPhanBo = listAllPhanBoProject.Where(x => x.ObjectId == nguoiLucInFor.ObjectId).ToList();
                                        if (listThoiGianPhanBo != null)
                                        {
                                            listThoiGianPhanBo.ForEach(nguonLuc =>
                                            {
                                                //Thời gian của công việc phải trong thời gian phân bổ
                                                if ((nguonLuc.StartTime.Value.Date <= parameter.Task.PlanStartTime.Value.Date) &&
                                                    (parameter.Task.PlanEndTime.Value.Date <= nguonLuc.EndTime.Value.Date))
                                                {
                                                    isTrongThoiGianPhanBo = true;
                                                }
                                            });
                                        }
                                        var taskResourceMapping = new TaskResourceMapping
                                        {
                                            TaskResourceMappingId = Guid.NewGuid(),
                                            TaskId = oldTask.TaskId,
                                            ResourceId = item.ProjectResourceId,
                                            Hours = item.Hours,
                                            IsChecker = item.IsCheck,
                                            IsPersonInCharge = item.IsPic
                                        };
                                        listTaskResource.Add(taskResourceMapping);
                                    }
                                    else
                                    {
                                        //Nguồn lực đang k tìm thấy trên hệ thống
                                    }
                                    listStatusPhanBo.Add(isTrongThoiGianPhanBo);
                                });
                                //Kiểm tra xem list nhân viên được phân công cho cviec có ai năm ngoài thời gian phân bổ k
                                var ngoaiThoiGianPhanBo = listStatusPhanBo.FirstOrDefault(x => x == false);
                                if (ngoaiThoiGianPhanBo != null)//Có người ít nhất 1 người năm ngoài thời gian phân bổ => trả về thông báo
                                {
                                    return new CreateOrUpdateTaskResult()
                                    {
                                        StatusCode = HttpStatusCode.ExpectationFailed,
                                        MessageCode = "Nguồn lực sử dụng cho công việc chưa được phân bổ trong khoảng thời gian dự kiến."
                                    };
                                }
                                context.TaskResourceMapping.AddRange(listTaskResource);
                            }
                        };
                        #endregion

                        #region Tài liệu
                        //var deleteListTaskDocument = context.TaskDocument.Where(c => c.TaskId == oldTask.TaskId).ToList();
                        //context.TaskDocument.RemoveRange(deleteListTaskDocument);

                        if (parameter.ListTaskDocument?.Count > 0)
                        {

                            var isSave = true;
                            parameter.ListTaskDocument?.ForEach(item =>
                            {
                                if (folder == null)
                                {
                                    isSave = false;
                                }

                                var folderName = ConvertFolderUrl(folder.Url);
                                var webRootPath = _hostingEnvironment.WebRootPath;
                                var newPath = Path.Combine(webRootPath, folderName);

                                if (!Directory.Exists(newPath))
                                {
                                    isSave = false;
                                }

                                if (isSave)
                                {
                                    var file = new FileInFolder()
                                    {
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        UpdatedById = parameter.UserId,
                                        UpdatedDate = DateTime.Now,
                                        FileInFolderId = Guid.NewGuid(),
                                        FileName = $"{item.FileInFolder.FileName}_{Guid.NewGuid()}",
                                        FolderId = folder.FolderId,
                                        ObjectId = oldTask.TaskId,
                                        ObjectType = item.FileInFolder.ObjectType,
                                        Size = item.FileInFolder.Size,
                                        FileExtension = item.FileSave.FileName.Substring(item.FileSave.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1),
                                    };

                                    context.FileInFolder.Add(file);

                                    var fileName = $"{file.FileName}.{file.FileExtension}";

                                    if (!isSave) return;
                                    var fullPath = Path.Combine(newPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        item.FileSave.CopyTo(stream);
                                        listFileDelete.Add(fullPath);
                                    }
                                }
                            });

                            if (!isSave)
                            {
                                listFileDelete.ForEach(File.Delete);

                                return new CreateOrUpdateTaskResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Bạn phải cấu hình thư mục để lưu"
                                };
                            }

                            // var projectCodeName = $"{project.ProjectCode} - {project.ProjectName}";
                            // var listTaskDocument = new List<TaskDocument>();
                            // string folderName = "FileUpload";
                            // string webRootPath = _hostingEnvironment.WebRootPath;
                            // string newPath = Path.Combine(webRootPath, folderName);
                            // newPath = Path.Combine(newPath, "Project");
                            // newPath = Path.Combine(newPath, projectCodeName);
                            // newPath = Path.Combine(newPath, "Task");
                            //
                            // parameter.ListTaskDocument.ForEach(item =>
                            // {
                            //     item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            //
                            //     var taskDocument = new TaskDocument
                            //     {
                            //         TaskDocumentId = Guid.NewGuid(),
                            //         TaskId = oldTask.TaskId,
                            //         Active = true,
                            //         DocumentName = item.DocumentName,
                            //         DocumentSize = item.DocumentSize,
                            //         DocumentUrl = item.DocumentUrl,
                            //         CreatedById = item.CreatedById,
                            //         CreatedDate = DateTime.Now,
                            //         UpdatedById = null,
                            //         UpdatedDate = null
                            //     };
                            //     listTaskDocument.Add(taskDocument);
                            // });
                            // context.TaskDocument.AddRange(listTaskDocument);
                        }
                        #endregion

                        #region Xoa tai lieu

                        if (parameter.ListTaskDocumentDelete?.Count > 0)
                        {
                            parameter.ListTaskDocumentDelete.ForEach(item =>
                            {
                                if (File.Exists(item.DocumentUrl))
                                {
                                    File.Delete(item.DocumentUrl);
                                }


                                var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == item.TaskDocumentId && x.ObjectId == item.TaskId);

                                context.FileInFolder.Remove(file);
                                context.SaveChanges();
                            });
                        }

                        #endregion

                        // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                        if (project != null)
                        {
                            project.LastChangeActivityDate = DateTime.Now;
                            context.Project.Update(project);
                        }
                        context.SaveChanges();
                        transaction.Commit();

                        if (oldTask.IsSendMail == false)
                        {
                            // Send Mail
                            #region Thông báo

                            NotificationHelper.AccessNotification(context, TypeModel.ProjectTaskDetail, "UPD", new Task(),
                                oldTask, true);
                            #endregion
                        }

                        //Lưu công việc liên quan của task
                        if (parameter.ListTaskRelate != null)
                        {
                            if(parameter.ListTaskRelate.Count > 0)
                            {
                                var listOldRelateTask = context.RelateTaskMapping.Where(x => x.TaskId == parameter.Task.TaskId).ToList();
                                var listRelateTask = new List<RelateTaskMapping>();
                                parameter.ListTaskRelate.ForEach(item =>
                                {
                                    var mapping = new RelateTaskMapping();
                                    mapping.RelateTaskMappingId = Guid.NewGuid();
                                    mapping.RelateTaskId = item.RelateTaskId;
                                    mapping.TaskId = parameter.Task.TaskId;
                                    mapping.ProjectId = item.ProjectId;
                                    mapping.CreatedDate = DateTime.Now;
                                    mapping.CreatedById = parameter.UserId;
                                    listRelateTask.Add(mapping);
                                });
                                context.RelateTaskMapping.RemoveRange(listOldRelateTask);
                                context.RelateTaskMapping.AddRange(listRelateTask);
                                context.SaveChanges();
                            }
                        }
                        if(parameter.ListTaskRelate == null || parameter.ListTaskRelate.Count == 0)
                        {
                            var listOldRelateTask = context.RelateTaskMapping.Where(x => x.TaskId == parameter.Task.TaskId).ToList();
                            context.RelateTaskMapping.RemoveRange(listOldRelateTask);
                            context.SaveChanges();
                        }
                    }
                }

                return new CreateOrUpdateTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public string ConvertFolderUrl(string url)
        {
            var stringResult = url.Split(@"\");
            string result = "";
            for (int i = 0; i < stringResult.Length; i++)
            {
                result = result + stringResult[i] + "\\";
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }

        public GetMasterDataCreateOrUpdateTaskResult GetMasterDataCreateOrUpdateTask(GetMasterDataCreateOrUpdateTaskParameter parameter)
        {
            try
            {
                int totalRecordsNote = 0;
                int pageSize = 10;
                int pageIndex = 1;
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCreateOrUpdateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataCreateOrUpdateTaskResult
                    {
                        MessageCode = "Nhân viên không tốn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var commonListEmployee = context.Employee
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        Active = m.Active
                    }).ToList();

                var commonListCategory = context.Category.Where(c => c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();

                #region Project
                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    TaskComplate = 20,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();
                if (project == null)
                {
                    return new GetMasterDataCreateOrUpdateTaskResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                project.ProjectTypeName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }
                #endregion

                #region Mốc dự án
                var listMileStones = context.ProjectMilestone.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new ProjectMilestoneEntityModel
                    {
                        ProjectMilestonesId = m.ProjectMilestonesId,
                        Description = m.Description,
                        Name = m.Name,
                        ProjectId = m.ProjectId,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,
                        Status = m.Status
                    }).ToList();
                #endregion

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = commonListCategory.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = commonListCategory.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                #region Nguồn lực
                var typeRoleResource = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NCNL")?.CategoryTypeId;
                var resourceNoiBoId = commonListCategory.FirstOrDefault(c => c.CategoryTypeId == typeRoleResource && c.CategoryCode == "NB")?.CategoryId;

                var typeResourceId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LNL")?.CategoryTypeId;
                var resourceNhanLucId = commonListCategory.FirstOrDefault(c => c.CategoryTypeId == typeResourceId && c.CategoryCode == "NLC")?.CategoryId;

                var typeVaiTroNguonLucId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "VTN")?.CategoryTypeId;
                var listVaiTroNguonLuc = commonListCategory.Where(c => c.CategoryTypeId == typeVaiTroNguonLucId).ToList();

                var listResource = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && (c.ResourceRole == resourceNoiBoId || c.ResourceType == resourceNhanLucId))
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ProjectId = m.ProjectId,
                        ResourceRole = m.ResourceRole,
                        EmployeeRole = m.EmployeeRole,
                        ObjectId = m.ObjectId,
                        NameResource = string.Empty,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,
                    }).ToList();
                var categoryCode = "NLC";
                listResource.ForEach(item =>
                {
                    if (item.ObjectId != Guid.Empty)
                    {
                        var emp = commonListEmployee.FirstOrDefault(c => c.EmployeeId == item.ObjectId);
                        item.NameResource = $"{emp?.EmployeeCode} - {emp?.EmployeeName}";
                        item.IsActive = emp?.Active;
                    }
                    item.EmployeeRoleName = listVaiTroNguonLuc.FirstOrDefault(c => c.CategoryId == item.EmployeeRole)?.CategoryName ?? string.Empty;

                    var categoryId = commonListCategory.FirstOrDefault(x => x.CategoryCode == categoryCode).CategoryId;
                    if (item.ResourceType == categoryId)
                    {
                        item.IsActive = commonListEmployee.FirstOrDefault(x => x.EmployeeId == item.ObjectId).Active;
                    }
                });

                #endregion

                var task = new TaskEntityModel();
                var scope = new ProjectScopeModel();
                var listNote = new List<NoteEntityModel>();
                var listTaskDocument = new List<TaskDocumentEntityModel>();
                var listTaskConstraintBefore = new List<TaskConstraintEntityModel>();
                var listTaskConstraintAfter = new List<TaskConstraintEntityModel>();

                #region Chi tiết của 1 task
                var projectScopes = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectId).ToList();
                var listProjectScope = projectScopes.Select(p => new ProjectScopeModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    ProjectScopeName = p.ProjectScopeName,
                }).ToList();

                if (parameter.TaskId != null && parameter.TaskId != Guid.Empty)
                {

                    task = context.Task.Where(c => c.TaskId == parameter.TaskId)
                        .Select(m => new TaskEntityModel
                        {
                            TaskId = m.TaskId,
                            TaskCode = m.TaskCode,
                            TaskName = m.TaskName,
                            MilestonesId = m.MilestonesId,
                            PlanStartTime = m.PlanStartTime,
                            PlanEndTime = m.PlanEndTime,
                            EstimateHour = m.EstimateHour,
                            ActualEndTime = m.ActualEndTime,
                            ActualStartTime = m.ActualStartTime,
                            ActualHour = 0,
                            Description = m.Description,
                            Status = m.Status,
                            Priority = m.Priority,
                            IncludeWeekend = m.IncludeWeekend,
                            IsSendMail = m.IsSendMail,
                            ProjectId = m.ProjectId,
                            ProjectScopeId = m.ProjectScopeId,
                            EstimateCost = m.EstimateCost,
                            TaskTypeId = m.TaskTypeId,
                            TaskComplate = m.TaskComplate,
                            TimeType = m.TimeType,
                            CreateBy = m.CreateBy,
                            SoLanMoLai = m.SoLanMoLai,
                            ListTaskResource = new List<TaskResourceMappingEntityModel>(),
                            ProjectScopeName = listProjectScope.FirstOrDefault(s => s.ProjectScopeId == m.ProjectScopeId) != null ? listProjectScope.FirstOrDefault(s => s.ProjectScopeId == m.ProjectScopeId).ProjectScopeCode + ". " + listProjectScope.FirstOrDefault(s => s.ProjectScopeId == m.ProjectScopeId).ProjectScopeName : "",
                        }).FirstOrDefault();

                    task.StatusCode = context.Category.FirstOrDefault(x => x.CategoryId == task.Status)?.CategoryCode;
                    task.StatusName = context.Category.FirstOrDefault(x => x.CategoryId == task.Status)?.CategoryName;


                    var listTimeSheetOfTask = context.TimeSheet.Where(c => c.TaskId == task.TaskId);

                    #region Phân quyền để lấy thời gian thực tế
                    var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                    if (position.PositionCode == "GD")
                    {
                        //Lấy tất cả thời gian thực tế(khai time sheet)
                        task.ActualHour = listTimeSheetOfTask.Sum(c => c.SpentHour);
                    }
                    else
                    {
                        var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                        if (projectMananger != null || project.ProjectManagerId == employee.EmployeeId)
                        {
                            // Là quản lý đọc : Lấy tất cả thời gian thực tế(khai time sheet)
                            task.ActualHour = listTimeSheetOfTask.Sum(c => c.SpentHour);
                        }
                        else
                        {
                            var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                            if (subPm == null)
                            {
                                // Là nhân viên chỉ lấy những timesheet mà bản thân khai báo
                                task.ActualHour = listTimeSheetOfTask.Where(c => c.PersonInChargeId == employee.EmployeeId).Sum(c => c.SpentHour);
                            }
                            else
                            {
                                //Là subPM : Lấy tất cả thời gian thực tế(khai time sheet)
                                task.ActualHour = listTimeSheetOfTask.Sum(c => c.SpentHour);
                            }
                        }
                    }
                    #endregion

                    scope = context.ProjectScope.Where(c => c.ProjectScopeId == task.ProjectScopeId)
                       .Select(m => new ProjectScopeModel
                       {
                           ProjectScopeId = m.ProjectScopeId,
                           ProjectId = m.ProjectId,
                           ProjectScopeCode = m.ProjectScopeCode,
                           ProjectScopeName = m.ProjectScopeName,
                           ParentId = m.ParentId
                       }).FirstOrDefault();

                    task.ListTaskResource = context.TaskResourceMapping.Where(c => c.TaskId == task.TaskId)
                        .Select(m => new TaskResourceMappingEntityModel
                        {
                            TaskResourceMappingId = m.TaskResourceMappingId,
                            TaskId = m.TaskId,
                            ResourceId = m.ResourceId,
                            Hours = m.Hours,
                            IsChecker = m.IsChecker,
                            IsPersonInCharge = m.IsPersonInCharge
                        }).ToList();

                    #region get list notes
                    listNote = context.Note.Where(w => w.Active == true && w.ObjectId == parameter.TaskId).Select(w => new NoteEntityModel
                    {
                        NoteId = w.NoteId,
                        Description = w.Description,
                        Type = w.Type,
                        ObjectId = w.ObjectId,
                        ObjectType = w.ObjectType,
                        NoteTitle = w.NoteTitle,
                        CreatedById = w.CreatedById,
                        CreatedDate = w.CreatedDate,
                        UpdatedById = w.UpdatedById,
                        UpdatedDate = w.UpdatedDate,
                        NoteDocList = context.NoteDocument.Where(ws => ws.NoteId == w.NoteId && ws.Active == true).Select(s => new NoteDocumentEntityModel
                        {
                            NoteDocumentId = s.NoteDocumentId,
                            NoteId = s.NoteId,
                            DocumentName = s.DocumentName,
                            DocumentSize = s.DocumentSize,
                            DocumentUrl = s.DocumentUrl,
                        }).ToList() ?? new List<NoteDocumentEntityModel>()
                    }).OrderByDescending(c => c.CreatedDate).ToList();

                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    //lấy tên người tạo, người chỉnh sửa cho note
                    listNote.ForEach(note =>
                    {
                        var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                        var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                        if (contact != null)
                        {
                            note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                        }
                    });

                    totalRecordsNote = listNote.Count;

                    listNote = listNote
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize).ToList();
                    #endregion

                    var listUser = context.User.ToList();

                    #region Tài liệu

                    // longhdh comment
                    // listTaskDocument = context.TaskDocument.Where(c => c.TaskId == task.TaskId)
                    //     .Select(m => new TaskDocumentEntityModel
                    //     {
                    //         TaskDocumentId = m.TaskDocumentId,
                    //         TaskId = m.TaskId,
                    //         DocumentName = m.DocumentName,
                    //         DocumentSize = m.DocumentSize,
                    //         DocumentUrl = m.DocumentUrl,
                    //         Active = m.Active,
                    //         CreatedById = m.CreatedById,
                    //         CreatedDate = m.CreatedDate,
                    //         CreateByName = string.Empty,
                    //     }).ToList();
                    //
                    // listTaskDocument.ForEach(item =>
                    // {
                    //     var tempU = listUser.FirstOrDefault(c => c.UserId == item.CreatedById);
                    //     var employee = commonListEmployee.FirstOrDefault(c => c.EmployeeId == tempU?.EmployeeId);
                    //     item.CreateByName = $"{employee?.EmployeeCode ?? string.Empty} - {employee?.EmployeeName ?? string.Empty}";
                    // });

                    var folderType = $"{project.ProjectCode}_TASK_FILE";
                    var folderUrl = context.Folder.FirstOrDefault(x => x.FolderType == folderType)?.Url;
                    var listFile = context.FileInFolder.Where(x => x.ObjectId == task.TaskId && x.ObjectType == "TASK").OrderByDescending(x => x.CreatedDate).ToList();

                    listFile.ForEach(item =>
                    {
                        var file = new TaskDocumentEntityModel
                        {
                            TaskDocumentId = item.FileInFolderId,
                            TaskId = (Guid)item.ObjectId,
                            DocumentName = item.FileName,
                            DocumentSize = item.Size,
                            Active = item.Active,
                            CreatedById = item.CreatedById,
                            CreatedDate = item.CreatedDate,
                            CreateByName = string.Empty,
                        };

                        var fileName = $"{item.FileName}.{item.FileExtension}";
                        var folderName = ConvertFolderUrl(folderUrl);
                        var webRootPath = _hostingEnvironment.WebRootPath;
                        file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                        listTaskDocument.Add(file);
                    });

                    listTaskDocument.ForEach(item =>
                    {
                        var tempU = listUser.FirstOrDefault(c => c.UserId == item.CreatedById);
                        var emp = commonListEmployee.FirstOrDefault(c => c.EmployeeId == tempU?.EmployeeId);
                        item.CreateByName = $"{emp?.EmployeeCode ?? string.Empty} - {emp?.EmployeeName ?? string.Empty}";
                    });
                    #endregion

                    #region Ràng buộc
                    var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId && c.TaskId != parameter.TaskId)
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        TaskCode = m.TaskCode,
                        TaskName = m.TaskName,
                        ParentId = m.ParentId,
                        PlanStartTime = m.PlanStartTime,
                        PlanEndTime = m.PlanEndTime,
                        Status = m.Status
                    }).ToList();

                    listAllTask.ForEach(item =>
                    {
                        var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                        item.StatusName = status?.CategoryName;
                        switch (status.CategoryCode)
                        {
                            case "NEW":
                                item.BackgroundColorForStatus = "#0F62FE";
                                item.IsDelete = true;
                                break;
                            case "DL":
                                item.BackgroundColorForStatus = "#FFC000";
                                item.IsDelete = false;
                                break;
                            case "HT":
                                item.BackgroundColorForStatus = "#63B646";
                                item.IsDelete = false;
                                break;
                            case "CLOSE":
                                item.BackgroundColorForStatus = "#9C00FF";
                                item.IsDelete = false;
                                break;
                        }
                    });

                    var listAllTaskContraint = context.TaskConstraint.Where(c => c.ProjectId == parameter.ProjectId)
                        .Select(m => new TaskConstraintEntityModel
                        {
                            TaskConstraintId = m.TaskConstraintId,
                            TaskId = m.TaskId,
                            ParentId = m.ParentId,
                            DelayTime = m.DelayTime,
                            ConstraintType = m.ConstraintType,
                            ConstraingRequired = m.ConstraingRequired,
                            ProjectId = m.ProjectId
                        }).ToList();

                    // Parent : Là những công việc thực hiện trước
                    var listParent = listAllTaskContraint.Where(c => c.TaskId == task.TaskId).ToList();

                    while (listParent.Count() != 0)
                    {
                        listTaskConstraintBefore.AddRange(listParent);
                        var listParentId = listParent.Select(c => c.ParentId).ToList();
                        listParent = listAllTaskContraint.Where(c => listParentId.Contains(c.TaskId)).ToList();
                    }

                    listTaskConstraintBefore.ForEach(item =>
                    {
                        var t = listAllTask.FirstOrDefault(c => c.TaskId == item.ParentId);
                        item.TaskCode = t.TaskCode;
                        item.TaskName = t.TaskName;
                        item.Status = t.Status;
                        item.StatusName = t.StatusName;
                        item.PlanStartTime = t.PlanStartTime;
                        item.PlanEndTime = t.PlanEndTime;
                        item.BackgroundColorForStatus = t.BackgroundColorForStatus;
                    });

                    // Child : là những công việc thực hiện sau
                    var lstChild = listAllTaskContraint.Where(c => c.ParentId == task.TaskId).ToList();
                    while (lstChild.Count() != 0)
                    {
                        listTaskConstraintAfter.AddRange(lstChild);
                        var listChildId = lstChild.Select(m => m.TaskId).ToList();
                        lstChild = listAllTaskContraint.Where(c => listChildId.Contains(c.ParentId.Value)).ToList();
                    }

                    listTaskConstraintAfter.ForEach(item =>
                    {
                        var t = listAllTask.FirstOrDefault(c => c.TaskId == item.TaskId);
                        item.TaskCode = t.TaskCode;
                        item.TaskName = t.TaskName;
                        item.Status = t.Status;
                        item.StatusName = t.StatusName;
                        item.PlanStartTime = t.PlanStartTime;
                        item.PlanEndTime = t.PlanEndTime;
                        item.BackgroundColorForStatus = t.BackgroundColorForStatus;
                    });
                    #endregion
                }
                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();
                var IsManager = true;
                var IsPresident = false;
                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            IsPresident = true;
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {

                            var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                            if (projectMananger == null && project.ProjectManagerId != employee.EmployeeId)
                            {
                                var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                                // Là nhân viên 
                                if (subPm == null)
                                {
                                    IsManager = false;
                                }
                            }

                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion


                ///lấy danh sách công việc liên quan

                var listAllTaskOfProject = context.Task.Where(x => x.ProjectId == parameter.ProjectId).ToList(); 
                 
                var  listRelateTaskParent = context.RelateTaskMapping.Where(x => x.TaskId == parameter.TaskId)
                    .Select(m => new RelateTaskMappingEntityModel
                    {
                        RelateTaskMappingId = m.RelateTaskMappingId,
                        RelateTaskId = m.RelateTaskId,
                        TaskId = m.TaskId,
                        ProjectId = m.ProjectId.Value,
                        CreatedDate = m.CreatedDate,
                    }).ToList();

                var listRelateTask = context.RelateTaskMapping.Where(x => x.RelateTaskId == parameter.TaskId)
                .Select(m => new RelateTaskMappingEntityModel
                {
                    RelateTaskMappingId = m.RelateTaskMappingId,
                    RelateTaskId = m.RelateTaskId,
                    TaskId = m.TaskId,
                    ProjectId = m.ProjectId.Value,
                    CreatedDate = m.CreatedDate,
                }).ToList();


                listRelateTask.ForEach(item =>
                {
                    var taskInfor = new Task();
                    if (item.TaskId == parameter.TaskId)
                    {
                        taskInfor = listAllTaskOfProject.FirstOrDefault(x => x.TaskId == item.RelateTaskId);
                    }
                    else
                    {
                        taskInfor = listAllTaskOfProject.FirstOrDefault(x => x.TaskId == item.TaskId);
                    }

                    item.TaskName = taskInfor.TaskName;
                    item.TaskCode = taskInfor.TaskCode;
                    item.StatusCode = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryCode;
                    item.StatusName = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryName;
                    item.ExpectedStartDate = taskInfor.PlanStartTime;
                    item.ExpectedEndDate = taskInfor.PlanEndTime;


                    //lấy màu backGround cho trạng thái
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == taskInfor.Status);
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            item.IsDelete = true;
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            item.IsDelete = false;
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            item.IsDelete = false;
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            item.IsDelete = false;
                            break;
                    }

                });

             


                return new GetMasterDataCreateOrUpdateTaskResult
                {
                    ListStatus = listStatus,
                    ListTaskType = listTaskType,
                    ListRelateTask = listRelateTask,
                    ListRelateTaskParent = listRelateTaskParent,
                    ListMilestone = listMileStones,
                    Project = project,
                    ListProjectResource = listResource,
                    ListTaskDocument = listTaskDocument,
                    Task = task,
                    Scope = scope,
                    ListNote = listNote,
                    ListTaskConstraintBefore = listTaskConstraintBefore,
                    ListTaskConstraintAfter = listTaskConstraintAfter,
                    StatusCode = HttpStatusCode.OK,
                    TotalRecordsNote = totalRecordsNote,
                    MessageCode = "Success",
                    listProject = listProject,
                    IsManager = IsManager,
                    IsPresident = IsPresident
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateOrUpdateTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetProjectScopeByProjectIdResult GetProjectScopeByProjectId(GetProjectScopeByProjectIdParameter parameter)
        {
            try
            {
                var projectScope = context.ProjectScope.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (projectScope == null)
                {
                    return new GetProjectScopeByProjectIdResult
                    {
                        MessageCode = "Hạng mục không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listProjectScope = context.ProjectScope.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new ProjectScopeModel
                    {
                        ProjectScopeId = m.ProjectScopeId,
                        ProjectId = m.ProjectId,
                        ProjectScopeCode = m.ProjectScopeCode,
                        ProjectScopeName = m.ProjectScopeName,
                        ParentId = m.ParentId,
                        Level = m.Level
                    }).ToList();

                return new GetProjectScopeByProjectIdResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListProjectScrope = listProjectScope
                };
            }
            catch (Exception ex)
            {
                return new GetProjectScopeByProjectIdResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataTimeSheetResult GetMasterDataTimeSheet(GetMasterDataTimeSheetParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống!",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.Where(c => c.EmployeeId == user.EmployeeId)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName
                    }).FirstOrDefault();

                #region Project
                var project = context.Project.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new ProjectEntityModel
                    {
                        ProjectId = m.ProjectId,
                        ProjectCode = m.ProjectCode,
                        ProjectName = m.ProjectName,
                        ContractId = m.ContractId,
                        ActualStartDate = m.ActualStartDate,
                        ActualEndDate = m.ActualEndDate
                    }).FirstOrDefault();
                #endregion

                #region Kiểu thời gian
                var typeTimeTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "KTG")?.CategoryTypeId;
                var listTimeType = context.Category.Where(c => c.CategoryTypeId == typeTimeTypeId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();
                #endregion

                #region Task
                //var resourceIdOfEmployee = context.ProjectResource.FirstOrDefault(c => c.ProjectId == parameter.ProjectId && c.ObjectId == employee.EmployeeId)?.ProjectResourceId;
                var resourceIdOfEmployee = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && c.ObjectId == employee.EmployeeId)?.Select(a => a.ProjectResourceId).Distinct().ToList();

                var listAllTaskOfProject = context.Task.Where(c => c.ProjectId == parameter.ProjectId).ToList();
                var listAllTaskOfProjectId = listAllTaskOfProject.Select(m => m.TaskId).ToList();

                var listAllTaskOfEmployeeId = context.TaskResourceMapping.Where(c => listAllTaskOfProjectId.Contains(c.TaskId) && resourceIdOfEmployee.Contains(c.ResourceId)).Select(m => m.TaskId).ToList();

                var listAllTaskOfEmployee = listAllTaskOfProject
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        TaskCode = m.TaskCode,
                        TaskName = m.TaskName,
                    }).ToList();

                listAllTaskOfEmployee.ForEach(item =>
                {
                    item.IsCreate = listAllTaskOfEmployeeId.Contains(item.TaskId);
                });
                #endregion

                #region Trạng thái khai báo thời gian
                var typeStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTKTG")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.Active == true && c.CategoryTypeId == typeStatusId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();
                #endregion

                #region Lấy ngày đầu tuần và cuối tuần hiện tại
                var firstOfWeek = DateTime.Now;
                // Lấy thứ 2 là ngày đâu tuần - bắt đầu 1 tuần mới
                while (firstOfWeek.DayOfWeek != DayOfWeek.Monday)
                {
                    firstOfWeek = firstOfWeek.AddDays(-1);
                }
                var lastOfWeek = firstOfWeek.AddDays(6);
                #endregion

                var listAllTimeSheetDetail = context.TimeSheetDetail
                    .Select(m => new TimeSheetDetailEntityModel
                    {
                        TimeSheetDetailId = m.TimeSheetDetailId,
                        TimeSheetId = m.TimeSheetId,
                        Date = m.Date,
                        SpentHour = m.SpentHour,
                        Status = m.Status
                    }).ToList();

                var listAllTimeSheetOfTask = context.TimeSheet.Where(c => listAllTaskOfProjectId.Contains(c.TaskId))
                    .Select(m => new TimeSheetEntityModel
                    {
                        TimeSheetId = m.TimeSheetId,
                        TaskId = m.TaskId,
                        SpentHour = m.SpentHour,
                        FromDate = m.FromDate,
                        ToDate = m.ToDate,
                        Status = m.Status,
                        Note = m.Note,
                        PersonInChargeId = m.PersonInChargeId,
                        TimeType = m.TimeType,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        UpdatedById = m.UpdatedById ?? m.CreatedById,
                        UpdatedDate = m.UpdatedDate ?? m.CreatedDate,
                        ListTimeSheetDetail = new List<TimeSheetDetailEntityModel>(),
                    }).ToList();

                #region get list notes
                var listALLNote = context.Note.Where(w => w.Active == true && w.ObjectType == "TIMESHEET").Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate
                }).OrderByDescending(c => c.CreatedDate).ToList();

                var listAllUser = context.User.Select(m => new { m.UserId, m.EmployeeId, m.UserName }).ToList();
                var listAllContact = context.Contact.ToList();
                //lấy tên người tạo, người chỉnh sửa cho note
                listALLNote.ForEach(note =>
                {
                    var empId = listAllUser.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = listAllContact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });
                #endregion

                listAllTimeSheetOfTask.ForEach(item =>
                {
                    item.ListTimeSheetDetail = listAllTimeSheetDetail.Where(c => c.TimeSheetId == item.TimeSheetId).OrderBy(c => c.Date).ToList();
                    item.ListNote = listALLNote.Where(c => c.ObjectId == item.TimeSheetId).ToList();
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusCode = status?.CategoryCode;

                    item.CreateByName = listAllUser.FirstOrDefault(c => c.UserId == item.CreatedById)?.UserName;
                    item.UpdateByName = listAllUser.FirstOrDefault(c => c.UserId == item.UpdatedById)?.UserName;
                });

                return new GetMasterDataTimeSheetResult
                {
                    ListTimeType = listTimeType,
                    ListTask = listAllTaskOfEmployee,
                    Employee = employee,
                    FromDate = firstOfWeek,
                    ToDate = lastOfWeek,
                    Project = project,
                    ListTimeSheet = listAllTimeSheetOfTask,
                    ListStatus = listStatus,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataTimeSheetResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private string GenerateTaskCode(Guid? projectId)
        {
            var strCode = string.Empty;
            var countTaskOfProject = context.Task.Where(c => c.ProjectId == projectId).Count();
            if (countTaskOfProject == 0)
            {
                strCode = $"#{countTaskOfProject + 1}";
            }
            else
            {
                var taskNumber = context.Task.Where(c => c.ProjectId == projectId).Select(m => Convert.ToInt32(m.TaskCode.Substring(m.TaskCode.IndexOf('#') + 1)))
                    .OrderByDescending(m => m).FirstOrDefault();
                strCode = $"#{taskNumber + 1}";
            }
            return strCode;
        }

        public GetMasterDataSearchTaskResult GetMasterDataSearchTask(GetMasterDataSearchTaskParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataSearchTaskResult
                    {
                        MessageCode = "Người dùng không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.Where(c => c.EmployeeId == user.EmployeeId)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName
                    }).FirstOrDefault();

                var commonListCategory = context.Category.Where(c => c.Active == true)
                  .Select(m => new CategoryEntityModel
                  {
                      CategoryId = m.CategoryId,
                      CategoryCode = m.CategoryCode,
                      CategoryName = m.CategoryName,
                      CategoryTypeId = m.CategoryTypeId
                  }).ToList();

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterDataSearchTaskResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                project.ProjectTypeName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }
                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = commonListCategory.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = commonListCategory.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                #region Nguồn lực
                var typeRoleResource = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NCNL")?.CategoryTypeId;
                var resourceNoiBoId = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeRoleResource && c.Active == true && c.CategoryCode == "NB")?.CategoryId;

                var listObjectId = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && c.ResourceRole == resourceNoiBoId)
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ProjectId = m.ProjectId,
                        ResourceRole = m.ResourceRole,
                        ObjectId = m.ObjectId,
                        NameResource = string.Empty,
                    }).Select(m => m.ObjectId).ToList();

                var listEmployee = context.Employee.Where(c => c.Active == true && listObjectId.Contains(c.EmployeeId))
                   .Select(m => new EmployeeEntityModel
                   {
                       EmployeeId = m.EmployeeId,
                       EmployeeCode = m.EmployeeCode,
                       EmployeeName = m.EmployeeName
                   }).ToList();
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                var contain = listEmployee.FirstOrDefault(c => c.EmployeeId == employee.EmployeeId);
                var isContainResource = contain != null;


                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion


                return new GetMasterDataSearchTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListPersionInCharged = listEmployee,
                    ListStatus = listStatus,
                    ListTaskType = listTaskType,
                    InforExportExcel = inforExportExcel,
                    Project = project,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour,
                    Employee = employee,
                    IsContainResource = isContainResource,
                    listProject = listProject,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchTaskResult SearchTask(SearchTaskParameter parameter)
        {
            try
            {
                var project = context.Project.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (project == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Common data

                var commonListCategory = context.Category.Where(c => c.Active == true)
                .Select(m => new CategoryEntityModel
                {
                    CategoryId = m.CategoryId,
                    CategoryCode = m.CategoryCode,
                    CategoryName = m.CategoryName,
                    CategoryTypeId = m.CategoryTypeId
                }).ToList();

                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                #region Trạng thái công việc

                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = commonListCategory.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();

                #endregion

                #region Loại Công Việc

                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = commonListCategory.Where(c => c.CategoryTypeId == typeTaskId).ToList();

                #endregion

                var listAllEmployee = context.Employee.ToList();


                var listAllTask = context.Task.Select(m => new TaskEntityModel
                {
                    TaskId = m.TaskId,
                    TaskCode = m.TaskCode,
                    TaskName = m.TaskName,
                    MilestonesId = m.MilestonesId,
                    PlanStartTime = m.PlanStartTime,
                    PlanEndTime = m.PlanEndTime,
                    EstimateHour = m.EstimateHour,
                    ActualEndTime = m.ActualEndTime,
                    ActualStartTime = m.ActualStartTime,
                    ActualHour = m.ActualHour,
                    Description = m.Description,
                    Status = m.Status,
                    Priority = m.Priority,
                    IncludeWeekend = m.IncludeWeekend,
                    IsSendMail = m.IsSendMail,
                    TaskComplate = m.TaskComplate,
                    ProjectId = m.ProjectId,
                    ProjectScopeId = m.ProjectScopeId,
                    EstimateCost = m.EstimateCost,
                    TaskTypeId = m.TaskTypeId,
                    TimeType = m.TimeType,
                    PersionInChargedName = string.Empty,
                    CreateDate = m.CreateDate.Date,
                    IsHavePic = false,
                    CreateBy = m.CreateBy,
                    UpdateDate = m.UpdateDate,
                }).ToList();

                #endregion

                #region Phân quyền

                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position.PositionCode == "GD")
                {
                    // Là giám đốc có thể xem được hết dữ liệu
                }
                else
                {
                    var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                    if (projectMananger != null || project.ProjectManagerId == employee.EmployeeId)
                    {
                        // là quản lý có quyền thấy hết
                    }
                    else
                    {
                        var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                        if (subPm == null)
                        {
                            var listReourceId = context.ProjectResource.Where(c => c.ProjectId == project.ProjectId && c.ObjectId == employee.EmployeeId).Select(m => m.ProjectResourceId).ToList();
                            var listTaskId = context.TaskResourceMapping.Where(c => listReourceId.Contains(c.ResourceId)).Select(m => m.TaskId).ToList();
                            listAllTask = listAllTask.Where(c => listTaskId.Contains(c.TaskId) || c.CreateBy == parameter.UserId).ToList();
                        }
                    }
                }
                #endregion

                // Lấy các bản ghi công việc theo nguồn lực
                var listAllTaskMapping = context.TaskResourceMapping.ToList();
                var listResourceOfProject = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId).ToList();

                var listTaskIdFollowResource = new List<Guid>();
                if (parameter.ListEmployeeId.Count != 0)
                {
                    var listResourceId = listResourceOfProject.Where(c => parameter.ListEmployeeId.Contains(c.ObjectId)).Select(m => m.ProjectResourceId).ToList();
                    listTaskIdFollowResource = listAllTaskMapping.Where(c => listResourceId.Contains(c.ResourceId)).Select(m => m.TaskId).ToList();
                }

                var listCreatedId = new List<Guid>();
                if (parameter.ListCreatedId!=null && parameter.ListCreatedId.Count != 0)
                {
                    listCreatedId = context.User.Where(x => parameter.ListCreatedId.Contains(x.EmployeeId))
                        .Select(y => y.UserId).ToList();
                }

                // lấy các bảng ghi công việc theo điều kiện search
                listAllTask = listAllTask.Where(c => c.ProjectId == project.ProjectId &&
                                                    (parameter.ListTaskTypeId == null || parameter.ListTaskTypeId.Count == 0 || parameter.ListTaskTypeId.Contains(c.TaskTypeId)) &&
                                                    (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(c.Status)) &&
                                                    (listTaskIdFollowResource.Count == 0 || listTaskIdFollowResource.Contains(c.TaskId)) &&
                                                    (listCreatedId.Count == 0 || listCreatedId.Contains(c.CreateBy)) &&
                                                    (parameter.ListPriority == null || parameter.ListPriority.Count == 0 || parameter.ListPriority.Contains(c.Priority.Value)) &&
                                                    (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || parameter.FromDate <= c.PlanEndTime) &&
                                                    (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || parameter.ToDate >= c.PlanEndTime)).ToList();

                listAllTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusName = status?.CategoryName;
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            item.IsDelete = true;
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            item.IsDelete = false;
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            item.IsDelete = false;
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            item.IsDelete = false;
                            break;
                    }
                    var taskType = listTaskType.FirstOrDefault(c => c.CategoryId == item.TaskTypeId);
                    item.TaskTypeCode = taskType?.CategoryCode;
                    item.TaskTypeName = taskType?.CategoryName;
                    switch (item.Priority)
                    {
                        case 0:
                            item.PriorityName = "Thấp";
                            break;
                        case 1:
                            item.PriorityName = "Trung bình";
                            break;
                        case 2:
                            item.PriorityName = "Cao";
                            break;
                        default: break;
                    }

                    var listResourceId = listAllTaskMapping.Where(c => c.TaskId == item.TaskId && c.IsPersonInCharge == true).Select(m => m.ResourceId).ToList();
                    
                    // Có người phụ trách
                    if (listResourceId.Count != 0)
                    {
                        item.IsHavePic = true;
                        var listObjectId = listResourceOfProject.Where(c => listResourceId.Contains(c.ProjectResourceId)).Select(m => m.ObjectId).ToList();
                        var listEmployeeName = listAllEmployee.Where(c => listObjectId.Contains(c.EmployeeId)).Select(m => m.EmployeeName).ToList();
                        item.PersionInChargedName = string.Join(", ", listEmployeeName);
                    }
                    // Không có người phụ trách
                    else
                    {
                        item.IsHavePic = false;
                    }

                    if (item.PlanEndTime != null && item.PlanEndTime.Value.Date > DateTime.Now.Date)
                    {
                        item.PlanEndTimeStr = item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorPlanEndTimeStr = "#333333";
                    }
                    else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date < DateTime.Now.Date)
                    {
                        var number = DateTime.Now.Date.Subtract(item.PlanEndTime.Value.Date).TotalDays;
                        item.PlanEndTimeStr = number == 1 ? "Hôm qua" : item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorPlanEndTimeStr = "#fd1b1b";
                    }
                    else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date == DateTime.Now.Date)
                    {
                        item.PlanEndTimeStr = "Hôm nay";
                        item.ColorPlanEndTimeStr = "#FD1B3E";
                    }
                    

                    // Ngày kết thúc thực tế
                    if (item.ActualEndTime != null && item.ActualEndTime.Value.Date > DateTime.Now.Date)
                    {
                        item.ActualEndTimeStr = item.ActualEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorActualEndTimeStr = "#333333";
                    }
                    else if (item.ActualEndTime != null && item.ActualEndTime.Value.Date < DateTime.Now.Date)
                    {
                        var number = DateTime.Now.Date.Subtract(item.ActualEndTime.Value.Date).TotalDays;
                        item.ActualEndTimeStr = number == 1 ? "Hôm qua" : item.ActualEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorActualEndTimeStr = "#fd1b1b";
                    }
                    else if (item.ActualEndTime != null && item.ActualEndTime.Value.Date == DateTime.Now.Date)
                    {
                        item.ActualEndTimeStr = "Hôm nay";
                        item.ColorActualEndTimeStr = "#FD1B3E";
                    }

                    if ((item.PlanEndTime != null && item.ActualEndTime != null &&
                         item.ActualEndTime.Value.Date > item.PlanEndTime.Value.Date) ||
                        ((status?.CategoryCode == "NEW" || status?.CategoryCode == "DL") && item.PlanEndTime != null &&
                         DateTime.Now.Date > item.PlanEndTime.Value.Date))
                    {
                        item.ColorPlanEndTimeStr = "#FD1B3E";
                        item.ColorActualEndTimeStr = "#fd1b1b";
                    }
                    else
                    {
                        item.ColorPlanEndTimeStr = "#333333";
                        item.ColorActualEndTimeStr = "#333333";
                    }

                    if (item.CreateDate > DateTime.Now.Date || (item.UpdateDate != null && ((DateTime)item.UpdateDate).Date > DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(item.UpdateDate == null ? item.CreateDate : ((DateTime)item.UpdateDate).Date);
                        item.CreateDateStr = $"{dayOfWeek} - {(item.UpdateDate == null ? item.CreateDate : ((DateTime)item.UpdateDate).Date):dd/MM/yyyy}";
                    }
                    else if (item.CreateDate < DateTime.Now.Date || (item.UpdateDate != null && ((DateTime)item.UpdateDate).Date < DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(item.UpdateDate == null ? item.CreateDate : ((DateTime)item.UpdateDate).Date);
                        item.CreateDateStr = $"{dayOfWeek} - {(item.UpdateDate == null ? item.CreateDate : ((DateTime)item.UpdateDate).Date):dd/MM/yyyy}";
                    }
                    else if (item.CreateDate == DateTime.Now.Date || (item.UpdateDate != null && ((DateTime)item.UpdateDate).Date == DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(item.UpdateDate == null ? item.CreateDate : ((DateTime)item.UpdateDate).Date);
                        item.CreateDateStr = $"{dayOfWeek} - Hôm nay";
                    }

                    if ((item.UpdateDate != null && ((DateTime)item.UpdateDate).Date > DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(((DateTime)item.UpdateDate).Date);
                        item.UpdateDateStr = $"{dayOfWeek} - {(((DateTime)item.UpdateDate).Date):dd/MM/yyyy}";
                    }
                    else if ((item.UpdateDate != null && ((DateTime)item.UpdateDate).Date < DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(((DateTime)item.UpdateDate).Date);
                        item.UpdateDateStr = $"{dayOfWeek} - {(((DateTime)item.UpdateDate).Date):dd/MM/yyyy}";
                    }
                    else if ((item.UpdateDate != null && ((DateTime)item.UpdateDate).Date == DateTime.Now.Date))
                    {
                        var dayOfWeek = GetDayOfWeek(((DateTime)item.UpdateDate).Date);
                        item.UpdateDateStr = $"{dayOfWeek} - Hôm nay";
                    }
                });

                // Id trạng thái công việc
                var newStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "NEW")?.CategoryId;
                var dlStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "DL")?.CategoryId;
                var htStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "HT")?.CategoryId;
                var closeStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;

                var listTask = new List<TaskEntityModel>();
                switch (parameter.Type)
                {
                    case "all":
                        listTask = listAllTask.ToList();
                        break;
                    case "me":
                        var resourceId = context.ProjectResource.FirstOrDefault(c =>
                            c.ObjectId == employee.EmployeeId && c.ProjectId == parameter.ProjectId)?.ProjectResourceId;
                        var listTaskId = context.TaskResourceMapping.Where(c => c.ResourceId == resourceId)
                            .Select(m => m.TaskId).ToList();
                        listTask = listAllTask.Where(c => listTaskId.Contains(c.TaskId)).ToList();
                        break;
                    case "overdue":
                        listTask = listAllTask.Where(c =>
                            c.PlanEndTime != null && DateTime.Now.Date > c.PlanEndTime.Value.Date).ToList();
                        break;
                    case "notassign":
                        var listTaskResourceId = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId)
                            .Select(m => m.ProjectResourceId).ToList();
                        var listTaskIdHaveAssign = context.TaskResourceMapping
                            .Where(c => listTaskResourceId.Contains(c.ResourceId)).Select(c => c.TaskId).ToList();
                        var listAllTaskIfOfProject = listAllTask.Select(c => c.TaskId).ToList();
                        var listTaskIdNotAssing = listAllTaskIfOfProject.Except(listTaskIdHaveAssign);
                        listTask = listAllTask.Where(c => listTaskIdNotAssing.Contains(c.TaskId)).ToList();
                        break;
                    default: break;
                }

                #region  Số lượng theo từng bảng ghi theo trạng thái

                var numberTotalTask = listTask.Count();
                var numberTaskNew = listTask.Count(c => c.Status == newStatusId);
                var numberTaskDL = listTask.Count(c => c.Status == dlStatusId);
                var numberTaskHT = listTask.Count(c => c.Status == htStatusId);
                var numberTaskClose = listTask.Count(c => c.Status == closeStatusId);

                #endregion

                switch (parameter.OptionStatus)
                {
                    case "TEMP":
                        break;
                    case "NEW":
                        listTask = listTask.Where(c => c.Status == newStatusId).ToList();
                        break;
                    case "DL":
                        listTask = listTask.Where(c => c.Status == dlStatusId).ToList();
                        break;
                    case "HT":
                        listTask = listTask.Where(c => c.Status == htStatusId).ToList();
                        break;
                    case "CLOSE":
                        listTask = listTask.Where(c => c.Status == closeStatusId).ToList();
                        break;
                    default: break;
                }

                listTask = listTask.OrderByDescending(c => c.UpdateDate.Value.Date).ThenByDescending(c => c.Priority)
                    .ThenByDescending(c => c.CreateDate).ToList();

                return new SearchTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTask = listTask,
                    NumberTaskNew = numberTaskNew,
                    NumberTaskDL = numberTaskDL,
                    NumberTaskHT = numberTaskHT,
                    NumberTaskClose = numberTaskClose,
                    NumberTotalTask = numberTotalTask,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour
                };
            }
            catch (Exception ex)
            {
                return new SearchTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateOrUpdateTimeSheetResult CreateOrUpdateTimeSheet(CreateOrUpdateTimeSheetParameter parameter)
        {
            try
            {
                var task = context.Task.FirstOrDefault(c => c.TaskId == parameter.TimeSheet.TaskId);
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (task == null)
                {
                    return new CreateOrUpdateTimeSheetResult
                    {
                        MessageCode = "Công việc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }


                //#region Kiểm tra nhân viên tạo task có trong thời gian phân bổ dự án không

                //var isInTime = true;
                //var message = "";

                //var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //DateTime taskStart = task.PlanStartTime.Value.Date;
                //DateTime taskEnd = task.PlanEndTime.Value.Date;

                //// Lấy danh sách những ngày khai timesheet
                //var lstDateOfTimeSheet = parameter.TimeSheet.ListTimeSheetDetail.FindAll(x => x.SpentHour > 0).Select(x => x.Date).ToList();

                //// check Công việc đang khai time sheet đang giao cho nguồn lực nào để check khoảng thời gian phân bổ
                //var taskResourceMap = context.TaskResourceMapping.FirstOrDefault(x => x.TaskId == task.TaskId && x.ResourceId == employee.EmployeeId);

                //var resources = context.ProjectResource.Where(x => x.ObjectId == employee.EmployeeId && x.ProjectId == task.ProjectId )
                // .OrderBy(y => y.CreateDate)
                // .ToList();
                //if(taskResourceMap != null)
                //{
                //    foreach (var resource in resources.Where(x => x.ProjectResourceId == taskResourceMap.ResourceId))
                //    {
                //        foreach(var date in lstDateOfTimeSheet)
                //        {
                //            if (date.Value.Date <= taskStart && date.Value.Date >= taskEnd)
                //            {
                //                isInTime = true;
                //                message = "";
                //                break;
                //            }
                //            else
                //            {
                //                isInTime = false;
                //                message = $"Nhân viên không thể khai báo timesheet ngoài khoảng thời gian phân bổ từ {taskStart:dd/MM/yyyy} đến {taskEnd:dd/MM/yyyy}";
                //            }
                //        }    
                //    }
                //}    
              

                //if (!isInTime)
                //{
                //    return new CreateOrUpdateTimeSheetResult()
                //    {
                //        Message = message,
                //        Status = false,
                //    };
                //}

                //#endregion


                Note note = new Note
                {
                    NoteId = Guid.NewGuid(),
                    ObjectType = "TAS",
                    ObjectId = task.TaskId,
                    Type = "SYS",
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    NoteTitle = "Đã thêm ghi chú"
                };
                var TimeSheetId = Guid.NewGuid();
                if (parameter.TimeSheet.TimeSheetId == null || parameter.TimeSheet.TimeSheetId == Guid.Empty)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var timeSheet = new TimeSheet
                        {
                            TimeSheetId = Guid.NewGuid(),
                            FromDate = parameter.TimeSheet.FromDate,
                            ToDate = parameter.TimeSheet.ToDate,
                            TaskId = parameter.TimeSheet.TaskId,
                            PersonInChargeId = parameter.TimeSheet.PersonInChargeId,
                            Status = parameter.TimeSheet.Status,
                            TimeType = parameter.TimeSheet.TimeType,
                            SpentHour = parameter.TimeSheet.SpentHour,
                            Note = parameter.TimeSheet.Note,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null
                        };
                        TimeSheetId = timeSheet.TimeSheetId;
                        context.TimeSheet.Add(timeSheet);
                        var listTimeSheetDetail = new List<TimeSheetDetail>();
                        parameter.TimeSheet.ListTimeSheetDetail.ForEach(item =>
                        {
                            var timeSheetDetail = new TimeSheetDetail
                            {
                                TimeSheetDetailId = Guid.NewGuid(),
                                Date = item.Date,
                                SpentHour = item.SpentHour,
                                TimeSheetId = timeSheet.TimeSheetId,
                                Status = 0,
                            };
                            listTimeSheetDetail.Add(timeSheetDetail);
                        });

                        context.TimeSheetDetail.AddRange(listTimeSheetDetail);
                        context.SaveChanges();

                        var listTimeSheet = context.TimeSheet.Where(c => c.TaskId == parameter.TimeSheet.TaskId).ToList();
                        //task.ActualHour = listTimeSheet.Sum(c => c.SpentHour);
                        //context.Task.Update(task);

                        note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                    + " đã khai báo timesheet cho công việc." + "</p>";
                        context.Note.Add(note);
                        // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                        var project = context.Project.FirstOrDefault(x => x.ProjectId == task.ProjectId);
                        if (project != null)
                        {
                            project.LastChangeActivityDate = DateTime.Now;
                            context.Project.Update(project);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
                else
                {
                    TimeSheetId = parameter.TimeSheet.TimeSheetId;
                    var oldTimeSheet = context.TimeSheet.FirstOrDefault(c => c.TimeSheetId == parameter.TimeSheet.TimeSheetId);
                    if (oldTimeSheet == null)
                    {
                        return new CreateOrUpdateTimeSheetResult
                        {
                            MessageCode = "Khai báo thời gian không tồn tại trong hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed
                        };
                    }
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        oldTimeSheet.SpentHour = parameter.TimeSheet.SpentHour;
                        oldTimeSheet.Status = parameter.TimeSheet.Status;
                        oldTimeSheet.TimeType = parameter.TimeSheet.TimeType;
                        oldTimeSheet.Note = parameter.TimeSheet.Note;
                        oldTimeSheet.UpdatedById = parameter.UserId;
                        oldTimeSheet.UpdatedDate = DateTime.Now;

                        context.TimeSheet.Update(oldTimeSheet);

                        var listTimeSheetDetail = new List<TimeSheetDetail>();
                        parameter.TimeSheet.ListTimeSheetDetail.ForEach(item =>
                        {
                            var timeSheetDetail = new TimeSheetDetail
                            {
                                TimeSheetDetailId = item.TimeSheetDetailId,
                                Date = item.Date,
                                SpentHour = item.SpentHour,
                                TimeSheetId = oldTimeSheet.TimeSheetId,
                                Status = item.Status,
                            };
                            listTimeSheetDetail.Add(timeSheetDetail);
                        });

                        context.TimeSheetDetail.UpdateRange(listTimeSheetDetail);
                        context.SaveChanges();

                        var listTimeSheet = context.TimeSheet.Where(c => c.TaskId == parameter.TimeSheet.TaskId).ToList();
                        task.ActualHour = listTimeSheet.Sum(c => c.SpentHour);
                        context.Task.Update(task);

                        note.Description = "<p>- <strong>" + user.UserName + "</strong>"
                                   + " đã khai báo timesheet cho công việc." + "</p>";
                        context.Note.Add(note);

                        // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                        var project = context.Project.FirstOrDefault(x => x.ProjectId == task.ProjectId);
                        if (project != null)
                        {
                            project.LastChangeActivityDate = DateTime.Now;
                            context.Project.Update(project);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }

                return new CreateOrUpdateTimeSheetResult
                {
                    TimeSheetId = TimeSheetId,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateTimeSheetResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public ChangeStatusTaskResult ChangeStatusTask(ChangeStatusTaskParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new ChangeStatusTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new ChangeStatusTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var task = context.Task.FirstOrDefault(c => c.TaskId == parameter.TaskId);
                if (task == null)
                {
                    return new ChangeStatusTaskResult
                    {
                        MessageCode = "Công việc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                Note note = new Note
                {
                    NoteId = Guid.NewGuid(),
                    ObjectType = "TAS",
                    ObjectId = task.TaskId,
                    Type = "SYS",
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    NoteTitle = "Đã thêm ghi chú"
                };

                #region Add nguồn lực khi công việc bắt đầu mà ko có người phụ trách
                var listProjectResource = context.ProjectResource.Where(c => c.ProjectId == task.ProjectId &&
                                                                             c.ObjectId == employee.EmployeeId).ToList();
                #endregion

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                var newStatus = listStatus.FirstOrDefault(c => c.CategoryCode == "NEW");
                var dlStatus = listStatus.FirstOrDefault(c => c.CategoryCode == "DL");
                var htStatus = listStatus.FirstOrDefault(c => c.CategoryCode == "HT");
                var closeStatus = listStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE");
                #endregion

                var project = context.Project.FirstOrDefault(x => x.ProjectId == task.ProjectId);
                var present = listStatus.FirstOrDefault(c => c.CategoryId == task.Status);
                var status = listStatus.FirstOrDefault(c => c.CategoryId == parameter.StatusId);
                switch (status?.CategoryCode)
                {
                    case "NEW":
                        break;
                    case "DL":
                        task.Status = parameter.StatusId;
                        task.UpdateBy = parameter.UserId;
                        task.UpdateDate = DateTime.Now;
                        if (parameter.Type == 2)
                        {
                            task.ActualStartTime = task.ActualStartTime ?? DateTime.Now;
                            if (task.ActualEndTime != null && task.ActualEndTime.Value.Date <= DateTime.Now.Date)
                            {
                                task.ActualEndTime = null;
                            }
                        }
                        else
                        {
                            #region Kiểm tra công việc đã có người phụ trách hay chưa. Nếu chưa có người phụ trách sẽ add nguồn lực của account đăng nhập
                            var picOfTask = context.TaskResourceMapping.Where(c => c.TaskId == task.TaskId && c.IsPersonInCharge == true).ToList();
                            if (picOfTask.Count() == 0)
                            {
                                var listTaskResource = new List<TaskResourceMapping>();
                                listProjectResource.ForEach(item =>
                                {
                                    var taskResourceMapping = new TaskResourceMapping
                                    {
                                        TaskResourceMappingId = Guid.NewGuid(),
                                        TaskId = task.TaskId,
                                        ResourceId = item.ProjectResourceId,
                                        Hours = (task.EstimateHour.Value / listProjectResource.Count),
                                        IsChecker = false,
                                        IsPersonInCharge = true
                                    };
                                    listTaskResource.Add(taskResourceMapping);
                                });
                                context.TaskResourceMapping.AddRange(listTaskResource);
                            }
                            #endregion
                            task.ActualStartTime = DateTime.Now;
                            if (project != null)
                            {
                                var today = DateTime.Now;
                                var statusCategoryId =
                                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT")?.CategoryTypeId;
                                var statusDTK = context.Category.FirstOrDefault(x =>
                                    x.CategoryTypeId == statusCategoryId && x.CategoryCode == "DTK");

                                project.ProjectStatus = statusDTK?.CategoryId;
                                if (project.ActualStartDate == null || project.ActualStartDate.Value.Date > today.Date)
                                {
                                    project.ActualStartDate = DateTime.Now;
                                }
                            }
                        }
                        note.Description = $"Đã thay đổi trạng thái công việc từ {present.CategoryName} sang {status.CategoryName}";
                        break;
                    case "HT":
                        task.ActualStartTime = task.ActualStartTime ?? DateTime.Now;
                        task.ActualEndTime = task.ActualStartTime ?? DateTime.Now;
                        task.Status = parameter.StatusId;
                        task.UpdateBy = parameter.UserId;
                        task.UpdateDate = DateTime.Now;
                        task.CompleteDate = DateTime.Now;
                        task.TaskComplate = 100;
                        note.Description = $"Đã thay đổi trạng thái công việc từ {present.CategoryName} sang {status.CategoryName}";
                        break;
                    case "CLOSE":
                        task.Status = parameter.StatusId;
                        task.ActualStartTime = task.ActualStartTime ?? DateTime.Now;
                        task.ActualEndTime = task.ActualEndTime ?? DateTime.Now;
                        task.UpdateBy = parameter.UserId;
                        task.UpdateDate = DateTime.Now;
                        note.Description = $"Đã thay đổi trạng thái công việc từ {present.CategoryName} sang {status.CategoryName}";
                        break;
                    default: break;
                }

                context.Task.Update(task);
                context.Note.Add(note);

                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                return new ChangeStatusTaskResult
                {
                    MessageCode = "Thay đổi trạng thái công việc thành công!",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ChangeStatusTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public DeleteTaskResult DeleteTask(DeleteTaskParameter parameter)
        {
            try
            {
                var task = context.Task.FirstOrDefault(c => c.TaskId == parameter.TaskId);
                if (task == null)
                {
                    return new DeleteTaskResult
                    {
                        MessageCode = "Công việc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    var delTaskResouceMappings = context.TaskResourceMapping.Where(c => c.TaskId == task.TaskId).ToList();
                    var delTaskNotes = context.Note.Where(c => c.ObjectType == "TAS" && c.ObjectId == task.TaskId).ToList();
                    var delTaskDocuments = context.TaskDocument.Where(c => c.TaskId == task.TaskId).ToList();
                    var delTimeSheets = context.TimeSheet.Where(c => c.TaskId == task.TaskId).ToList();
                    var delTimeSheetId = delTimeSheets.Select(m => m.TimeSheetId).ToList();
                    var delTimeSheetDetail = context.TimeSheetDetail.Where(c => delTimeSheetId.Contains(c.TimeSheetId)).ToList();

                    context.TaskResourceMapping.RemoveRange(delTaskResouceMappings);
                    context.Note.RemoveRange(delTaskNotes);
                    if (delTaskDocuments.Count > 0)
                    {
                        delTaskDocuments.ForEach(item =>
                        {
                            if (File.Exists(item.DocumentUrl))
                            {
                                File.Delete(item.DocumentUrl);
                            }
                        });
                    }

                    context.TimeSheet.RemoveRange(delTimeSheets);
                    context.TimeSheetDetail.RemoveRange(delTimeSheetDetail);
                    context.Task.Remove(task);
                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == task.ProjectId);
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }

                return new DeleteTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DeleteTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchTaskResult SearchTaskFromProjectScope(SearchTaskParameter parameter)
        {
            try
            {
                var project = context.Project.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (project == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new SearchTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var listNote = new List<NoteEntityModel>();
                var projectScopes = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectId).ToList();
                var projectScopeGuids = projectScopes.Select(ps => ps.ProjectScopeId).ToList();
                var tasks = context.Task.Where(x => projectScopeGuids.Contains((Guid)x.ProjectScopeId)).ToList();

                #region Trạng thái công việc
                var listCategory = context.Category.Where(x => x.CategoryType.CategoryTypeCode == "TTCV" && x.Active == true).Select(y =>
                                   new CategoryEntityModel
                                   {
                                       CategoryId = y.CategoryId,
                                       CategoryName = y.CategoryName,
                                       CategoryCode = y.CategoryCode,
                                       CategoryTypeId = Guid.Empty,
                                       CreatedById = Guid.Empty,
                                       CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                       CountCategoryById = 0
                                   }).ToList();

                var listStatus = listCategory.Where(c => c.CategoryTypeCode == "TTCV").ToList();
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                #endregion

                #region Danh sách scope thuộc dự án
                var listProjectScope = projectScopes.Select(p => new ProjectScopeModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    ResourceType = p.ResourceType,
                    ProjectScopeName = p.ProjectScopeName,
                    ProjectScopeCode = p.ProjectScopeCode,
                    TenantId = p.TenantId,
                    ParentId = p.ParentId,
                    ProjectId = p.ProjectId
                }).ToList();

                listProjectScope.ForEach(item =>
                {
                    // danh sách các task thuộc hạng mục
                    var listTaskId = tasks.Where(x => x.ProjectScopeId == item.ProjectScopeId).Select(x => x.TaskId).ToList();
                    var listTaskRe = context.TaskResourceMapping.Where(x => x.IsPersonInCharge == true && listTaskId.Contains(x.TaskId)).Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId)).Select(a => a.ObjectId).ToList();
                    var lstEmp = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId)).Select(em => em.EmployeeName).ToList();
                    var listResourceScope = new List<string>();
                    lstEmp.ForEach(emp =>
                    {
                        listResourceScope.Add(lstEmp.Count > 0 ? emp : string.Empty);
                    });
                    item.ListEmployee = listResourceScope;
                });
                listProjectScope = SetTTChildren(listProjectScope);
                #endregion

                var listAllEmployee = context.Employee.ToList();
                var listAllTaskMapping = context.TaskResourceMapping.ToList();
                var listResourceOfProject = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId).ToList();

                #region Dánh sách task thuộc dự án - tab danh sách công việc
                var listProjectTask = tasks.Select(p => new TaskEntityModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    TaskCode = p.TaskCode,
                    TaskId = p.TaskId,
                    TaskName = p.TaskName,
                    PlanStartTime = p.PlanStartTime,
                    PlanEndTime = p.PlanEndTime,
                    Status = p.Status,
                    ActualEndTime = p.ActualEndTime,
                    ActualStartTime = p.ActualStartTime,
                    ActualHour = p.ActualHour,
                    EstimateCost = p.EstimateCost,
                    EstimateHour = p.EstimateHour,
                    TaskComplate = p.TaskComplate,
                    ProjectScopeName = listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId) != null ? listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId).ProjectScopeCode + ". " + listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId).ProjectScopeName : "",

                }).ToList();
                // Danh sách bộ lọc gói công việc            
                listProjectTask.ForEach(p =>
                {
                    var category = listStatus.FirstOrDefault(c => c.CategoryId == p.Status);
                    if (category != null)
                    {
                        p.StatusName = category.CategoryName;
                        p.CanEdit = category.CategoryCode != "CLOSE" ? true : false;
                    }
                    // danh sách các task thuộc hạng mục
                    var listTaskRe = context.TaskResourceMapping.Where(x => x.IsPersonInCharge == true && x.TaskId == p.TaskId).Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId)).Select(a => a.ObjectId).ToList();
                    var employees = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId)).Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                    p.Employee = employees.Count > 0 ? employees.Aggregate((a, x) => a + ", " + x) : string.Empty;
                });

                #endregion

                listProjectTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    if (status != null)
                    {
                        item.StatusName = status?.CategoryName;
                        switch (status.CategoryCode)
                        {
                            case "NEW":
                                item.BackgroundColorForStatus = "#0F62FE";
                                break;
                            case "DL":
                                item.BackgroundColorForStatus = "#FFC000";
                                break;
                            case "HT":
                                item.BackgroundColorForStatus = "#63B646";
                                break;
                            case "CLOSE":
                                item.BackgroundColorForStatus = "#9C00FF";
                                break;
                        }
                    }
                    item.ProjectScopeId = listProjectScope.FirstOrDefault(c => c.ProjectScopeId == ((Guid)item.ProjectScopeId))?.ProjectScopeId;
                    var listResourceId = listAllTaskMapping.Where(c => c.TaskId == item.TaskId && c.IsPersonInCharge == true).Select(m => m.ResourceId).ToList();
                    if (listResourceId.Count != 0)
                    {
                        var listObjectId = listResourceOfProject.Where(c => listResourceId.Contains(c.ProjectResourceId)).Select(m => m.ObjectId).ToList();
                        var listEmployeeName = listAllEmployee.Where(c => listObjectId.Contains(c.EmployeeId)).Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                        //item.Employee = string.Join(", ", listEmployeeName);
                        item.Employee = listEmployeeName.Count > 0 ? listEmployeeName.Aggregate((a, x) => a + ", " + x) : string.Empty;
                    }
                });

                var listTask = new List<TaskEntityModel>();
                listTask = listProjectTask.Where(c =>
                                             (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                                             parameter.ListStatusId.Contains(c.Status.Value)) &&
                                            (parameter.ListWorkpackageId == null || parameter.ListWorkpackageId.Count == 0 || parameter.ListWorkpackageId.Contains(c.ProjectScopeId)) &&
                                            //(parameter.ListEmployeeId == null || parameter.ListEmployeeId.Count == 0 ||
                                            //parameter.ListEmployeeId.Contains(c.ListTaskResource.Select(x => (Guid) x.ResourceId).ToList())) &&
                                            (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || parameter.FromDate.Value.Date <= c.PlanStartTime.Value.Date) &&
                                            (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || parameter.ToDate.Value.Date >= c.PlanStartTime.Value.Date)
                                            &&
                                            (parameter.FromEndDate == null || parameter.FromEndDate == DateTime.MinValue || parameter.FromEndDate <= c.PlanEndTime) &&
                                            (parameter.ToEndDate == null || parameter.ToEndDate == DateTime.MinValue || parameter.ToEndDate >= c.PlanEndTime)
                                            &&
                                            (parameter.FromPercent == 0 || c.TaskComplate >= parameter.FromPercent) && (parameter.ToPercent == 0 || c.TaskComplate <= parameter.ToPercent)

                                            ).ToList();

                return new SearchTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTask = listTask
                };
            }
            catch (Exception ex)
            {
                return new SearchTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }
        private List<ProjectScopeModel> SetTTChildren(List<ProjectScopeModel> listQuoteScope)
        {
            var listnew = new List<ProjectScopeModel>();

            listQuoteScope.Where(l => l.ParentId == null).ToList().ForEach(item =>
            {
                item.ProjectScopeCode = "";
                var list = listQuoteScope.Where(l => l.ParentId == item.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                int index = 1;
                foreach (var item1 in list)
                {
                    item1.ProjectScopeCode = index.ToString();
                    listnew.Add(item1);

                    var list2 = listQuoteScope.Where(l => l.ParentId == item1.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                    int index2 = 1;
                    foreach (var item2 in list2)
                    {
                        item2.ProjectScopeCode = item1.ProjectScopeCode + "." + index2;
                        listnew.Add(item2);

                        var list3 = listQuoteScope.Where(l => l.ParentId == item2.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                        int index3 = 1;
                        foreach (var item3 in list3)
                        {
                            item3.ProjectScopeCode = item2.ProjectScopeCode + "." + index3;
                            listnew.Add(item3);

                            var list4 = listQuoteScope.Where(l => l.ParentId == item3.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                            int index4 = 1;
                            foreach (var item4 in list4)
                            {
                                item4.ProjectScopeCode = item3.ProjectScopeCode + "." + index4;
                                listnew.Add(item4);

                                var list5 = listQuoteScope.Where(l => l.ParentId == item4.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                                int index5 = 1;
                                foreach (var item5 in list5)
                                {
                                    item5.ProjectScopeCode = item4.ProjectScopeCode + "." + index5;
                                    listnew.Add(item5);

                                    index5++;
                                }
                                index4++;
                            }
                            index3++;
                        }
                        index2++;
                    }
                    index++;
                }

                listnew.Add(item);
            });

            return listnew.OrderBy(o => o.ProjectScopeCode).ToList();
        }

        public void CaculatorProjectTask(Guid projectId, out decimal projectComplete, out decimal totalEstimateHour)
        {
            var listTask = context.Task.Where(c => c.ProjectId == projectId).ToList();
            var total = 0M;
            var taskComplete = 0M;
            listTask.ForEach(item =>
            {
                taskComplete += (item?.TaskComplate ?? 0) / 100 * (item?.EstimateHour ?? 0);
                total += item?.EstimateHour ?? 0;
            });
            totalEstimateHour = total;

            projectComplete = total != 0 ? taskComplete / total * 100 : 0M;
        }

        public GetMasterDataCreateConstraintResult GetMasterDataCreateConstraint(GetMasterDataCreateConstraintParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCreateConstraintResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataCreateConstraintResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var task = context.Task.FirstOrDefault(c => c.TaskId == parameter.TaskId);
                if (task == null)
                {
                    return new GetMasterDataCreateConstraintResult
                    {
                        MessageCode = "Công việc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId && c.Active == true).ToList();
                #endregion

                var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId && c.TaskId != parameter.TaskId)
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        TaskCode = m.TaskCode,
                        TaskName = m.TaskName,
                        ParentId = m.ParentId,
                        Status = m.Status
                    }).ToList();

                listAllTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusName = status?.CategoryName;
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            item.IsDelete = true;
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            item.IsDelete = false;
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            item.IsDelete = false;
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            item.IsDelete = false;
                            break;
                    }
                });

                var listAllTaskContraint = context.TaskConstraint.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new TaskConstraintEntityModel
                    {
                        TaskConstraintId = m.TaskConstraintId,
                        TaskId = m.TaskId,
                        ParentId = m.ParentId,
                        DelayTime = m.DelayTime,
                        ConstraintType = m.ConstraintType,
                        ConstraingRequired = m.ConstraingRequired,
                        ProjectId = m.ProjectId
                    }).ToList();

                var typeConstranintId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LRB")?.CategoryTypeId;
                var listConstraint = context.Category.Where(c => c.CategoryTypeId == typeConstranintId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName
                    }).ToList();

                var listTask = new List<TaskEntityModel>();

                var listBefore = new List<TaskConstraintEntityModel>();
                var listAfter = new List<TaskConstraintEntityModel>();
                // Parent : Là những công việc thực hiện trước
                var listParent = listAllTaskContraint.Where(c => c.TaskId == task.TaskId).ToList();
                while (listParent.Count() != 0)
                {
                    listBefore.AddRange(listParent);
                    var listParentId = listParent.Select(c => c.ParentId);
                    listParent = listAllTaskContraint.Where(c => listParentId.Contains(c.TaskId)).ToList();
                }

                // Child : là những công việc thực hiện sau
                var lstChild = listAllTaskContraint.Where(c => c.ParentId == task.TaskId).ToList();
                while (lstChild.Count() != 0)
                {
                    listAfter.AddRange(lstChild);
                    var listChildId = lstChild.Select(m => m.TaskId).ToList();
                    lstChild = listAllTaskContraint.Where(c => listChildId.Contains(c.ParentId.Value)).ToList();
                }

                var listTaskId = new List<Guid>();
                listTaskId.AddRange(listBefore.Select(m => m.ParentId.Value).ToList());
                listTaskId.AddRange(listAfter.Select(m => m.TaskId).ToList());

                var listTaskHasConstraint = listAllTask.Where(c => listTaskId.Contains(c.TaskId)).ToList();

                listTask = listAllTask.Except(listTaskHasConstraint).OrderBy(c => c.CreateDate).ToList();

                return new GetMasterDataCreateConstraintResult
                {
                    ListConstraint = listConstraint,
                    ListTask = listTask,
                    MessageCode = "scucess",
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateConstraintResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateConstraintTaskResult CreateConstraintTask(CreateConstraintTaskParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new CreateConstraintTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var taskConstraint = new TaskConstraint
                {
                    TaskConstraintId = Guid.NewGuid(),
                    TaskId = parameter.TaskConstraint.TaskId,
                    ParentId = parameter.TaskConstraint.ParentId,
                    ConstraingRequired = parameter.TaskConstraint.ConstraingRequired,
                    ConstraintType = parameter.TaskConstraint.ConstraintType,
                    DelayTime = parameter.TaskConstraint.DelayTime,
                    ProjectId = parameter.TaskConstraint.ProjectId,
                    CreatedById = user.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDate = null
                };

                context.TaskConstraint.Add(taskConstraint);
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.TaskConstraint.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                return new CreateConstraintTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateConstraintTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateRelateTaskResult CreateRelateTask(CreateRelateTaskParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new CreateRelateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var taskRelate = new RelateTaskMapping
                {
                    RelateTaskMappingId = Guid.NewGuid(),
                    TaskId = parameter.TaskRelate.TaskId.Value,
                    RelateTaskId = parameter.TaskRelate.RelateTaskId,
                    ProjectId = parameter.TaskRelate.ProjectId,
                    CreatedById = user.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDate = null
                };

                context.RelateTaskMapping.Add(taskRelate);
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.TaskRelate.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                ///Trả công việc vừa thêm về client

                var commonListCategory = context.Category.Where(c => c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                          CategoryId = m.CategoryId,
                          CategoryCode = m.CategoryCode,
                          CategoryName = m.CategoryName,
                          CategoryTypeId = m.CategoryTypeId
                    }).ToList();


                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = commonListCategory.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion


                var taskRelateResult = new RelateTaskMappingEntityModel
                {
                    RelateTaskMappingId = taskRelate.RelateTaskMappingId,
                    TaskId = taskRelate.TaskId,
                    RelateTaskId = taskRelate.RelateTaskId,
                    ProjectId = taskRelate.ProjectId.Value,
                    CreatedById = taskRelate.CreatedById,
                    CreatedDate = taskRelate.CreatedDate,
                    UpdatedById = null,
                    UpdatedDate = null
                };

                var taskInfor = new Task();
                taskInfor = context.Task.FirstOrDefault(x => x.TaskId == taskRelate.RelateTaskId);

                taskRelateResult.TaskName = taskInfor.TaskName;
                taskRelateResult.TaskCode = taskInfor.TaskCode;
                taskRelateResult.StatusCode = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryCode;
                taskRelateResult.StatusName = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryName;
                taskRelateResult.ExpectedStartDate = taskInfor.PlanStartTime;
                taskRelateResult.ExpectedEndDate = taskInfor.PlanEndTime;

                //lấy màu backGround cho trạng thái
                var status = listStatus.FirstOrDefault(c => c.CategoryId == taskInfor.Status);
                switch (status.CategoryCode)
                {
                    case "NEW":
                        taskRelateResult.BackgroundColorForStatus = "#0F62FE";
                        taskRelateResult.IsDelete = true;
                        break;
                    case "DL":
                        taskRelateResult.BackgroundColorForStatus = "#FFC000";
                        taskRelateResult.IsDelete = false;
                        break;
                    case "HT":
                        taskRelateResult.BackgroundColorForStatus = "#63B646";
                        taskRelateResult.IsDelete = false;
                        break;
                    case "CLOSE":
                        taskRelateResult.BackgroundColorForStatus = "#9C00FF";
                        taskRelateResult.IsDelete = false;
                        break;
                }

                return new CreateRelateTaskResult
                {
                    TaskRelate = taskRelateResult,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateRelateTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateRequiredConstrantResult UpdateRequiredConstrant(UpdateRequiredConstrantParameter parameter)
        {
            try
            {
                var taskConstraint = context.TaskConstraint.FirstOrDefault(c => c.TaskConstraintId == parameter.TaskConstraintId);
                if (taskConstraint == null)
                {
                    return new UpdateRequiredConstrantResult
                    {
                        MessageCode = "Ràng buộc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                taskConstraint.ConstraingRequired = !taskConstraint.ConstraingRequired;
                context.TaskConstraint.Update(taskConstraint);
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == taskConstraint.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                return new UpdateRequiredConstrantResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new UpdateRequiredConstrantResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public DeleteTaskConstraintResult DeleteTaskConstraint(DeleteTaskConstraintParameter paramter)
        {
            try
            {
                var taskConstraint = context.TaskConstraint.FirstOrDefault(c => c.TaskConstraintId == paramter.TaskConstraintId);
                if (taskConstraint == null)
                {
                    return new DeleteTaskConstraintResult
                    {
                        MessageCode = "Ràng buộc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.TaskConstraint.Remove(taskConstraint);
                    context.SaveChanges();
                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == taskConstraint.ProjectId);
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    transaction.Commit();
                }

                return new DeleteTaskConstraintResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DeleteTaskConstraintResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public DeleteRelateTaskResult DeleteRelateTask(DeleteRelateTaskParameter parameter)
        {
            try
            {
                var taskRelate = context.RelateTaskMapping.FirstOrDefault(c => c.RelateTaskMappingId == parameter.RelateTaskMappingId);
                if (taskRelate == null)
                {
                    return new DeleteRelateTaskResult
                    {
                        MessageCode = "Công việc liên quan không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.RelateTaskMapping.Remove(taskRelate);
                    context.SaveChanges();
                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == taskRelate.ProjectId);
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    transaction.Commit();
                }

                return new DeleteRelateTaskResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DeleteRelateTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private string GetDayOfWeek(DateTime date)
        {
            var str = string.Empty;
            var dayOfWeek = date.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    str = "Thứ 2";
                    break;
                case DayOfWeek.Tuesday:
                    str = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    str = "Thứ 4";
                    break;
                case DayOfWeek.Thursday:
                    str = "Thứ 5";
                    break;
                case DayOfWeek.Friday:
                    str = "Thứ 6";
                    break;
                case DayOfWeek.Saturday:
                    str = "Thứ 7";
                    break;
                case DayOfWeek.Sunday:
                    str = "Chủ nhật";
                    break;
            }
            return str;
        }

        public GetMasterDataSearchTimeSheetResult GetMasterDataSearchTimeSheet(GetMasterDataSearchTimeSheetParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataSearchTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataSearchTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterDataSearchTimeSheetResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                project.ProjectTypeName = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }
                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                #region Trạng thái khai báo thời gian
                var statusTypeTimeSheetId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTKTG")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeTimeSheetId && c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();
                #endregion

                #region Kiểu thời gian
                var typeTimeTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "KTG")?.CategoryTypeId;
                var listTimeType = context.Category.Where(c => c.CategoryTypeId == typeTimeTypeId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();
                #endregion

                #region Nguồn lực
                var typeRoleResource = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NCNL")?.CategoryTypeId;
                var resourceNoiBoId = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeRoleResource && c.Active == true && c.CategoryCode == "NB")?.CategoryId;

                var listObjectId = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && c.ResourceRole == resourceNoiBoId)
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ProjectId = m.ProjectId,
                        ResourceRole = m.ResourceRole,
                        ObjectId = m.ObjectId,
                        NameResource = string.Empty,
                    }).Select(m => m.ObjectId).ToList();

                var listEmployee = context.Employee.Where(c => c.Active == true && listObjectId.Contains(c.EmployeeId))
                   .Select(m => new EmployeeEntityModel
                   {
                       EmployeeId = m.EmployeeId,
                       EmployeeCode = m.EmployeeCode,
                       EmployeeName = m.EmployeeName
                   }).ToList();
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                #region Lấy ngày đầu tuần và cuối tuần hiện tại
                var firstOfWeek = DateTime.Now;
                // Lấy thứ 2 là ngày đâu tuần - bắt đầu 1 tuần mới
                while (firstOfWeek.DayOfWeek != DayOfWeek.Monday)
                {
                    firstOfWeek = firstOfWeek.AddDays(-1);
                }
                var lastOfWeek = firstOfWeek.AddDays(6);
                #endregion

                #region Phân quyền
                var isRoot = true;
                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position.PositionCode == "GD")
                {
                    // Là giám đốc có thể xem được hết dữ liệu
                }
                else
                {
                    var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                    if (projectMananger != null || project.ProjectManagerId == employee.EmployeeId)
                    {
                        // là quản lý có quyền thấy hết
                    }
                    else
                    {
                        var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                        if (subPm == null)
                        {
                            // Là nhân viên 
                            isRoot = false;
                        }
                    }
                }
                #endregion

                var listAllTaskId = context.Task.Where(c => c.ProjectId == parameter.ProjectId).Select(m => m.TaskId).ToList();
                var totalHouseUsed = context.TimeSheet.Where(c => listAllTaskId.Contains(c.TaskId)).Sum(c => c.SpentHour);

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRootOrganization = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRootOrganization)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion

                return new GetMasterDataSearchTimeSheetResult
                {
                    ListEmployee = listEmployee,
                    ListStatus = listStatus,
                    ListTimeStyle = listTimeType,
                    Project = project,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour,
                    InforExportExcel = inforExportExcel,
                    FromDate = firstOfWeek,
                    ToDate = lastOfWeek,
                    IsRoot = isRoot,
                    TotalHourUsed = totalHouseUsed.Value,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProject = listProject,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchTimeSheetResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchTimeSheetResult SearchTimeSheet(SearchTimeSheetParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new SearchTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new SearchTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var project = context.Project.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (project == null)
                {
                    return new SearchTimeSheetResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region commonData
                // Lấy tất cả công việc của dự án
                var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId).Select(m => new { m.TaskId, m.TaskCode, m.TaskName }).ToList();
                var listAllTaskIdOfProject = context.Task.Where(c => c.ProjectId == parameter.ProjectId).Select(m => m.TaskId).ToList();

                // Lấy tất cả nhân viên
                var listAllEmployee = context.Employee.Select(m => new { m.EmployeeId, m.EmployeeCode, m.EmployeeName }).ToList();

                var typeStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTKTG")?.CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.Active == true && c.CategoryTypeId == typeStatusId).ToList();

                var nhapStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "NHAP")?.CategoryId;

                var typeTimeStyle = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "KTG")?.CategoryTypeId;
                var listAllTimeType = context.Category.Where(c => c.Active == true && c.CategoryTypeId == typeTimeStyle).ToList();
                #endregion

                var listAllTimeSheetDetail = context.TimeSheetDetail.ToList();
                var listAllTimesheet = context.TimeSheet.Where(c => listAllTaskIdOfProject.Contains(c.TaskId)
                        && (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(c.Status.Value))
                        && (parameter.ListTimeTypeId == null || parameter.ListTimeTypeId.Count == 0 || parameter.ListTimeTypeId.Contains(c.TimeType.Value))
                        && (parameter.ListPersionInChargedId == null || parameter.ListPersionInChargedId.Count == 0 || parameter.ListPersionInChargedId.Contains(c.PersonInChargeId.Value)))
                       .Select(m => new TimeSheetEntityModel
                       {
                           TimeSheetId = m.TimeSheetId,
                           TaskId = m.TaskId,
                           PersonInChargeId = m.PersonInChargeId,
                           FromDate = m.FromDate,
                           ToDate = m.ToDate,
                           Status = m.Status,
                           Note = m.Note,
                           SpentHour = m.SpentHour,
                           TimeType = m.TimeType,
                           Monday = 0,
                           Tuesday = 0,
                           Wednesday = 0,
                           Thursday = 0,
                           Friday = 0,
                           Saturday = 0,
                           Sunday = 0,
                           CreatedById = m.CreatedById,
                           CreatedDate = m.CreatedDate
                       }).OrderByDescending(c => c.CreatedDate).ToList();

                if (!parameter.IsShowAll)
                {
                    listAllTimesheet = listAllTimesheet.Where(c => c.FromDate.Value.Date == parameter.FromDate.Value.Date && c.ToDate.Value.Date == parameter.ToDate.Value.Date).ToList();
                }

                #region Phân quyền
                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position.PositionCode == "GD")
                {
                    // Là giám đốc đọc đc nhưng timesheet ở trạng thái ngoài nháp
                    listAllTimesheet = listAllTimesheet.Where(c => c.PersonInChargeId == employee.EmployeeId || c.Status != nhapStatusId).ToList();
                }
                else
                {
                    var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                    if (projectMananger != null || project.ProjectManagerId == employee.EmployeeId)
                    {
                        // Là quản lý đọc đc nhưng timesheet ở trạng thái ngoài nháp
                        listAllTimesheet = listAllTimesheet.Where(c => c.PersonInChargeId == employee.EmployeeId || c.Status != nhapStatusId).ToList();
                    }
                    else
                    {
                        var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                        if (subPm == null)
                        {
                            // Là nhân viên chỉ lấy những timesheet mà bản thân khai báo
                            listAllTimesheet = listAllTimesheet.Where(c => c.PersonInChargeId == employee.EmployeeId).ToList();
                        }
                        else
                        {
                            //Là subPM đọc đc những timesheet ở trạng thái ngoài nháp
                            listAllTimesheet = listAllTimesheet.Where(c => c.PersonInChargeId == employee.EmployeeId || c.Status != nhapStatusId).ToList();
                        }
                    }
                }
                #endregion

                listAllTimesheet.ForEach(item =>
                {
                    var lstDetailOfTimeSheet = listAllTimeSheetDetail.Where(c => c.TimeSheetId == item.TimeSheetId).ToList();
                    // Giá trị của detail chỉ có tối đa 7 bản ghi
                    lstDetailOfTimeSheet.ForEach(detail =>
                    {
                        switch (detail.Date.Value.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Monday = 0;
                                }
                                else
                                {
                                    item.Monday = detail.SpentHour;
                                }
                                item.MondayCheck = detail.Status ;
                                item.MondayId = detail.TimeSheetDetailId;
                                break;
                            case DayOfWeek.Tuesday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Tuesday = 0;
                                }
                                else
                                {
                                    item.Tuesday = detail.SpentHour;
                                }
                                item.TuesdayCheck = detail.Status;
                                item.TuesdayId = detail.TimeSheetDetailId;
                                break;    
                            case DayOfWeek.Wednesday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Wednesday = 0;
                                }
                                else
                                {
                                    item.Wednesday = detail.SpentHour;
                                }
                                item.WednesdayCheck = detail.Status;
                                item.WednesdayId = detail.TimeSheetDetailId;
                                break;

                            case DayOfWeek.Thursday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Thursday = 0;
                                }
                                else
                                {
                                    item.Thursday = detail.SpentHour;
                                }
                                item.ThursdayCheck = detail.Status;
                                item.ThursdayId = detail.TimeSheetDetailId;
                                break;
                            case DayOfWeek.Friday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Friday = 0;
                                }
                                else
                                {
                                    item.Friday = detail.SpentHour;
                                }
                                item.FridayCheck = detail.Status;
                                item.FridayId = detail.TimeSheetDetailId;
                                break;
                            case DayOfWeek.Saturday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Saturday = 0;
                                }
                                else
                                {
                                    item.Saturday = detail.SpentHour;
                                }
                                item.SaturdayCheck = detail.Status;
                                item.SaturdayId = detail.TimeSheetDetailId;
                                break;
                            case DayOfWeek.Sunday:
                                if (employee.EmployeeId != item.PersonInChargeId && detail.Status == 0)
                                {
                                    item.Sunday = 0;
                                }
                                else
                                {
                                    item.Sunday = detail.SpentHour;
                                }
                                item.SundayCheck = detail.Status;
                                item.SundayId = detail.TimeSheetDetailId;
                                break;
                        }
                    });

                    var task = listAllTask.FirstOrDefault(c => c.TaskId == item.TaskId);
                    item.TaskCodeName = $"{task?.TaskCode ?? string.Empty}: {task?.TaskName ?? string.Empty}";
                    item.WeekName = $"{item.FromDate.Value:dd/MM/yyyy} - {item.ToDate.Value:dd/MM/yyyy}";
                    var pic = listAllEmployee.FirstOrDefault(c => c.EmployeeId == item.PersonInChargeId);
                    item.PersonInChargeName = $"{pic.EmployeeCode} - {pic.EmployeeName}";
                    var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusCode = status?.CategoryCode ?? string.Empty;
                    item.StatusName = status?.CategoryName ?? string.Empty;

                    var timeStyle = listAllTimeType.FirstOrDefault(c => c.CategoryId == item.TimeType);
                    item.TimeTypeName = timeStyle?.CategoryName;

                    switch (status.CategoryCode)
                    {
                        case "NHAP":
                            item.BackgroundColorForStatus = "#99BDEE";
                            item.SortOrderStatus = 0;
                            break;
                        case "GPD":
                            item.BackgroundColorForStatus = "#FFBC8B";
                            item.SortOrderStatus = 1;
                            break;
                        case "DPD":
                            item.BackgroundColorForStatus = "#49C846";
                            item.SortOrderStatus = 2;
                            break;
                        case "TCKB":
                            item.BackgroundColorForStatus = "#F44336";
                            item.SortOrderStatus = 3;
                            break;
                    }
                });

                listAllTimesheet = listAllTimesheet.OrderBy(c => c.SortOrderStatus).ThenByDescending(c => c.CreatedDate).ToList();

                return new SearchTimeSheetResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTimeSheet = listAllTimesheet
                };
            }
            catch (Exception ex)
            {
                return new SearchTimeSheetResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateStatusTimeSheetResult UpdateStatusTimeSheet(UpdateStatusTimeSheetParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new UpdateStatusTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new UpdateStatusTimeSheetResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Trạng thái timesheet
                var typeStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTKTG")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusId).ToList();

                var waitApprovalStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "GPD")?.CategoryId;
                var approvalStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "DPD")?.CategoryId;
                var rejectStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "TCKB")?.CategoryId;
                #endregion
                var listAllTimesheetChangeStatus = context.TimeSheet.Where(c => parameter.ListTimeSheetId.Contains(c.TimeSheetId)).ToList();
                var lstNote = new List<Note>();

                var listAllTimeSheetDetail = context.TimeSheetDetail.Where(c => parameter.ListTimeSheetId.Contains(c.TimeSheetId)).ToList();
                switch (parameter.Type)
                {
                    case "SEND_APPROVAL":
                        listAllTimesheetChangeStatus.ForEach(item =>
                        {
                            item.Status = waitApprovalStatusId;
                        });
                        break;
                    //case "AP" +
                    //"PROVAL":
                    //    listAllTimesheetChangeStatus.ForEach(item =>
                    //    {
                    //        item.Status = approvalStatusId;
                    //        Note note = new Note
                    //        {
                    //            NoteId = Guid.NewGuid(),
                    //            ObjectType = "TIMESHEET",
                    //            ObjectId = item.TimeSheetId,
                    //            Type = "SYS",
                    //            Active = true,
                    //            CreatedById = parameter.UserId,
                    //            CreatedDate = DateTime.Now,
                    //            NoteTitle = "Đã phê duyệt",
                    //            Description = parameter.Description?.Trim()
                    //        };
                    //        lstNote.Add(note);
                    //    });
                    //    break;
                    //case "REJECT":
                    //    listAllTimesheetChangeStatus.ForEach(item =>
                    //    {
                    //        item.Status = rejectStatusId;
                    //        Note note = new Note
                    //        {
                    //            NoteId = Guid.NewGuid(),
                    //            ObjectType = "TIMESHEET",
                    //            ObjectId = item.TimeSheetId,
                    //            Type = "SYS",
                    //            Active = true,
                    //            CreatedById = parameter.UserId,
                    //            CreatedDate = DateTime.Now,
                    //            NoteTitle = "Đã từ chối",
                    //            Description = parameter.Description?.Trim()
                    //        };
                    //        lstNote.Add(note);
                    //    });
                    //    break;
                }
                var listTimeSheetDetail = new List<TimeSheetDetail>();

                listAllTimesheetChangeStatus.ForEach(item =>
                {
                    var timeSheet = listAllTimeSheetDetail.Where(x => x.TimeSheetId == item.TimeSheetId).ToList();
                    timeSheet.ForEach(x => {
                        switch (parameter.Type)
                        {
                            case "SEND_APPROVAL":
                                if (x.SpentHour > 0 && x.Status == 0)
                                {
                                    x.Status = 1;
                                }
                                if (x.SpentHour > 0 && x.Status == 2)
                                {
                                    x.Status = 1;
                                }
                                break;
                            case "AP" +
                      "PROVAL":
                                if (x.SpentHour > 0 && x.Status == 1)
                                {
                                    x.Status = 3;
                                }
                                break;
                            case "REJECT":
                                if (x.SpentHour > 0 && x.Status == 1)
                                {
                                    x.Status = 2;
                                }
                                break;
                        }
                        listTimeSheetDetail.Add(x);
                    });
                });



                context.TimeSheetDetail.UpdateRange(listTimeSheetDetail);
                context.TimeSheet.UpdateRange(listAllTimesheetChangeStatus);
                context.Note.AddRange(lstNote);
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project 
                var projectId = context.Task.FirstOrDefault(x => x.TaskId == listAllTimesheetChangeStatus.FirstOrDefault().TaskId).ProjectId;
                var project = context.Project.FirstOrDefault(x => x.ProjectId == projectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                return new UpdateStatusTimeSheetResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new UpdateStatusTimeSheetResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }


        public AcceptOrRejectByDayResult AcceptOrRejectByDay(AcceptOrRejectByDayParameter parameter)
        {
            try
            {
                var timeSheetDetail = context.TimeSheetDetail.FirstOrDefault(x => x.TimeSheetDetailId == parameter.TimeSheetDetail);
                if (parameter.Check)
                {
                    timeSheetDetail.Status = 3;
                }
                else
                {
                    timeSheetDetail.Status = 2;

                }
                context.TimeSheetDetail.Update(timeSheetDetail);
                context.SaveChanges();
                return new AcceptOrRejectByDayResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new AcceptOrRejectByDayResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        //Công việc liên quan
        public GetMasterDataCreateRelateTaskResult GetMasterDataCreateRelateTask(GetMasterDataCreateRelateTaskParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCreateRelateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataCreateRelateTaskResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var task = context.Task.FirstOrDefault(c => c.TaskId == parameter.TaskId);
                if (task == null && parameter.TaskId != null)
                {
                    return new GetMasterDataCreateRelateTaskResult
                    {
                        MessageCode = "Công việc không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listTaskTypeTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(x => x.CategoryTypeId == listTaskTypeTypeId).Select( obj => new CategoryEntityModel
                {
                    CategoryId = obj.CategoryId,
                    CategoryName = obj.CategoryName,
                    CategoryCode = obj.CategoryCode,
                }).ToList();
                var listRelateTaskChose = context.Task.Where(x => x.ProjectId == parameter.ProjectId).Select(m => new TaskEntityModel
                {
                    TaskId = m.TaskId,
                    TaskCode = m.TaskCode,
                    TaskName = m.TaskName,
                    ParentId = m.ParentId,
                    TaskTypeId = m.TaskTypeId,
                    Status = m.Status
                }).ToList();

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                var listAllTaskOfProject = context.Task.Where(x => x.ProjectId == parameter.ProjectId).ToList();
                //Nếu là thêm mới công việc lquan ở màn detail ( sẽ có taskId)
                if (parameter.TaskId != null)
                {
                    //Loại bỏ chính nó 
                    listRelateTaskChose.RemoveAll(y => y.TaskId == parameter.TaskId);


                    //lấy ra các công việc lquan của task đó
                    var listRelateTask = context.RelateTaskMapping.Where(x => x.TaskId == parameter.TaskId || x.RelateTaskId == parameter.TaskId).ToList();

                    //lấy danh sách Id của các task lquan đến task đang thao tác
                    List<Guid> listTaskRelateToTaskId = new List<Guid>();

                    listRelateTask.ForEach(item =>
                    {
                        if (item.TaskId == parameter.TaskId)
                        {
                            listTaskRelateToTaskId.Add(item.RelateTaskId);
                        }
                        else
                        {
                            listTaskRelateToTaskId.Add(item.TaskId);
                        }
                    });

                    //lọc công việc đã có liên kết
                    //listTaskRelateToTaskId.ForEach(item =>
                    //{
                    //    listRelateTaskChose.RemoveAll(x => x.TaskId == item);
                    //});
                }


                listRelateTaskChose.ForEach(item =>
                {
                    var taskInfor = new Task();
                    taskInfor = listAllTaskOfProject.FirstOrDefault(x => x.TaskId == item.TaskId);

                    item.TaskName = taskInfor.TaskName;
                    item.TaskCode = taskInfor.TaskCode;
                    item.StatusCode = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryCode;
                    item.StatusName = listStatus.FirstOrDefault(x => x.CategoryId == taskInfor.Status).CategoryName;
                    item.PlanStartTime = taskInfor.PlanStartTime;
                    item.PlanEndTime = taskInfor.PlanEndTime;
                    //lấy màu backGround cho trạng thái
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == taskInfor.Status);
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            item.IsDelete = true;
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            item.IsDelete = false;
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            item.IsDelete = false;
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            item.IsDelete = false;
                            break;
                    }
                });

                return new GetMasterDataCreateRelateTaskResult
                {
                    ListRelateTask = listRelateTaskChose,
                    ListTaskType = listTaskType,
                    MessageCode = "scucess",
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateRelateTaskResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }



    }
}
