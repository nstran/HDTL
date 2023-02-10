export class Vacancies {
    vacanciesId: string;
    recruitmentCampaignId: string;
    vacanciesName: string;
    quantity: number;
    priority: number;
    personInChargeId: string;
    typeOfWork: string;
    placeOfWork: string;
    experienceId: string;
    currency: string;
    salarType: number;
    salaryFrom: number;
    salaryTo: number;
    vacanciesDes: string;
    professionalRequirements: string;
    candidateBenefits: string;
    createdById: string;
    createdDate: Date;
    updatedById: string;
    updatedDate: Date;
    recruitmentCampaignName: string;
    priorityName: string;
    salary: string;
    experienceName: string;
    typeOfWorkName: string;
    personInChargeName: string;

    constructor() {
        this.vacanciesId = '00000000-0000-0000-0000-000000000000';
        this.recruitmentCampaignId = '00000000-0000-0000-0000-000000000000';
        this.vacanciesName = '';
        this.quantity = 0;
        this.priority = 0;
        this.personInChargeId = '00000000-0000-0000-0000-000000000000';
        this.typeOfWork = null;
        this.placeOfWork = null;
        this.experienceId = '00000000-0000-0000-0000-000000000000';
        this.currency = '';
        this.salarType = 0;
        this.salaryFrom = 0;
        this.salaryTo = 0;
        this.vacanciesDes = null;
        this.professionalRequirements = null;
        this.candidateBenefits = null;
        this.vacanciesDes = null;
        this.createdById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.createdDate = new Date();
        this.updatedById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.updatedDate = new Date();
        this.recruitmentCampaignName = null;
        this.priorityName = null;
        this.salary = null;
        this.experienceName = null;
        this.typeOfWorkName = null;
        this.personInChargeName = null;
    }
}
