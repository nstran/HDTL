import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductOptionsListComponent } from './product-options-list.component';

describe('ProductOptionsListComponent', () => {
  let component: ProductOptionsListComponent;
  let fixture: ComponentFixture<ProductOptionsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductOptionsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductOptionsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
