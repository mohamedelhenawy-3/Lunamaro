import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GoogleAuthServiceService {

  private isInitialized = false;

  initializeGoogleSignIn(callback: (resp: any) => void) {
    if (this.isInitialized) return;

    // @ts-ignore
    google.accounts.id.initialize({
      client_id: "663635706581-kp80hqsusm46eofpglum06dpmpcsnqqg.apps.googleusercontent.com",
      callback: callback,
      use_fedcm_for_prompt: true // Fixes the FedCM Migration Warning
    });

    this.isInitialized = true;
  }

  renderButton(elementId: string) {
    const element = document.getElementById(elementId);
    if (element) {
      // @ts-ignore
      google.accounts.id.renderButton(element, {
        theme: 'outline',
        size: 'large'
      });
    } else {
      console.error(`Element with id ${elementId} not found.`);
    }
  }
}
