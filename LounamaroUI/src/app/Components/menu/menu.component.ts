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
import { Router, RouterLink } from '@angular/router';
import { AuthService } from 'src/app/Service/auth.service';



 
@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, CategoryListComponent, ItemListComponent,RouterLink],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
    encapsulation: ViewEncapsulation.None  // 👈 Add this line

})
export class MenuComponent implements OnInit {
 
categories: Category[] = [];
  items: Item[] = [];
  
  // متغيرات الـ Pagination الجديدة
  currentPage: number = 1;
  pageSize: number = 12;
  totalCount: number = 0;
  selectedCategoryId: number = 0;
  isLoading: boolean = false; // للـ Skeleton Shimmer

  constructor(
    private categoryApi: CategoryService,
    private itemsapi: ItemService,
    private cartsrviceapi: UsercartService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.categoryApi.getallCategories().subscribe(data => this.categories = data);
    this.loadMenuItems(); // استدعاء دالة التحميل المركزية
  }

  // دالة موحدة لجلب البيانات
  loadMenuItems(): void {
    this.isLoading = true;
    this.itemsapi.getItems(this.currentPage, this.pageSize, this.selectedCategoryId)
      .subscribe(response => {
        // لاحظ هنا: الـ response يحتوي على items و totalCount
        this.items = response.items;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      });
  }

  onCategorySelected(catId: number): void {
    this.selectedCategoryId = catId;
    this.currentPage = 1; // إعادة التعيين للصفحة الأولى عند تغيير الفلتر
    this.loadMenuItems();
  }

  // دالة لتغيير الصفحة (ستحتاجها في الـ HTML لاحقاً)
  onPageChange(newPage: number): void {
    this.currentPage = newPage;
    this.loadMenuItems();
    window.scrollTo(0, 0); // للعودة لأعلى الصفحة عند التنقل
  }

  addtocart(itemid: number): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    const dto: AddToCart = { itemId: itemid, quantity: 1 };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
      this.cartsrviceapi.fetchCartCount();
    });
  }


}