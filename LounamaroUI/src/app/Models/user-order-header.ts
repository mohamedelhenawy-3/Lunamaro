import { PaymentType } from "./Admin/PaymentType";
import { OrderItem } from "./order-item";
import { OrderStatus } from "./orderStatus";

export interface UserOrderHeader {
  id: number;
  userId?: string | null;
  dateOfOrder: string;                 // DateTime → string in JSON
  dateOfShipping?: string | null;

  // 💰 Financial breakdown
  originalTotalAmount: number;         // before any discount
  offerDiscountAmount: number;         // product offers
  tierDiscountAmount: number;          // tier/global discount
  totalDiscountAmount: number;         // offer + tier
  finalTotalAmount: number;            // final price to pay
  freeProductId?: number | null;       // optional free product

  // 💳 Payment info
  stripeSessionId?: string | null;
  stripePaymentIntentId?: string | null;
  paymentStatus?: string | null;
  paymentProcessDate?: string | null;
  transactionId?: string | null;
  paymentType: PaymentType;

  // 📦 Shipping info
  phoneNumber: string;
  deliveryStreetAddress: string;
  city: string;
  state: string;
  postalCode: number;
  name: string;
  trackingNumber?: string | null;
  carrier?: string | null;

  // 🔹 Status & items
  orderStatus: OrderStatus;            // enum in TS
  orderItems?: OrderItem[];            // list of items
}