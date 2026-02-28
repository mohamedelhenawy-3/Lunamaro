import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeeklyDealsComponentComponent } from './weekly-deals-component.component';

describe('WeeklyDealsComponentComponent', () => {
  let component: WeeklyDealsComponentComponent;
  let fixture: ComponentFixture<WeeklyDealsComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeeklyDealsComponentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeeklyDealsComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
