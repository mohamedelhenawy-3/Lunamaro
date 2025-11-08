import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserorderhistoryComponent } from './userorderhistory.component';

describe('UserorderhistoryComponent', () => {
  let component: UserorderhistoryComponent;
  let fixture: ComponentFixture<UserorderhistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserorderhistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UserorderhistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
