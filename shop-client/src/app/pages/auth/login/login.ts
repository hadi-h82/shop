import {
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  OnDestroy,
  signal,
  viewChild,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { ToastService } from '../../../core/services/toast/toast.service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  private readonly otpInput = viewChild<ElementRef<HTMLInputElement>>('otpInput');

  readonly otpCode = signal('');

  readonly otpDigits = computed(() =>
    Array.from({ length: 6 }, (_, index) => this.otpCode()[index] ?? ''),
  );

  readonly otpTotalSeconds = signal(120);
  readonly otpClockVisible = signal(true);

  readonly isSubmitting = signal(false);
  readonly requestError = signal('');
  readonly hidePassword = signal(true);
  readonly otpStep = signal<'phone' | 'verify'>('phone');
  readonly challengeId = signal<string | null>(null);
  readonly otpExpiresInSeconds = signal(0);
  readonly verifyError = signal('');
  readonly resendRemainingSeconds = signal(0);

  readonly otpFormattedTime = computed(() => {
    const totalSeconds = this.otpExpiresInSeconds();

    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  });

  readonly resendWaitSeconds = computed(() =>
    Math.max(this.otpExpiresInSeconds(), this.resendRemainingSeconds()),
  );

  private otpTimer: ReturnType<typeof setInterval> | null = null;
  private otpExpiresAt = 0;

  private resendTimer: ReturnType<typeof setInterval> | null = null;
  private resendAvailableAt = 0;

  private readonly otpAutoFocus = effect(() => {
    if (this.otpStep() !== 'verify') {
      return;
    }

    const input = this.otpInput();

    if (input && !input.nativeElement.disabled) {
      input.nativeElement.focus();
    }
  });

  readonly loginForm = this.formBuilder.nonNullable.group({
    phoneNumber: ['', [Validators.required, Validators.pattern(/^09\d{9}$/)]],
  });

  readonly otpForm = this.formBuilder.nonNullable.group({
    code: ['', [Validators.required, Validators.pattern(/^[0-9]{6}$/)]],
  });

  submit(): void {
    if (this.loginForm.invalid || this.isSubmitting()) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.requestError.set('');

    const { phoneNumber } = this.loginForm.getRawValue();

    this.authService.requestOtp(phoneNumber).subscribe({

      next: (response) => {
        this.isSubmitting.set(false);
        this.requestError.set('');

        this.challengeId.set(response.challengeId);
        this.startOtpTimer(response.expiresInSeconds);
        this.startResendTimer(response.retryAfterSeconds);
        console.log('OTP TIMERS:', {
          expiresInSeconds: this.otpExpiresInSeconds(),
          resendRemainingSeconds: this.resendRemainingSeconds(),
          responseRetryAfter: response.retryAfterSeconds
        });

        this.otpStep.set('verify');

        this.toast.info('کد تأیید ارسال شد.');

        console.log('OTP request successful', {
          expiresInSeconds: response.expiresInSeconds,
          retryAfterSeconds: response.retryAfterSeconds,
        });
      },


      error: (error) => {
        this.isSubmitting.set(false);

        const errorCode = error.error?.errors?.[0]?.code;
        const retryAfter = error.error?.retryAfterSeconds;

        switch (errorCode) {
          case 'PhoneHourlyLimitExceeded':
            this.requestError.set(
              'تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً یک ساعت دیگر تلاش کنید.',
            );
            break;

          case 'PhoneCooldownActive':
            this.requestError.set(`لطفاً ${retryAfter ?? 90} ثانیه دیگر برای دریافت کد تلاش کنید.`);
            break;

          default:
            this.requestError.set('ارسال کد تأیید انجام نشد. لطفاً دوباره تلاش کنید.');
            break;
        }

        this.toast.error(this.requestError());

        console.log('OTP ERROR DEBUG', {
          status: error.status,
          errorCode,
          retryAfter,
          requestError: this.requestError(),
        });
      },
    });
  }

  verifyOtp(): void {
    if (this.otpExpiresInSeconds() === 0) {
      this.verifyError.set('زمان اعتبار کد به پایان رسیده است. لطفاً کد جدید دریافت کنید.');
      return;
    }

    if (this.otpForm.invalid || this.isSubmitting()) {
      this.otpForm.markAllAsTouched();
      return;
    }

    const challengeId = this.challengeId();

    if (!challengeId) {
      this.verifyError.set('درخواست کد تأیید معتبر نیست. لطفاً دوباره کد دریافت کنید.');
      return;
    }

    const { code } = this.otpForm.getRawValue();

    this.isSubmitting.set(true);
    this.verifyError.set('');

    this.authService.verifyOtp(challengeId, code).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);
        this.verifyError.set('');

        this.authService.setAuthenticatedUser(response);

        this.stopOtpTimer();
        this.stopResendTimer();


        void this.router.navigateByUrl('/').then((navigated) => {
          if (navigated) {
            this.toast.success('با موفقیت وارد حساب شدید.');
          }
        });

      },


      error: (error) => {
        this.isSubmitting.set(false);

        const errorCode = error.error?.errors?.[0]?.code;

        switch (errorCode) {
          case 'InvalidOtp':
            this.verifyError.set(
              'کد تأیید واردشده صحیح نیست.'
            );
            break;

          case 'InvalidOrExpiredOtp':
            this.verifyError.set(
              'کد تأیید نامعتبر یا منقضی شده است. لطفاً کد جدید دریافت کنید.'
            );
            break;

          default:
            this.verifyError.set(
              'تأیید کد انجام نشد. لطفاً دوباره تلاش کنید.'
            );
            break;
        }

        this.toast.error(this.verifyError());

        console.error('OTP verification failed', {
          status: error.status,
          errorCode
        });
      }

    });
  }


  resendOtp(): void {
    if (
      this.isSubmitting() ||
      this.resendRemainingSeconds() > 0 ||
      this.otpStep() !== 'verify'
    ) {
      return;
    }

    const { phoneNumber } = this.loginForm.getRawValue();

    this.isSubmitting.set(true);
    this.verifyError.set('');

    this.authService.requestOtp(phoneNumber).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);

        const expiresInSeconds = Number(response.expiresInSeconds);
        const retryAfterSeconds = Number(response.retryAfterSeconds);

        console.log('OTP RESEND RESPONSE:', response);

        if (
          !response.challengeId ||
          !Number.isFinite(expiresInSeconds) ||
          expiresInSeconds <= 0
        ) {
          this.toast.error('پاسخ ارسال مجدد کد معتبر نیست.');
          return;
        }

        // شناسه کد جدید
        this.challengeId.set(response.challengeId);

        // پاک‌کردن کد قبلی
        this.resetOtpInput();
        this.verifyError.set('');

        // راه‌اندازی مجدد ساعت و محدودیت ارسال
        this.startOtpTimer(expiresInSeconds);
        this.startResendTimer(
          Number.isFinite(retryAfterSeconds)
            ? retryAfterSeconds
            : 0
        );

        this.toast.info('کد تأیید جدید ارسال شد.');

        console.log('OTP RESEND STATE:', {
          challengeId: this.challengeId(),
          expiresInSeconds: this.otpExpiresInSeconds(),
          resendRemainingSeconds: this.resendRemainingSeconds(),
          otpFormValid: this.otpForm.valid
        });
      },

      error: (error) => {
        this.isSubmitting.set(false);

        if (error.status === 429) {
          this.verifyError.set(
            'محدودیت ارسال کد فعال است. لطفاً کمی بعد تلاش کنید.'
          );

          const retryAfter = Number(
            error.error?.retryAfterSeconds
          );

          if (Number.isFinite(retryAfter) && retryAfter > 0) {
            this.startResendTimer(retryAfter);
          }
        } else {
          this.verifyError.set(
            'ارسال مجدد کد انجام نشد. لطفاً دوباره تلاش کنید.'
          );
        }

        this.toast.error(this.verifyError());
      }
    });
  }


  backToPhone(): void {
    if (this.isSubmitting()) {
      return;
    }
    this.stopOtpTimer();
    this.otpStep.set('phone');
    this.challengeId.set(null);
    this.otpExpiresInSeconds.set(0);

    this.resetOtpInput();
    this.verifyError.set('');
    this.requestError.set('');
  }

