import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ReservationService } from '../../Service/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.css']
})
export class ReservationComponent {

  reservationForm: FormGroup;
  availableTables: any[] = [];
  message: string = '';
  messageType: 'success-text' | 'error-text' = 'error-text';

  isSubmitting = false;

  timeSlots: string[] = [];
  today: string = new Date().toISOString().split('T')[0];

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private router: Router
  ) {
    this.reservationForm = this.fb.group({
      tableId: ['', Validators.required],
      date: ['', Validators.required],
      timeSlot: ['', Validators.required],
      startTime: [''],
      endTime: [''],
      guests: [1, [Validators.required, Validators.min(1)]],
      notes: ['']
    });
  }

  // =========================
  // TIME SLOTS
  // =========================
  generateTimeSlots() {
    this.timeSlots = [];

    let start = 9 * 60;
    let end = 23 * 60 + 30;
    while (start <= end) {
      const hours = Math.floor(start / 60);
      const minutes = start % 60;

      this.timeSlots.push(this.formatTime(hours, minutes));
      start += 30;
    }

    this.reservationForm.patchValue({
      timeSlot: '',
      startTime: '',
      endTime: ''
    });

    this.availableTables = [];
  }

  formatTime(hours: number, minutes: number): string {
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const h = hours % 12 || 12;
    const m = minutes.toString().padStart(2, '0');
    return `${h}:${m} ${ampm}`;
  }

  // =========================
  // TIME CHANGE
  // =========================
  onTimeChange() {
    const date = this.reservationForm.value.date;
    const time = this.reservationForm.value.timeSlot;

    if (!date || !time) return;

    const start = this.combine(date, time);
    const end = new Date(start);
    end.setHours(end.getHours() + 1);

    if (start < new Date(Date.now() + 60 * 60 * 1000)) {
      this.messageType = 'error-text';
      this.message = "Reservation must be at least 1 hour in advance";
      return;
    }

    this.reservationForm.patchValue({
      startTime: start.toISOString(),
      endTime: end.toISOString()
    });

    this.loadAvailableTables();
  }

  // =========================
  // COMBINE
  // =========================
  combine(date: string, time: string): Date {
    const [t, modifier] = time.split(' ');
    let [hours, minutes] = t.split(':').map(Number);

    if (modifier === 'PM' && hours !== 12) hours += 12;
    if (modifier === 'AM' && hours === 12) hours = 0;

    const result = new Date(date);
    result.setHours(hours, minutes, 0, 0);

    return result;
  }

  // =========================
  // LOAD TABLES
  // =========================
  loadAvailableTables() {
    const { startTime, endTime, guests } = this.reservationForm.value;

    if (!startTime || !endTime || !guests) return;

    this.reservationService.getAvailableTables(startTime, endTime, guests)
      .subscribe({
        next: (tables) => {
          this.availableTables = tables;

          this.messageType = tables.length === 0 ? 'error-text' : 'success-text';
          this.message = tables.length === 0
            ? "No tables available for this time."
            : "Tables loaded successfully.";
        },
        error: () => {
          this.messageType = 'error-text';
          this.message = "Error loading tables.";
        }
      });
  }

  // =========================
  // SUBMIT
  // =========================
  onsubmit() {
    if (this.reservationForm.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    this.message = '';

    const form = this.reservationForm.value;

    const payload = {
      tableId: form.tableId,
      guests: form.guests,
      notes: form.notes,
      startTime: form.startTime,
      endTime: form.endTime
    };

    this.reservationService.addreservation(payload)
      .subscribe({
        next: () => {
          this.isSubmitting = false;

          this.messageType = 'success-text';
          this.message = "Reservation confirmed successfully!";

          this.router.navigate(['/Home']);
        },

        error: (err) => {
          this.isSubmitting = false;

          this.messageType = 'error-text';

          if (typeof err?.error === 'string') {
            this.message = err.error;
          }
          else if (err?.error?.message) {
            this.message = err.error.message;
          }
          else if (err?.status === 400) {
            this.message = "Invalid reservation data.";
          }
          else {
            this.message = "Something went wrong. Please try again.";
          }
        }
      });
  }
}