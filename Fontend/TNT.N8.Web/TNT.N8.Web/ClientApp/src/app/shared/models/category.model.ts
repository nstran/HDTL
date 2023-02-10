export class CategoryModel {
  CategoryId: string;
  CategoryName: string;
  CategoryCode: string;
  SortOrder: number;
  CategoryTypeId: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: string;
  Active: Boolean;
  IsEdit: Boolean;
  IsDefault: Boolean;
  StatusName: string;
  PotentialName: string;
  CategoryTypeName: string;
  constructor() { }
}
