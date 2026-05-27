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
import { catchError, of } from 'rxjs';
import { OfflineService } from 'src/app/Service/OfflineSerivce/offline-service.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, CategoryListComponent, ItemListComponent, RouterLink],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
  encapsulation: ViewEncapsulation.None
})
export class MenuComponent implements OnInit {

  categories:         Category[] = [];
  items:              Item[]     = [];
  currentPage:        number     = 1;
  pageSize:           number     = 12;
  totalCount:         number     = 0;
  selectedCategoryId: number     = 0;
  isLoading:          boolean    = false;
  isOffline:          boolean    = false;

  constructor(
    private categoryApi:    CategoryService,
    private itemsapi:       ItemService,
    private cartsrviceapi:  UsercartService,
    private authService:    AuthService,
    private router:         Router,
    public  offlineService: OfflineService
  ) {}

  ngOnInit(): void {
    // Track online/offline changes
    this.offlineService.isOnline$.subscribe(online => {
      this.isOffline = !online;
      // Auto-refresh when coming back online
      if (online) this.loadMenuItems();
    });

    this.categoryApi.getallCategories().pipe(
      catchError(() => of([]))
    ).subscribe(data => this.categories = data);

    this.loadMenuItems();
  }

  loadMenuItems(): void {
    this.isLoading = true;
    this.itemsapi.getItems(this.currentPage, this.pageSize, this.selectedCategoryId)
      .pipe(catchError(() => of({ items: [], totalCount: 0 })))
      .subscribe(response => {
        this.items      = response.items      ?? [];
        this.totalCount = response.totalCount ?? 0;
        this.isLoading  = false;
      });
  }

  onCategorySelected(catId: number): void {
    this.selectedCategoryId = catId;
    this.currentPage        = 1;
    this.loadMenuItems();
  }

  onPageChange(newPage: number): void {
    this.currentPage = newPage;
    this.loadMenuItems();
    window.scrollTo(0, 0);
  }

  addtocart(itemid: number): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    if (!this.offlineService.isOnline) return; // ✅ block offline
    const dto: AddToCart = { itemId: itemid, quantity: 1 };
    this.cartsrviceapi.addToCart(dto).subscribe(() => {
      this.cartsrviceapi.fetchCartCount();
    });
  }
}