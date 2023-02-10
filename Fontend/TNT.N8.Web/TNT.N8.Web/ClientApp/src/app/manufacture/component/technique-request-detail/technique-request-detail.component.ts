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
    this.techniqueRequestCode = null;
  }
}

class Organization {
  organizationId: string;
  organizationName: string;
}

@Component({
  selector: 'app-technique-request-detail',
  templateUrl: './technique-request-detail.component.html',
  styleUrls: ['./technique-request-detail.component.css']
})
export class TechniqueRequestDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /* FORM: Tiến trình bình thường */
  techniqueRequestForm: FormGroup;

  organizationControl: FormControl;
  techniqueNameControl: FormControl;
  techniqueCodeControl: FormControl;
  descriptionControl: FormControl;
  /* END */

  /* FORM: Tiến trình đặc biệt (Esp = especially) */
  techniqueRequestEspForm: FormGroup;

  parentEspControl: FormControl;
  techniqueNameEspControl: FormControl;
  techniqueCodeEspControl: FormControl;
  descriptionEspControl: FormControl;
  /* END */

  techniqueRequestId: string = null;
  techniqueRequest: TechniqueRequest = new TechniqueRequest();
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
    this.route.params.subscribe(params => { this.techniqueRequestId = params['techniqueRequestId'] });

    this.setForm();

    this.loading = true;
    this.manufactureService.getTechniqueRequestById(this.techniqueRequestId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {

        this.techniqueRequest = result.techniqueRequest;
        this.listOrganization = result.listOrganization;
        this.listParent = result.listParent;

        this.mapDataToForm();
      } else {
        let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    this.organizationControl = new FormControl(null, [Validators.required]);
    this.techniqueNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.techniqueCodeControl = new FormControl(null, [forbiddenSpaceText]);
    this.descriptionControl = new FormControl(null);

    this.techniqueRequestForm = new FormGroup({
      organizationControl: this.organizationControl,
      techniqueNameControl: this.techniqueNameControl,
      techniqueCodeControl: this.techniqueCodeControl,
      descriptionControl: this.descriptionControl
    });

    this.parentEspControl = new FormControl(null, [Validators.required]);
    this.techniqueNameEspControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.techniqueCodeEspControl = new FormControl(null, [forbiddenSpaceText]);
    this.descriptionEspControl = new FormControl(null);

    this.techniqueRequestEspForm = new FormGroup({
      parentEspControl: this.parentEspControl,
      techniqueNameEspControl: this.techniqueNameEspControl,
      techniqueCodeEspControl: this.techniqueCodeEspControl,
      descriptionEspControl: this.descriptionEspControl
    });
  }

  mapDataToForm() {
    if (this.techniqueRequest.parentId == null) {
      this.option = 'a';

      let selectedOrganization = this.listOrganization.find(x => x.organizationId == this.techniqueRequest.organizationId);
      if (selectedOrganization) {
        this.organizationControl.setValue(selectedOrganization);
      }

      this.techniqueNameControl.setValue(this.techniqueRequest.techniqueName);
      this.descriptionControl.setValue(this.techniqueRequest.description);
      this.techniqueCodeControl.setValue(this.techniqueRequest.techniqueRequestCode);

    } else {
      this.option = 'b';

      let selectedParent = this.listParent.find(x => x.techniqueRequestId == this.techniqueRequest.parentId);
      if (selectedParent) {
        this.parentEspControl.setValue(selectedParent);
      }

      this.techniqueNameEspControl.setValue(this.techniqueRequest.techniqueName);
      this.descriptionEspControl.setValue(this.techniqueRequest.description);
      this.techniqueCodeEspControl.setValue(this.techniqueRequest.techniqueRequestCode);
    }
  }

  updateTechniqueRequest() {
    if (this.option == 'a') {
      if (!this.techniqueRequestForm.valid) {
        Object.keys(this.techniqueRequestForm.controls).forEach(key => {
          if (this.techniqueRequestForm.controls[key].valid == false) {
            this.techniqueRequestForm.controls[key].markAsTouched();
          }
        });
      } else {
        let organizationId: Organization = this.organizationControl.value;
        this.techniqueRequest.organizationId = organizationId.organizationId;
        this.techniqueRequest.techniqueName = this.techniqueNameControl.value.trim();
        this.techniqueRequest.techniqueRequestCode = this.techniqueCodeControl.value;
        this.techniqueRequest.description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();

        this.loading = true;
        this.manufactureService.updateTechniqueRequest(this.techniqueRequest).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let msg = { severity:'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
            this.showMessage(msg);
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
        let parent: TechniqueRequest = this.parentEspControl.value;
        this.techniqueRequest.parentId = parent.techniqueRequestId;
        this.techniqueRequest.techniqueName = this.techniqueNameEspControl.value.trim();
        this.techniqueRequest.techniqueRequestCode = this.techniqueCodeEspControl.value;
        this.techniqueRequest.description = this.descriptionEspControl.value == null ? null : this.descriptionEspControl.value.trim();

        this.loading = true;
        this.manufactureService.updateTechniqueRequest(this.techniqueRequest).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let msg = { severity:'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity:'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    }
  }

  cancel() {
    this.router.navigate(['/manufacture/technique-request/list']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

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
