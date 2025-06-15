export interface Item {
  id: number;
  name: string;
  description: string;
  price: number;
  quantity:number;
  categoryId: number;
  imageUrl: string;  // Add this too if you're using it
}
