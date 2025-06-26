export interface UserOrderHeader {
  id: number;
  userId: string;
  user: any; // Use specific User interface if available
  dateOfOrder: string;
  dateOfShipping: string | null;
  totalAmount: number;
  stripeSessionId: string | null;
  stripePaymentIntentId: string | null;
  paymentStatus: string | null;
  paymentProcessDate: string | null;
  transactionId: string | null;
  phoneNumber: string;
  deliveryStreetAddress: string;
  city: string;
  state: string | null;
  postalCode: number;
  name: string;
  trackingNumber: string | null;
  carrier: string | null;
  orderStatus: number;
  orderItems:[]|null; // Use OrderItem[] if you have it
}
