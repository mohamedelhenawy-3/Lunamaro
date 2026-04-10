export interface CreateWeeklyDealDTO {
  productId: number;
  discountPercentage: number;
  expiryDate: string;
  isActive: boolean;
}