import { OrderItem } from "./order-item";

export interface OrderDetails {
orderId: number;
  fullName: string;
  email: string;
  shippingAddress: string;
  orderDate: string;
  totalAmount: number;
  items: OrderItem[];
}
