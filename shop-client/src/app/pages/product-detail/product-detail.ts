import {
  Component,
  computed,
  inject,
  signal
} from '@angular/core';

import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import {
  ProductOption,
  ProductOptionValue
} from '../../core/models/product-option.model';
import { Seo } from '../../core/services/seo';

const SITE_URL = 'https://sevart.ir';

@Component({
  selector: 'app-product-detail',
  imports: [
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly seo = inject(Seo);

  readonly slug = this.route.snapshot.paramMap.get('id');

  readonly product = computed(() =>
    MOCK_PRODUCTS.find(
      product => product.slug === this.slug
    )
  );

  readonly selectedOptions =
    signal<Record<number, ProductOptionValue>>({});

  readonly sortedOptions = computed(() =>
    [...(this.product()?.options ?? [])].sort(
      (firstOption, secondOption) =>
        firstOption.displayOrder - secondOption.displayOrder
    )
  );

  readonly finalPrice = computed(() => {
    const product = this.product();

    if (!product) {
      return 0;
    }

    const basePrice =
      product.discountPrice ?? product.price;

    const optionsPrice = Object.values(
      this.selectedOptions()
    ).reduce(
      (total, selectedValue) =>
        total + selectedValue.priceAdjustment,
      0
    );

    return basePrice + optionsPrice;
  });

  readonly requiredOptionsSelected = computed(() => {
    const selectedOptions = this.selectedOptions();

    return this.sortedOptions()
      .filter(option => option.isRequired)
      .every(option => Boolean(selectedOptions[option.id]));
  });

  constructor() {
    const product = this.product();

    if (product) {
      this.seo.update({
        title: `${product.title} | فروشگاه آنلاین`,
        description:
          `خرید ${product.title} از دسته‌بندی ${product.categoryName} با قیمت مناسب و ارسال سریع.`,
        canonicalUrl: `${SITE_URL}/products/${product.slug}`,
        imageUrl: `${SITE_URL}${product.imageUrl}`,
        type: 'product'
      });

      return;
    }

    this.seo.update({
      title: 'محصول پیدا نشد | فروشگاه آنلاین',
      description:
        'محصول موردنظر شما در فروشگاه پیدا نشد.',
      canonicalUrl: `${SITE_URL}/products/${this.slug}`,
      type: 'website'
    });
  }

  sortedOptionValues(
    option: ProductOption
  ): ProductOptionValue[] {
    return [...option.values].sort(
      (firstValue, secondValue) =>
        firstValue.displayOrder - secondValue.displayOrder
    );
  }

  selectOption(
    optionId: number,
    optionValue: ProductOptionValue
  ): void {
    if (!optionValue.isActive) {
      return;
    }

    this.selectedOptions.update(currentOptions => ({
      ...currentOptions,
      [optionId]: optionValue
    }));
  }

  selectFromDropdown(
    option: ProductOption,
    selectedValueId: number | null
  ): void {
    const selectedValue = option.values.find(
      value => value.id === selectedValueId
    );

    if (!selectedValue) {
      this.removeSelectedOption(option.id);
      return;
    }

    this.selectOption(option.id, selectedValue);
  }

  removeSelectedOption(optionId: number): void {
    this.selectedOptions.update(currentOptions => {
      const updatedOptions = { ...currentOptions };

      delete updatedOptions[optionId];

      return updatedOptions;
    });
  }

  isOptionSelected(
    optionId: number,
    valueId: number
  ): boolean {
    return (
      this.selectedOptions()[optionId]?.id === valueId
    );
  }

  addToCart(): void {
    const product = this.product();

    if (
      !product ||
      !product.isAvailable ||
      !this.requiredOptionsSelected()
    ) {
      return;
    }

    console.log({
      product,
      selectedOptions: this.selectedOptions(),
      finalPrice: this.finalPrice()
    });
  }

  onImageError(event: Event): void {
    const image = event.target;

    if (
      !(image instanceof HTMLImageElement) ||
      image.dataset['fallbackApplied'] === 'true'
    ) {
      return;
    }

    image.dataset['fallbackApplied'] = 'true';
    image.src = '/images/home/default-product-image.webp';
  }
}
