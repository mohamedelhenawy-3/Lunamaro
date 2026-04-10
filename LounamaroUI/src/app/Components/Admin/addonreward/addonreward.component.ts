import { Component } from '@angular/core';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-addonreward',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './addonreward.component.html',
  styleUrl: './addonreward.component.css'
})
export class AddonrewardComponent {
cancel() {
throw new Error('Method not implemented.');
}
searchTerm: string = '';
products: any[] = [];
selectedProduct: any = null;
minimumAmount:number=0;

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

  addonreword(){
       if (!this.selectedProduct) {
      alert('Select product first');
      return;
    }

    const dto = {
      freeProductId: this.selectedProduct.id,
      minimumAmount: this.minimumAmount,
      isActive: true
    };

    this.offersservice.createAddOnReward(dto).subscribe({
      next:()=>{
        alert('add on Reward Created');
        this.router.navigate(['Admin/offers']);
      }
    })
  }
}
