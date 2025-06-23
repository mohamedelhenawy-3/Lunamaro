import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderItem } from '../../Models/order-item';
import { OrderService } from '../../Service/Order/order.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { Usercart } from '../../Models/usercart';
import { Updatequantity } from '../../Models/updatequantity';

@Component({
  selector: 'app-user-cart-items',
  standalone: true,
  imports: [CommonModule,FormsModule], 
  templateUrl: './user-cart-items.component.html',
  styleUrls: ['./user-cart-items.component.css']
})
export class UserCartItemsComponent implements OnInit {
  cartitems?: Usercart[];

  constructor(private usercart: UsercartService,private router: Router) {}

  ngOnInit(): void {
    this.usercart.getCartItems().subscribe({
      next: (data) =>{this.cartitems = data
        console.log(data);
      } ,

      error: (err) => console.error('Failed to load cart items', err)
    });
  }
  goToOrderPreview() {
  this.router.navigate(['/orderpervew']);
} 
  updateQuantity(item: Usercart) {
    const updatedto: Updatequantity = {
      cartItemId: item.userCartId,
      newQuantity: item.quantity
    };

    this.usercart.updatequantity(updatedto).subscribe({
      next: () => console.log('Quantity updated'),
      error: (err) => console.error('Failed to update quantity', err)
    });
  }

  increaseQuantity(item: Usercart) {
    if (item.quantity < 10) {
      item.quantity++;
      this.updateQuantity(item);
    }
  }

  decreaseQuantity(item: Usercart) {
    if (item.quantity > 1) {
      item.quantity--;
      this.updateQuantity(item);
    }
  }
Remove(userCartId: number): void {
  this.usercart.deleteCart(userCartId).subscribe({
    next: () => {
      // Remove item from cartitems list after deletion
      this.cartitems = this.cartitems?.filter(item => item.userCartId !== userCartId);
    },
    error: (err) => {
      console.error('Error removing item from cart:', err);
    }
  });
}

}
