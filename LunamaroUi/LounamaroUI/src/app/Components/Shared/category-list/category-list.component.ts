import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Category } from '../../../Models/category';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent {
  @Input() categories: Category[] = [];
  @Output() categorySelected = new EventEmitter<number>();

  onCategoryClick(catId: number) {
    this.categorySelected.emit(catId);
  }
}
