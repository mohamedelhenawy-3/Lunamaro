import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateitemsComponent } from './updateitems.component';

describe('UpdateitemsComponent', () => {
  let component: UpdateitemsComponent;
  let fixture: ComponentFixture<UpdateitemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateitemsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdateitemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
