import { OrderStatus } from "../../orderStatus";
import { PaymentType } from "../PaymentType";

export interface orderhistory {
    orderId: number;
  customerName: string;
  phoneNumber: string;
  totalAmount: number;
  orderStatus: OrderStatus;
  orderDate: string;
  paymentType: PaymentType; // Cash / Online
}