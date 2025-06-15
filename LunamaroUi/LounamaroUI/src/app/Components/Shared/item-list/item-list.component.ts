import { Component, EventEmitter, Input, Output, output } from '@angular/core';
import { Item } from '../../../Models/item';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
   @Input() items: Item[] = [];
   @Output() AddToCartClick= new EventEmitter<number>();



    onAddToCart(itemId: number) {
    this.AddToCartClick.emit(itemId);
  }
}
