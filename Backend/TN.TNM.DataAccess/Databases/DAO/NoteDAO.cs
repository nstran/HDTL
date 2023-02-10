using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Results.Note;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Helper;
using TN.TNM.Common.NotificationSetting;
using System.Net;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class NoteDAO : BaseDAO, INoteDataAccess
    {
        private IHostingEnvironment _hostingEnvironment;
        public NoteDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public CreateNoteResult CreateNote(CreateNoteParameter parameter)
        {
            // Get Lead
            //var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
            // Create Note
            //if (lead != null)
            //{
            parameter.Note.ObjectId = parameter.LeadId.Value;
            parameter.Note.Active = true;
            parameter.Note.CreatedById = parameter.UserId;
            parameter.Note.CreatedDate = DateTime.Now;
            parameter.Note.ObjectType = "LEA";
            parameter.Note.NoteId = Guid.NewGuid();
            switch (parameter.Note.Type)
            {
                case "ADD":
                    parameter.Note.NoteTitle = "đã thêm ghi chú";
                    if (parameter.FileList != null)
                    {
                        parameter.Note.NoteTitle = "đã thêm tài liệu";
                        CreatedNoteDocument(parameter.Note.NoteId, parameter.LeadId.Value, parameter.UserId, parameter.FileList);
                    }
                    break;
                case "NEW":
                    parameter.Note.NoteTitle = "đã tạo";
                    break;
                case "EDT":
                    parameter.Note.Description = "";
                    break;
                case "UNF":
                    break;
                default:
                    parameter.Note.NoteTitle = "";
                    break;
            }
            //}
            context.Note.Add(parameter.Note);
            context.SaveChanges();

            return new CreateNoteResult
            {
                Status = true,
                Message = CommonMessage.Note.CREATE_SUCCESS
            };

        }
        /// <summary>
        /// DisableNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DisableNoteResult DisableNote(DisableNoteParameter parameter)
        {
            try
            {
                // Get note from noteId
                var note = context.Note.FirstOrDefault(n => n.NoteId == parameter.NoteId);
                if (note != null)
                {
                    var noteDocList = context.NoteDocument.Where(nd => nd.NoteId == note.NoteId).ToList();
                    noteDocList.ForEach(doc =>
                    {
                        context.NoteDocument.Remove(doc);
                    });

                    context.Note.Remove(note);
                    context.SaveChanges();
                }

                return new DisableNoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Note.DISABLE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new DisableNoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private void CreatedNoteDocument(Guid noteId, Guid leadId, Guid userId, List<IFormFile> fileList)
        {
            List<NoteDocument> docList = new List<NoteDocument>();
            string folderName = "FileUpload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            foreach (var file in fileList)
            {
                NoteDocument noteDoc = new NoteDocument()
                {
                    NoteDocumentId = Guid.NewGuid(),
                    NoteId = noteId,
                    DocumentName = file.FileName,
                    DocumentSize = file.Length.ToString(),
                    DocumentUrl = Path.Combine(newPath, file.FileName),
                    CreatedById = userId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };
                docList.Add(noteDoc);
            }

            if (docList.Count > 0)
            {
                docList.ForEach(dl => { context.NoteDocument.Add(dl); });
            }
        }

        public CreateNoteAndNoteDocumentResult CreateNoteAndNoteDocument(CreateNoteAndNoteDocumentParameter parameter)
        {
            var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
            List<NoteDocument> docList = new List<NoteDocument>();
            Note note = new Note();
            string folderName = "FileUpload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            // Create Note
            if (lead != null)
            {
                note.ObjectId = lead.LeadId;
                note.Active = true;

                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.ObjectType = "LEA";
                note.NoteId = Guid.NewGuid();
                note.Type = "UPF";
                note.NoteTitle = "đã thêm tài liệu";
                if (parameter.FileList != null && parameter.FileList.Count > 0)
                {
                    foreach (var file in parameter.FileList)
                    {
                        NoteDocument noteDoc = new NoteDocument()
                        {
                            NoteDocumentId = Guid.NewGuid(),
                            NoteId = note.NoteId,
                            DocumentName = file.FileName,
                            DocumentSize = file.Length.ToString(),
                            DocumentUrl = Path.Combine(newPath, file.FileName),
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            Active = true
                        };
                        docList.Add(noteDoc);
                    }
                }
            }
            context.Note.Add(note);
            if (docList.Count > 0)
            {
                docList.ForEach(dl => { context.NoteDocument.Add(dl); });
            }
            context.SaveChanges();

            return new CreateNoteAndNoteDocumentResult()
            {
                Status = true
            };
        }

        public EditNoteByIdResult EditNoteById(EditNoteByIdParameter parameter)
        {
            var note = context.Note.FirstOrDefault(n => n.NoteId == parameter.NoteId);
            if (note != null)
            {
                note.Description = parameter.NoteDescription;
                note.UpdatedById = parameter.UserId;
                note.UpdatedDate = DateTime.Now;

                if (parameter.FileList != null)
                {
                    CreatedNoteDocument(parameter.NoteId, parameter.LeadId, parameter.UserId, parameter.FileList);
                }
                context.Note.Update(note);
                context.SaveChanges();
            }

            return new EditNoteByIdResult()
            {
                Status = true,
                Message = CommonMessage.Note.EDIT_SUCCESS
            };
        }

        public SearchNoteResult SearchNote(SearchNoteParameter parameter)
        {
            var noteList = context.Note.Where(n => (n.Description.Contains(parameter.Keyword) || parameter.Keyword == null || parameter.Keyword == "") &&
                                                   (parameter.FromDate == null || n.CreatedDate >= parameter.FromDate.Value) &&
                                                   (parameter.ToDate == null || n.CreatedDate <= parameter.ToDate.Value) &&
                                                   n.Active == true &&
                                                   n.ObjectId == parameter.LeadId)
                .Select(s => new NoteEntityModel()
                {
                    Active = s.Active,
                    NoteId = s.NoteId,
                    NoteTitle = s.NoteTitle,
                    CreatedById = s.CreatedById,
                    CreatedDate = s.CreatedDate,
                    Description = s.Description,
                    ObjectType = s.ObjectType,
                    Type = s.Type,
                    UpdatedById = s.UpdatedById,
                    UpdatedDate = s.UpdatedDate,
                    ObjectId = s.ObjectId,
                    NoteDocList = context.NoteDocument.Where(nd => nd.NoteId == s.NoteId).Select(nd => new NoteDocumentEntityModel()
                    {
                        DocumentName = nd.DocumentName,
                        DocumentSize = nd.DocumentSize,
                        DocumentUrl = nd.DocumentUrl,
                        CreatedById = nd.CreatedById,
                        NoteDocumentId = nd.NoteDocumentId,
                        NoteId = nd.NoteId
                    }).ToList()
                })
                .OrderByDescending(n => n.CreatedDate).ToList();
            foreach (var note in noteList)
            {
                var empId = context.User.FirstOrDefault(e => e.UserId == note.CreatedById)?.EmployeeId;
                note.ResponsibleName = context.Employee.FirstOrDefault(e => e.EmployeeId == empId)
                    ?.EmployeeName;
                note.ResponsibleAvatar = context.Contact.FirstOrDefault(c => c.ObjectId == empId)?.AvatarUrl;
                foreach (var noteDoc in note.NoteDocList)
                {
                    Byte[] bytes = File.ReadAllBytes(noteDoc.DocumentUrl);
                    String base64File = Convert.ToBase64String(bytes);
                    bool isImageOrPdf = (noteDoc.DocumentUrl.IndexOf(".jpg", StringComparison.Ordinal) > 0 ||
                                         noteDoc.DocumentUrl.IndexOf(".jpeg", StringComparison.Ordinal) > 0 ||
                                         noteDoc.DocumentUrl.IndexOf(".png", StringComparison.Ordinal) > 0 ||
                                         noteDoc.DocumentUrl.IndexOf(".gif", StringComparison.Ordinal) > 0 ||
                                         noteDoc.DocumentUrl.IndexOf(".pdf", StringComparison.Ordinal) > 0);
                    noteDoc.Base64Url = isImageOrPdf ? base64File : "";
                }
            }
            return new SearchNoteResult()
            {
                Status = noteList.Count > 0,
                NoteList = noteList.Count > 0 ? noteList : null,
                Message = noteList.Count > 0 ? "" : CommonMessage.Note.NO_NOTE
            };
        }

        public CreateNoteForCustomerDetailResult CreateNoteForCustomerDetail(CreateNoteForCustomerDetailParameter parameter)
        {
            try
            {
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    var list_note_document_old =
                        context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument();
                            noteDocument.NoteDocumentId = Guid.NewGuid();
                            noteDocument.NoteId = old_note.NoteId;
                            noteDocument.DocumentName = item.DocumentName;
                            noteDocument.DocumentSize = item.DocumentSize;
                            noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            noteDocument.Active = true;
                            noteDocument.CreatedById = parameter.UserId;
                            noteDocument.CreatedDate = DateTime.Now;

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "CUS" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }
                   
                    return new CreateNoteForCustomerDetailResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note();
                        note.NoteId = Guid.NewGuid();
                        note.Description = parameter.Note.Description;
                        note.Type = "ADD";
                        note.ObjectId = parameter.Note.ObjectId;
                        note.ObjectType = "CUS";
                        note.Active = true;
                        note.CreatedById = parameter.UserId;
                        note.CreatedDate = DateTime.Now;
                        note.NoteTitle = "đã thêm ghi chú";

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "CUS" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Gửi mail thông báo

                        var customer = context.Customer.FirstOrDefault(x => x.CustomerId == note.ObjectId);
                        var cusStatus = context.Category.FirstOrDefault(x => x.CategoryId == customer.StatusId);
                        if (customer != null)
                        {
                            // Nếu là khách hàng tiềm năng
                            if (cusStatus.CategoryCode == "MOI")
                            {
                                NotificationHelper.AccessNotification(context, "POTENTIAL_CUSTOMER_DETAIL", "COMM", new Customer(), customer, true, note);
                            }
                            // Nếu là khác hàng định danh
                            else if (cusStatus.CategoryCode == "HDO")
                            {
                                NotificationHelper.AccessNotification(context, "CUSTOMER_DETAIL", "COMM", new Customer(), customer, true, note);
                            }
                        }

                        #endregion
                      
                        return new CreateNoteForCustomerDetailResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            MessageCode = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new CreateNoteForCustomerDetailResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForLeadDetailResult CreateNoteForLeadDetail(CreateNoteForLeadDetailParameter parameter)
        {
            try
            {
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument();
                            noteDocument.NoteDocumentId = Guid.NewGuid();
                            noteDocument.NoteId = old_note.NoteId;
                            noteDocument.DocumentName = item.DocumentName;
                            noteDocument.DocumentSize = item.DocumentSize;
                            noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            noteDocument.Active = true;
                            noteDocument.CreatedById = parameter.UserId;
                            noteDocument.CreatedDate = DateTime.Now;

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "LEA" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForLeadDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note();
                        note.NoteId = Guid.NewGuid();
                        note.Description = parameter.Note.Description;
                        note.Type = "ADD";
                        note.ObjectId = parameter.Note.ObjectId;
                        note.ObjectType = "LEA";
                        note.Active = true;
                        note.CreatedById = parameter.UserId;
                        note.CreatedDate = DateTime.Now;
                        note.NoteTitle = parameter.Note.NoteTitle;

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "LEA" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        #region Tạo thông báo

                        var lead = context.Lead.FirstOrDefault(x => x.LeadId == note.ObjectId);
                        if (lead != null)
                        {
                            NotificationHelper.AccessNotification(context, "LEAD_DETAIL", "COMM",
                                new Lead(), lead, true, note);
                        }

                        var task = context.Task.FirstOrDefault(x => x.TaskId == note.ObjectId);
                        if (task != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectTaskDetail, "COMM",
                                new Task(), task, true, note);
                        }

                        #endregion

                        return new CreateNoteForLeadDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new CreateNoteForLeadDetailResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateNoteForOrderDetailResult CreateNoteForOrderDetail(CreateNoteForOrderDetailParameter parameter)
        {
            try
            {
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, parameter.Note.ObjectType);

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument();
                            noteDocument.NoteDocumentId = Guid.NewGuid();
                            noteDocument.NoteId = old_note.NoteId;
                            noteDocument.DocumentName = item.DocumentName;
                            noteDocument.DocumentSize = item.DocumentSize;
                            noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            noteDocument.Active = true;
                            noteDocument.CreatedById = parameter.UserId;
                            noteDocument.CreatedDate = DateTime.Now;
                            noteDocument.UpdatedById = item.UpdatedById;
                            noteDocument.UpdatedDate = item.UpdatedDate;

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "ORDER" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForOrderDetailResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        NoteId = parameter.Note.NoteId,
                        MessageCode = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note();
                        note.NoteId = Guid.NewGuid();
                        note.Description = parameter.Note.Description;
                        note.Type = "ADD";
                        note.ObjectId = parameter.Note.ObjectId;
                        note.ObjectType = parameter.Note.ObjectType;
                        note.Active = true;
                        note.CreatedById = parameter.UserId;
                        note.CreatedDate = DateTime.Now;
                        note.NoteTitle = parameter.Note.NoteTitle;

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, note.ObjectType);

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "ORDER" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo

                        var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == note.ObjectId);
                        if (customerOrder != null)
                        {
                            NotificationHelper.AccessNotification(context, "CUSTOMER_ORDER_DETAIL", "COMM",
                                new CustomerOrder(), customerOrder, true, note);
                        }

                        var procurementRequest = context.ProcurementRequest.FirstOrDefault(x => x.ProcurementRequestId == note.ObjectId);
                        if (procurementRequest != null)
                        {
                            NotificationHelper.AccessNotification(context, "PROCUREMENT_REQUEST_DETAIL", "COMM",
                                new ProcurementRequest(), procurementRequest, true, note);
                        }

                        #endregion

                        return new CreateNoteForOrderDetailResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            NoteId = note.NoteId,
                            MessageCode = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new CreateNoteForOrderDetailResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForQuoteDetailResult CreateNoteForQuoteDetail(CreateNoteForQuoteDetailParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    // lấy ra các note cũ
                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, "QUOTE");

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = old_note.NoteId,
                                DocumentName = item.DocumentName,
                                DocumentSize = item.DocumentSize,
                                DocumentUrl = Path.Combine(newPath, item.DocumentName),
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate
                            };

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "QUOTE" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForQuoteDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, "Quote");

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "QUOTE" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo

                        var quote = context.Quote.FirstOrDefault(x => x.QuoteId == note.ObjectId);
                        if (quote != null)
                        {
                            NotificationHelper.AccessNotification(context, "QUOTE_DETAIL", "COMM",
                                new Quote(), quote, true, note);
                        }

                        #endregion

                        return new CreateNoteForQuoteDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForQuoteDetailResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }



        public CreateNoteForSaleBiddingDetailResult CreateNoteForSaleBiddingDetail(CreateNoteForSaleBiddingDetailParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    // lấy ra các note cũ
                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, "HST");

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = old_note.NoteId,
                                DocumentName = item.DocumentName,
                                DocumentSize = item.DocumentSize,
                                DocumentUrl = Path.Combine(newPath, item.DocumentName),
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate
                            };

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "HST" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForSaleBiddingDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = "HST",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, "HST");

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "HST" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo

                        var saleBidding = context.SaleBidding.FirstOrDefault(x => x.SaleBiddingId == note.ObjectId);
                        if (saleBidding != null)
                        {
                            NotificationHelper.AccessNotification(context, "SALE_BIDDING_DETAIL", "COMM",
                                new SaleBidding(), saleBidding, true, note);
                        }

                        #endregion

                        return new CreateNoteForSaleBiddingDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForSaleBiddingDetailResult()
                {
                    Status = false,
                    Message = ex.Message
                };

            }
        }
        public CreateNoteForContractResult CreateNoteForContract(CreateNoteForContractParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    // lấy ra các note cũ
                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, "CONTRACT");

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = old_note.NoteId,
                                DocumentName = item.DocumentName,
                                DocumentSize = item.DocumentSize,
                                DocumentUrl = Path.Combine(newPath, item.DocumentName),
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate
                            };

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "CONTRACT" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForContractResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = "CONTRACT",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, "CONTRACT");

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "CONTRACT" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();
                        #region Tạo thông báo

                        var contract = context.Contract.FirstOrDefault(x => x.ContractId == note.ObjectId);
                        if (contract != null)
                        {
                            switch (parameter.ObjType)
                            {
                                case "CHO":
                                    NotificationHelper.AccessNotification(context, TypeModel.ContractDetail, "GPD", new Contract(),
                                        contract, true, note, empId: contract.EmployeeId);
                                    break;
                                case "REJECT":
                                    NotificationHelper.AccessNotification(context, TypeModel.ContractDetail, "TCPD", new Contract(),
                                        contract, true, note, empId: contract.EmployeeId);
                                    break;
                                default:
                                    NotificationHelper.AccessNotification(context, TypeModel.ContractDetail, "COMM",
                               new Contract(), contract, true, note);
                                    break;
                            }
                        }

                        #endregion

                        return new CreateNoteForContractResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForContractResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public CreateNoteForBillSaleDetailResult CreateNoteForBillSaleDetail(CreateNoteForBillSaleDetailParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    // lấy ra các note cũ
                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, "BILL");

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = old_note.NoteId,
                                DocumentName = item.DocumentName,
                                DocumentSize = item.DocumentSize,
                                DocumentUrl = Path.Combine(newPath, item.DocumentName),
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate
                            };

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "BILL" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForBillSaleDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = "BILL",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, "BILL");

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "BILL" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo

                        var billOfSale = context.BillOfSale.FirstOrDefault(x => x.BillOfSaLeId == note.ObjectId);
                        if (billOfSale != null)
                        {
                            NotificationHelper.AccessNotification(context, "BILL_SALE_DETAIL", "COMM",
                                new BillOfSale(), billOfSale, true, note);
                        }

                        #endregion

                        return new CreateNoteForBillSaleDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForBillSaleDetailResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public CreateNoteForObjectResult CreateNoteForObject(CreateNoteForObjectParameter parameter)
        {
            var listFileDelete = new List<string>();
            try
            {
                #region Tạo hoặc Cập nhật Note

                //Lấy folder
                var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);
                if (folder == null)
                {
                    return new CreateNoteForObjectResult()
                    {
                        Status = false,
                        Message = "Thư mục để lưu file không tồn tại"
                    };
                }

                var listNote = new List<NoteEntityModel>();

                //Update Note
                if (parameter.Note.NoteId != null && parameter.Note.NoteId != Guid.Empty)
                {
                    var note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    note.Description = parameter.Note.Description;
                    context.Note.Update(note);

                    #region Cập nhật lại list File

                    //Xóa list File hiện tại
                    var listFile = context.FileInFolder
                        .Where(x => x.ObjectId == parameter.Note.NoteId && x.ObjectType == "NOTE").ToList();
                    context.FileInFolder.RemoveRange(listFile);

                    //Thêm list File mới
                    var listFileResult = new List<FileInFolderEntityModel>();
                    bool isSave = true;
                    parameter.ListFile.ForEach(_file =>
                    {
                        string folderName = ConvertFolderUrl(folder.Url);
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

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
                                FileInFolderId = Guid.NewGuid(),
                                FileName = _file.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = folder.FolderId,
                                ObjectId = note.NoteId,
                                ObjectType = _file.FileInFolder.ObjectType,
                                Size = _file.FileInFolder.Size,
                                FileExtension =
                                    _file.FileSave.FileName.Substring(_file.FileSave.FileName.LastIndexOf(".") + 1)
                            };
                            context.FileInFolder.Add(file);
                            listFileResult.Add(new FileInFolderEntityModel()
                            {
                                Active = true,
                                FileInFolderId = file.FileInFolderId,
                                FileName = file.FileName,
                                FolderId = file.FolderId,
                                ObjectId = file.ObjectId,
                                ObjectType = file.ObjectType,
                                Size = file.Size,
                                FileExtension = file.FileExtension
                            });

                            string fileName = file.FileName + "." + file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    _file.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });

                    if (!isSave)
                    {
                        listFileDelete.ForEach(item =>
                        {
                            Directory.Delete(item);
                        });

                        return new CreateNoteForObjectResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }

                    #endregion

                    context.SaveChanges();

                    #region Lấy list ghi chú

                    listNote = context.Note
                        .Where(x => x.ObjectId == parameter.Note.ObjectId &&
                                    x.ObjectType == parameter.Note.ObjectType && x.Active == true)
                        .Select(
                            y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>(),
                                ListFile = new List<FileInFolderEntityModel>()
                            }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var _listAllEmployee = context.Employee.ToList();
                        var _listFile = context.FileInFolder
                            .Where(x => listNoteId.Contains(x.ObjectId.Value) && x.ObjectType == "NOTE").Select(
                                y => new FileInFolderEntityModel
                                {
                                    FileInFolderId = y.FileInFolderId,
                                    FolderId = y.FolderId,
                                    ObjectId = y.ObjectId,
                                    FileName = y.FileName,
                                    Size = y.Size,
                                    FileExtension = y.FileExtension,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate
                                }).ToList();

                        listNote.ForEach(item =>
                        {
                            var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.ListFile = _listFile.Where(x => x.ObjectId.Value == item.NoteId)
                                .OrderByDescending(z => z.CreatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    #endregion
                }
                //Create Note
                else
                {
                    var note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.Description = parameter.Note.Description;
                    note.Type = "ADD";
                    note.ObjectId = parameter.Note.ObjectId;
                    note.ObjectType = parameter.Note.ObjectType;
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.NoteTitle = "Đã thêm ghi chú";

                    context.Note.Add(note);

                    #region Thêm list File

                    //Thêm list File mới
                    var listFileResult = new List<FileInFolderEntityModel>();
                    bool isSave = true;
                    parameter.ListFile.ForEach(_file =>
                    {
                        string folderName = ConvertFolderUrl(folder.Url);
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

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
                                FileInFolderId = Guid.NewGuid(),
                                FileName = _file.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = folder.FolderId,
                                ObjectId = note.NoteId,
                                ObjectType = _file.FileInFolder.ObjectType,
                                Size = _file.FileInFolder.Size,
                                FileExtension =
                                    _file.FileSave.FileName.Substring(_file.FileSave.FileName.LastIndexOf(".") + 1)
                            };
                            context.FileInFolder.Add(file);
                            listFileResult.Add(new FileInFolderEntityModel()
                            {
                                Active = true,
                                FileInFolderId = file.FileInFolderId,
                                FileName = file.FileName,
                                FolderId = file.FolderId,
                                ObjectId = file.ObjectId,
                                ObjectType = file.ObjectType,
                                Size = file.Size,
                                FileExtension = file.FileExtension
                            });

                            string fileName = file.FileName + "." + file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    _file.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });

                    if (!isSave)
                    {
                        listFileDelete.ForEach(item =>
                        {
                            Directory.Delete(item);
                        });

                        return new CreateNoteForObjectResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }

                    #endregion

                    context.SaveChanges();

                    #region Gửi mail thông báo

                    var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == note.ObjectId);
                    if (vendorOrder != null)
                    {
                        NotificationHelper.AccessNotification(context, "VENDOR_ORDER_DETAIL", "COMM",
                            new VendorOrder(), vendorOrder, true, note);
                    }

                    #endregion

                    #region Lấy list ghi chú

                    listNote = context.Note
                        .Where(x => x.ObjectId == parameter.Note.ObjectId &&
                                    x.ObjectType == parameter.Note.ObjectType && x.Active == true)
                        .Select(
                            y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>(),
                                ListFile = new List<FileInFolderEntityModel>()
                            }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var _listAllEmployee = context.Employee.ToList();
                        var listFile = context.FileInFolder
                            .Where(x => listNoteId.Contains(x.ObjectId.Value) && x.ObjectType == "NOTE").Select(
                                y => new FileInFolderEntityModel
                                {
                                    FileInFolderId = y.FileInFolderId,
                                    FolderId = y.FolderId,
                                    ObjectId = y.ObjectId,
                                    FileName = y.FileName,
                                    Size = y.Size,
                                    FileExtension = y.FileExtension,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate
                                }).ToList();

                        listNote.ForEach(item =>
                        {
                            var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.ListFile = listFile.Where(x => x.ObjectId.Value == item.NoteId)
                                .OrderByDescending(z => z.CreatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    #endregion
                }

                #endregion

                return new CreateNoteForObjectResult()
                {
                    Status = true,
                    Message = "Success",
                    ListNote = listNote
                };
            }
            catch (Exception e)
            {
                listFileDelete.ForEach(item =>
                {
                    Directory.Delete(item);
                });

                return new CreateNoteForObjectResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
        public CreateNoteForProjectDetailResult CreateNoteForProjectDetail(CreateNoteForProjectDetailParameter parameter)
        {
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new CreateNoteForProjectDetailResult()
                {
                    Status = false,
                    Message = "Thư mục upload không tồn tại"
                };
            }

            var listFileDelete = new List<string>();
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == old_note.ObjectId);
                    if (project != null)
                    {
                        project.UpdateDate = DateTime.Now;
                        project.UpdateBy = parameter.UserId;
                        context.Update(project);
                        context.SaveChanges();
                    }

                    // lấy ra các note cũ
                    // var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    // context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument?.Count > 0)
                    {
                        var isSave = true;
                        parameter.ListNoteDocument?.ForEach(item =>
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
                                    ObjectId = old_note.NoteId,
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

                            return new CreateNoteForProjectDetailResult()
                            {
                                Status = false,
                                Message = "Bạn phải cấu hình thư mục để lưu"
                            };
                        }
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "Project" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid) x.ObjectId))
                            .ToList();
                        var listNoteDocument = new List<NoteDocumentEntityModel>();

                        listFileInFolder.ForEach(item =>
                        {
                            var file = new NoteDocumentEntityModel
                            {
                                DocumentName = item.FileName,
                                DocumentSize = item.Size,
                                CreatedById = item.CreatedById,
                                CreatedDate = item.CreatedDate,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate,
                                NoteDocumentId = item.FileInFolderId,
                                NoteId = (Guid)item.ObjectId
                            };

                            var fileName = $"{item.FileName}.{item.FileExtension}";
                            var folderName = ConvertFolderUrl(folder.Url);
                            var webRootPath = _hostingEnvironment.WebRootPath;
                            file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                            listNoteDocument.Add(file);
                        });
                        
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForProjectDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.Note.ObjectId);
                        if (project != null)
                        {
                            project.UpdateDate = DateTime.Now;
                            project.UpdateBy = parameter.UserId;
                            context.Update(project);
                            context.SaveChanges();
                        }

                        if (parameter.ListNoteDocument?.Count > 0)
                        {
                            var isSave = true;
                            parameter.ListNoteDocument?.ForEach(item =>
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
                                        ObjectId = note.NoteId,
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

                                return new CreateNoteForProjectDetailResult()
                                {
                                    Status = false,
                                    Message = "Bạn phải cấu hình thư mục để lưu"
                                };
                            }
                        }

                        context.SaveChanges();

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "Project" && x.Active == true)
                            .Select(y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>()
                            }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();

                            var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid)x.ObjectId))
                                .ToList();
                            var listNoteDocument = new List<NoteDocumentEntityModel>();

                            listFileInFolder.ForEach(item =>
                            {
                                var file = new NoteDocumentEntityModel
                                {
                                    DocumentName = item.FileName.Substring(0, item.FileName.LastIndexOf("_", StringComparison.Ordinal)),
                                    DocumentSize = item.Size,
                                    CreatedById = item.CreatedById,
                                    CreatedDate = item.CreatedDate,
                                    UpdatedById = item.UpdatedById,
                                    UpdatedDate = item.UpdatedDate,
                                    NoteDocumentId = item.FileInFolderId,
                                    NoteId = (Guid)item.ObjectId
                                };

                                var fileName = $"{item.FileName}.{item.FileExtension}";
                                var folderName = ConvertFolderUrl(folder.Url);
                                var webRootPath = _hostingEnvironment.WebRootPath;
                                file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                                listNoteDocument.Add(file);
                            });


                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo


                        if (project != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectDetail, "COMM",
                                new Project(), project, true, note);
                        }

                        #endregion

                        return new CreateNoteForProjectDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForProjectDetailResult()
                {
                    Status = false,
                    Message = ex.Message + " " + ex.InnerException
                };
            }
        }

        public DeleteNoteDocumentResult DeleteNoteDocument(DeleteNoteDocumentParameter parameter)
        {
            try
            {
                var noteDocument = context.NoteDocument.FirstOrDefault(x => x.NoteId == parameter.NoteId && x.NoteDocumentId == parameter.NoteDocumentId);
                if (noteDocument != null)
                {
                    context.NoteDocument.Remove(noteDocument);
                    context.SaveChanges();

                    return new DeleteNoteDocumentResult()
                    {
                        Status = true,
                        Message = "Xóa file thành công"
                    };
                }
                else
                {
                    return new DeleteNoteDocumentResult()
                    {
                        Status = false,
                        Message = "File không tồn tại trong hệ thống"
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteNoteDocumentResult()
                {
                    Status = false,
                    Message = e.Message
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

        #region Project Resource - Project Scope
        public CreateNoteForProjectResourceResult CreateNoteForProjectResource(CreateNoteForProjectResourceParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project resource
                    var projectResource = context.ProjectResource.FirstOrDefault(x => x.ProjectResourceId == old_note.ObjectId);
                    if (projectResource != null)
                    {
                        projectResource.UpdateDate = DateTime.Now;
                        projectResource.UpdateBy = parameter.Note.UpdatedById;
                        context.Update(projectResource);
                        context.SaveChanges();
                    }
                    context.SaveChanges();

                    return new CreateNoteForProjectResourceResult()
                    {
                        Status = true,
                        Message = "Success"
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "NEW",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        var projectResource = context.ProjectResource.FirstOrDefault(x => x.ProjectResourceId == parameter.Note.ObjectId);
                        if (projectResource != null)
                        {
                            projectResource.UpdateDate = DateTime.Now;
                            projectResource.UpdateBy = parameter.Note.UpdatedById;
                            context.Update(projectResource);
                            context.SaveChanges();
                        }
                        
                        transaction.Commit();
                        #region Tạo thông báo
                        if (projectResource != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectResource, "COMM",
                                new ProjectResource(), projectResource, true, note);
                        }

                        #endregion

                        return new CreateNoteForProjectResourceResult()
                        {
                            Status = true,
                            Message = "Success",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForProjectResourceResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
        public CreateNoteForProjectScopeResult CreateNoteForProjectScope(CreateNoteForProjectScopeParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project resource
                    var projectScope = context.ProjectScope.FirstOrDefault(x => x.ProjectId == old_note.ObjectId);
                    if (projectScope != null)
                    {
                        projectScope.UpdateDate = DateTime.Now;
                        projectScope.UpdateBy = parameter.Note.UpdatedById;
                        context.Update(projectScope);
                        context.SaveChanges();
                    }
                    context.SaveChanges();                 

                    return new CreateNoteForProjectScopeResult()
                    {
                        Status = true,
                        Message = "Success"
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "NEW",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        var projectScope = context.ProjectScope.FirstOrDefault(x => x.ProjectId == parameter.Note.ObjectId);
                        if (projectScope != null)
                        {
                            projectScope.UpdateDate = DateTime.Now;
                            projectScope.UpdateBy = parameter.Note.UpdatedById;
                            context.Update(projectScope);
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        #region Tạo thông báo
                        if (projectScope != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectScope, "COMM",
                                new ProjectScope(), projectScope, true, note);
                        }

                        #endregion

                        return new CreateNoteForProjectScopeResult()
                        {
                            Status = true,
                            Message = "Success"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForProjectScopeResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        #endregion
        public CreateNoteForProductionOrderDetailResult CreateNoteForProductionOrderDetail(CreateNoteForProductionOrderDetailParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    // lấy ra các note cũ
                    var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument.Count > 0)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        newPath = Path.Combine(newPath, "PROOR");// ProductionOrder

                        var listDocument = new List<NoteDocument>();
                        parameter.ListNoteDocument.ForEach(item =>
                        {
                            var noteDocument = new NoteDocument
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = old_note.NoteId,
                                DocumentName = item.DocumentName,
                                DocumentSize = item.DocumentSize,
                                DocumentUrl = Path.Combine(newPath, item.DocumentName),
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate
                            };

                            if (item.UpdatedById == null)
                            {
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;
                            }

                            listDocument.Add(noteDocument);
                        });

                        context.NoteDocument.AddRange(listDocument);
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();
                    // PROOR- ProductionOrder
                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "PROOR" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForProductionOrderDetailResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note()
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = "PROOR", // ProductionOrder
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        if (parameter.ListNoteDocument.Count > 0)
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);

                            newPath = Path.Combine(newPath, "ProductionOrder");

                            var listDocument = new List<NoteDocument>();
                            parameter.ListNoteDocument.ForEach(item =>
                            {
                                var noteDocument = new NoteDocument();
                                noteDocument.NoteDocumentId = Guid.NewGuid();
                                noteDocument.NoteId = note.NoteId;
                                noteDocument.DocumentName = item.DocumentName;
                                noteDocument.DocumentSize = item.DocumentSize;
                                noteDocument.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                                noteDocument.Active = true;
                                noteDocument.CreatedById = parameter.UserId;
                                noteDocument.CreatedDate = DateTime.Now;
                                noteDocument.UpdatedById = parameter.UserId;
                                noteDocument.UpdatedDate = DateTime.Now;

                                listDocument.Add(noteDocument);
                            });

                            context.NoteDocument.AddRange(listDocument);
                            context.SaveChanges();
                        }

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "PROOR" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();
                            var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                                y => new NoteDocumentEntityModel
                                {
                                    DocumentName = y.DocumentName,
                                    DocumentSize = y.DocumentSize,
                                    DocumentUrl = y.DocumentUrl,
                                    CreatedById = y.CreatedById,
                                    CreatedDate = y.CreatedDate,
                                    UpdatedById = y.UpdatedById,
                                    UpdatedDate = y.UpdatedDate,
                                    NoteDocumentId = y.NoteDocumentId,
                                    NoteId = y.NoteId
                                }).ToList();
                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        return new CreateNoteForProductionOrderDetailResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForProductionOrderDetailResult()
                {
                    Status = false,
                    Message = ex.Message
                };

            }
        }

        public CreateNoteTaskResult CreateNoteTask(CreateNoteTaskParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project resource
                    var task = context.Task.FirstOrDefault(x => x.TaskId == old_note.ObjectId);
                    if (task != null)
                    {
                        task.UpdateDate = DateTime.Now;
                        task.UpdateBy = parameter.UserId;
                        context.Task.Update(task);
                        context.SaveChanges();
                    }
                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();
                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == "TAS" && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteTaskResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();
                        var task = context.Task.FirstOrDefault(x => x.TaskId == parameter.Note.ObjectId);
                        if (task != null)
                        {
                            task.UpdateDate = DateTime.Now;
                            task.UpdateBy = parameter.Note.UpdatedById;
                            context.Update(task);
                            context.SaveChanges();
                        }
                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == "TAS" && x.Active == true).Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                        }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();

                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();
                        #region Tạo thông báo
                        if (task != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectTaskDetail, "COMM",
                                new Task(), task, true, note);
                        }
                        #endregion

                        return new CreateNoteTaskResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteTaskResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public CreateNoteMilestoneResult CreateNoteMilestone(CreateNoteMilestoneParameter parameter)
        {
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project resource
                    var task = context.Task.FirstOrDefault(x => x.TaskId == old_note.ObjectId);
                    if (task != null)
                    {
                        task.UpdateDate = DateTime.Now;
                        task.UpdateBy = parameter.UserId;
                        context.Task.Update(task);
                        context.SaveChanges();
                    }
                    context.SaveChanges();

                    return new CreateNoteMilestoneResult()
                    {
                        Status = true,
                        Message = "Success"
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.Note.ObjectType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();
                        var task = context.Task.FirstOrDefault(x => x.ProjectId == parameter.Note.ObjectId);
                        if (task != null)
                        {
                            task.UpdateDate = DateTime.Now;
                            task.UpdateBy = parameter.Note.UpdatedById;
                            context.Update(task);
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        #region Tạo thông báo
                        if (task != null)
                        {
                            NotificationHelper.AccessNotification(context, TypeModel.ProjectTask, "COMM",
                                new Task(), task, true, note);
                        }
                        #endregion

                        return new CreateNoteMilestoneResult()
                        {
                            Status = true,
                            Message = "Success"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteMilestoneResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
        public CreateNoteForAllRecruitmentCampaignResult CreateDocumentRecruitmentCampaign(CreateNoteForAllRecruitmentCampaignParameter parameter)
        {
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.ObjType);

            if (folder == null)
            {
                return new CreateNoteForAllRecruitmentCampaignResult()
                {
                    Status = false,
                    Message = "Thư mục upload không tồn tại"
                };
            }
            var listFileDelete = new List<string>();
            try
            {
                // Update note
                if (parameter.Note.NoteId != Guid.Empty)
                {
                    var old_note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);

                    old_note.Description = parameter.Note.Description;
                    old_note.UpdatedById = parameter.UserId;
                    old_note.UpdatedDate = DateTime.Now;
                    context.Note.Update(old_note);

                    //update ngay cap nhat project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == old_note.ObjectId);
                    if (project != null)
                    {
                        project.UpdateDate = DateTime.Now;
                        project.UpdateBy = parameter.UserId;
                        context.Update(project);
                        context.SaveChanges();
                    }

                    // lấy ra các note cũ
                    // var list_note_document_old = context.NoteDocument.Where(x => x.NoteId == parameter.Note.NoteId).ToList();

                    // xóa các file đính kèm trong note cũ trong database
                    // context.NoteDocument.RemoveRange(list_note_document_old);

                    if (parameter.ListNoteDocument?.Count > 0)
                    {
                        var isSave = true;
                        parameter.ListNoteDocument?.ForEach(item =>
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
                                    ObjectId = old_note.NoteId,
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

                            return new CreateNoteForAllRecruitmentCampaignResult()
                            {
                                Status = false,
                                Message = "Bạn phải cấu hình thư mục để lưu"
                            };
                        }
                    }

                    context.SaveChanges();

                    var listNote = new List<NoteEntityModel>();

                    listNote = context.Note.Where(x => x.ObjectId == old_note.ObjectId && x.ObjectType == parameter.ObjType && x.Active == true).Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var listEmployee = context.Employee.ToList();
                        var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid)x.ObjectId))
                            .ToList();
                        var listNoteDocument = new List<NoteDocumentEntityModel>();

                        listFileInFolder.ForEach(item =>
                        {
                            var file = new NoteDocumentEntityModel
                            {
                                DocumentName = item.FileName,
                                DocumentSize = item.Size,
                                CreatedById = item.CreatedById,
                                CreatedDate = item.CreatedDate,
                                UpdatedById = item.UpdatedById,
                                UpdatedDate = item.UpdatedDate,
                                NoteDocumentId = item.FileInFolderId,
                                NoteId = (Guid)item.ObjectId
                            };

                            var fileName = $"{item.FileName}.{item.FileExtension}";
                            var folderName = ConvertFolderUrl(folder.Url);
                            var webRootPath = _hostingEnvironment.WebRootPath;
                            file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                            listNoteDocument.Add(file);
                        });

                        listNote.ForEach(item =>
                        {
                            var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                            item.ResponsibleName = employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    return new CreateNoteForAllRecruitmentCampaignResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListNote = listNote
                    };
                }
                else
                {
                    // Tạo note mới
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = parameter.Note.Description,
                            Type = "ADD",
                            ObjectId = parameter.Note.ObjectId,
                            ObjectType = parameter.ObjType,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = parameter.Note.NoteTitle
                        };

                        context.Note.Add(note);
                        context.SaveChanges();

                        var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.Note.ObjectId);
                        if (project != null)
                        {
                            project.UpdateDate = DateTime.Now;
                            project.UpdateBy = parameter.UserId;
                            context.Update(project);
                            context.SaveChanges();
                        }

                        if (parameter.ListNoteDocument?.Count > 0)
                        {
                            var isSave = true;
                            parameter.ListNoteDocument?.ForEach(item =>
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
                                        ObjectId = note.NoteId,
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

                                return new CreateNoteForAllRecruitmentCampaignResult()
                                {
                                    Status = false,
                                    Message = "Bạn phải cấu hình thư mục để lưu"
                                };
                            }
                        }

                        context.SaveChanges();

                        var listNote = new List<NoteEntityModel>();

                        listNote = context.Note.Where(x => x.ObjectId == note.ObjectId && x.ObjectType == parameter.ObjType && x.Active == true)
                            .Select(y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>()
                            }).ToList();

                        if (listNote.Count > 0)
                        {
                            var listNoteId = listNote.Select(x => x.NoteId).ToList();
                            var listUser = context.User.ToList();
                            var listEmployee = context.Employee.ToList();

                            var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid)x.ObjectId))
                                .ToList();
                            var listNoteDocument = new List<NoteDocumentEntityModel>();

                            listFileInFolder.ForEach(item =>
                            {
                                var file = new NoteDocumentEntityModel
                                {
                                    DocumentName = item.FileName.Substring(0, item.FileName.LastIndexOf("_", StringComparison.Ordinal)),
                                    DocumentSize = item.Size,
                                    CreatedById = item.CreatedById,
                                    CreatedDate = item.CreatedDate,
                                    UpdatedById = item.UpdatedById,
                                    UpdatedDate = item.UpdatedDate,
                                    NoteDocumentId = item.FileInFolderId,
                                    NoteId = (Guid)item.ObjectId
                                };

                                var fileName = $"{item.FileName}.{item.FileExtension}";
                                var folderName = ConvertFolderUrl(folder.Url);
                                var webRootPath = _hostingEnvironment.WebRootPath;
                                file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                                listNoteDocument.Add(file);
                            });


                            listNote.ForEach(item =>
                            {
                                var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                                item.ResponsibleName = employee.EmployeeName;
                                item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                    .OrderByDescending(z => z.UpdatedDate).ToList();
                            });

                            //Sắp xếp lại listNote
                            listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                        }

                        transaction.Commit();

                        #region Tạo thông báo


                        //if (project != null)
                        //{
                        //    NotificationHelper.AccessNotification(context, TypeModel.ProjectDetail, "COMM",
                        //        new Project(), project, true, note);
                        //}

                        #endregion

                        return new CreateNoteForAllRecruitmentCampaignResult()
                        {
                            Status = true,
                            Message = "Success",
                            ListNote = listNote
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreateNoteForAllRecruitmentCampaignResult()
                {
                    Status = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = ex.Message + " " + ex.InnerException
                };
            }
        }
        public GetListNoteResult GetListNote(GetListNoteParameter parameter)
        {
            try
            {
                var folderCommon = context.Folder.ToList();

                int pageSize = parameter.PageSize;
                int pageIndex = parameter.PageIndex;

                var listNote = new List<NoteEntityModel>();
                var totalRecordsNote = 0;

                var folderUrl = folderCommon.FirstOrDefault(x => x.FolderType == parameter.ObjectType)?.Url;
                var webRootPath = _hostingEnvironment.WebRootPath + "\\";

                // list ghi chú 
                if (parameter.ObjectNumber != null)
                {
                    listNote = context.Note.Where(x => x.ObjectNumber == parameter.ObjectNumber && x.ObjectType == parameter.ObjectType && x.Active == true)
                        .Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectNumber = y.ObjectNumber,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList(); ;
                }
                else
                {
                    listNote = context.Note.Where(x => x.ObjectId == parameter.ObjectId && x.ObjectType == parameter.ObjectType && x.Active == true)
                        .Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectNumber = y.ObjectNumber,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList(); ;
                }

                if (listNote.Count > 0)
                {
                    var listNoteId = listNote.Select(x => x.NoteId).ToList();
                    var listUser = context.User.ToList();
                    var _listAllEmployee = context.Employee.ToList();
                    var listNoteDocument = new List<NoteDocumentEntityModel>();

                    var listFileInFolder =
                        context.FileInFolder.Where(x => listNoteId.Contains(x.ObjectId.Value)).ToList();

                    listFileInFolder.ForEach(item =>
                    {
                        var file = new NoteDocumentEntityModel
                        {
                            DocumentName = item.FileName.Substring(0, item.FileName.LastIndexOf("_")),
                            DocumentSize = item.Size,
                            CreatedById = item.CreatedById,
                            CreatedDate = item.CreatedDate,
                            UpdatedById = item.UpdatedById,
                            UpdatedDate = item.UpdatedDate,
                            NoteDocumentId = item.FileInFolderId,
                            NoteId = item.ObjectId.Value
                        };

                        var fileName = $"{item.FileName}.{item.FileExtension}";
                        var folderName = ConvertFolderUrl(folderUrl);

                        file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                        listNoteDocument.Add(file);
                    });

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        if (_user != null)
                        {
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        }
                    });

                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    totalRecordsNote = listNote.Count();

                    listNote = listNote
                        .Skip(pageSize * pageIndex)
                        .Take(pageSize).ToList();
                }

                return new GetListNoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListNote = listNote,
                    TotalRecordsNote = totalRecordsNote
                };
            }
            catch (Exception e)
            {
                return new GetListNoteResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ThemMoiGhiChuResult ThemMoiGhiChu(ThemMoiGhiChuParameter parameter)
        {
            try
            {
                var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.ObjectType);

                if (folder == null)
                {
                    return new ThemMoiGhiChuResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Thư mục upload không tồn tại"
                    };
                }

                string folderName = ConvertFolderUrl(folder.Url);
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    return new ThemMoiGhiChuResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Bạn phải cấu hình thư mục để lưu"
                    };
                }

                Guid noteId;

                //Tạo mới
                if (parameter.Note.NoteId == Guid.Empty)
                {
                    var note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.Description = parameter.Note.Description;
                    note.Type = "ADD";
                    note.ObjectId = parameter.Note.ObjectId;
                    note.ObjectNumber = parameter.Note.ObjectNumber;
                    note.ObjectType = parameter.Note.ObjectType;
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.NoteTitle = "Đã thêm ghi chú";

                    context.Note.Add(note);

                    noteId = note.NoteId;
                }
                //Cập nhật
                else
                {
                    var note = context.Note.FirstOrDefault(x => x.NoteId == parameter.Note.NoteId);
                    if (note == null)
                    {
                        return new ThemMoiGhiChuResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Ghi chú không tồn tại trên hệ thống"
                        };
                    }

                    note.Description = parameter.Note.Description;
                    context.Note.Update(note);

                    noteId = note.NoteId;
                }

                #region Thêm file

                parameter.ListFile?.ForEach(_file =>
                {
                    var file = new FileInFolder()
                    {
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        FileInFolderId = Guid.NewGuid(),
                        FileName = _file.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                        FolderId = folder.FolderId,
                        ObjectId = noteId,
                        ObjectType = _file.FileInFolder.ObjectType,
                        Size = _file.FileInFolder.Size,
                        FileExtension = _file.FileInFolder.FileExtension
                    };

                    context.FileInFolder.Add(file);

                    string fileName = file.FileName + "." + file.FileExtension;

                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        _file.FileSave.CopyTo(stream);
                    }
                });

                #endregion

                context.SaveChanges();

                return new ThemMoiGhiChuResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new ThemMoiGhiChuResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public XoaGhiChuResult XoaGhiChu(XoaGhiChuParameter parameter)
        {
            try
            {
                var note = context.Note.FirstOrDefault(x => x.NoteId == parameter.NoteId);
                if (note == null)
                {
                    return new XoaGhiChuResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Ghi chú không tồn tại trên hệ thống"
                    };
                }

                var listFile = context.FileInFolder.Where(x => x.ObjectId == note.NoteId &&
                                                               x.ObjectType == note.ObjectType).ToList();

                using (var trans = context.Database.BeginTransaction())
                {
                    context.Note.Remove(note);
                    context.FileInFolder.RemoveRange(listFile);
                    context.SaveChanges();
                    trans.Commit();
                }

                return new XoaGhiChuResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công"
                };
            }
            catch (Exception e)
            {
                return new XoaGhiChuResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public XoaFileGhiChuResult XoaFileGhiChu(XoaFileGhiChuParameter parameter)
        {
            try
            {
                var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == parameter.FileInFolderId);
                if (file == null)
                {
                    return new XoaFileGhiChuResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "File không tồn tại trên hệ thống"
                    };
                }

                context.FileInFolder.Remove(file);
                context.SaveChanges();

                return new XoaFileGhiChuResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công"
                };
            }
            catch (Exception e)
            {
                return new XoaFileGhiChuResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
