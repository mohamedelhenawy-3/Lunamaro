import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlledtableComponent } from './controlledtable.component';

describe('ControlledtableComponent', () => {
  let component: ControlledtableComponent;
  let fixture: ComponentFixture<ControlledtableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ControlledtableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ControlledtableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
