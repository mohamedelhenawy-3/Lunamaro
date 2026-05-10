import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { CommonModule, NgIf } from '@angular/common';
import { AuthService } from '../../../Service/auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule,NgIf,RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService,public router:Router) {
this.registerForm = this.fb.group({
fullName: ['', [
  Validators.required,
  Validators.pattern(/^\s*\S+\s+\S+\s+\S+.*$/)
]],
  email: ['', [
    Validators.required,
    Validators.email
  ]],
  password: ['', [
    Validators.required,
    Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{6,}$/)
  ]],
  confirmPassword: ['', Validators.required]
}, { validators: this.passwordsMatchValidator });

  }

getError(controlName: string): string {
  const control = this.registerForm.get(controlName);

  if (!control || !control.touched) return '';
if (control.hasError('pattern') && controlName === 'fullName') {
  return 'Full name must contain at least 3 words';
}
  if (control.hasError('required')) return 'This field is required';

  if (control.hasError('email')) return 'Enter a valid email';

  if (control.hasError('pattern')) {
  return 'Password must include uppercase, lowercase, number, and special character';
}
  if (control.hasError('minlength')) {
    const length = control.getError('minlength').requiredLength;
    return `Minimum ${length} characters required`;
  }

  return '';
}

getPasswordMatchError(): string {
  if (
    this.registerForm.errors?.['passwordMismatch'] &&
    this.registerForm.get('confirmPassword')?.touched
  ) {
    return 'Passwords do not match';
  }
  return '';
}
passwordsMatchValidator(group: AbstractControl): { [key: string]: boolean } | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { mismatch: true };
}

showPassword = false;
showConfirmPassword = false;

togglePassword() {
  this.showPassword = !this.showPassword;
}

toggleConfirmPassword() {
  this.showConfirmPassword = !this.showConfirmPassword;
}
register() {
  if (this.registerForm.valid) {
    const formValue = this.registerForm.value;

    // Map frontend "confirmPassword" to backend "ConfirmPassword"
    const requestData = {
      FullName: formValue.fullName,
    Email: formValue.email,
      Password: formValue.password,
      ConfirmPassword: formValue.confirmPassword // <-- match backend typo
    };

    console.log("Payload being sent to backend:", requestData);

    this.auth.register(requestData).subscribe({
      next: (response) => {
        alert('Registered Successfully');
        console.log('Registration successful', response);
        this.router.navigate(['/Login']);
      },
      error: (err) => {
        console.error("Registration error:", err);
        alert('Enter Valid Data');
      }
    });
  }
}


}
