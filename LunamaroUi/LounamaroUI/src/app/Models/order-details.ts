import { OrderItem } from "./order-item";
import { UserOrderHeader } from "./user-order-header";
import { Usercart } from "./usercart";

export interface OrderDetails {
 orderId: number;
  userCartList: Usercart[];
 userOrderHeader:UserOrderHeader
}
