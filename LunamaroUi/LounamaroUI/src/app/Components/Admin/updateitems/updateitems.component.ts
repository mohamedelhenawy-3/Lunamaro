import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ItemService } from '../../../Service/Item/item.service';

@Component({
  selector: 'app-update-item',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './updateitems.component.html',
  styleUrls: ['./updateitems.component.css']
})
export class UpdateItemComponent implements OnInit {

  itemId!: number;
  form!: FormGroup;
  selectedFile: File | null = null;
  previewImage: string | ArrayBuffer | null = null;

  constructor(
    private route: ActivatedRoute,
    private itemService: ItemService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.itemId = Number(this.route.snapshot.paramMap.get('id'));

    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: ['', Validators.required],
      categoryId: ['', Validators.required],
      imageUrl: [''],
      quantity: [''],
      IsSpecial: [false] // ✅ DEFAULT FALSE
    });

    this.loadItem();
  }

  loadItem() {
    this.itemService.getitembyid(this.itemId).subscribe(res => {
      this.form.patchValue({
        name: res.name,
        description: res.description,
        price: res.price,
        categoryId: res.categoryId,
        imageUrl: res.imageUrl,
        quantity: res.quantity,
  
      });

      this.previewImage = 'http://localhost:5218' + res.imageUrl;
    });
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0] ?? null;

    if (this.selectedFile) {
      const reader = new FileReader();
      reader.onload = () => (this.previewImage = reader.result);
      reader.readAsDataURL(this.selectedFile);
    }
  }

  submit() {
    if (this.form.invalid) return;

    const formData = new FormData();
    formData.append('Name', this.form.value.name);
    formData.append('Description', this.form.value.description);
    formData.append('Price', this.form.value.price);
    formData.append('CategoryId', this.form.value.categoryId);
    formData.append('Quantity', this.form.value.quantity);
    formData.append('IsSpecial', String(this.form.value.IsSpecial)); // ✅ SAFE

    if (this.selectedFile) {
      formData.append('File', this.selectedFile);
    }

    this.itemService.updateItem(this.itemId, formData).subscribe({
      next: () => {
        alert('Item updated successfully!');
        this.router.navigateByUrl('/menu');
      },
      error: () => alert('Update failed!')
    });
  }
}
