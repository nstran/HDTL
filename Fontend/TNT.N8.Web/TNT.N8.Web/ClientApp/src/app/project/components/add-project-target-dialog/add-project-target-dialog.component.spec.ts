import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddProjectTargetDialogComponent } from './add-project-target-dialog.component';

describe('AddProjectTargetDialogComponent', () => {
  let component: AddProjectTargetDialogComponent;
  let fixture: ComponentFixture<AddProjectTargetDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddProjectTargetDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddProjectTargetDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
