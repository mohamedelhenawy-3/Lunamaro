import { Component, OnInit, OnDestroy } from '@angular/core';
import { OrderService } from '../../Service/Order/order.service';
import { userorderhostory } from '../../Models/userorderhistory';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-userorderhistory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './userorderhistory.component.html',
  styleUrls: ['./userorderhistory.component.css']
})
export class UserorderhistoryComponent implements OnInit, OnDestroy {
  orderhistroy: userorderhostory[] = [];
  private refreshInterval: any;

  constructor(
    private service: OrderService,
    private router: Router,
    private loc: Location
  ) {}

  ngOnInit(): void {
    this.loadOrderHistory();

    // Auto-refresh every 10 seconds
    this.refreshInterval = setInterval(() => {
      this.loadOrderHistory();
    }, 10000);
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

  viewDetails(orderId: number): void {
    this.router.navigate(['details', orderId]);
  }


}
