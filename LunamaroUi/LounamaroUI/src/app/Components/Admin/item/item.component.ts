import { Component, Input, OnInit } from '@angular/core';
import { Item } from '../../../Models/item';
import { ItemService } from '../../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-item',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './item.component.html',
  styleUrl: './item.component.css'
})
export class ItemComponent implements OnInit {
Items:Item[] =[] as Item[];




constructor(private _apiItem:ItemService,private router:Router){

}

ngOnInit(): void {
  this.loadCatgeories();
}

loadCatgeories(){
  this._apiItem.getallItems().subscribe({
    next: (res) => {
      // Filter out null or undefined entries
    this.Items = res.filter(item => item != null && item.name != null);

      console.log(this.Items);
    },
    error: (error) => {
      console.log(error);
    }
  });
}

deleteItem(id:number){
  this._apiItem.deleteItem(id).subscribe({
    next:()=>{
  console.log(this.Items)
        this.Items = this.Items.filter(item => item.id !== id);
    },
    error: (err) => {
      console.error('Failed to delete item', err);
    }
  })
}
ViewItem(id:number){
 this.router.navigate(['Admin/update-item',id]);
}
}
