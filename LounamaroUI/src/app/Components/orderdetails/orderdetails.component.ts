import { Component, OnInit } from '@angular/core';
import { OrderHistoryDetails } from '../../Models/orderdetails';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../Service/Order/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment.prod';

@Component({
  selector: 'app-orderdetails',
  standalone: true,
  imports: [CommonModule,FormsModule,RouterLink],
  templateUrl: './orderdetails.component.html',
  styleUrl: './orderdetails.component.css'
})
export class OrderdetailsComponent implements OnInit {
  imageBaseUrl=environment.imageUrl
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
  // console.log(result);
      });
  }

}
