
export interface PricingResult {
  subtotal: number;
  tierDiscount: number;
  finalTotal: number;
  freeProductId?: number | null;
}