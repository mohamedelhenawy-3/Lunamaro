import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateItemComponent } from './updateitems.component';


describe('UpdateitemsComponent', () => {
  let component: UpdateItemComponent;
  let fixture: ComponentFixture<UpdateItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateItemComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdateItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
