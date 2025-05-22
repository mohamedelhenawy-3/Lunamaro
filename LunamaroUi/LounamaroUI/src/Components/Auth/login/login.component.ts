import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../app/Service/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
LoginrForm: FormGroup;

  constructor(private fb: FormBuilder,private router:Router,private auth: AuthService) {

  console.log("LoginComponent loaded");

this.LoginrForm = this.fb.group({
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(6)]]
});


  
}

login(){
    if (this.LoginrForm.invalid) {
      this.LoginrForm.markAllAsTouched(); // show validation errors
      return;
    }
    const formval=this.LoginrForm.value;;

    const logindata={
      Email:formval.email,
      Password:formval.password
    }
  
    this.auth.login(this.LoginrForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.auth.setToken(response.token);
          console.log(response.token);
          alert('Login successful');
          this.router.navigate(['/home']); // Redirect to home or dashboard
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
