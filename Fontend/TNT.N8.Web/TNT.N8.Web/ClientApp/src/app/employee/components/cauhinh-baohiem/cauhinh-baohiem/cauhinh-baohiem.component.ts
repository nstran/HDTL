import { Component, OnInit} from "@angular/core";
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';

@Component({
  selector: "app-cauhinh-baohiem",
  templateUrl: "./cauhinh-baohiem.component.html",
  styleUrls: ["./cauhinh-baohiem.component.css"],
})
export class CauHinhBaoHiemComponent implements OnInit {

  constructor(
    private getPermission: GetPermission,
    private router: Router
  ) { }

  ngOnInit(): void {
    this._getPermission();
  }

  async _getPermission() {
    let resource = "hrm/employee/cauhinh-baohiem/";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) { 
      this.router.navigate(["/home"]);
      return;
    }
  }
}