import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../Service/Order/order.service';
import { OrderDetails } from '../../Models/order-details';
import { OrderRes } from '../../Models/User/orderres';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OrderInfo } from '../../Models/User/orderUserInfo';
import { Router } from '@angular/router';
import { Usercart } from '../../Models/usercart';
import { UsercartService } from '../../Service/UserCart/usercart.service';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {

  order?: OrderDetails;

  // ✅ Model bound to form
  orderInfo: OrderInfo = {
    name: '',
    phoneNumber: '',
    deliveryStreetAddress: '',
    city: '',
    state: '',
    postalCode: 0,
      IsPayOnDelivery: true  // default Cash

  };

  constructor(private orderservice: OrderService,private router:Router,private cartcount:UsercartService) {}

  ngOnInit(): void {
    this.orderservice.GetOrderPerview().subscribe({
      next: (data) => {
        this.order = data;
        console.log("Preview Data:", data);
      },
      error: (err) => console.log(err)
    });
  }

  placeOrder() {
    this.orderservice.placeOrder(this.orderInfo).subscribe({
      next: (res: OrderRes) => {
      if(res.paymentUrl){
        this.cartcount.fetchCartCount();
        console.log("Order Created ✅", res);
        window.location.href = res.paymentUrl;
      }else{
              alert("Order placed successfully! Pay on delivery.");
               this.router.navigate(['/Home']);

      }
      },
      error: (err) => console.log(err)
    });
  }

}
