import { Component, OnInit } from '@angular/core';
import { Table } from '../../../Models/tables';
import { TableService } from '../../../Service/Table/table.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { updatetablestatus } from '../../../Models/updatetablestatus';
import { Router } from '@angular/router';

@Component({
  selector: 'app-controlledtable',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './controlledtable.component.html',
  styleUrls: ['./controlledtable.component.css']
})
export class ControlledtableComponent implements OnInit {
  allTables: Table[] = [];
  Table: Table[] = [];

  searchTerm: string = '';
  statusFilter: string = '';

  private timeout: any;

  constructor(private tableservice: TableService, private router: Router) {}

  ngOnInit(): void {
    this.loadTables();
  }

  private loadTables(): void {
    this.tableservice.getAllTables().subscribe({
      next: (tables: Table[]) => {
        this.allTables = tables ?? [];
        this.Table = tables ?? [];
      },
      error: (err: any) => {
        console.error('ControlledtableComponent: failed to load tables', err);
        this.Table = [];
      }
    });
  }

  applyFilter() {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {
      let filtered = [...this.allTables];

      if (this.searchTerm) {
        const term = this.searchTerm.toLowerCase();
        filtered = filtered.filter(t =>
          t.tableNumber.toString().includes(term) ||
          t.location.toLowerCase().includes(term) ||
          t.isAvailable.toLowerCase().includes(term)
        );
      }

      if (this.statusFilter) {
        filtered = filtered.filter(t => t.isAvailable === this.statusFilter);
      }

      this.Table = filtered;
    }, 300);
  }

  onStatusChange(r: any) {
    const dto: updatetablestatus = { IsAvailable: r.isAvailable };
    this.tableservice.UpdatetTableStatus(r.id, dto).subscribe({
      next: () => this.loadTables(),
      error: (err) => console.error('Error updating:', err)
    });
  }

  refresh(): void {
    this.loadTables();
  }

  trackByIndex(index: number, _item: Table): number {
    return index;
  }

  UpdateTable(id: number) {
    this.router.navigate(['Admin/details', id]);
  }

  AddTable() {
    this.router.navigateByUrl('Admin/AddNewTable');
  }
}