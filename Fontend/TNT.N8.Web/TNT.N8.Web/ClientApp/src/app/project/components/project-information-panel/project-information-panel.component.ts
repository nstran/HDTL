import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProjectModel } from '../../models/project.model';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-project-information-panel',
  templateUrl: './project-information-panel.component.html',
  styleUrls: ['./project-information-panel.component.css']
})
export class ProjectInformationPanelComponent implements OnInit {

  auth = JSON.parse(localStorage.getItem("auth"));
  projectId: string = '00000000-0000-0000-0000-000000000000';

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  project: ProjectModel = null;
  totalPercent: number = 0; // Tiến độ dự án
  totalEstimateTime: string = '0';
  totalHourUsed: number = 0;
  totalEE: number = 0;
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  constructor(
    private projectService: ProjectService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.projectId = params['projectId'];
      this.getMasterData();
    });
  }

  getMasterData(){
    this.projectService.getMasterDataProjectInformation(this.projectId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      if (result.statusCode === 200) {
        this.project = result.project;
        this.totalPercent = result.projectTaskComplete.toFixed(2);
        this.totalEstimateTime = result.totalEstimateHour;
        this.totalHourUsed = result.totalHourUsed;
        this.totalEE = result.totalEE;
      } 
    });
  }

}
