import { Component, OnInit } from '@angular/core';
import { OrderDetails } from '../../Models/order-details';
import { OrderService } from '../../Service/Order/order.service';
import { AuthService } from '../../Service/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-orderperview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orderperview.component.html',
  styleUrl: './orderperview.component.css'
})
export class OrderperviewComponent implements OnInit {
  order?: OrderDetails;

  constructor(private orderService: OrderService, private authService: AuthService) {}

  ngOnInit(): void {
    const userId = this.authService.getUserId();
    if (userId !== null) {
      this.orderService.GetOrderPerview().subscribe({
        next: (data) => {
          this.order = data;
          console.log(data);
        },
        error: (err) => console.error('Error loading order', err)
      });
    } else {
      console.error('User ID not found in token');
    }
  }
}
