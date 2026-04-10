import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-addonrewardedit',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './addonrewardedit.component.html',
  styleUrl: './addonrewardedit.component.css'
})
export class AddonrewardeditComponent {
  cancel() {
  throw new Error('Method not implemented.');
  }
   id!: number;
  
    searchTerm: string = '';
    products: any[] = [];
    selectedProduct: any = null;
  
    minimumAmount: number = 0;
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
    this.offersService.getAddOnRewardById(this.id).subscribe(res => {
  
      const deal = res.data || res;
  
      console.log("DEAL DATA:", deal); 
  
      if (!deal) return;
  
      this.minimumAmount = deal.minimumAmount;
      this.isActive = deal.isActive;
  
      if (deal.freeProduct) {
        this.selectedProduct = deal.freeProduct;
        this.searchTerm = deal.freeProduct.name;
      } else {
        console.warn("Free product is NULL ❌");
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
    freeProductId: this.selectedProduct.id, 
    minimumAmount: this.minimumAmount,
    isActive: this.isActive
  };

  this.offersService.UpdateAddOnRewards(this.id, dto).subscribe({
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
