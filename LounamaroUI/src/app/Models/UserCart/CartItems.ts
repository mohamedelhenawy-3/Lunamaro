import { AddOn } from "../item/AddOns";

export interface CartItem {
  itemId: number;
  userCartId: number;
  itemName: string;
  price: number;
  quantity: number;

  addOns: AddOn[];
  availableAddOns: AddOn[];

  totalPrice: number;
}