import { ApplicationConfig, isDevMode } from '@angular/core';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { tokenInterceptor } from './Components/Auth/token.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/loading.interceptor';
import { provideRouter } from '@angular/router';
import { provideServiceWorker } from '@angular/service-worker';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
  provideServiceWorker('ngsw-worker.js', {
  enabled: !isDevMode(),
  registrationStrategy: 'registerWhenStable:30000' // ← change this
}),
    provideHttpClient(
      withInterceptors([tokenInterceptor, errorInterceptor, loadingInterceptor])
    )
  ]
};