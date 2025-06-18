export interface OrderItem {
  orderItemId: number;
  itemId: number;
  itemName: string;
  imageUrl: string;
  description: string;
  unitPrice: number;
  quantity: number;
  total: number;
}
