import { Component, OnInit } from '@angular/core';
import { BreadCrumMenuModel } from '../../../shared/models/breadCrumMenu.model';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-menu-left-project',
  templateUrl: './menu-left-project.component.html',
  styleUrls: ['./menu-left-project.component.css']
})
export class MenuLeftProjectComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  lstBreadCrumLeftMenuData: Array<BreadCrumMenuModel> = [
    {
      Name: "DASHBOARD", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-chart-pie", IsDefault: false, CodeParent: "pro_sub_detail_dashboard", Display: "none", LstChildren: [
        { Name: "Dashboard", Path: "/project/dashboard", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_detail_dashboard", LstChildren: [], Display: "none", Code: '' },
        // { Name: "Báo cáo sử dụng nguồn lực", Path: "/project/bc-su-dung-nguon-luc", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_detail_dashboard", LstChildren: [], Display: "none", Code: '' },
        // { Name: "Báo cáo tổng hợp các dự án", Path: "/project/bc-tong-hop-cac-du-an", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_detail_dashboard", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "THÔNG TIN CHUNG", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-info-circle", IsDefault: false, CodeParent: "pro_sub_detail_info", Display: "none", LstChildren: [
        { Name: "Thông tin chung", Path: "/project/detail", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_detail_info", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "HẠNG MỤC DỰ ÁN", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-sitemap", IsDefault: false, CodeParent: "pro_sub_hang_muc", Display: "none", LstChildren: [
        { Name: "Hạng mục", Path: "/project/scope", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_hang_muc", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "NGUỒN LỰC DỰ ÁN", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-users", IsDefault: false, CodeParent: "pro_sub_nguon_luc", Display: "none", LstChildren: [
        { Name: "Nguồn lực", Path: "/project/resource", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_nguon_luc", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "MỐC DỰ ÁN", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fas fa-sliders-h", IsDefault: false, CodeParent: "pro_sub_moc_du_an", Display: "none", LstChildren: [
        { Name: "Mốc dự án", Path: "/project/milestone", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_moc_du_an", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "QUẢN LÝ CÔNG VIỆC", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fas fa-clipboard-check", IsDefault: false, CodeParent: "pro_sub_work", Display: "none", LstChildren: [
        { Name: "Tạo công việc", Path: `/project/create-project-task`, ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_work", LstChildren: [], Display: "none", Code: '' },
        { Name: "Danh sách công việc", Path: `/project/list-project-task`, ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_work", LstChildren: [], Display: "none", Code: '' },
        { Name: "Danh sách timesheet", Path: `/project/search-time-sheet`, ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_work", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "QUẢN LÝ TÀI LIỆU", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-file-alt", IsDefault: false, CodeParent: "pro_sub_doc", Display: "none", LstChildren: [
        { Name: "Tài liệu", Path: "/project/document", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_doc", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "QUẢN LÝ RỦI RO", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-file-alt", IsDefault: false, CodeParent: "pro_sub_rick", Display: "none", LstChildren: [
        { Name: "Rủi ro", Path: "/#", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_rick", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "QUẢN LÝ CHẤT LƯỢNG ", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fas fa-award", IsDefault: false, CodeParent: "pro_sub_quality", Display: "none", LstChildren: [
        { Name: "Chất lượng", Path: "/#", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_quality", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "CÁC BÊN LIÊN QUAN", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-handshake-o", IsDefault: false, CodeParent: "pro_sub_lien_quan", Display: "none", LstChildren: [
        { Name: "Các bên liên quan", Path: "/#", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_lien_quan", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "QUẢN LÝ GIAO TIẾP", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-comments", IsDefault: false, CodeParent: "pro_sub_comunicate", Display: "none", LstChildren: [
        { Name: "Giao tiếp", Path: "/#", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_sub_comunicate", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
  ];

  lstBreadCrumLeftMenu: Array<BreadCrumMenuModel> = [];

  rowclick: number = -1;
  active: boolean = true;
  activeParent: boolean = true;
  countLengthParrent: number = 0;
  rowclickParent: number = -1;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
  ) { }

  ngOnInit(): void {
    this.getMasterData();
  }

  getMasterData() {
    this.route.params.subscribe(params => {
      let projectId = params['projectId'];
      if (projectId) {
        this.projectService.getPermission(projectId, this.auth.UserId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            this.getMenuFollowPermistion(result.permissionStr);
          }
        });
      }
    });
  }

  getMenuFollowPermistion(permissionStr: string) {
    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenuData.forEach(item => {
      let lstMenuChildren = item.LstChildren.filter(c => permissionStr.includes(c.Path));
      if (lstMenuChildren.length != 0) {
        var breadcrumb = new BreadCrumMenuModel();
        breadcrumb.Name = item.Name;
        breadcrumb.Active = item.Active;
        breadcrumb.nameIcon = item.nameIcon;
        breadcrumb.LevelMenu = 2;
        breadcrumb.ObjectType = item.ObjectType;
        breadcrumb.Display = item.Display;
        breadcrumb.Path = "";
        breadcrumb.LstChildren = lstMenuChildren;
        this.lstBreadCrumLeftMenu.push(breadcrumb);
      }
    });
  }

  addRemoveIcon(index) {
    for (let i = 0; i < this.lstBreadCrumLeftMenu.length; i++) {
      $(".module-remove" + i).hide();
      $(".module-add" + i).show();
    }
    if (this.rowclick !== index) {
      $(".module-remove" + index).show();
      $(".module-add" + index).hide();
      this.active = true;

      for (let i = 0; i < this.countLengthParrent; i++) {
        $(".module-remove-parent" + i).hide();
        $(".module-add-parent" + i).show();
      }
      this.activeParent = true;
    }
    else {
      if (!this.active) {
        $(".module-remove" + index).show();
        $(".module-add" + index).hide();
      }
      else {
        $(".module-remove" + index).hide();
        $(".module-add" + index).show();
      }
      this.active = !this.active;
    }

    this.rowclick = index;
  }

  addRemoveIconParren(index, countLength) {
    this.countLengthParrent = countLength;
    for (let i = 0; i < countLength; i++) {
      $(".module-remove-parent" + i).hide();
      $(".module-add-parent" + i).show();
    }
    if (this.rowclickParent !== index) {
      $(".module-remove-parent" + index).show();
      $(".module-add-parent" + index).hide();
      this.activeParent = true;
    }
    else {
      if (!this.activeParent) {
        $(".module-remove-parent" + index).show();
        $(".module-add-parent" + index).hide();
      }
      else {
        $(".module-remove-parent" + index).hide();
        $(".module-add-parent" + index).show();
      }
      this.activeParent = !this.activeParent;
    }

    this.rowclickParent = index;
  }

  goToPage(resource) {
    this.route.params.subscribe(params => {
      let projectId = params['projectId'];

      if (projectId) {
        this.router.navigate([resource.Path, { projectId: projectId }]);
      }
    });
  }

}
