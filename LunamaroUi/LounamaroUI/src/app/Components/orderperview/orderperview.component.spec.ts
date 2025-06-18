import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderperviewComponent } from './orderperview.component';

describe('OrderperviewComponent', () => {
  let component: OrderperviewComponent;
  let fixture: ComponentFixture<OrderperviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderperviewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OrderperviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
