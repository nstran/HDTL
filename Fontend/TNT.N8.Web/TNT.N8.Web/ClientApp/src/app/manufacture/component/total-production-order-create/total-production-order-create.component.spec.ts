import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TotalProductionOrderCreateComponent } from './total-production-order-create.component';

describe('TotalProductionOrderCreateComponent', () => {
  let component: TotalProductionOrderCreateComponent;
  let fixture: ComponentFixture<TotalProductionOrderCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TotalProductionOrderCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TotalProductionOrderCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
