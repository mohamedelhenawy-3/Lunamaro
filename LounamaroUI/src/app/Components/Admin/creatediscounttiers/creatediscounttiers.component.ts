import { Component } from '@angular/core';
import { OffersservicesService } from '../../../Service/Offers/offersservices.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-creatediscounttiers',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './creatediscounttiers.component.html',
  styleUrl: './creatediscounttiers.component.css'
})
export class CreatediscounttiersComponent {
cancel() {
throw new Error('Method not implemented.');
}
minimumAmount: number = 0;
  discountAmount: number = 0;

  constructor(
    private offersService: OffersservicesService,
    private router: Router
  ) {}

  create() {

    if (this.minimumAmount <= 0 || this.discountAmount <= 0) {
      alert('Invalid values');
      return;
    }

    const dto = {
      minimumAmount: this.minimumAmount,
      discountAmount: this.discountAmount
    };

    this.offersService.createDiscountTier(dto).subscribe({
      next: () => {
        alert('Discount Tier Created ✅');
        this.router.navigate(['/admin/discount-tiers']);
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Error creating tier');
      }
    });
  }
}
