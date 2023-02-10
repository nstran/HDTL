import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService, MessageService, Table } from 'primeng';
import { tap } from 'rxjs/internal/operators/tap';
import { MilestoneConfigurationEntityModel } from '../../model/list-milestone';
import { ListMilestoneService } from '../../service/list-milestone.service';
import { moveItemInArray, CdkDragDrop } from "@angular/cdk/drag-drop";
@Component({
  selector: 'app-list-mission',
  templateUrl: './list-mission.component.html',
  styleUrls: ['./list-mission.component.css']
})
export class ListMissionComponent implements OnInit {
  @Output() createMilestoneOutput = new EventEmitter<MilestoneConfigurationEntityModel>();
  showModal: boolean = false;
  @ViewChild('dt') table: Table;
  loading = false;
  cols: any[];
  rows = 10;
  listMilestone: MilestoneConfigurationEntityModel[] = []
  createMilestone: MilestoneConfigurationEntityModel = new MilestoneConfigurationEntityModel();
  updateListMilestone: MilestoneConfigurationEntityModel[] = [];
  id: string;
  constructor(
    private confirmationService: ConfirmationService,
    private milestoneService: ListMilestoneService,
    private messageService: MessageService,
    private _activatedRoute: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this._activatedRoute.params.subscribe(params => {
      this.id = params['optionId'];
      this.loadTable();
    });
  }

  loadTable() {
    this.cols = [
      { field: 'sortOrder', header: 'STT', width: "60px", textAlign: "center" },
      { field: 'name', header: 'Tên điểm báo cáo', width: "auto", textAlign: "left" },
    ]
    if (this.id != null) this.getlistMilestoneServiceByOptionId(this.id);
  }

  deleteMilestroneConfigure(id: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.milestoneService.deleteMilestoneConfigure(id).subscribe(res => {
          if (res.statusCode == 200) {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Gửi phê duyệt thành công' };
            this.showMessage(msg);
            this.loadTable();
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: res.messageCode };
            this.showMessage(msg);
          }
        })
      }
    })
  }

  showToast(severity: string, summary: string, detail: string): void {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  open(): void {
    this.showModal = true;
  }

  close(): void {
    this.showModal = false;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  save() {
    this.loading = true;
    this.createMilestone.optionId = this.id;
    if (this.createMilestone.optionId) {
      this.milestoneService.createMilestoneConfigure(this.createMilestone).pipe(
        tap(() => {
          this.loading = false;

        })
      )
        .subscribe(result => {
          if (result.statusCode == 200) {
            this.showToast('success', 'Thông báo', 'Thêm tùy chọn thành công');
            this.getlistMilestoneServiceByOptionId(this.id);
            this.createMilestone = new MilestoneConfigurationEntityModel();
          } else {
            this.showToast('error', 'Thông báo', 'Thêm tùy chọn thất bại');
          }
        })
    }
    else {
      this.createMilestoneOutput.emit(this.createMilestone)
    }

  }
  
  getlistMilestoneServiceByOptionId(id: string): void {
    this.loading = false;
    this.milestoneService.getlistMilestoneServiceByOptionId(id)
      .pipe(
        tap(() => {
          this.loading = false;
        })
      ).subscribe(res => {
        this.listMilestone = res.listMilestoneConfiguration;
      })
  }

  cancel() {
    this.getlistMilestoneServiceByOptionId(this.id)
  }

  saveChange() {
    this.loading = true;
    this.listMilestone.forEach(e => {
      e.optionId = this.id;
    });
    this.milestoneService.UpdateListMilestoneConfigure(this.listMilestone).pipe(
      tap(() => {
        this.loading = false;
      })
    )
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Thay đổi thứ tự thành công');
          this.getlistMilestoneServiceByOptionId(this.id);
        } else {
          this.showToast('error', 'Thông báo', 'Thay đổi thứ tự thất bại');
        }
      })
  }
}
