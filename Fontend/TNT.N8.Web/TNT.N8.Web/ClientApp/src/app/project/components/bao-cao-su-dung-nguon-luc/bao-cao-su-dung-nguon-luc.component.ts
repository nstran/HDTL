import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-bao-cao-su-dung-nguon-luc',
  templateUrl: './bao-cao-su-dung-nguon-luc.component.html',
  styleUrls: ['./bao-cao-su-dung-nguon-luc.component.css']
})
export class BaoCaoSuDungNguonLucComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  projectId: string = null;

  cols1: Array<any> = [];
  cols2: Array<any> = [];

  listProject: Array<any> = [];
  selectedProject: Array<any> = [];
  
  listDefineData: Array<any> = [];
  listData: Array<any> = [];
  selectedRow: any = null;

  listHeaderRow1: Array<any> = [];
  listHeaderRow2: Array<any> = [];
  listDataFooter: Array<any> = [];

  listThang: Array<any> = [
    { name: 'Tháng 1', value: 1 },
    { name: 'Tháng 2', value: 2 },
    { name: 'Tháng 3', value: 3 },
    { name: 'Tháng 4', value: 4 },
    { name: 'Tháng 5', value: 5 },
    { name: 'Tháng 6', value: 6 },
    { name: 'Tháng 7', value: 7 },
    { name: 'Tháng 8', value: 8 },
    { name: 'Tháng 9', value: 9 },
    { name: 'Tháng 10', value: 10 },
    { name: 'Tháng 11', value: 11 },
    { name: 'Tháng 12', value: 12 },
  ];
  selectedThang: any = this.listThang.find(x => x.value == (new Date().getMonth() + 1));

  listNam: Array<any> = this.setListNam();
  selectedNam: any = this.listNam.find(x => x.value == new Date().getFullYear());

  listNhanVienKhongThamGiaDuAn: Array<any> = [];
  selectedDt2Row: any = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private projectService: ProjectService,
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getMasterData();
  }

  initTable() {
    this.cols2 = [
      { field: 'stt', header: 'STT', textAlign: 'center', width: '5%'},
      { field: 'employeeCodeName', header: 'Họ tên', textAlign: 'left', width: '45%'},
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'left', width: '25%'},
      { field: 'positionName', header: 'Chức vụ', textAlign: 'left', width: '25%'},
    ];
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.projectService.getMasterDataBaoCaoSuDungNguonLuc();
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }
    this.listProject = result.listProject;
    this.getBaoCaoSuDungNguonLuc();
  }

  async getBaoCaoSuDungNguonLuc() {
    let nam = this.selectedNam.value;
    let thang = this.selectedThang.value;
    let listProjectId = this.selectedProject.map(x => x.projectId);
    this.loading = true;
    let result: any = await this.projectService.getBaoCaoSuDungNguonLuc(thang, nam, listProjectId);
    this.loading = false;
    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }
    this.listHeaderRow1 = result.listHeaderRow1;
    this.listHeaderRow2 = result.listHeaderRow2;
    this.listDefineData = result.listData;
    this.listDataFooter = result.listDataFooter;
    this.listNhanVienKhongThamGiaDuAn = result.listNhanVienKhongThamGiaDuAn;
    this.listNhanVienKhongThamGiaDuAn.forEach((item, index) => {
      item.stt = index + 1;
    });

    this.buildTable();
  }

  buildTable() {
    this.cols1 = [];
    this.listData = [];

    /* build header */
    let temp = this.listDefineData[0];

    temp?.forEach((item, index) => {
      let header = { field: item.columnKey, header: item.columnValue, width: item.width, textAlign: item.textAlign};

      this.cols1 = [...this.cols1, header];
    });

    /* build data */
    this.listDefineData.forEach(item => {
      let data = {};

      item.forEach(_data => {
        if (_data.columnValue != null && _data.valueType == 1) {
          data[_data.columnKey] = ParseStringToFloat(_data.columnValue);
        }
        else {
          data[_data.columnKey] = _data.columnValue;
        }
      });

      this.listData = [...this.listData, data];
    });

    /* build footer */
    this.listDataFooter.forEach(item => {
      if (item.columnValue != null && item.valueType == 1) {
        item.columnValue = ParseStringToFloat(item.columnValue);
      }
    });
  }

  resetFilter() {
    this.selectedThang = this.listThang.find(x => x.value == (new Date().getMonth() + 1));
    this.selectedNam = this.listNam.find(x => x.value == new Date().getFullYear());
    this.selectedProject = [];

    this.getBaoCaoSuDungNguonLuc();
  }

  xuatExcel() {
    
  }

  setListNam() {
    let listNam = [];
    let currentYear = new Date().getFullYear();
    let start = currentYear - 5;
    let end = currentYear + 1;

    for(let i = start; i <= end; i++) {
      let option = { name: i.toString(), value: i };
      listNam.push(option);
    }

    return listNam;
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({ severity: severity, summary: 'Thông báo', detail: detail });
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
