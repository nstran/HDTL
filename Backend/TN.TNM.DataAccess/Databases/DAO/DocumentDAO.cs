using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Messages.Results.Document;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DocumentDAO : BaseDAO, IDocumentDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public DocumentDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public DownloadDocumentByIdResult DownloadDocumentById(DownloadDocumentByIdParameter Parameter)
        {
            var document = context.Document.Where(w => w.DocumentId == Parameter.DocumentId).FirstOrDefault();
            if (document != null)
            {
                var dataByte = File.ReadAllBytes(document.DocumentUrl);
                return new DownloadDocumentByIdResult
                {
                    ExcelFile = dataByte,
                    NameFile = document.Name
                };
            }
            else
            {
                return new DownloadDocumentByIdResult
                {
                    ExcelFile = null,
                    NameFile = null
                };
            }
        }

        public UpdateProjectDocumentResults UpdateProjectDocument(UpdateProjectDocumentParameter parameter)
        {
            try
            {
                var projectId = Guid.Empty;
                #region Xoa file vat ly

                // xoa file vat ly cong viec
                
                parameter.ListTaskDocument?.ForEach(item => {
                    if (File.Exists(item.DocumentUrl))
                    {
                        File.Delete(item.DocumentUrl);
                    }
                });

                //xoa file vat ly tai lieu
                parameter.ListDocument?.ForEach(item => {
                    if (File.Exists(item.DocumentUrl))
                    {
                        File.Delete(item.DocumentUrl);
                    }
                });

                #endregion

                #region Xóa file DB

                // xoa file DB cong viec
                var listTaskFile = new List<FileInFolder>();

                parameter.ListTaskDocument?.ForEach(item => {
                    var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == item.TaskDocumentId && x.ObjectId == item.TaskId);
                    if (file != null)
                    {
                        projectId = item.ObjectId;
                        listTaskFile.Add(file); 
                    }
                });


                context.FileInFolder.RemoveRange(listTaskFile);
                context.SaveChanges();

                // xoa file db tai lieu
                var listFile = new List<FileInFolder>();

                parameter.ListDocument?.ForEach(item => {
                    var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == item.NoteDocumentId && x.ObjectId == item.ObjectId && x.ObjectType == item.ObjectType);
                    if (file != null)
                    {
                        projectId = item.ObjectId;
                        listFile.Add(file);
                    }
                });
              
                context.FileInFolder.RemoveRange(listFile);

                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == projectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }

                context.SaveChanges();

                #endregion

                #region list common FOlder

                var listCommonFolders = context.Folder.Where(x => x.Active && x.ObjectId == parameter.ProjectId && !x.FolderType.Contains("TASK_FILE"))
                    .Select(y => new FolderEntityModel
                    {
                        FolderId = y.FolderId,
                        ParentId = y.ParentId,
                        Name = y.Name,
                        Url = y.Url,
                        IsDelete = y.IsDelete,
                        Active = y.Active,
                        FolderType = y.FolderType,
                        FolderLevel = y.FolderLevel,
                        ObjectId = y.ObjectId,
                    })
                    .OrderBy(z => z.Url)
                    .ToList();

                listCommonFolders.ForEach(item =>
                {
                    item.HasChild = context.Folder.FirstOrDefault(x => x.ParentId == item.FolderId) != null;
                });
                listCommonFolders.ForEach(item =>
                {
                    item.FileNumber = GetAllFile(item.FolderId, listCommonFolders, context.FileInFolder.ToList()).Count;
                });

                var listCommonFile = context.FileInFolder.ToList();

                #endregion

                return new UpdateProjectDocumentResults
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu cập nhật thành công",
                    ListFolders = listCommonFolders,
                };
            }
            catch (Exception e)
            {
                return new UpdateProjectDocumentResults()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateFolderResult CreateFolder(CreateFolderParameter parameter)
        {
            try
            {

                var webRootPath = hostingEnvironment.WebRootPath + "\\";
                var parentUrl = Path.Combine(webRootPath, parameter.FolderParent.Url);

                if (!Directory.Exists(parentUrl))
                {
                    return new CreateFolderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode =  "Thư mục cha không tồn tại, vui lòng cấu hình thư mục hoạc load lại trang để tiếp tục",
                    };
                }

                #region 

                var folderNameString = parameter.FolderName.Trim();
                string[] arrayFolderName = folderNameString.Split(' ');

                var folderNameStr = "";
                for (int i = 0; i < arrayFolderName.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arrayFolderName[i]))
                    {
                        var tmp = arrayFolderName[i].Substring(0, 1).ToUpper();

                        folderNameStr += tmp;
                    }
                }



                #endregion

                var folder = new Folder()
                {
                    FolderId = Guid.NewGuid(),
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = parameter.UserId,
                    UpdatedDate = DateTime.Now,
                    FolderLevel = parameter.FolderParent.FolderLevel + 1,
                    IsDelete = true,
                    Name = parameter.FolderName,
                    ParentId = parameter.FolderParent.FolderId,
                    Url = parameter.FolderParent.Url + @"\" + parameter.FolderName,
                    FolderType = $"{parameter.ProjectCode.ToUpper()}_{folderNameStr}",
                    ObjectId = parameter.FolderParent.ObjectId,
                };

                var folderName = ConvertFolderUrl(folder.Url);
                var newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    context.Folder.Add(folder);
                    context.SaveChanges();
                    Directory.CreateDirectory(newPath);
                }
                else
                {
                    return new CreateFolderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Folder đã tồn tại trong hệ thống.",
                    };
                }

                var folderResult = new FolderEntityModel()
                {
                    Active = folder.Active,
                    FolderId = folder.FolderId,
                    FolderLevel = folder.FolderLevel,
                    IsDelete = folder.IsDelete,
                    Name = folder.Name,
                    ParentId = folder.ParentId,
                    Url = folder.Url,
                };

                return new CreateFolderResult()
                {
                    Folder = folderResult,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo folder lưu trữ mới thành công",
                };
            }
            catch (Exception e)
            {
                return new CreateFolderResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public UploadFileDocumentResult UploadFileDocument(UploadFileDocumentParameter parameter)
        {

            var totalSize = 0M;
            var projectId = Guid.Empty;
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType && x.FolderId == parameter.FolderId);

            if (folder == null)
            {
                return new UploadFileDocumentResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Thư mục upload không tồn tại"
                };
            }

            var webRootPath = hostingEnvironment.WebRootPath + "\\";

            var filePath = Path.Combine(webRootPath, ConvertFolderUrl(folder.Url));

            var listFileDelete = new List<string>();

            try
            {
                var listFileResult = new List<FileInFolderEntityModel>();

                if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                {
                    bool isSave = true;
                    parameter.ListFile.ForEach(item =>
                    {
                        if (!Directory.Exists(filePath))
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
                                FileName = item.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = parameter.FolderId,
                                ObjectId = item.FileInFolder.ObjectId,
                                ObjectType = item.FileInFolder.ObjectType,
                                Size = item.FileInFolder.Size,
                                FileExtension =
                                    item.FileSave.FileName.Substring(item.FileSave.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1)
                            };
                            context.FileInFolder.Add(file);
                            projectId = (Guid)item.FileInFolder.ObjectId;
                            string fileName = file.FileName + "." + file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(filePath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    item.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });
                    if (!isSave)
                    {
                        listFileDelete.ForEach(File.Delete);

                        return new UploadFileDocumentResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Thư mục không tồn tại, bạn phải tạo thư mục để lưu",
                        };
                    }
                }
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == projectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }

                context.SaveChanges();

                #region list common FOlder

                var listCommonFolders = context.Folder.Where(x => x.Active && x.ObjectId == parameter.ObjectId && !x.FolderType.Contains("TASK_FILE"))
                    .Select(y => new FolderEntityModel
                    {
                        FolderId = y.FolderId,
                        ParentId = y.ParentId,
                        Name = y.Name,
                        Url = y.Url,
                        IsDelete = y.IsDelete,
                        Active = y.Active,
                        FolderType = y.FolderType,
                        FolderLevel = y.FolderLevel,
                        ObjectId = y.ObjectId,
                    })
                    .OrderBy(z => z.Url)
                    .ToList();

                listCommonFolders.ForEach(item =>
                {
                    item.HasChild = context.Folder.FirstOrDefault(x => x.ParentId == item.FolderId) != null;
                });
                listCommonFolders.ForEach(item =>
                {
                    item.FileNumber = GetAllFile(item.FolderId, listCommonFolders, context.FileInFolder.ToList()).Count;
                });

                var listCommonFile = context.FileInFolder.ToList();

                #endregion

                #region Lấy list file theo folder 

                listFileResult = GetAllFile(folder.FolderId, listCommonFolders, listCommonFile);

                listFileResult.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                    x.FileFullName = $"{x.FileName}.{x.FileExtension}";
                    x.FileUrl = Path.Combine(webRootPath, folder.Url, x.FileFullName);
                });

                #endregion

                listFileResult = listFileResult.OrderBy(x => x.CreatedDate).ToList();

                listFileResult.ForEach(item =>
                {
                    totalSize += Convert.ToDecimal(item.Size);
                });


                return new UploadFileDocumentResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "successful",
                    ListFileInFolder = listFileResult,
                    ListFolders = listCommonFolders,
                    TotalSize = totalSize,
                };
            }
            catch (Exception e)
            {
                return new UploadFileDocumentResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public LoadFileByFolderResult LoadFileByFolder(LoadFileByFolderParameter parameter)
        {
            var listCommonFolders = context.Folder.Where(x => x.ObjectId == parameter.ObjectId && !x.FolderType.Contains("TASK_FILE"))
                .Select(y => new FolderEntityModel
                {
                    FolderId = y.FolderId,
                    ParentId = y.ParentId,
                    Name = y.Name,
                    //Active = y.Active,
                    //IsDelete = y.IsDelete,
                    //Url = y.Url,
                    //UpdatedDate = y.UpdatedDate,
                }).ToList();

            listCommonFolders.ForEach(item =>
            {
                item.HasChild = context.Folder.FirstOrDefault(x => x.ParentId == item.FolderId) != null;
            });

            var listCommonFile = context.FileInFolder.ToList();

            try
            {
                var listFileResult = new List<FileInFolderEntityModel>();

                var webRootPath = hostingEnvironment.WebRootPath + "\\";

                var folder = context.Folder.FirstOrDefault(x =>
                    x.FolderId == parameter.FolderId && x.FolderType == parameter.FolderType);

                #region Lấy list file theo ObjectId và folder

                listFileResult = GetAllFile(parameter.FolderId, listCommonFolders, listCommonFile);

                #endregion


                listFileResult.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                    x.FileFullName = $"{x.FileName}.{x.FileExtension}";
                    var folderUrl = context.Folder.FirstOrDefault(item => item.FolderId == x.FolderId)?.Url;
                    x.FileUrl = Path.Combine(webRootPath, folderUrl, x.FileFullName);
                });

                listFileResult = listFileResult.OrderBy(x => x.CreatedDate).ToList();


                //#region lay list folder theo parent ID

                //var listFolder = listCommonFolders.Where(x => x.ParentId == parameter.FolderId)
                //    .Select( y => new FolderEntityModel{ 
                //       Active = y.Active,
                //       FolderId = y.FolderId,
                //       FolderLevel = y.FolderLevel,
                //       ParentId = y.ParentId,
                //       Name = y.Name,
                //       IsDelete = y.IsDelete,
                //       Url = y.Url,
                //     }).ToList();

                //#endregion

                return new LoadFileByFolderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "successful",
                    ListFileInFolder = listFileResult,
                    //ListFolders = listFolder,
                };
            }
            catch (Exception e)
            {
                return new LoadFileByFolderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        private List<FileInFolderEntityModel> GetAllFile(Guid folderId, List<FolderEntityModel> listCommonFolders, List<FileInFolder> listCommonFile)
        {
            var listResult = new List<FileInFolderEntityModel>();

            var listFile = listCommonFile.Where(x => x.FolderId == folderId).ToList();
            var listAllUser = context.User.ToList();
            var listAllEmp = context.Employee.ToList();
            listFile.ForEach(item =>
            {
                var fileInFolder = new FileInFolderEntityModel
                {
                    FileInFolderId = item.FileInFolderId,
                    FolderId = item.FolderId,
                    FileName = item.FileName,
                    ObjectId = item.ObjectId,
                    ObjectType = item.ObjectType,
                    FileExtension = item.FileExtension,
                    Size = item.Size,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    UpdatedById = item.UpdatedById,
                    UpdatedDate = item.UpdatedDate
                };

                var empId = listAllUser.FirstOrDefault(x => x.UserId == item.CreatedById)?.EmployeeId;
                var empCode = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                var empName = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                fileInFolder.CreatedByName = $"{empCode ?? ""} - {empName ?? ""}";

                listResult.Add(fileInFolder);
            });

            var folder = listCommonFolders.FirstOrDefault(x => x.FolderId == folderId);

            if (folder != null && folder.HasChild)
            {
                var listFolderChild = listCommonFolders.Where(x => x.ParentId == folderId).ToList();

                listFolderChild.ForEach(item =>
                {
                    listResult.AddRange(GetAllFile(item.FolderId, listCommonFolders, listCommonFile));
                });
            }

            return listResult;
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
    }
}
