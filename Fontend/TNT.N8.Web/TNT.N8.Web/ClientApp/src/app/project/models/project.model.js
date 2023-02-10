"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProjectModel = void 0;
var ProjectModel = /** @class */ (function () {
    function ProjectModel() {
        this.projectId = '00000000-0000-0000-0000-000000000000';
        this.projectCode = '';
        this.projectName = '';
        this.customerId = null;
        this.contractId = null;
        this.employeeId = null;
        this.employeeSM = [];
        this.employeeSub = [];
        this.description = null;
        this.projectType = null;
        this.projectSize = null;
        this.projectStatus = null;
        this.butget = null;
        this.butgetType = null;
        this.priority = null;
        this.projectStart = null;
        this.projectEnd = null;
        this.actualStart = null;
        this.actualEnd = null;
        this.includeWeekend = null;
        this.createdById = '00000000-0000-0000-0000-000000000000';
        this.createdDate = new Date();
        this.updatedById = null;
        this.updatedDate = null;
    }
    return ProjectModel;
}());
exports.ProjectModel = ProjectModel;
//# sourceMappingURL=project.model.js.map