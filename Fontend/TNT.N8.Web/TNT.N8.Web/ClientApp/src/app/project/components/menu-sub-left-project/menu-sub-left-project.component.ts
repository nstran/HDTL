import { Component, OnInit } from '@angular/core';
import { BreadCrumMenuModel } from '../../../shared/models/breadCrumMenu.model';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-menu-sub-left-project',
  templateUrl: './menu-sub-left-project.component.html',
  styleUrls: ['./menu-sub-left-project.component.css']
})
export class MenuSubLeftProjectComponent implements OnInit {

  lstBreadCrumLeftMenu: Array<BreadCrumMenuModel> = [
    {
      Name: "DASHBOARD", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-chart-pie", IsDefault: false, CodeParent: "pro_detail_dashboard", Display: "none", LstChildren: [
        { Name: "Dashboard", Path: "/project/list", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_detail_dashboard", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
    {
      Name: "THÔNG TIN CHUNG", Path: "", ObjectType: "pro_du_an", LevelMenu: 2, Active: false, nameIcon: "fa fa-info-circle", IsDefault: false, CodeParent: "pro_detail_info", Display: "none", LstChildren: [
        { Name: "Thông tin chung", Path: "/project/detail", ObjectType: "pro", LevelMenu: 3, Active: false, nameIcon: "fa format_list_bulleted", IsDefault: false, CodeParent: "pro_detail_info", LstChildren: [], Display: "none", Code: '' },
      ], Code: ''
    },
  ];

  rowclick: number = -1;
  active: boolean = true;
  activeParent: boolean = true;
  countLengthParrent: number = 0;
  rowclickParent: number = -1;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {

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
