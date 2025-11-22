import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TableService } from '../../../Service/Table/table.service';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UpdateTable } from '../../../Models/Admin/Table/updatetable';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-updated-table',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './updated-table.component.html',
  styleUrl: './updated-table.component.css'
})
export class UpdatedTableComponent  implements OnInit{
    table: any = {};
tableId!: number;

  form!:FormGroup


constructor(private route: ActivatedRoute , private servvices:TableService,  private router:Router,  private fb: FormBuilder,

 
){
  
}
  ngOnInit(): void {
    this.tableId= Number(this.route.snapshot.paramMap.get("id"))
     this.loadtable();
  }
  loadtable(){
     this.servvices.getitembyid(this.tableId).subscribe(res=>{
        this.table= res;
       this.form = this.fb.group({
      tableNumber: [this.table.tableNumber, Validators.required],
      capacity: [this.table.capacity, Validators.required],
      location: [this.table.location, Validators.required]
       })



      })
  }
  submit(){
      if (this.form.invalid) return;
      const dto:UpdateTable={
    tableNumber: this.form.value.tableNumber,
    capacity: this.form.value.capacity,
    location: this.form.value.location
      }
        this.servvices.UpdatetTable(this.tableId, dto).subscribe(() => {
    alert('Updated successfully!');
  });

    this.router.navigateByUrl("/Home");
  }
}
