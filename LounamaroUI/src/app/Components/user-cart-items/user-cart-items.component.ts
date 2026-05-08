import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UsercartService } from '../../Service/UserCart/usercart.service';
import { AddOn } from '../../Models/item/AddOns';
import { RecommendationService } from 'src/app/Service/ReommendedItems/recommendation.service';
import { CartItem } from 'src/app/Models/UserCart/CartItems';

@Component({
  selector: 'app-user-cart-items',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './user-cart-items.component.html',
  styleUrls: ['./user-cart-items.component.css']
})
export class UserCartItemsComponent implements OnInit {

  cartItems: CartItem[] = [];
  suggestions: any[] = [];

  expandedItemId: number | null = null;

  selectedAddOns: Map<number, Set<number>> = new Map();

  isLoading = true;
  isAddingToCart = false;

  constructor(
    private usercart: UsercartService,
    private router: Router,
    private recommendationService: RecommendationService
  ) {}

  ngOnInit(): void {
    this.reloadCart();
  }

  reloadCart() {
    this.isLoading = true;
    this.usercart.getCart().subscribe({
      next: (data) => {
        this.cartItems = data;
        this.isLoading = false;
        this.loadSuggestions();
      },
      error: (err) => {
        console.error('Failed to load cart', err);
        this.isLoading = false;
      }
    });
  }

  loadSuggestions() {
    this.recommendationService.getSuggestions().subscribe({
      next: (res) => this.suggestions = res,
      error: (err) => console.error('Suggestions error', err)
    });
  }

  // ── Addon panel toggle ─────────────────────────
  toggleAddonPanel(item: CartItem) {
    this.expandedItemId = this.expandedItemId === item.userCartId
      ? null
      : item.userCartId;

    // Init selection set if not exists
    if (!this.selectedAddOns.has(item.userCartId)) {
      // Pre-select already chosen addons
      const current = new Set(item.addOns.map(a => a.id));
      this.selectedAddOns.set(item.userCartId, current);
    }
  }

  isExpanded(item: CartItem): boolean {
    return this.expandedItemId === item.userCartId;
  }

  // ── Addon checkbox ─────────────────────────────
  toggleAddOn(cartItemId: number, addOn: AddOn, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    const set = this.selectedAddOns.get(cartItemId) ?? new Set<number>();

    if (checked) {
      set.add(addOn.id);
    } else {
      set.delete(addOn.id);
    }

    this.selectedAddOns.set(cartItemId, set);
  }

  isAddOnSelected(cartItemId: number, addOnId: number): boolean {
    return this.selectedAddOns.get(cartItemId)?.has(addOnId) ?? false;
  }

  // ── Update cart item with selected addons ──────
updateCartWithAddOns(item: CartItem) {
  const addOnIds = Array.from(
    this.selectedAddOns.get(item.userCartId) ?? []
  );

  this.usercart.updateCartAddOns(item.userCartId, addOnIds).subscribe({
    next: () => {
      this.expandedItemId = null;
      this.reloadCart();
    },
    error: (err) => console.error('Update addon error', err)
  });
}

  getAddonTotal(cartItemId: number, availableAddOns: AddOn[]): number {
    const selected = this.selectedAddOns.get(cartItemId) ?? new Set();
    return availableAddOns
      .filter(a => selected.has(a.id))
      .reduce((sum, a) => sum + a.price, 0);
  }

  // ── Quantity ───────────────────────────────────
  increaseQuantity(item: CartItem) {
    if (item.quantity < 10) {
      item.quantity++;
      this.updateQuantity(item);
    }
  }

  decreaseQuantity(item: CartItem) {
    if (item.quantity > 1) {
      item.quantity--;
      this.updateQuantity(item);
    }
  }

  updateQuantity(item: CartItem) {
    this.usercart.updatequantity({
      cartItemId: item.userCartId,
      newQuantity: item.quantity
    }).subscribe({
      error: (err) => console.error('Update qty error', err)
    });
  }

  remove(userCartId: number) {
    this.usercart.deleteCart(userCartId).subscribe({
      next: () => this.reloadCart(),
      error: (err) => console.error('Remove error', err)
    });
  }

  addSuggestionToCart(item: any) {
    this.isAddingToCart = true;
    this.usercart.addToCart2({
      itemId: item.id,
      quantity: 1,
      addOnIds: []
    }).subscribe({
      next: () => {
        this.isAddingToCart = false;
        this.reloadCart();
        this.usercart.fetchCartCount();
      },
      error: (err) => {
        console.error('Add suggestion error', err);
        this.isAddingToCart = false;
      }
    });
  }

  // ── Checkout ───────────────────────────────────
  goToOrderPreview() {
    this.router.navigate(['/orderpervew']);
  }

  // ── Total ──────────────────────────────────────
  get cartTotal(): number {
  return this.cartItems.reduce((sum, item) => sum + item.totalPrice, 0);
}
  trackById(index: number, item: any) {
    return item.userCartId ?? item.id;
  }
}