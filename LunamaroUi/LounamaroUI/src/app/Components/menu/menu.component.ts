import { Component, OnInit, ViewEncapsulation } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Category } from '../../Models/category';
import { Item } from '../../Models/item';
import { CategoryListComponent } from "../Shared/category-list/category-list.component";
import { ItemListComponent } from "../Shared/item-list/item-list.component";
import { CategoryService } from '../../Service/Category/category.service';
import { ItemService } from '../../Service/Item/item.service';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { AddToCart } from '../../Models/add-to-cart';



 
@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, CategoryListComponent, ItemListComponent],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
    encapsulation: ViewEncapsulation.None  // 👈 Add this line

})
export class MenuComponent implements OnInit {
 
categories: Category[] = [];
items:Item[]=[];
constructor( private categoryApi:CategoryService ,private  itemsapi:ItemService,private cartsrviceapi:UsercartService){
  
}
ngOnInit(): void {
  this.categoryApi.getallCategories().subscribe(
    data => {this.categories = data
      console.log(data);
    });
  this.itemsapi.getallItems().subscribe(data => this.items = data); // ← FIXED
}


  onCategorySelected(catId:number){
   this.itemsapi.getItemsByCategoryId(catId).subscribe(data => this.items = data);
  }


    addtocart(itemid:number){
  
    const dto: AddToCart = {     // Use dynamic userId if available
     itemId: itemid,
      quantity: 1               // default quantity
    };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
    this.cartsrviceapi.fetchCartCount(); // No need for userId
  });
    }
}