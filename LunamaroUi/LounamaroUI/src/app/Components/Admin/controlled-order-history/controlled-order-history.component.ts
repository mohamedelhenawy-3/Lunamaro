import { Component, OnInit } from '@angular/core';
import { orderhistory } from '../../../Models/Admin/Orders/orderhistory';
import { OrderService } from '../../../Service/Order/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentType } from '../../../Models/Admin/PaymentType';
import { OrderStatus } from '../../../Models/orderStatus';
import { Router } from '@angular/router';

@Component({
  selector: 'app-controlled-order-history',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './controlled-order-history.component.html',
  styleUrls: ['./controlled-order-history.component.css'] // plural "styleUrls"

})
export class ControlledOrderHistoryComponent implements OnInit {
 OrdersHistory:orderhistory[]= [];
paymentType=PaymentType;
orderStatus=OrderStatus;
 constructor(private service:OrderService ,private router:Router){

 }
  ngOnInit(): void {
    this.loadOrderHistory();
  }

private loadOrderHistory(): void {
  this.service.getOrders().subscribe({
    next: res => {
      this.OrdersHistory = res.map(o => ({
        ...o,
        orderStatus: o.orderStatus as OrderStatus
      }));
      console.log('Orders loaded:', this.OrdersHistory);
    },
    error: err => console.error(err)
  });
}


updateStatus(orderId: number, nextStatus: OrderStatus) {
  this.service.updateOrderStatus(orderId, nextStatus).subscribe({
    next: () => {
      console.log(`Order ${orderId} updated to ${nextStatus}`);
      this.loadOrderHistory(); // refresh the orders list
    },
    error: err => console.error(err)
  });
}


  
getNextStatus(order: orderhistory): { text: string; value: OrderStatus } | null {
  switch(order.orderStatus) {
    case OrderStatus.Pending: return { text: 'Accept', value: OrderStatus.Accepted };
    case OrderStatus.Accepted: return { text: 'Processing', value: OrderStatus.Processing };
    case OrderStatus.Processing: return { text: 'Out For Delivery', value: OrderStatus.OutForDelivery };
    case OrderStatus.OutForDelivery: return { text: 'Delivered', value: OrderStatus.Delivered };
    default: return null; // Delivered or Cancelled → no button
  }
}



viewDetails(orderId: number) {
this.router.navigate(['Admin/order/details', orderId]);

}
}
