import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ReservationService } from '../../Service/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.css']
})
export class ReservationComponent {
  reservationForm: FormGroup;
  availableTables: any[] = [];
  message: string = '';
  

  constructor(private fb: FormBuilder, private reservationService: ReservationService,private router: Router // <-- Inject Router
) {
    this.reservationForm = this.fb.group({
      tableId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      guests: ['', [Validators.required, Validators.min(1)]],
      notes: ['']
    });
  }

  loadAvailableTables() {
    const startTime = this.reservationForm.value.startTime;
    const endTime = this.reservationForm.value.endTime;
    const guests = this.reservationForm.value.guests;

    if (!startTime || !endTime || !guests) return;

    this.reservationService.getAvailableTables(startTime, endTime, guests).subscribe({
      next: (tables: any[]) => this.availableTables = tables,
      error: (err) => console.error(err)
    });
  }
  onsubmit() {
    if (this.reservationForm.valid) {
      this.reservationService.addreservation(this.reservationForm.value)
        .subscribe({
          next: res => {
            this.message = 'Reservation added successfully!';
            setTimeout(() => this.router.navigate(['/Home']), 1000);
          },
          error: err => this.message = 'Failed to add reservation.'
        });
    }
  }
}
