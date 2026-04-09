import { Component, OnInit } from '@angular/core';
import { userrecervation } from '../../Models/User/userreciervation';
import { ReservationService } from '../../Service/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-rescervations',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './user-rescervations.component.html',
  styleUrl: './user-rescervations.component.css'
})
export class UserRescervationsComponent implements OnInit {
 userrecervation:userrecervation[] =[];



 constructor(private recservice:ReservationService){

 }
  ngOnInit(): void {
    this.loadReservations();
  }

  private loadReservations(): void {
    // Assumes ReservationService exposes a method that returns Observable<userrecervation[]>
    // Rename `getUserReservations` if your service uses a different name.
    this.recservice.UserReservation()?.subscribe({
      next: (items) => (this.userrecervation = items ?? [] ),
      error: (err) => {
        console.error('Failed to load user reservations', err);
        this.userrecervation = [];
      },
    });
  }
    cancelReservation(id:number):void{
        if (confirm('Are you sure you want to cancel this reservation?')) {
          this.recservice.cancelReservation(id).subscribe({
                next: () => {
        alert('Reservation cancelled successfully');
        this.loadReservations(); // refresh list
      },
      error: (err) => console.error('Error cancelling reservation:', err)
          })
        }
    }

}
