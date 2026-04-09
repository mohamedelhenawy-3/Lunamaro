export interface Reservation {
  tableId: number;
  startTime: string;
  endTime: string;
  guests: number;
  notes?: string;
}
