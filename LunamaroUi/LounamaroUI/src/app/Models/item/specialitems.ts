export interface specialItem {
  id: number;
  name: string;
  description: string;
  price: number;
  quantity:number;
  categoryId: number;
  imageUrl: string;  // Add this too if you're using it
  isSpecial?: boolean; // optional to avoid breaking old code

}
