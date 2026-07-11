import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

import { CartService } from '../../core/services/cart';

@Component({
  selector: 'app-checkout',
  imports: [
    ReactiveFormsModule,
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss'
})
export class Checkout {
  private readonly formBuilder = inject(FormBuilder);

  readonly cart = inject(CartService);

  readonly checkoutForm = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    mobile: [
      '',
      [
        Validators.required,
        Validators.pattern(/^09\d{9}$/)
      ]
    ],
    province: ['', Validators.required],
    city: ['', Validators.required],
    postalCode: [
      '',
      [
        Validators.required,
        Validators.pattern(/^\d{10}$/)
      ]
    ],
    address: ['', [Validators.required, Validators.minLength(10)]]
  });

  submit(): void {
    if (this.checkoutForm.invalid || this.cart.items().length === 0) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    console.log({
      customer: this.checkoutForm.getRawValue(),
      items: this.cart.items(),
      totalPrice: this.cart.totalPrice()
    });
  }
}