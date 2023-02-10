import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddProjectScopeDialogComponent } from './add-project-scope-dialog.component';

describe('AddProjectTaskDialogComponent', () => {
  let component: AddProjectScopeDialogComponent;
  let fixture: ComponentFixture<AddProjectScopeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AddProjectScopeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddProjectScopeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
