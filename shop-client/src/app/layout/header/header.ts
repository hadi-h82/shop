import {
  Component,
  DestroyRef,
  inject,
  signal
} from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive
} from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CartService } from '../../core/services/cart/cart';
import { ThemeService } from '../../core/services/theme/theme';
import { AuthService } from '../../core/services/auth/auth.service';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';




@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    RouterLinkActive,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  readonly cart = inject(CartService);
  readonly themeService = inject(ThemeService);

  readonly authService = inject(AuthService);
  readonly isLoggingOut = signal(false);


  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  readonly searchControl = new FormControl('', {
    nonNullable: true
  });

  constructor() {
    this.route.queryParamMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        this.searchControl.setValue(params.get('q') ?? '', {
          emitEvent: false
        });
      });
  }

  search(event?: SubmitEvent): void {
    event?.preventDefault();
    const query = this.searchControl.value.trim();
    this.router.navigate(['/products'], {
      queryParams: query ? { q: query } : {}
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');

    this.router.navigate(['/products']);
  }



  logout(): void {
    if (this.isLoggingOut()) {
      return;
    }

    this.isLoggingOut.set(true);

    this.authService.logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isLoggingOut.set(false);
          void this.router.navigate(['/']);
        },
        error: (error) => {
          this.isLoggingOut.set(false);

          console.error('Logout request failed', {
            status: error.status
          });
        }
      });
  }

}
