
import { inject } from '@angular/core';
import {
    HttpInterceptorFn
} from '@angular/common/http';

import { AuthService } from '../services/auth/auth.service';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const token = authService.accessToken();

    const apiBaseUrl = environment.apiUrl.replace(/\/+$/, '');

    const isApiRequest =
        req.url.startsWith(`${apiBaseUrl}/`);

    if (!isApiRequest || !token) {
        return next(req);
    }

    const authenticatedRequest = req.clone({
        setHeaders: {
            Authorization: `Bearer ${token}`
        }
    });

    return next(authenticatedRequest);
};
