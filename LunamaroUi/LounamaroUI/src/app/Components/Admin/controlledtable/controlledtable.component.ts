import { Component, OnInit } from '@angular/core';
import { Table } from '../../../Models/tables';
import { TableService } from '../../../Service/Table/table.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { updatetablestatus } from '../../../Models/updatetablestatus';

@Component({
  selector: 'app-controlledtable',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './controlledtable.component.html',
  styleUrl: './controlledtable.component.css'
})
export class ControlledtableComponent implements OnInit{
  Table:Table[] =[];

  constructor( private tableservice:TableService){

  }
  ngOnInit(): void {
    this.loadTables();
  }

  private loadTables(): void {
    // Assumes TableService exposes a getTables(): Observable<Table[]>
    this.tableservice.getAllTables().subscribe({
      next: (tables: Table[]) => (
         console.log(tables),  
        this.Table = tables ?? []
      ),
      error: (err: any) => {
        console.error('ControlledtableComponent: failed to load tables', err);
        this.Table = [];
      }
    });
  }
  onStatusChange(r:any){
    const dto :updatetablestatus ={IsAvailable :r.isAvailable};
    this.tableservice.UpdatetTableStatus(r.id,dto).subscribe({
          next: () => {
            this.loadTables();
    },
    error: (err) => console.error('Error updating:', err)
    })
  }
  // helper to refresh data from the service (can be called from the template)
  refresh(): void {
    this.loadTables();
  }

  // optional trackBy for *ngFor performance
  trackByIndex(index: number, _item: Table): number {
    return index;
  }
}
