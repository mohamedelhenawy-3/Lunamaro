import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';
import { ItemService } from '../../Service/Item/item.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent {
  categories: any[] = [];
  items: any[] = [];


  constructor(private apiitem: ItemService) {}




}
