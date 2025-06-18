import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderItem } from '../../Models/order-item';
import { OrderService } from '../../Service/Order/order.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-cart-items',
  standalone: true,
  imports: [CommonModule,FormsModule], 
  templateUrl: './user-cart-items.component.html',
  styleUrls: ['./user-cart-items.component.css']
})
export class UserCartItemsComponent implements OnInit {
  order?: OrderItem[];

  constructor(private orderService: OrderService,private router: Router) {}

  ngOnInit(): void {
    this.orderService.GetUserCartItems().subscribe({
      next: (data) =>{this.order = data
        console.log(data);
      } ,

      error: (err) => console.error('Failed to load cart items', err)
    });
  }
  goToOrderPreview() {
  this.router.navigate(['/orderpervew']);
}

}
