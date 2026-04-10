import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-weaklydealedit',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './weaklydealedit.component.html',
  styleUrl: './weaklydealedit.component.css'
})
export class WeaklydealeditComponent {
cancel() {
throw new Error('Method not implemented.');
}
 id!: number;

  searchTerm: string = '';
  products: any[] = [];
  selectedProduct: any = null;

  discount: number = 0;
  expiryDate: string = '';
  isActive: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private offersService: OffersservicesService,
    private router: Router
  ) {}
   ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

  }
  onSearch() {
    if (!this.searchTerm) {
      this.products = [];
      return;
    }

    this.offersService.searchProducts(this.searchTerm).subscribe(res => {
      this.products = res;
    });
  }

  selectProduct(p: any) {
    this.selectedProduct = p;
    this.searchTerm = p.name;
    this.products = [];
  }

  updateDeal() {

    if (!this.selectedProduct) {
      alert('Select product');
      return;
    }

    const dto = {
      productId: this.selectedProduct.id,
      discountPercentage: this.discount,
      expiryDate: this.expiryDate,
      isActive: this.isActive
    };

    this.offersService.updateWeeklyDeal(this.id, dto).subscribe({
      next: () => {
        alert('Updated successfully ✅');
        this.router.navigate(['/admin/weekly-deals']);
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Update failed');
      }
    });
  }
}
