import { EmployeeService } from './../../../employee/services/employee.service';
import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-chon-nv-dialog',
  templateUrl: './chon-nv-dialog.component.html',
  styleUrls: ['./chon-nv-dialog.component.css']
})
export class ChonNvDialogComponent implements OnInit {
  listEmployee: Array<any> = [];
  listSelectedEmployee: Array<any> = [];

  constructor(
    private messageService: MessageService,
    public employeeService: EmployeeService,
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
  ) { 
    
  }

  ngOnInit(): void {
    this.employeeService.getAllEmployee().subscribe(res => {
      let result: any = res;

      if (result.statusCode == 200) {
        this.listEmployee = result.employeeList;

        if (this.config.data.listEmployeeId?.length > 0) {
          this.listSelectedEmployee = this.listEmployee.filter(x => this.config.data.listEmployeeId.includes(x.employeeId));
          this.listEmployee = this.listEmployee.filter(x => !this.config.data.listEmployeeId.includes(x.employeeId));
        }
      }
      else {
        this.showMessage('error', result.messageCode);
      }
    });
  }

  dong() {
    this.ref.close();
  }

  xacNhan() {
    this.ref.close(this.listSelectedEmployee);
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
