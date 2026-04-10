import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddonrewardComponent } from './addonreward.component';

describe('AddonrewardComponent', () => {
  let component: AddonrewardComponent;
  let fixture: ComponentFixture<AddonrewardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddonrewardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddonrewardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
