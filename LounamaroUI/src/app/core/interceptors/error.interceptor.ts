import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Service/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      let errorMessage = 'Something went wrong';

if (error.status === 401) {

  const isAuthRequest =
    req.url.includes('/Auth/login') ||
    req.url.includes('/Auth/register');

  if (isAuthRequest) {
    return throwError(() => error);
  }

  const token = authService.getAccessToken();

  if (token) {
    authService.logout();
    router.navigate(['/login']);
  }

  // guest user → DO NOTHING
  return throwError(() => error);
}

      // ✅ HANDLE 400 (validation)
      if (error.status === 400) {
        errorMessage = error.error?.message || 'Bad request';
      }

      // ✅ HANDLE OTHER ERRORS
      else if (error.error?.message) {
        errorMessage = error.error.message;
      }

      return throwError(() => errorMessage);
    })
  );
};