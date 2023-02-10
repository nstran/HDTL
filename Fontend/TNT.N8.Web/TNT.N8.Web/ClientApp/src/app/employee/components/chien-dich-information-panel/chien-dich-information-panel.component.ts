import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RecruitmentCampaign } from '../../models/recruitment-campaign.model';
import { EmployeeService } from '../../services/employee.service';

@Component({
  selector: 'app-chien-dich-information-panel',
  templateUrl: './chien-dich-information-panel.component.html',
  styleUrls: ['./chien-dich-information-panel.component.css']
})
export class RecruitmentCampaignInforPanelComponent implements OnInit {

  auth = JSON.parse(localStorage.getItem("auth"));
  recruitmentCampaignId: string = '00000000-0000-0000-0000-000000000000';
  vacanciesId: string = '00000000-0000-0000-0000-000000000000';

  /*START: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  recruitmentCampaign: RecruitmentCampaign = null;
  /*END: BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/

  constructor(
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.recruitmentCampaignId = params['recruitmentCampaignId'];
      this.vacanciesId = params['vacanciesId'];
      this.getMasterData();
    });
  }

  getMasterData() {
    this.employeeService.getMasterDataRecruitmentCampaignInformation(this.recruitmentCampaignId, this.auth.UserId).subscribe(response => {
      let result: any = response;
      if (result.statusCode === 200) {
        this.recruitmentCampaign = result.recruitmentCampaign;
      }
    });
  }

}
