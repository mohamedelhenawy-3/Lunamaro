import { OrderItem } from "./order-item";
import { PricingResult } from "./PricingRes/PricingResult";
import { UserOrderHeader } from "./user-order-header";
import { Usercart } from "./usercart";

export interface OrderDetails {
 orderId: number;
  userCartList: Usercart[];
 userOrderHeader:UserOrderHeader;
}
