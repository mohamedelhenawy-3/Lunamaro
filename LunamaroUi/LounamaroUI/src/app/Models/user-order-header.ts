import { OrderItem } from "./order-item";
import { OrderStatus } from "./orderStatus";

export interface UserOrderHeader {
  id: number;
  userId?: string | null;
  dateOfOrder: string;                 // DateTime → string in JSON
  dateOfShipping?: string | null;
  totalAmount: number;
  stripeSessionId?: string | null;
  stripePaymentIntentId?: string | null;
  paymentStatus?: string | null;
  paymentProcessDate?: string | null;
  transactionId?: string | null;
  phoneNumber: string;
  deliveryStreetAddress: string;
  city: string;
  state: string;
  postalCode: number;
  name: string;
  trackingNumber?: string | null;
  carrier?: string | null;
  orderStatus: OrderStatus;           // enum in TS
  orderItems?: OrderItem[];           // list of items
}
