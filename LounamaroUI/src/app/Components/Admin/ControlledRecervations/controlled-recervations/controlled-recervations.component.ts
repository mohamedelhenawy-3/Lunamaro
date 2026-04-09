import { Component, OnInit } from '@angular/core';
import { RecievedReservation } from '../../../../Models/Reseviedreservations';
import { ReservationService } from '../../../../Service/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { FormsModule, NgModel } from '@angular/forms';
import { UpdateStatus } from '../../../../Models/updateStatus';
import { filterList } from '../../../../Shared/Utiles/search.helper';

@Component({
  selector: 'app-controlled-recervations',
  standalone: true,
  imports: [CommonModule,FormsModule ],
  templateUrl: './controlled-recervations.component.html',
  styleUrl: './controlled-recervations.component.css'
})
export class ControlledRecervationsComponent implements OnInit{


searchTerm: string = '';
allReservations: any[] = [];
recervations: any[] = [];
  // recervations:RecievedReservation[] =[];

    constructor(private recervationsservice:ReservationService){

    }


 ngOnInit(): void {
  this.recervationsservice.AllRecervations().subscribe({
    next: (data) => {
      this.allReservations = data;
      this.recervations = data; // 🔥 VERY IMPORTANT
      console.log(data);
    },
    error: (err) => {
      console.error(err);
    }
  });
}
 applyFilter() {
  if (!this.searchTerm) {
    this.recervations = this.allReservations;
    return;
  }

  this.recervations = filterList(
    this.allReservations,
    this.searchTerm,
    ['userEmail', 'tableName', 'status']
  );
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