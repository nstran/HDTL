import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductOptionsDetailComponent } from './product-options-detail.component';

describe('ProductOptionsDetailComponent', () => {
  let component: ProductOptionsDetailComponent;
  let fixture: ComponentFixture<ProductOptionsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductOptionsDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductOptionsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
