
import {
    Component,
    inject,
    OnDestroy
} from '@angular/core';

import {
    MAT_SNACK_BAR_DATA,
    MatSnackBarRef
} from '@angular/material/snack-bar';

import { MatIconModule } from '@angular/material/icon';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastData {
    message: string;
    type: ToastType;
}

@Component({
    selector: 'app-toast',
    standalone: true,
    imports: [MatIconModule],
    template: `
    <div
      role="status"
      class="flex items-center gap-3 rounded-xl px-4 py-3 text-white shadow-lg"


[class.bg-emerald-600]="data.type === 'success'"
[class.bg-rose-600]="data.type === 'error'"
[class.bg-blue-600]="data.type === 'info'"


      (mouseenter)="pauseTimer()"
      (mouseleave)="resumeTimer()"
    >
      @switch (data.type) {
        @case ('success') {
          <mat-icon>check_circle</mat-icon>
        }
        @case ('error') {
          <mat-icon>error</mat-icon>
        }
        @case ('info') {
          <mat-icon>info</mat-icon>
        }
      }

      <span class="flex-1 text-sm font-medium">
        {{ data.message }}
      </span>

      <button
        type="button"
        class="flex !h-6 !w-6 shrink-0 cursor-pointer items-center justify-center rounded-full p-0 transition-colors hover:bg-white/20"
        (click)="close()"
        aria-label="بستن اعلان"
        title="بستن"
      >
        <mat-icon
          class="!m-0 !flex !h-4 !w-4 items-center justify-center !text-base !leading-none"
        >
          close
        </mat-icon>
      </button>
    </div>
  `
})
export class AppToast implements OnDestroy {
    readonly data = inject<ToastData>(MAT_SNACK_BAR_DATA);

    private readonly snackBarRef =
        inject(MatSnackBarRef<AppToast>);

    private readonly duration = 5000;

    private remainingTime = this.duration;
    private startedAt = 0;

    private timer: ReturnType<typeof setTimeout> | null = null;

    constructor() {
        this.resumeTimer();
    }






    



    pauseTimer(): void {
        console.log('TOAST: mouse entered');

        if (this.timer === null) {
            return;
        }

        clearTimeout(this.timer);
        this.timer = null;

        const elapsed = Date.now() - this.startedAt;

        this.remainingTime = Math.max(
            0,
            this.remainingTime - elapsed
        );

        console.log('TOAST: paused', {
            remainingTime: this.remainingTime
        });
    }

    resumeTimer(): void {
        console.log('TOAST: timer resumed');

        if (this.timer !== null) {
            return;
        }

        if (this.remainingTime <= 0) {
            this.close();
            return;
        }

        this.startedAt = Date.now();

        this.timer = setTimeout(() => {
            this.timer = null;
            this.close();
        }, this.remainingTime);
    }

    ngOnDestroy(): void {
        console.log('TOAST: component destroyed');
        this.clearTimer();
    }


    close(): void {
        this.clearTimer();
        this.snackBarRef.dismiss();
    }

    private clearTimer(): void {
        if (this.timer !== null) {
            clearTimeout(this.timer);
            this.timer = null;
        }
    }


}
