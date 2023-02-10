import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddProjectResourceDialogComponent } from './add-project-resource-dialog.component';

describe('AddProjectResourceDialogComponent', () => {
  let component: AddProjectResourceDialogComponent;
  let fixture: ComponentFixture<AddProjectResourceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddProjectResourceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddProjectResourceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
