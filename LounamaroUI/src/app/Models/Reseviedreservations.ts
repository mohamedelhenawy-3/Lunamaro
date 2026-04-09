export interface RecievedReservation {
  id: number;
  tableId: number;
  tableName: string;
  userEmail: string;
  startTime: Date;
  endTime: Date;
  guests: number;
  notes: string;
  status: string;
  createdAt: Date;
}
