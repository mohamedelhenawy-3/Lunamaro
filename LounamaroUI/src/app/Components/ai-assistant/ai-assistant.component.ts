import { Component, ElementRef, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AiService, ChatMessage, PendingReservation } from '../../Service/Ai/ai.service';
import { AuthService } from 'src/app/Service/auth.service';

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-assistant.component.html',
  styleUrls: ['./ai-assistant.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush  // ✅ fixes INP
})
export class AiAssistantComponent {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  isOpen = false;
  isLoading = false;
  userMessage = '';
  messages: ChatMessage[] = [];
  pendingReservation: PendingReservation | null = null;
  showLoginPrompt = false;

  constructor(
    private aiService: AiService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef  // ✅ needed with OnPush
  ) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
    if (this.isOpen && this.messages.length === 0) {
      this.addWelcomeMessage();
    }
    this.cdr.markForCheck();
  }

  private addWelcomeMessage() {
    const isLoggedIn = this.authService.isLoggedIn();
    this.messages.push({
      role: 'assistant',
      content: isLoggedIn
        ? 'Hello! I am Lunamaro AI Assistant 🍽️\n\nI can help you with:\n• Menu recommendations\n• Table reservations\n• Current offers & deals\n• Restaurant information\n\nHow can I help you today?'
        : 'Hello! I am Lunamaro AI Assistant 🍽️\n\nI can help you with:\n• Menu recommendations\n• Current offers & deals\n• Restaurant information\n\nNote: Login required for table reservations.\n\nHow can I help you today?'
    });
  }

  // ✅ Fixed input handler - no ngModel blocking
  onInputChange(event: Event) {
    this.userMessage = (event.target as HTMLInputElement).value;
  }

  sendMessage(overrideMessage?: string) {
    const msg = (overrideMessage || this.userMessage).trim();
    if (!msg || this.isLoading) return;

    this.messages = [...this.messages, { role: 'user', content: msg }];
    this.userMessage = '';
    this.showLoginPrompt = false;
    this.isLoading = true;
    this.cdr.markForCheck();
    this.scrollToBottom();

    this.aiService.chat({
      message: msg,
      history: this.messages.slice(0, -1),
      pendingReservation: this.pendingReservation
    }).subscribe({
      next: (res) => {
        this.isLoading = false;

        if (res.requiresLogin) {
          this.messages = [...this.messages, { role: 'assistant', content: res.message }];
          this.showLoginPrompt = true;
          this.cdr.markForCheck();
          this.scrollToBottom();
          return;
        }

        this.messages = [...this.messages, { role: 'assistant', content: res.message }];
        this.pendingReservation = res.pendingReservation ?? null;

        if (res.reservationCompleted) {
          this.pendingReservation = null;
          this.showLoginPrompt = false;
        }

        this.cdr.markForCheck();
        this.scrollToBottom();
      },
      error: () => {
        this.isLoading = false;
        this.messages = [...this.messages, {
          role: 'assistant',
          content: 'Sorry, I am having trouble right now. Please try again.'
        }];
        this.cdr.markForCheck();
        this.scrollToBottom();
      }
    });
  }

  confirmReservation() {
    this.sendMessage('yes');
  }

  cancelReservation() {
    this.sendMessage('no');
    this.pendingReservation = null;
    this.cdr.markForCheck();
  }

  goToLogin() {
    this.showLoginPrompt = false;
    this.isOpen = false;
    this.router.navigate(['/login']);
  }

  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  clearChat() {
    this.messages = [];
    this.pendingReservation = null;
    this.showLoginPrompt = false;
    this.isOpen = false;
    this.cdr.markForCheck();
  }

  private scrollToBottom() {
    setTimeout(() => {
      if (this.messagesContainer) {
        this.messagesContainer.nativeElement.scrollTop =
          this.messagesContainer.nativeElement.scrollHeight;
      }
    }, 100);
  }
}