import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeaklydealeditComponent } from './weaklydealedit.component';

describe('WeaklydealeditComponent', () => {
  let component: WeaklydealeditComponent;
  let fixture: ComponentFixture<WeaklydealeditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeaklydealeditComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeaklydealeditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
