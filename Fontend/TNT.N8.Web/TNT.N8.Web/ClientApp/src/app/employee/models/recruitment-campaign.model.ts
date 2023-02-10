export class RecruitmentCampaign {
    recruitmentCampaignId: string;
    recruitmentCampaignName: string;
    startDate: Date;
    endDateDate: Date;
    personInChargeId: string;
    recruitmentCampaignDes: string;
    createdById: string;
    createdDate: Date;
    updatedById: string;
    updatedDate: Date;
    quantityVacancies: number;
    personInChargeName: string;
    statusName: string;

    constructor() {
        this.recruitmentCampaignId = '00000000-0000-0000-0000-000000000000';
        this.recruitmentCampaignName = '';
        this.startDate = new Date();
        this.endDateDate = new Date();
        this.personInChargeId = '00000000-0000-0000-0000-000000000000';
        this.recruitmentCampaignDes = '';
        this.createdById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.createdDate = new Date();
        this.updatedById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.updatedDate = new Date();
        this.quantityVacancies = 0;
        this.personInChargeName = '';
        this.statusName = '';
    }
}
