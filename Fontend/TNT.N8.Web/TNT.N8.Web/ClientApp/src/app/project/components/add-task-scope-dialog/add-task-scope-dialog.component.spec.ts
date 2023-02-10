import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddTaskScopeDialogComponent } from './add-task-scope-dialog.component';

describe('AddTaskScopeDialogComponent', () => {
  let component: AddTaskScopeDialogComponent;
  let fixture: ComponentFixture<AddTaskScopeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AddTaskScopeDialogComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddTaskScopeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
