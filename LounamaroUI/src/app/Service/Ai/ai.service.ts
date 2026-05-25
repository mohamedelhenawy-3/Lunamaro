import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.prod';

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface PendingReservation {
  tableId?: number;
  tableNumber?: string;
  startTime?: string;
  endTime?: string;
  guests?: number;
  notes?: string;
  awaitingConfirmation: boolean;
}

export interface AiChatRequest {
  message: string;
  history: ChatMessage[];
  pendingReservation?: PendingReservation | null;
}

export interface AiChatResponse {
  message: string;
  pendingReservation?: PendingReservation | null;
  reservationCompleted: boolean;
  requiresLogin: boolean;
}

@Injectable({ providedIn: 'root' })
export class AiService {
  constructor(private http: HttpClient) {}

  chat(request: AiChatRequest): Observable<AiChatResponse> {
    return this.http.post<AiChatResponse>(
      `${environment.baseurl}/Ai/chat`,
      request
    );
  }
}