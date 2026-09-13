import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import {
  inject,
  Injectable,
  PLATFORM_ID,
  signal
} from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly document = inject(DOCUMENT);

  private readonly storageKey = 'shop-theme';

  readonly theme = signal<ThemeMode>('light');

  constructor() {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const documentTheme =
      this.document.documentElement.dataset['theme'];

    this.theme.set(
      documentTheme === 'dark' ? 'dark' : 'light'
    );
  }

  toggle(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const nextTheme: ThemeMode =
      this.theme() === 'dark' ? 'light' : 'dark';

    this.theme.set(nextTheme);

    this.document.documentElement.dataset['theme'] =
      nextTheme;

    localStorage.setItem(
      this.storageKey,
      nextTheme
    );
  }
}