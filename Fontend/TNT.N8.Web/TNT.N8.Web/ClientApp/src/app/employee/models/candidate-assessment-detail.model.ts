export class CandidateAssessmentDetail {
    CandidateAssessmentDetailId: string;
    CandidateAssessmentId: string;
    ReviewsSectionId: string;
    Rating: number;
    SortOrder: number;
    Review: string;
    CreatedDate: Date;
    CreatedById: string;
    UpdatedDate: Date;
    UpdatedById: string;
    constructor() {
        this.CandidateAssessmentDetailId = '00000000-0000-0000-0000-000000000000';
        this.CandidateAssessmentId = '00000000-0000-0000-0000-000000000000';
        this.ReviewsSectionId = '00000000-0000-0000-0000-000000000000';
        this.Rating = 0;
        this.SortOrder = 0;
        this.Review = null;
        this.CreatedDate = new Date();
        this.CreatedById = JSON.parse(localStorage.getItem('auth')).UserId;
        this.UpdatedDate = new Date();
        this.UpdatedById = JSON.parse(localStorage.getItem('auth')).UserId;
    }
}