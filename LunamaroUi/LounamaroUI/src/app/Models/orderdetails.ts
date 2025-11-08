import { OrderItemsHistory } from "./orderitemhistory";

export interface OrderHistoryDetails {
  orderId: number;
  dateOfOrder: string;
  orderStatus: string;
  totalAmount: number;
  orderItems: OrderItemsHistory[];
}
