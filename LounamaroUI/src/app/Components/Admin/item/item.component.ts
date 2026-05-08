import { Component, OnInit } from '@angular/core';
import { Item } from '../../../Models/item';
import { ItemService } from '../../../Service/Item/item.service';
import { CategoryService } from '../../../Service/Category/category.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment.prod';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-item',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
   Items: Item[] = [];
  categories: any[] = [];
  searchTerm = '';
  private searchSubject = new Subject<string>();
  selectedCategoryId = 0;

  currentPage = 1;
  pageSize = 12;
  totalCount = 0;
  totalPages = 0;

  // State
  isLoading = false;
  isLoadingMore = false;
  hasError = false;

  constructor(
    private _apiItem: ItemService,
    private _categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadItems();
    this.loadCategories();
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.resetAndLoad();
    });
  }

   onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  onCategoryClick(catId: number) {
    this.selectedCategoryId = catId;
    this.resetAndLoad();
  }
    resetAndLoad() {
    this.currentPage = 1;
    this.Items = [];
    this.loadItems(true);
  }
 loadItems(isFirst = false) {
    if (isFirst) {
      this.isLoading = true;
    } else {
      this.isLoadingMore = true;
    }
    this.hasError = false;

    this._apiItem.getPaginatedItems(
      this.currentPage,
      this.pageSize,
      this.selectedCategoryId || undefined,
      this.searchTerm || undefined
    ).subscribe({
      next: (res) => {
        if (isFirst) {
          this.Items = res.items;
        } else {
          this.Items = [...this.Items, ...res.items]; // append for load more
        }
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.isLoading = false;
        this.isLoadingMore = false;
      },
      error: (err) => {
        console.error(err);
        this.hasError = true;
        this.isLoading = false;
        this.isLoadingMore = false;
      }
    });
  }
  loadMore() {
    if (this.currentPage >= this.totalPages) return;
    this.currentPage++;
    this.loadItems(false);
  }

  get hasMore(): boolean {
    return this.currentPage < this.totalPages;
  }
  loadCategories() {
    this._categoryService.getallCategories().subscribe({
      next: (res) => this.categories = res,
      error: (err) => console.log(err)
    });
  }
  deleteItem(id: number) {
    if (!confirm('Are you sure you want to delete this item?')) return;
    this._apiItem.deleteItem(id).subscribe({
      next: () => {
        this.Items = this.Items.filter(item => item.id !== id);
        this.totalCount--;
      },
      error: (err) => console.error('Failed to delete item', err)
    });
  }
 trackById(index: number, item: Item): number {
    return item.id;
  }
  ViewItem(id: number) {
    this.router.navigate(['Admin/update-item', id]);
  }
}