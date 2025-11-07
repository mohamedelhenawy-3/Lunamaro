import { Usercart } from "./usercart";
import { UserOrderHeader } from "./user-order-header";

export interface OrderDto {
  userCartList: Usercart[];
  userOrderHeader: UserOrderHeader;
  paymentUrl?: string;   // optional
}