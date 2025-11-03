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
  styleUrls: ['./login.component.css']  // fix typo from styleUrl to styleUrls
})
export class LoginComponent {
  LoginrForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router, private auth: AuthService) {
    console.log("LoginComponent loaded");

    this.LoginrForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  login() {
    if (this.LoginrForm.invalid) {
      this.LoginrForm.markAllAsTouched(); // show validation errors
      return;
    }

    const formval = this.LoginrForm.value;

    const logindata = {
      email: formval.email,    // be consistent with casing expected by backend
      password: formval.password
    };

    this.auth.login(logindata).subscribe({
      next: (response) => {
        if (response.success) {
          this.auth.setToken(response.token);

          // Correctly call the function to get role
          const role = this.auth.getUserRole();


          alert('Login successful');
          this.router.navigate(['/Home']); // Redirect after login
        } else {
          alert('Login failed');
        }
      },
      error: () => {
        alert('Login error');
      }
    });
  }
}
