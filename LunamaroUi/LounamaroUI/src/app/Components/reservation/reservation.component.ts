import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReservationService } from '../../Service/reservation/reservation.service';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.css'
})
export class ReservationComponent {
reservationForm:FormGroup;
message:string =''; 




constructor(private fg:FormBuilder,private reservationservice:ReservationService){
  this.reservationForm = this.fg.group({
        tableId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      guests: ['', [Validators.required, Validators.min(1)]],
      notes: ['']
  })
}


onsubmit(){
   if(this.reservationForm.valid){
    this.reservationservice.addreservation(this.reservationForm.value).subscribe({
          next: res => this.message = 'Reservation added successfully!',
          error: err => this.message = 'Failed to add reservation.'
    })
   }
}
}
