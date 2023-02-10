import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ManufactureService } from '../../services/manufacture.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng';

class TechniqueRequest {
  techniqueRequestId: string;
  parentId: string;
  organizationId: string;
  techniqueName: string;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  techniqueRequestCode: string;
  constructor() {
    this.techniqueRequestId = '00000000-0000-0000-0000-000000000000';
    this.parentId = null;
    this.organizationId = null;
    this.techniqueName = null;
    this.description = null;
    this.active = true;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}

class Organization {
  organizationId: string;
  organizationName: string;
}

@Component({
  selector: 'app-technique-request-create',
  templateUrl: './technique-request-create.component.html',
  styleUrls: ['./technique-request-create.component.css']
})
export class TechniqueRequestCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /* FORM: Tiến trình bình thường */
  techniqueRequestForm: FormGroup;
  
  organizationControl: FormControl;
  techniqueNameControl: FormControl;
  descriptionControl: FormControl;
  /* END */

  /* FORM: Tiến trình đặc biệt (Esp = especially) */
  techniqueRequestEspForm: FormGroup;
  
  parentEspControl: FormControl;
  techniqueNameEspControl: FormControl;
  descriptionEspControl: FormControl;
  /* END */

  option: string = 'a';
  listOrganization: Array<Organization> = [];
  listParent: Array<TechniqueRequest> = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private manufactureService: ManufactureService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
  ) { }

  ngOnInit() {
    this.setForm();
    this.getMasterData();
  }

  setForm() {
    this.organizationControl = new FormControl(null, [Validators.required]);
    this.techniqueNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.descriptionControl = new FormControl(null);

    this.techniqueRequestForm = new FormGroup({
      organizationControl: this.organizationControl,
      techniqueNameControl: this.techniqueNameControl,
      descriptionControl: this.descriptionControl
    });

    this.parentEspControl = new FormControl(null, [Validators.required]);
    this.techniqueNameEspControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.descriptionEspControl = new FormControl(null);

    this.techniqueRequestEspForm = new FormGroup({
      parentEspControl: this.parentEspControl,
      techniqueNameEspControl: this.techniqueNameEspControl,
      descriptionEspControl: this.descriptionEspControl
    });
  }

  getMasterData() {
    this.loading = true;
    this.manufactureService.getMasterDataCreateTechniqueRequest().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listOrganization = result.listOrganization;
        this.listParent = result.listParent;
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  createTechniqueRequest(mode: boolean) {
    if (this.option == 'a') {
      if (!this.techniqueRequestForm.valid) {
        Object.keys(this.techniqueRequestForm.controls).forEach(key => {
          if (this.techniqueRequestForm.controls[key].valid == false) {
            this.techniqueRequestForm.controls[key].markAsTouched();
          }
        });
      } else { 
        let techniqueRequest = new TechniqueRequest();
        let organizationId: Organization = this.organizationControl.value;
        techniqueRequest.organizationId = organizationId.organizationId;
        techniqueRequest.techniqueName = this.techniqueNameControl.value.trim();
        techniqueRequest.description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
      
        this.loading = true;
        this.awaitResult = true;
        this.manufactureService.createTechniqueRequest(techniqueRequest).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            if (mode) {
              this.listParent = result.listParent;

              this.resetForm();
              let msg = { severity:'success', summary: 'Thông báo:', detail: 'Tạo Tiến trình thành công' };
              this.showMessage(msg);
            } else {
              //Lưu
              this.router.navigate(['/manufacture/technique-request/detail', { techniqueRequestId: result.techniqueRequestId }]);
            }
          } else {
            let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    } else if (this.option == 'b') {
      if (!this.techniqueRequestEspForm.valid) {
        Object.keys(this.techniqueRequestEspForm.controls).forEach(key => {
          if (this.techniqueRequestEspForm.controls[key].valid == false) {
            this.techniqueRequestEspForm.controls[key].markAsTouched();
          }
        });
      } else { 
        let techniqueRequest = new TechniqueRequest();
        let parent: TechniqueRequest = this.parentEspControl.value;
        techniqueRequest.parentId = parent.techniqueRequestId;
        techniqueRequest.techniqueName = this.techniqueNameEspControl.value.trim();
        techniqueRequest.description = this.descriptionEspControl.value == null ? null : this.descriptionEspControl.value.trim();

        this.loading = true;
        this.awaitResult = true;
        this.manufactureService.createTechniqueRequest(techniqueRequest).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            if (mode) {
              this.listParent = result.listParent;

              this.resetForm();
              let msg = { severity:'success', summary: 'Thông báo:', detail: 'Tạo Tiến trình thành công' };
              this.showMessage(msg);
            } else {
              //Lưu
              this.router.navigate(['/manufacture/technique-request/detail', { techniqueRequestId: result.techniqueRequestId }]);
            }
          } else {
            let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    }
  }

  resetForm() {
    this.organizationControl.reset();
    this.techniqueNameControl.reset();
    this.descriptionControl.reset();
    this.parentEspControl.reset();
    this.techniqueNameEspControl.reset();
    this.descriptionEspControl.reset();

    this.awaitResult = false;
  }

  cancel() {
    this.router.navigate(['/manufacture/technique-request/list']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
};

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
