
import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map, of, tap } from 'rxjs';
import { finalize } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface RequestOtpResponse {
    challengeId: string;
    message: string;
    expiresInSeconds: number;
    retryAfterSeconds: number;
}


export interface VerifyOtpResponse {
    userId: string;
    fullName: string | null;
    phoneNumber: string;
    email: string | null;
    roles: string[];
    tokenType: string;
    accessToken: string;
    accessTokenExpiresAt: string;
}


export interface AuthUser {
    userId: string;
    fullName: string | null;
    phoneNumber: string;
    email: string | null;
    roles: string[];
}



@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly http = inject(HttpClient);

    private readonly apiUrl = `${environment.apiUrl}/auth`;

    private readonly currentUserState = signal<AuthUser | null>(null);
    private readonly accessTokenState = signal<string | null>(null);

    readonly accessToken = this.accessTokenState.asReadonly();
    readonly currentUser = this.currentUserState.asReadonly();

    readonly isAuthenticated = computed(
        () => this.currentUserState() !== null
    );


    requestOtp(phoneNumber: string): Observable<RequestOtpResponse> {
        return this.http.post<RequestOtpResponse>(
            `${this.apiUrl}/request-otp`,
            { phoneNumber }
        );
    }


    verifyOtp(
        challengeId: string,
        code: string
    ): Observable<VerifyOtpResponse> {
        return this.http.post<VerifyOtpResponse>(
            `${this.apiUrl}/verify-otp`,
            {
                challengeId,
                code
            },
            {
                withCredentials: true
            }
        );
    }


    refreshSession(): Observable<VerifyOtpResponse> {
        return this.http.post<VerifyOtpResponse>(
            `${this.apiUrl}/refresh`,
            {},
            {
                withCredentials: true
            }
        );
    }



    restoreSession(): Observable<boolean> {
        return this.refreshSession().pipe(
            tap((response) => {
                this.setAuthenticatedUser(response);
            }),

            map(() => true),

            catchError(() => {
                this.clearAuthState();
                return of(false);
            })
        );
    }



    logout(): Observable<void> {
        return this.http.post<void>(
            `${this.apiUrl}/logout`,
            {},
            { withCredentials: true }
        ).pipe(
            finalize(() => {
                this.clearAuthState();
            })
        );
    }




    setAuthenticatedUser(response: VerifyOtpResponse): void {
        this.accessTokenState.set(response.accessToken);
        this.currentUserState.set({
            userId: response.userId,
            fullName: response.fullName,
            phoneNumber: response.phoneNumber,
            email: response.email,
            roles: response.roles
        });
    }


    private clearAuthState(): void {
        this.accessTokenState.set(null);
        this.currentUserState.set(null);
    }



}
