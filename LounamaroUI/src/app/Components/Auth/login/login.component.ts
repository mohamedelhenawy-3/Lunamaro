import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../Service/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  LoginrForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private auth: AuthService
  ) {
    this.LoginrForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  login() {
    if (this.LoginrForm.invalid) {
      this.LoginrForm.markAllAsTouched();
      return;
    }

    this.auth.login(this.LoginrForm.value).subscribe({
      next: () => {
        const role = this.auth.getUserRole();
        alert('Login successful');
        this.router.navigate(['/Home']);
      },
      error: () => {
        alert('Email or Password Invalid');
      }
    });
  }
}
