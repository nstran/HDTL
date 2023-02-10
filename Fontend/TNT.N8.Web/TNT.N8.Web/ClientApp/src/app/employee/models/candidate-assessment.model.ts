export class CandidateAssessment {
    CandidateAssessmentId: string;
    CandidateId: string;
    VacanciesId: string;
    Status: number;
    OtherReview: string;
    EmployeeId: string;
    CreatedDate: Date;
    CreatedById: string;
    UpdatedDate: Date;
    UpdatedById: string;

    constructor() {
        this.CandidateAssessmentId = '00000000-0000-0000-0000-000000000000';
        this.CandidateId = '00000000-0000-0000-0000-000000000000';
        this.VacanciesId = '00000000-0000-0000-0000-000000000000';
        this.Status = 0;
        this.OtherReview = null;
        this.EmployeeId = JSON.parse(localStorage.getItem('auth')).UserId;
        this.CreatedDate = new Date();
        this.CreatedById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.UpdatedDate = new Date();
        this.UpdatedById = JSON.parse(localStorage.getItem('auth')).UserId;
    }
}