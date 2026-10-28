import {
  ApplicationConfig,
  inject,
  PLATFORM_ID,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideClientHydration } from '@angular/platform-browser';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { isPlatformBrowser } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './core/services/auth/auth.service';


import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'top'
      })
    ),


    provideHttpClient(
      withInterceptors([authInterceptor])
    ),


    provideAppInitializer(() => {
      const platformId = inject(PLATFORM_ID);
      const authService = inject(AuthService);

      if (!isPlatformBrowser(platformId)) {
        return;
      }

      return firstValueFrom(authService.restoreSession());
    }),


    provideClientHydration()
  ]
};