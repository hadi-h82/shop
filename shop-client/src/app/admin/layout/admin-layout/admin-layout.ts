import {
  Component,
  PLATFORM_ID,
  inject,
  signal
} from '@angular/core';

import {
  isPlatformBrowser
} from '@angular/common';

import {
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';

import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-admin-layout',

  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatIconModule
  ],

  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss'
})
export class AdminLayout {

  private readonly platformId =
    inject(PLATFORM_ID);

  readonly sidebarOpen = signal(true);


  toggleSidebar(): void {
    this.sidebarOpen.update(
      value => !value
    );
  }


  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }


  closeSidebarOnMobile(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    if (window.innerWidth <= 768) {
      this.closeSidebar();
    }
  }
}