using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;
using TN.TNM.DataAccess.Messages.Results.MenuBuild;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class MenuBuildDAO : BaseDAO, IMenuBuildDataAccess
    {
        public MenuBuildDAO(Databases.TNTN8Context _context)
        {
            this.context = _context;
        }

        public GetMenuBuildResult GetMenuBuild(GetMenuBuildParameter parameter)
        {
            try
            {
                var ListMenuBuild = new List<MenuBuildEntityModel>();

                return new GetMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListMenuBuild = ListMenuBuild
                };
            }
            catch (Exception e)
            {
                return new GetMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMenuModuleResult GetMenuModule(GetMenuModuleParameter parameter)
        {
            try
            {
                var ListMenuModule = new List<MenuBuildEntityModel>();
                ListMenuModule = context.MenuBuild.Where(x => x.Level == 0).Select(y => new MenuBuildEntityModel
                {
                    MenuBuildId = y.MenuBuildId,
                    ParentId = y.ParentId,
                    Code = y.Code,
                    CodeParent = y.CodeParent,
                    Name = y.Name,
                    Path = y.Path,
                    NameIcon = y.NameIcon,
                    Level = y.Level,
                    IndexOrder = y.IndexOrder,
                    IsShow = y.IsShow
                }).OrderBy(z => z.IndexOrder).ToList();

                return new GetMenuModuleResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListMenuModule = ListMenuModule
                };
            }
            catch (Exception e)
            {
                return new GetMenuModuleResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateMenuBuildResult CreateMenuBuild(CreateMenuBuildParameter parameter)
        {
            try
            {
                var menuBuild = new MenuBuild();
                menuBuild.MenuBuildId = Guid.NewGuid();
                menuBuild.ParentId = parameter.MenuBuild.ParentId;
                menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                menuBuild.Code = parameter.MenuBuild.Code?.Trim();
                menuBuild.CodeParent = parameter.MenuBuild.CodeParent?.Trim();
                menuBuild.Level = parameter.MenuBuild.Level;
                menuBuild.Path = parameter.MenuBuild.Path?.Trim();
                menuBuild.NameIcon = parameter.MenuBuild.NameIcon?.Trim();
                menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                menuBuild.IsPageDetail = parameter.MenuBuild.IsPageDetail;

                context.MenuBuild.Add(menuBuild);

                #region Tạo thêm item mask cho sub menu module khi tạo menu module

                if (parameter.MenuBuild.Level == 0)
                {
                    var subMenuMask = new MenuBuild();
                    subMenuMask.MenuBuildId = Guid.NewGuid();
                    subMenuMask.ParentId = menuBuild.MenuBuildId;
                    subMenuMask.Name = menuBuild.Name;
                    subMenuMask.Code = menuBuild.Code + "_mask";
                    subMenuMask.CodeParent = menuBuild.Code;
                    subMenuMask.Level = 1;
                    subMenuMask.Path = null;
                    subMenuMask.NameIcon = null;
                    subMenuMask.IndexOrder = 1;

                    context.MenuBuild.Add(subMenuMask);
                }

                #endregion

                context.SaveChanges();

                return new CreateMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new CreateMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetSubMenuModuleByMenuModuleCodeResult GetSubMenuModuleByMenuModuleCode(
            GetSubMenuModuleByMenuModuleCodeParameter parameter)
        {
            try
            {
                var ListSubMenuModule = new List<MenuBuildEntityModel>();
                ListSubMenuModule = context.MenuBuild.Where(x => x.CodeParent == parameter.MenuModuleCode).Select(y =>
                    new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Name = y.Name,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder,
                        IsShow = y.IsShow
                    }).OrderBy(z => z.IndexOrder).ToList();

                return new GetSubMenuModuleByMenuModuleCodeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListSubMenuModule = ListSubMenuModule
                };
            }
            catch (Exception e)
            {
                return new GetSubMenuModuleByMenuModuleCodeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMenuPageBySubMenuCodeResult GetMenuPageBySubMenuCode(GetMenuPageBySubMenuCodeParameter parameter)
        {
            try
            {
                var ListMenuPage = new List<MenuBuildEntityModel>();
                ListMenuPage = context.MenuBuild.Where(x => x.CodeParent == parameter.SubMenuCode).Select(y =>
                    new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Name = y.Name,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder,
                        IsPageDetail = y.IsPageDetail,
                        IsShow = y.IsShow
                    }).OrderBy(z => z.IndexOrder).ToList();

                return new GetMenuPageBySubMenuCodeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListMenuPage = ListMenuPage
                };
            }
            catch (Exception e)
            {
                return new GetMenuPageBySubMenuCodeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateIsPageDetailResult UpdateIsPageDetail(UpdateIsPageDetailParameter parameter)
        {
            try
            {
                var menuBuild = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == parameter.MenuBuildId);

                if (menuBuild == null)
                {
                    return new UpdateIsPageDetailResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Menu Build không tồn tại trên hệ thống"
                    };
                }

                menuBuild.IsPageDetail = parameter.IsPageDetail;
                context.MenuBuild.Update(menuBuild);
                context.SaveChanges();

                return new UpdateIsPageDetailResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateIsPageDetailResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateMenuBuildResult UpdateMenuBuild(UpdateMenuBuildParameter parameter)
        {
            try
            {
                var menuBuild = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == parameter.MenuBuild.MenuBuildId);

                if (menuBuild == null)
                {
                    return new UpdateMenuBuildResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "MenuBuild không tồn tại trên hệ thống"
                    };
                }

                //Nếu là Menu
                if (menuBuild.Level == 0)
                {
                    //Cập nhật lại CodeParent, Code cho SubMenu nếu Menu Code thay đổi
                    if (menuBuild.Code != parameter.MenuBuild.Code?.Trim())
                    {
                        var listSubMenu = context.MenuBuild.Where(x => x.ParentId == menuBuild.MenuBuildId).ToList();
                        var listSubMenuId = listSubMenu.Select(y => y.MenuBuildId).ToList();
                        var listPage = context.MenuBuild.Where(x => listSubMenuId.Contains(x.ParentId.Value))
                            .ToList();

                        listSubMenu.ForEach(item =>
                        {
                            var newCode = parameter.MenuBuild.Code?.Trim() + "_" +
                                          item.Code.Substring(item.Code.IndexOf("_") + 1);
                            item.Code = newCode;
                            item.CodeParent = parameter.MenuBuild.Code?.Trim();

                            //Thay đổi CodeParent cho Page
                            var listPageBySubMenu = listPage.Where(x => x.ParentId == item.MenuBuildId).ToList();
                            listPageBySubMenu.ForEach(page => { page.CodeParent = newCode; });
                            context.MenuBuild.UpdateRange(listPageBySubMenu);
                            context.SaveChanges();
                        });
                        context.MenuBuild.UpdateRange(listSubMenu);
                        context.SaveChanges();
                    }

                    menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                    menuBuild.Code = parameter.MenuBuild.Code?.Trim();
                    menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                    menuBuild.NameIcon = parameter.MenuBuild.NameIcon?.Trim();
                }
                //Nếu là SubMenu
                else if (menuBuild.Level == 1)
                {
                    //Cập nhật lại CodeParent cho Page nếu SubMenu Code thay đổi
                    if (menuBuild.Code != parameter.MenuBuild.Code?.Trim())
                    {
                        var listPage = context.MenuBuild.Where(x => x.ParentId == menuBuild.MenuBuildId).ToList();

                        listPage.ForEach(page => { page.CodeParent = parameter.MenuBuild.Code?.Trim(); });
                        context.MenuBuild.UpdateRange(listPage);
                        context.SaveChanges();
                    }

                    menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                    menuBuild.Code = parameter.MenuBuild.Code?.Trim();
                    menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                    menuBuild.Path = parameter.MenuBuild.Path?.Trim();
                }
                //Nếu là Page
                else if (menuBuild.Level == 2)
                {
                    menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                    menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                    menuBuild.Path = parameter.MenuBuild.Path?.Trim();
                    menuBuild.NameIcon = parameter.MenuBuild.NameIcon?.Trim();
                }

                context.MenuBuild.Update(menuBuild);
                context.SaveChanges();

                return new UpdateMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateMenuBuildResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateIsShowResult UpdateIsShow(UpdateIsShowParameter parameter)
        {
            try
            {
                var menuBuild = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == parameter.MenuBuildId);

                if (menuBuild == null)
                {
                    return new UpdateIsShowResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "MenuBuild không tồn tại trên hệ thống"
                    };
                }

                //Nếu là Menu
                if (menuBuild.Level == 0)
                {
                    menuBuild.IsShow = parameter.IsShow;
                    context.MenuBuild.Update(menuBuild);
                    context.SaveChanges();

                    //Lấy list SubMenu
                    var listSubMenu = context.MenuBuild.Where(x => x.ParentId == menuBuild.MenuBuildId).ToList();
                    var listSubMenuId = listSubMenu.Select(y => y.MenuBuildId).ToList();

                    //Lấy list Page
                    var listPage = context.MenuBuild.Where(x => listSubMenuId.Contains(x.ParentId.Value)).ToList();

                    //Ẩn/Hiện tất cả SubMenu và Page con
                    listSubMenu.ForEach(item => { item.IsShow = parameter.IsShow; });
                    listPage.ForEach(item => { item.IsShow = parameter.IsShow; });

                    context.MenuBuild.UpdateRange(listSubMenu);
                    context.MenuBuild.UpdateRange(listPage);
                    context.SaveChanges();
                }
                //Nếu là SubMenu
                else if (menuBuild.Level == 1)
                {
                    menuBuild.IsShow = parameter.IsShow;
                    context.MenuBuild.Update(menuBuild);
                    context.SaveChanges();

                    //Lấy list Page
                    var listPage = context.MenuBuild.Where(x => x.ParentId == menuBuild.MenuBuildId).ToList();

                    //Ẩn/Hiện tất cả các Page con
                    listPage.ForEach(item => { item.IsShow = parameter.IsShow; });
                    context.MenuBuild.UpdateRange(listPage);
                    context.SaveChanges();

                    //Nếu là ẩn
                    if (!parameter.IsShow)
                    {
                        //Nếu tất cả các SubMenu con đã ẩn hết thì ẩn Menu cha
                        var countSubMenu =
                            context.MenuBuild.Count(x =>
                                x.ParentId == menuBuild.ParentId && x.IsShow == true && x.IndexOrder != 1);

                        if (countSubMenu == 0)
                        {
                            var menu = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == menuBuild.ParentId);
                            menu.IsShow = parameter.IsShow;
                            context.MenuBuild.Update(menu);
                            context.SaveChanges();

                            //Ẩn Sub Menu Mask
                            var subMenuMask = context.MenuBuild
                                .FirstOrDefault(x =>
                                    x.ParentId == menuBuild.ParentId && x.IndexOrder == 1);
                            subMenuMask.IsShow = false;
                            context.MenuBuild.Update(subMenuMask);
                            context.SaveChanges();
                        }
                    }
                    //Nếu là hiện
                    else
                    {
                        var menu = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == menuBuild.ParentId);

                        //Nếu Sub Menu Mask đang ẩn thì hiển thị
                        var subMenuMask = context.MenuBuild.FirstOrDefault(x =>
                            x.IndexOrder == 1 && x.ParentId == menuBuild.ParentId);

                        if (!subMenuMask.IsShow.Value)
                        {
                            subMenuMask.IsShow = true;
                            context.MenuBuild.Update(subMenuMask);
                            context.SaveChanges();
                        }

                        //Nếu menu cha đang ẩn thì hiển thị
                        if (!menu.IsShow.Value)
                        {
                            menu.IsShow = parameter.IsShow;
                            context.MenuBuild.Update(menu);
                            context.SaveChanges();
                        }
                    }
                }
                //Nếu là Page
                else if (menuBuild.Level == 2)
                {
                    menuBuild.IsShow = parameter.IsShow;
                    context.MenuBuild.Update(menuBuild);
                    context.SaveChanges();

                    //Lấy Sub Menu cha
                    var subMenu = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == menuBuild.ParentId);

                    //Lấy sub menu mask
                    var subMenuMask =
                        context.MenuBuild.FirstOrDefault(x => x.ParentId == subMenu.ParentId && x.IndexOrder == 1);

                    //Lấy Menu cha
                    var menu = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == subMenu.ParentId);

                    //Nếu là Ẩn
                    if (!parameter.IsShow)
                    {
                        //Lấy list page
                        var countPage =
                            context.MenuBuild.Count(x => x.ParentId == menuBuild.ParentId && x.IsShow == true);

                        //Nếu tất cả page đều ẩn thì ẩn Sub Menu cha
                        if (countPage == 0)
                        {
                            subMenu.IsShow = false;
                            context.MenuBuild.Update(subMenu);
                            context.SaveChanges();

                            //Lấy tất cả Sub Menu đều ẩn thì ẩn Menu
                            var conutSubMenu =
                                context.MenuBuild.Count(x =>
                                    x.ParentId == menu.MenuBuildId && x.IsShow == true && x.IndexOrder != 1);

                            if (conutSubMenu == 0)
                            {
                                //Ẩn sub menu mask
                                subMenuMask.IsShow = false;
                                context.MenuBuild.Update(subMenuMask);
                                context.SaveChanges();

                                menu.IsShow = false;
                                context.MenuBuild.Update(menu);
                                context.SaveChanges();
                            }
                        }
                    }
                    //Nếu là Hiện
                    else
                    {
                        //Nếu Sub Menu cha đang ẩn thì hiển thị nó lên
                        if (!subMenu.IsShow.Value)
                        {
                            subMenu.IsShow = true;
                            context.MenuBuild.Update(subMenu);
                            context.SaveChanges();

                            //Hiển thị sub menu mask
                            subMenuMask.IsShow = true;
                            context.MenuBuild.Update(subMenuMask);
                            context.SaveChanges();

                            //Nếu Menu cha đang ẩn thì hiển thị nó lên
                            if (!menu.IsShow.Value)
                            {
                                menu.IsShow = true;
                                context.MenuBuild.Update(menu);
                                context.SaveChanges();
                            }
                        }
                    }
                }

                return new UpdateIsShowResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Cập nhật thành công",
                };
            }
            catch (Exception e)
            {
                return new UpdateIsShowResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
