import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TableService } from '../../../Service/Table/table.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-newtable',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './newtable.component.html',
  styleUrl: './newtable.component.css'
})
export class NewtableComponent  implements OnInit{
  Tableform!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router:Router,
    private tableservice: TableService,
  ) {
    this.Tableform = this.fb.group({
      tableNumber: ['', Validators.required],
      capacity: ['', Validators.required],
      location:['',Validators.required],
      isAvailable: ['', Validators.required],
    });
  }
  ngOnInit(): void {
    // Initialization logic can be added here if needed
    // For example, loading existing tables or setting default values
  }

  onSubmit(): void {
    if (this.Tableform.valid) {
      this.tableservice.AddTable(this.Tableform.value).subscribe(
        (response) => {
          console.log('Table created successfully:', response);
          this.Tableform.reset();
        },
        (error) => {
          console.error('Error creating table:', error);
        }
      );
    }
   this.router.navigateByUrl("/Home");
  }

  resetForm(): void {
    this.Tableform.reset();
  }

}
