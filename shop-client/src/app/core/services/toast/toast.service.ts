
import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import {
  AppToast,
  ToastType
} from '../../components/app-toast/app-toast';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private readonly snackBar = inject(MatSnackBar);


private show(message: string, type: ToastType): void {
  this.snackBar.openFromComponent(AppToast, {
    data: { message, type },
    duration: 0,
    horizontalPosition: 'center',
    verticalPosition: 'bottom',
    direction: 'rtl',
    panelClass: ['shop-toast-overlay']
  });
}


  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  info(message: string): void {
    this.show(message, 'info');
  }
}
