import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatedTableComponent } from './updated-table.component';

describe('UpdatedTableComponent', () => {
  let component: UpdatedTableComponent;
  let fixture: ComponentFixture<UpdatedTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdatedTableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdatedTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
