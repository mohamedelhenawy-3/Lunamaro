// token.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take } from 'rxjs';
import { AuthService } from '../../Service/auth.service';

// Shared refresh state — lives outside the function so all requests share it
let isRefreshing = false;
const refreshDone$ = new BehaviorSubject<string | null>(null);

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (req.url.includes('/Auth/refresh') || req.url.includes('/Auth/login')) {
    return next(req);
  }

  const accessToken = authService.getAccessToken();
  const authReq = accessToken
    ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) return throwError(() => error);

      if (isRefreshing) {
        return refreshDone$.pipe(
          filter(token => token !== null),
          take(1),
          switchMap(token =>
            next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }))
          )
        );
      }

      isRefreshing = true;
      refreshDone$.next(null);

      return authService.refreshToken().pipe(
        switchMap(tokens => {
          isRefreshing = false;
          authService.setToken(tokens.accessToken, tokens.refreshToken);
          refreshDone$.next(tokens.accessToken); // Unblock all waiting requests
          return next(req.clone({ setHeaders: { Authorization: `Bearer ${tokens.accessToken}` } }));
        }),
        catchError(err => {
          isRefreshing = false;
          authService.logout();
          return throwError(() => err);
        })
      );
    })
  );
};