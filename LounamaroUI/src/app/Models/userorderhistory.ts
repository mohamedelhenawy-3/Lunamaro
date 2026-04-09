import { OrderStatus } from "./orderStatus";

export interface userorderhostory {
    orderId:number ;
    dateOfOrder:string;
    orderStatus:OrderStatus;
    totalAmount:number;
    
}
