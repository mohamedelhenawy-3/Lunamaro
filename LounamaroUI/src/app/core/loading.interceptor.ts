import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../Service/LoadingService/loading.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
const SILENT_URLS = ['/Auth/refresh', '/Auth/login'];

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
const loader = inject(LoadingService);

  const isSilent = SILENT_URLS.some(url => req.url.includes(url));
  if (isSilent) return next(req);

  loader.show();
  return next(req).pipe(finalize(() => loader.hide()));};
