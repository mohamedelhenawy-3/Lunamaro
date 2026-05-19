import { Component, EventEmitter, Input, Output, output } from '@angular/core';
import { Item } from '../../../Models/item';
import { CommonModule } from '@angular/common';
import { environment } from 'src/environments/environment.prod';

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
  imageBaseUrl = environment.imageUrl;
   @Input() items: Item[] = [];
   @Output() AddToCartClick= new EventEmitter<number>();
  @Input() ClassMenu:string ='';


  
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
