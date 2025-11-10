import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlledOrderHistoryComponent } from './controlled-order-history.component';

describe('ControlledOrderHistoryComponent', () => {
  let component: ControlledOrderHistoryComponent;
  let fixture: ComponentFixture<ControlledOrderHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ControlledOrderHistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ControlledOrderHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
