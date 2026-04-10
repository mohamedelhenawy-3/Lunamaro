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
    this.loaditem();

  }

loaditem() {
  this.offersService.getWeeklyDealById(this.id).subscribe(res => {

    const deal = res.data || res;

    console.log("DEAL DATA:", deal); 

    if (!deal) return;

    this.discount = deal.discountPercentage;
    this.expiryDate = deal.expiryDate?.split('T')[0];
    this.isActive = deal.isActive;

    if (deal.product) {
      this.selectedProduct = deal.product;
      this.searchTerm = deal.product.name;
    } else {
      console.warn("Product is NULL ❌");
      this.searchTerm = '';
    }

  });
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
    console.log('selected pr',p)
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
        this.router.navigate(['Admin/offers']);
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Update failed');
      }
    });
  }
}
