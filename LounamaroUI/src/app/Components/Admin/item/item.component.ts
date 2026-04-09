import { Component, OnInit } from '@angular/core';
import { Item } from '../../../Models/item';
import { ItemService } from '../../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-item',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
  allItems: Item[] = [];
  Items: Item[] = [];
  searchTerm: string = '';

  constructor(private _apiItem: ItemService, private router: Router) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this._apiItem.getallItems().subscribe({
      next: (res) => {
        this.allItems = res.filter(item => item != null && item.name != null);
        this.Items = [...this.allItems];
      },
      error: (error) => console.log(error)
    });
  }

  applyFilter() {
    const term = this.searchTerm.toLowerCase();
    this.Items = this.allItems.filter(item =>
      item.name.toLowerCase().includes(term) ||
      item.description.toLowerCase().includes(term)
    );
  }

  deleteItem(id: number) {
    this._apiItem.deleteItem(id).subscribe({
      next: () => {
        this.allItems = this.allItems.filter(item => item.id !== id);
        this.Items = this.Items.filter(item => item.id !== id);
      },
      error: (err) => console.error('Failed to delete item', err)
    });
  }

  ViewItem(id: number) {
    this.router.navigate(['Admin/update-item', id]);
  }
}