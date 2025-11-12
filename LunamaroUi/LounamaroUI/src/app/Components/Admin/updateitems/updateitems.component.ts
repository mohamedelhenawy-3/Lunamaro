import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ItemService } from '../../../Service/Item/item.service';
import { FormsModule } from '@angular/forms';
import { UpdateItem } from '../../../Models/item/UpdateItem';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-updateitems',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './updateitems.component.html',
  styleUrls: ['./updateitems.component.css']
})
export class UpdateitemsComponent  {
  
}
