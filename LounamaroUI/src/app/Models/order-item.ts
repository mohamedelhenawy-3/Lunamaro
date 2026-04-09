import { Item } from "./item";
import { UserOrderHeader } from "./user-order-header";

export interface OrderItem {
  orderItemId: number;
  userOrderHeaderId: number;
  userOrderHeader?: UserOrderHeader;
  itemId: number;
  item?: Item;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}
