import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatediscounttiersComponent } from './creatediscounttiers.component';

describe('CreatediscounttiersComponent', () => {
  let component: CreatediscounttiersComponent;
  let fixture: ComponentFixture<CreatediscounttiersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatediscounttiersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreatediscounttiersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
