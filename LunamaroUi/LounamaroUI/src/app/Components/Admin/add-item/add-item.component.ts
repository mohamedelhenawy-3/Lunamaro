import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ItemService } from '../../../Service/Item/item.service';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../../Service/Category/category.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-item',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './add-item.component.html',
  styleUrl: './add-item.component.css'
})
export class AddItemComponent implements OnInit {
  Itemform: FormGroup;
  categories: any[] = [];
  selectedFile: File | null = null;  // ✅ Store file separately



  formChanges:boolean = false;
  constructor(
    private fb: FormBuilder,
    private apiitem: ItemService,
    private catservice: CategoryService,
    private router:Router
  ) {
    this.Itemform = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      quantity:['',[Validators.required,Validators.min(1)]],
      price: ['', [Validators.required,Validators.min(1)]],
      categoryId: ['', [Validators.required,Validators.min(1)]]
    });


    this.Itemform.valueChanges.subscribe(()=>{
      this.formChanges = true;
    })
  }

  ngOnInit(): void {
    this.catservice.getallCategories().subscribe({
      next: (data) => {
        this.categories = data;
        console.log(data);
      }
    });
  }

  // ✅ Called when file input changes
  onFileSelected(event: any): void {
    if (event.target.files && event.target.files.length > 0) {
      this.selectedFile = event.target.files[0];
    }
  }


  canExit(): boolean {
    if (this.formChanges) {
      alert("You have unsaved changes. Do you really want to leave?");
      return confirm("You have unsaved changes. Do you really want to leave?");
    }
    return true;
  }
  AddItem(): void {
    if (this.Itemform.valid && this.selectedFile) {
      const formData = new FormData();
      formData.append('name', this.Itemform.get('name')?.value);
      formData.append('description', this.Itemform.get('description')?.value);
      formData.append('price', this.Itemform.get('price')?.value);
       formData.append('quantity', this.Itemform.get('quantity')?.value);
      formData.append('categoryId', this.Itemform.get('categoryId')?.value);
      formData.append('file', this.selectedFile);  // ✅ must be 'file' to match backend

      this.apiitem.addtem(formData).subscribe({
        next: () => {
          console.log('Item added successfully!');
          console.log(formData);
          this.formChanges = false; // Reset after save
          this.Itemform.reset();
          this.selectedFile = null;
          
        },
        error: (err) => {
          console.error(err);
        }
      });
      alert("Item Add Succefully");
      this.router.navigate(['/menu']);
    } else {
      alert("Form is invalid or no image selected!");
    }
  }
}
