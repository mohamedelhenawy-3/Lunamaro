import { Component, OnInit } from '@angular/core';
import { AvTable } from '../../Models/usersTables';
import { TableService } from '../../Service/Table/table.service';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css'
})
export class TableComponent implements OnInit{
 tables:AvTable[]=[];



 constructor(private Tablesservice:TableService){

 }
  ngOnInit(): void {
     this.Tablesservice.getAllAvailableTables().subscribe({
      next:(data)=> {
        this.tables = data;
        console.log(data)
      }
     })
  }


}
