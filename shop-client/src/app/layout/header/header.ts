import {
  Component,
  DestroyRef,
  inject
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

import { CartService } from '../../core/services/cart';
import { ThemeService } from '../../core/services/theme';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    RouterLinkActive,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  readonly cart = inject(CartService);
  readonly themeService = inject(ThemeService);

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

  search(): void {
    const query = this.searchControl.value.trim();

    this.router.navigate(['/products'], {
      queryParams: query ? { q: query } : {}
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');

    this.router.navigate(['/products']);
  }
}