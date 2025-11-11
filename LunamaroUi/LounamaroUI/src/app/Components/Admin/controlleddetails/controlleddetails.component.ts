import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../../Service/Order/order.service';
import { OrderHistoryDetails } from '../../../Models/orderdetails';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-controlleddetails',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './controlleddetails.component.html',
  styleUrl: './controlleddetails.component.css'
})
export class ControlleddetailsComponent  implements OnInit{
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
