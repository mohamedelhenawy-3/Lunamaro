import { Component, OnInit } from '@angular/core';
import { Category } from '../../../Models/category';
import { CategoryService } from '../../../Service/Category/category.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent  implements OnInit{
Categories:Category[] =[] as Category[];
 newcategory: string = '';
constructor(private _apicat:CategoryService){

}

ngOnInit(): void {
  this.loadCatgeories();
}

loadCatgeories(){
  this._apicat.getallCategories().subscribe({
    next: (res) => {
      // Filter out null or undefined entries
      this.Categories = res.filter(cat => cat != null && cat.name != null);
      console.log(this.Categories);
    },
    error: (error) => {
      console.log(error);
    }
  });
}

addCategory() {
  const newCategoryName = this.newcategory.trim();
  if (!newCategoryName) return;

  this._apicat.createCategory({name: newCategoryName }).subscribe({
    next: (createdCategory) => {
      this.Categories.push(createdCategory);
      this.newcategory = '';
    },
    error: (err) => {
      console.error('Failed to add category', err);
    }
  });
}
logCategory(cat: any) {
  console.log(cat);
}

// deleteCategory(id: number) {
//   this._apicat.deleteCategory(id).subscribe({
//     next: () => {
      
//       // Remove deleted category from the list without refreshing the page
//       this.Categories = this.Categories.filter(cat => cat.id !== id);
//     },
//     error: (err) => {
//       console.error('Failed to delete category', err);
//     }
//   });
// }






}
