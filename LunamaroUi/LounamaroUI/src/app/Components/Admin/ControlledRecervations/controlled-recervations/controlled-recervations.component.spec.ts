import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlledRecervationsComponent } from './controlled-recervations.component';

describe('ControlledRecervationsComponent', () => {
  let component: ControlledRecervationsComponent;
  let fixture: ComponentFixture<ControlledRecervationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ControlledRecervationsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ControlledRecervationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
