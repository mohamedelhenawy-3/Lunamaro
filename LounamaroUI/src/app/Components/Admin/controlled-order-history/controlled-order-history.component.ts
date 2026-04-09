import { Component, OnInit } from '@angular/core';
import { orderhistory } from '../../../Models/Admin/Orders/orderhistory';
import { OrderService } from '../../../Service/Order/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentType } from '../../../Models/Admin/PaymentType';
import { OrderStatus } from '../../../Models/orderStatus';
import { Router } from '@angular/router';
import { filterList } from '../../../Shared/Utiles/search.helper';

@Component({
  selector: 'app-controlled-order-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './controlled-order-history.component.html',
  styleUrls: ['./controlled-order-history.component.css']
})
export class ControlledOrderHistoryComponent implements OnInit {

  // 🔍 Search
  searchTerm: string = '';
  private timeout: any;

  // 📊 Data
  allOrders: orderhistory[] = [];
  OrdersHistory: orderhistory[] = [];

  // enums
  paymentType = PaymentType;
  orderStatus = OrderStatus;

  constructor(private service: OrderService, private router: Router) {}

  ngOnInit(): void {
    this.loadOrderHistory();
  }

  private loadOrderHistory(): void {
    this.service.getOrders().subscribe({
      next: res => {
        const mapped = res.map(o => ({
          ...o,
          orderStatus: o.orderStatus as OrderStatus
        }));

        this.allOrders = mapped;      // ✅ original data
        this.OrdersHistory = mapped;  // ✅ displayed data
      },
      error: err => console.error(err)
    });
  }

  // 🔍 Apply Search Filter
  applyFilter() {
    clearTimeout(this.timeout);

    this.timeout = setTimeout(() => {
      if (!this.searchTerm) {
        this.OrdersHistory = this.allOrders;
        return;
      }

      this.OrdersHistory = filterList(
        this.allOrders,
        this.searchTerm,
        ['customerName', 'phoneNumber', 'orderId']
      );
    }, 300);
  }

  // 🔄 Update Status
  updateStatus(orderId: number, nextStatus: OrderStatus) {
    this.service.updateOrderStatus(orderId, nextStatus).subscribe({
      next: () => {
        this.loadOrderHistory();
        this.applyFilter(); // 🔥 keep search active
      },
      error: err => console.error(err)
    });
  }

  // 🔄 Next Status Logic
  getNextStatus(order: orderhistory): { text: string; value: OrderStatus } | null {
    switch(order.orderStatus) {
      case OrderStatus.Pending: return { text: 'Accept', value: OrderStatus.Accepted };
      case OrderStatus.Accepted: return { text: 'Processing', value: OrderStatus.Processing };
      case OrderStatus.Processing: return { text: 'Out For Delivery', value: OrderStatus.OutForDelivery };
      case OrderStatus.OutForDelivery: return { text: 'Delivered', value: OrderStatus.Delivered };
      default: return null;
    }
  }

  // 👁 View Details
  viewDetails(orderId: number) {
    this.router.navigate(['Admin/order/details', orderId]);
  }
}