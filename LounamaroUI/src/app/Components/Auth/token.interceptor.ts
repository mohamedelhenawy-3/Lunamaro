import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take, finalize } from 'rxjs';
import { AuthService } from '../../Service/auth.service';
import { LoadingService } from '../../Service/LoadingService/loading.service';

let isRefreshing = false;
const refreshDone$ = new BehaviorSubject<string | null>(null);

// These run silently in background — no spinner
const SILENT_URLS = ['/Auth/refresh', '/Auth/logout', '/cart/count'];

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const loading = inject(LoadingService); // ✅ inject loading service

  const isSilent = SILENT_URLS.some(u => req.url.includes(u));
  if (!isSilent) loading.show(); // ✅ show spinner for ALL requests including login

  const accessToken = authService.getAccessToken();
  const authReq = accessToken
    ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : req;

  return next(authReq).pipe(
    finalize(() => { if (!isSilent) loading.hide(); }), // ✅ ALWAYS hide — success or error
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
          refreshDone$.next(tokens.accessToken);
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