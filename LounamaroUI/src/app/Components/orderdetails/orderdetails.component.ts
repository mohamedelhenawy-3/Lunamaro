import { Component, OnInit } from '@angular/core';
import { OrderHistoryDetails } from '../../Models/orderdetails';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../Service/Order/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-orderdetails',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './orderdetails.component.html',
  styleUrl: './orderdetails.component.css'
})
export class OrderdetailsComponent implements OnInit {
  orderId!: number;
  orderDetails!: OrderHistoryDetails ;


  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) {}
  ngOnInit(): void {
    this.orderId = Number(this.route.snapshot.paramMap.get("id"));
    
    this.orderService.getOrderHistoryDetailsAd(this.orderId)
      .subscribe(result => {
  this.orderDetails = result;
  console.log(result);
      });
  }

}
