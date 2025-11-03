import { Component, OnInit } from '@angular/core';
import { RecievedReservation } from '../../../../Models/Reseviedreservations';
import { ReservationService } from '../../../../Service/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { FormsModule, NgModel } from '@angular/forms';
import { UpdateStatus } from '../../../../Models/updateStatus';

@Component({
  selector: 'app-controlled-recervations',
  standalone: true,
  imports: [CommonModule,FormsModule ],
  templateUrl: './controlled-recervations.component.html',
  styleUrl: './controlled-recervations.component.css'
})
export class ControlledRecervationsComponent implements OnInit{
  recervations:RecievedReservation[] =[];

    constructor(private recervationsservice:ReservationService){

    }


     ngOnInit(): void {
    this.recervationsservice.AllRecervations().subscribe({
      next: (data) => {
        this.recervations= data
        console.log(data);  // 👈 print the data from backend
      },
      error: (err) => {
        console.error(err); // 👈 print any error if occurs
      }
    });
 
  }
  onStatusChange(r:any){
    const dto :UpdateStatus ={Status :r.status};
    this.recervationsservice.UpdateReservation(r.id,dto).subscribe({
          next: () => {
    },
    error: (err) => console.error('Error updating:', err)
    })
  }

}