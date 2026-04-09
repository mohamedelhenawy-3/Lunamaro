export interface Usercart {
  itemId: number;       // add ItemId for identification
  userCartId: number;
  itemName: string;
  price: number;
  imageUrl?: string;    // optional because backend can send null
  description: string;
  quantity: number;
  totalPrice: number;
}
