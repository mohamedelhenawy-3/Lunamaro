import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink, RouterModule } from '@angular/router';
import { AuthService } from '../../../Service/auth.service';

declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule,RouterLink,
],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
loginWithFacebook() {
throw new Error('Method not implemented.');
}
  errorMessage: string = '';
  LoginrForm: FormGroup;
private static googleInitialized = false;
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private auth: AuthService,
      private route: ActivatedRoute,

  ) {
    this.LoginrForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

 ngOnInit() {
  // 1. Initialize once per application lifecycle
  if (!LoginComponent.googleInitialized) {
    google.accounts.id.initialize({
      client_id: '663635706581-kp80hqsusm46eofpglum06dpmpcsnqqg.apps.googleusercontent.com',
      callback: (response: any) => {
        this.handleGoogleLogin(response.credential);
      }
    });
    LoginComponent.googleInitialized = true;
  }

  setTimeout(() => {
    const btnElement = document.getElementById("google-btn");
    if (btnElement) {
      google.accounts.id.renderButton(btnElement, { 
        type: 'icon',     
        shape: 'circle',  
        theme: 'outline', 
        size: 'large'     
      });
    }
  }, 0);
}
private handleAfterLogin() {
  const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/Home';
  this.router.navigate([returnUrl]);
}
handleGoogleLogin(idToken: string) {
  this.auth.loginWithSocial('GOOGLE', idToken).subscribe({
    next: () => {
      this.handleAfterLogin();
    },
    error: (err: any) => {
      this.errorMessage = 'Google login failed. Please try again.';
    }
  });
}

login() {
  if (this.LoginrForm.invalid) {
    this.LoginrForm.markAllAsTouched();
    return;
  }

  this.errorMessage = '';

  this.auth.login(this.LoginrForm.value).subscribe({
    next: () => {
      this.handleAfterLogin();
    },
    error: (err: string) => {
      this.errorMessage = err;
    }
  });
}

}