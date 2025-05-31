import { Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { Category } from '../../Models/category';
import { Item } from '../../Models/item';
import { CategoryListComponent } from "../Shared/category-list/category-list.component";
import { ItemListComponent } from "../Shared/item-list/item-list.component";
import { CategoryService } from '../../Service/Category/category.service';
import { ItemService } from '../../Service/Item/item.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, CategoryListComponent, ItemListComponent],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent implements OnInit {
 
categories: Category[] = [];
items:Item[]=[];
constructor( private categoryApi:CategoryService ,private  itemsapi:ItemService){
  
}
ngOnInit(): void {
  this.categoryApi.getallCategories().subscribe(data => this.categories = data);
  this.itemsapi.getallItems().subscribe(data => this.items = data); // ← FIXED
  console.log(this.categories);
}


  onCategorySelected(catId:number){
   this.itemsapi.getItemsByCategoryId(catId).subscribe(data => this.items = data);
  }

}