import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserRescervationsComponent } from './user-rescervations.component';

describe('UserRescervationsComponent', () => {
  let component: UserRescervationsComponent;
  let fixture: ComponentFixture<UserRescervationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserRescervationsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UserRescervationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
