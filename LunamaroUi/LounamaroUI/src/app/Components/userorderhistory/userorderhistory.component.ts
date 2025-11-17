import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../Service/Order/order.service';
import { userorderhostory } from '../../Models/userorderhistory';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-userorderhistory',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './userorderhistory.component.html',
  styleUrl: './userorderhistory.component.css'
})
export class UserorderhistoryComponent implements OnInit{
  orderhistroy:userorderhostory[] =[];
    private refreshInterval: any;

constructor(private service:OrderService,private router:Router,private Loc:Location){
}
  ngOnInit(): void {

        // 🔥 Auto-refresh every 10 seconds
    this.refreshInterval = setInterval(() => {
         this.loadOrderHistory();
    }, 2000);
  }

  private loadOrderHistory(): void {
    this.service.orderhistory().subscribe({
      next: (data: userorderhostory[]) => {
        this.orderhistroy = data ?? [];
        console.log(data);
      },
      error: (err: any) => {
        console.error('Failed to load user order history', err);
        this.orderhistroy = [];
      }
    });
  }
  ngOnDestroy(): void {
    clearInterval(this.refreshInterval);
  }

viewDetails(orderId: number) {
  this.router.navigate(['details', orderId]);
}

reload(){
  setTimeout(()=>{
    window.location.reload();
  },100);
}

}
