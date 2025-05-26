import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { CommonModule } from '@angular/common';
import { CategoryService } from '../../../Service/Category/category.service';

@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [ CommonModule,ReactiveFormsModule],
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css'
})
export class AddCategoryComponent {
 categoryForm: FormGroup;


  message = '';

 constructor(private fb:FormBuilder,private categoryservice:CategoryService){

  this.categoryForm=this.fb.group({
    name:['', Validators.required]
  })
 }
  onSubmit() {
    if (this.categoryForm.valid) {
      this.categoryservice.createCategory(this.categoryForm.value).subscribe({
        next: () => {
           this.message = 'Category added successfully!';
          this.categoryForm.reset();
      
        },
        error: () => {
          this.message = 'Failed to add category.';
        }
      });
    }
}
}
