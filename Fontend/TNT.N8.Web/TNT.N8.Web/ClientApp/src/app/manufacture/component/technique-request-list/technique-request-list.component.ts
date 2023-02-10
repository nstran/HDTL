import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogService } from 'primeng';

class TechniqueRequest {
  techniqueRequestId: string;
  parentId: string;
  parentName: string;
  organizationId: string;
  organizationName: string;
  techniqueName: string;
  isDefault: boolean;
  description: string;
  active: boolean;
}

class Organization {
  organizationId: string;
  organizationName: string;
}

@Component({
  selector: 'app-technique-request-list',
  templateUrl: './technique-request-list.component.html',
  styleUrls: ['./technique-request-list.component.css']
})
export class TechniqueRequestListComponent implements OnInit {
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;

  cols: any;
  selectedColumns: any[];

  @ViewChild('myTable') myTable: Table;

  techniqueRequestList: Array<TechniqueRequest> = [];

  techniqueName: string = null;
  description: string = null;

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  innerWidth: number = 0; //number window size first

  listOrganization: Array<Organization> = [];
  selectedOrganization: Organization = null;

  listParent: Array<TechniqueRequest> = [];
  selectedParent: TechniqueRequest = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "man/manufacture/technique-request/list";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }

      this.initTable();
      this.getMasterData();
    }
  }

  initTable() {
    this.cols = [
      { field: 'techniqueName', header: 'Tên tiến trình', textAlign: 'left', display: 'table-cell' },
      // { field: 'parentName', header: 'Tiến trình cha', textAlign: 'left', display: 'table-cell' },
      { field: 'organizationName', header: 'Phòng ban', textAlign: 'left', display: 'table-cell' },
      { field: 'description', header: 'Mô tả', textAlign: 'left', display: 'table-cell' }
    ];
    this.selectedColumns = this.cols;
  }

  getMasterData() {
    this.manufactureService.getMasterDataSearchTechniqueRequest().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listOrganization = result.listOrganization;
        this.listParent = result.listParent;

        this.searchTechniqueRequest();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  searchTechniqueRequest() {
    let organizationId: string = this.selectedOrganization == null ? null : this.selectedOrganization.organizationId;
    let parentId: string = this.selectedParent == null ? null : this.selectedParent.techniqueRequestId;

    this.loading = true;
    this.manufactureService.searchTechniqueRequest(this.techniqueName, organizationId, parentId, this.description).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.techniqueRequestList = result.techniqueRequestList;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  refreshFilter() {
    this.techniqueName = null;
    this.selectedOrganization = null;
    this.selectedParent = null;
    this.description = null;
    this.searchTechniqueRequest();
  }

  goToCreate() {
    this.router.navigate(['/manufacture/technique-request/create']);
  }

  goToDetail(id: string) {
    this.router.navigate(['/manufacture/technique-request/detail', { techniqueRequestId: id }]);
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}
