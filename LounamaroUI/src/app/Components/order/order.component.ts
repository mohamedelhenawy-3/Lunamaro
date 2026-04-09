import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../Service/Order/order.service';
import { OrderDetails } from '../../Models/order-details';
import { OrderRes } from '../../Models/User/orderres';

import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OrderInfo } from '../../Models/User/orderUserInfo';
import { Router } from '@angular/router';
import { Usercart } from '../../Models/usercart';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
isSubmitting = false;
  formChanges:boolean = false;

  order?: OrderDetails;
 temporaryKey: string | null = null;

  // ✅ Model bound to form
  orderInfo: OrderInfo = {
    temporaryKey:'',
    name: '',
    phoneNumber: '',
    deliveryStreetAddress: '',
    city: '',
    state: '',
    postalCode: 0,
      IsPayOnDelivery: true  // default Cash

  };

  constructor(private orderservice: OrderService,private router:Router,private cartcount:UsercartService)   
  {
  }

  ngOnInit(): void {
    this.orderservice.GetOrderPerview().subscribe({
      next: (data) => {
        this.order = data;
        console.log("Preview Data:", data);
      },
      error: (err) => console.log(err)
    });
  }
canExit():boolean{
  if(this.formChanges && !this.isSubmitting){
    return confirm("you havent submitted the form yet, are you sure you want to Leave?");
  }
  return true;
}
 
  placeOrder(form:NgForm) {
    if(form.invalid){
     alert("Please Complete the Form Correctly");
     return;
    }

    if(this.isSubmitting) return;

    this.isSubmitting=true;
 if (!this.temporaryKey) {
    this.temporaryKey = uuidv4();
  }

  // 🔥 THIS IS THE IMPORTANT LINE
  this.orderInfo.temporaryKey = this.temporaryKey;

    this.orderservice.placeOrder(this.orderInfo).subscribe({
      next: (res: OrderRes) => {
          this.isSubmitting = false;
       
      if(res.paymentUrl){
        this.cartcount.fetchCartCount();
        console.log("Order Created ✅", res);
        window.location.href = res.paymentUrl;
      }else{
              alert("Order placed successfully! Pay on delivery.");
               this.router.navigate(['/Home']);

      }
      this.temporaryKey = null;
      },
      error: err => {
  this.isSubmitting = false;

  if (err.status === 409) {
    alert('You already submitted this order.');
  } else if (err.status === 400) {
    alert('Invalid data, please check the form.');
  } else {
    alert('You make This order Befor plz dont play in this buttom .');
  }
}

    });
  }

}
