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
import { environment } from 'src/environments/environment.prod';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
  imageBaseUrl=environment.imageUrl
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
    IsPayOnDelivery: true  // default Cash

  };

  constructor(private orderservice: OrderService,private router:Router,private cartcount:UsercartService)   
  {
  }

  ngOnInit(): void {
    this.orderservice.GetOrderPerview().subscribe({
      next: (data) => {
        this.order = data;

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
getAddOnsTotal(item: any): number {
  if (!item?.addOns) return 0;

  return item.addOns.reduce((sum: number, a: any) => {
    return sum + (a.price || 0);
  }, 0);
}
getItemTotal(item: any): number {
  const addOnsTotal = this.getAddOnsTotal(item);
  return (item.price + addOnsTotal) * item.quantity;
}
 
placeOrder(form: NgForm) {
  if (form.invalid) {
    alert("Please Complete the Form Correctly");
    return;
  }

  if (this.isSubmitting) return;
  this.isSubmitting = true;

  if (!this.temporaryKey) {
    this.temporaryKey = uuidv4();
  }

  this.orderInfo.temporaryKey = this.temporaryKey;

  this.orderservice.placeOrder(this.orderInfo).subscribe({
    next: (res: OrderRes) => {
      this.isSubmitting = false;
      this.temporaryKey = null;

      // ← Reset cart count ONCE here for BOTH cases
      this.cartcount.resetCartCount();

      if (res.paymentUrl) {
        // Online payment — redirect to Stripe
        window.location.href = res.paymentUrl;

      } else {
        // Cash on delivery — go home
        alert("Order placed successfully! Pay on delivery.");
        this.router.navigate(['/Home']);
      }
    },
    error: (err) => {
      this.isSubmitting = false;

      if (err.status === 409) {
        alert('You already submitted this order.');
      } else if (err.status === 400) {
        alert('Invalid data, please check the form.');
      } else {
        alert('Something went wrong. Please try again.');
      }
    }
  });
}
// Add this function to your component
getFormattedImageUrl(path: string): string {
  if (!path) return 'assets/images/default-food.png'; // Fallback

  if (path.includes('/uploads/')) {
    const baseUrl = 'https://lunamaro-api-b4fjgkfdd0gde4hm.uaenorth-01.azurewebsites.net';
    return baseUrl + path; 
  }

  // Otherwise, use your standard full imageBaseUrl
  return this.imageBaseUrl + path;
}
}
