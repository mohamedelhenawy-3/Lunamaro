import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddonrewardeditComponent } from './addonrewardedit.component';

describe('AddonrewardeditComponent', () => {
  let component: AddonrewardeditComponent;
  let fixture: ComponentFixture<AddonrewardeditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddonrewardeditComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddonrewardeditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
