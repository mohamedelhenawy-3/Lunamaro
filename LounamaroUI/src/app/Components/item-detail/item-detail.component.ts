import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ItemService } from 'src/app/Service/Item/item.service';
import { UsercartService } from 'src/app/Service/UserCart/usercart.service';
import { AuthService } from 'src/app/Service/auth.service';
import { OfflineService } from 'src/app/Service/OfflineSerivce/offline-service.service';
import { AddToCart } from '../../Models/add-to-cart';

@Component({
  selector: 'app-item-detail',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.css']
})
export class ItemDetailComponent implements OnInit {

  private route       = inject(ActivatedRoute);
  private router      = inject(Router);
  private itemService = inject(ItemService);
  private cartService = inject(UsercartService);
  private authService = inject(AuthService);
  private offline     = inject(OfflineService);

  item:    any | null = null;
  loading  = true;
  error    = false;

  quantity     = 1;
  addingToCart = false;
  addedSuccess = false;
  showToast    = false;

  private toastTimer: any;

  // ── Lifecycle ─────────────────────────────────────────────
  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.itemService.getitembyid(id).subscribe({
      next:  (data) => { this.item = data;
        console.log("item data",this.item);
         this.loading = false; },
      error: ()     => { this.error = true;  this.loading = false; }
    });
  }

  // ── Computed ──────────────────────────────────────────────
  get lineTotal(): number {
    return (this.item?.price ?? 0) * this.quantity;
  }

  // ── Quantity ──────────────────────────────────────────────
  changeQty(delta: number): void {
    const next = this.quantity + delta;
    if (next >= 1 && next <= (this.item?.quantity ?? 1)) {
      this.quantity = next;
    }
  }

  // ── Add to Cart ───────────────────────────────────────────
  addToCart(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    if (!this.offline.isOnline) return;

    if (!this.item || this.addingToCart || this.item.quantity === 0) return;

    this.addingToCart = true;

    const dto: AddToCart = {
      itemId:   this.item.id,
      quantity: this.quantity
    };

    this.cartService.addToCart(dto).subscribe({
      next: () => {
        // fetchCartCount() is already called inside the service via tap()
        this.addingToCart = false;
        this.addedSuccess = true;
        this.triggerToast();
        setTimeout(() => (this.addedSuccess = false), 2500);
      },
      error: (err: any) => {
        console.error('Add to cart failed:', err.status, err.error);
        this.addingToCart = false;
      }
    });
  }

  // ── Toast ─────────────────────────────────────────────────
  private triggerToast(): void {
    clearTimeout(this.toastTimer);
    this.showToast  = true;
    this.toastTimer = setTimeout(() => (this.showToast = false), 2800);
  }

  // ── Navigation ────────────────────────────────────────────
  goBack(): void {
    this.router.navigate(['/menu']);
  }
}