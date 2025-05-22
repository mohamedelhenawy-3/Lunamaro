import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../app/Service/auth.service';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule,NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService) {
this.registerForm = this.fb.group({
  fullName: ['', Validators.required], // ✅ fix spelling from fulName
  userName: ['', [Validators.required, Validators.minLength(4)]],
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(6)]],
  confirmPassword: ['', Validators.required] // ✅ match model
}, { validators: this.passwordsMatchValidator });


  }
passwordsMatchValidator(group: AbstractControl): { [key: string]: boolean } | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { mismatch: true };
}

register() {
  if (this.registerForm.valid) {
    const formValue = this.registerForm.value;

    // map camelCase keys from form to PascalCase keys expected by backend
    const requestData = {
      FullName: formValue.fullName,
      UserName: formValue.userName,
      Email: formValue.email,
      Password: formValue.password,
      ConfirmPassword: formValue.confirmPassword
    };

    console.log("Payload being sent to backend:", requestData);

    this.auth.register(requestData).subscribe({
      next: (response) => {
        alert('Registered Successfully');
        console.log('Registration successful', response);
      },
      error: (err) => {
        console.error("Registration error:", err);
        alert('Error during registration');
      }
    });
  }
}



}
