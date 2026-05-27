import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Item } from '../../../Models/item';
import { CommonModule } from '@angular/common';
import { environment } from 'src/environments/environment.prod';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
  imageBaseUrl = environment.imageUrl;

  @Input() items:     Item[]   = [];
  @Input() ClassMenu: string   = '';
  @Input() isOffline: boolean  = false; // ✅ new

  @Output() AddToCartClick = new EventEmitter<number>();

  onAddToCart(itemId: number) {
    this.AddToCartClick.emit(itemId);
  }

  onImageLoad(event: any) {
    event.target.classList.add('loaded');
  }

  trackById(index: number, item: any) {
    return item.id;
  }
}