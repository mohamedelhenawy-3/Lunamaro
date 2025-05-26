import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../app/Service/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

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
          console.log('User Role:', role);
          console.log(response.token);

          alert('Login successful');
          this.router.navigate(['/home']); // Redirect after login
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
