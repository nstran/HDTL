export class ProjectScopeModel {
  projectScopeId: string;
  projectScopeCode: string;
  projectScopeName: string;
  description: string;
  projectId: string;
  level: number;
  createBy: string;
  styleClass: string;
  ischange: boolean;
  parentId: string;
  tenantId: string;
  resourceType: string;
  scopeChildList: Array<ProjectScopeModel>;
  listTask: Array<any>;
  listEmployee: Array<any>;
  children: Array<any>;
  supplyId: string;
  constructor() { }


}