private startOtpTimer(seconds: number): void {
  this.stopOtpTimer();

  // تنظیم مجدد زمان و انیمیشن ساعت
  this.otpClockVisible.set(false);
  this.otpTotalSeconds.set(seconds);

  this.otpExpiresAt = Date.now() + seconds * 1000;
  this.otpExpiresInSeconds.set(seconds);

  // ساخت مجدد عنصر ساعت برای شروع انیمیشن از ابتدا
  setTimeout(() => {
    this.otpClockVisible.set(true);
  }, 0);

  this.otpTimer = setInterval(() => {
    const remaining = Math.max(
      0,
      Math.ceil((this.otpExpiresAt - Date.now()) / 1000)
    );

    this.otpExpiresInSeconds.set(remaining);

    if (remaining === 0) {
      this.stopOtpTimer();
    }
  }, 1000);
}

  private stopOtpTimer(): void {
    if (this.otpTimer !== null) {
      clearInterval(this.otpTimer);
      this.otpTimer = null;
    }
  }



  private startResendTimer(seconds: number): void {
    this.stopResendTimer();

    this.resendAvailableAt = Date.now() + seconds * 1000;
    this.resendRemainingSeconds.set(seconds);

    if (seconds <= 0) {
      return;
    }

    this.resendTimer = setInterval(() => {
      const remaining = Math.max(0, Math.ceil((this.resendAvailableAt - Date.now()) / 1000));

      this.resendRemainingSeconds.set(remaining);

      if (remaining === 0) {
        this.stopResendTimer();
      }
    }, 1000);
  }

  private stopResendTimer(): void {
    if (this.resendTimer !== null) {
      clearInterval(this.resendTimer);
      this.resendTimer = null;
    }
  }

  ngOnDestroy(): void {
    this.stopOtpTimer();
    this.stopResendTimer();
  }

  logout(): void {
    if (this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);

    this.authService.logout().subscribe({
      next: () => {
        this.isSubmitting.set(false);

        this.otpStep.set('phone');
        this.challengeId.set(null);
        this.resetOtpInput();
        this.focusOtpInput();

        this.stopOtpTimer();
        this.stopResendTimer();

        this.otpExpiresInSeconds.set(0);
        this.resendRemainingSeconds.set(0);

        this.verifyError.set('');
        this.requestError.set('');

        console.log('Logout successful');
      },
      error: (error) => {
        this.isSubmitting.set(false);

        console.error('Logout failed', {
          status: error.status,
        });

        this.requestError.set('خروج از سرور تأیید نشد. لطفاً دوباره تلاش کنید.');
      },
    });
  }

  onOtpInput(event: Event): void {
    const input = event.target as HTMLInputElement;

    const normalized = input.value
      .replace(/[۰-۹]/g, (digit) => String('۰۱۲۳۴۵۶۷۸۹'.indexOf(digit)))
      .replace(/[٠-٩]/g, (digit) => String('٠١٢٣٤٥٦٧٨٩'.indexOf(digit)))
      .replace(/\D/g, '')
      .slice(0, 6);

    input.value = normalized;

    this.otpCode.set(normalized);
    this.otpForm.controls.code.setValue(normalized);
  }

  focusOtpInput(): void {
    this.otpInput()?.nativeElement.focus();
  }


  private resetOtpInput(): void {
    this.otpCode.set('');
    this.otpForm.controls.code.reset('');

    const input = this.otpInput();
    if (input) {
      input.nativeElement.value = '';
    }
  }

}
