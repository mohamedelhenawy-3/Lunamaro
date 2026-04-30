import { Component } from '@angular/core';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-addweeklydeal',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './addweeklydeal.component.html',
  styleUrl: './addweeklydeal.component.css'
})
export class AddweeklydealComponent {

  searchTerm: string = '';
  products: any[] = [];
  selectedProduct: any = null;

  discount: number = 0;
  expiryDate: string = '';


  constructor(private offersservice:OffersservicesService,private router:Router){

  }

   onSearch() {
    if (!this.searchTerm) {
      this.products = [];
      return;
    }

    this.offersservice.searchProducts(this.searchTerm).subscribe(res => {
      this.products = res;
      console.log("search result:",res);
    });
  }

    selectProduct(p: any) {
    this.selectedProduct = p;
    this.searchTerm = p.name;
    this.products = [];
  }
 createDeal() {

    if (!this.selectedProduct) {
      alert('Select product first');
      return;
    }

    const dto = {
      productId: this.selectedProduct.id,
      discountPercentage: this.discount,
      expiryDate: this.expiryDate,
      isActive: true
    };

    this.offersservice.createWeeklyDeal(dto).subscribe({
      next: () => {
        alert('Created successfully ✅');

        this.router.navigate(['Admin/offers']);
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Error');
      }
    });
  }

  cancel() {
this.router.navigate(['Admin/offers']);}
}
