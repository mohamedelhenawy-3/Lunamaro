import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlleddetailsComponent } from './controlleddetails.component';

describe('ControlleddetailsComponent', () => {
  let component: ControlleddetailsComponent;
  let fixture: ComponentFixture<ControlleddetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ControlleddetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ControlleddetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
