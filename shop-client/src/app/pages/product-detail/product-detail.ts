import {
  Component,
  computed,
  inject,
  signal,
} from '@angular/core';

import { DecimalPipe } from '@angular/common';
import {
  ActivatedRoute,
  RouterLink,
} from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Product } from '../../core/models/product.model';

import {
  ProductOption,
  ProductOptionInputType,
  ProductOptionValue,
} from '../../core/models/product-option.model';

import { SelectedCartOption } from '../../core/models/cart-item.model';

import { Seo } from '../../core/services/seo/seo';
import { CartService } from '../../core/services/cart/cart';

import {
  ProductResponse,
  ProductService,
} from '../../core/services/product/product';

const SITE_URL = 'https://sevart.ir';

@Component({
  selector: 'app-product-detail',

  imports: [
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
  ],

  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
})
export class ProductDetail {
  private readonly route = inject(ActivatedRoute);

  private readonly productService =
    inject(ProductService);

  private readonly seo =
    inject(Seo);

  private readonly cartService =
    inject(CartService);

  private readonly snackBar =
    inject(MatSnackBar);

  readonly slug =
    this.route.snapshot.paramMap.get('id');

  readonly ProductOptionInputType =
    ProductOptionInputType;

  readonly product =
    signal<Product | undefined>(undefined);

  readonly loading =
    signal(true);

  readonly selectedOptions =
    signal<Record<number, ProductOptionValue>>({});

  readonly quantity = signal(1);

readonly sortedOptions = computed(() =>
  [...(this.product()?.options ?? [])]
    .sort(
      (firstOption, secondOption) =>
        firstOption.displayOrder -
        secondOption.displayOrder,
    ),
);

  readonly finalPrice = computed(() => {
    const product = this.product();

    if (!product) {
      return 0;
    }

    const basePrice =
      product.discountPrice ??
      product.price;

    const optionsPrice =
      Object.values(
        this.selectedOptions(),
      ).reduce(
        (total, selectedValue) =>
          total +
          selectedValue.priceAdjustment,
        0,
      );

    return basePrice + optionsPrice;
  });

  readonly requiredOptionsSelected =
    computed(() => {
      const selectedOptions =
        this.selectedOptions();

      return this.sortedOptions()
        .filter(
          option =>
            option.isRequired,
        )
        .every(
          option =>
            Boolean(
              selectedOptions[
                option.id
              ],
            ),
        );
    });

  constructor() {
    this.loadProduct();
  }

  private loadProduct(): void {
    const slug = this.slug;

    if (!slug) {
      this.loading.set(false);
      this.setNotFoundSeo();

      return;
    }

    this.productService
      .getBySlug(slug)
      .subscribe({
        next: (
          response: ProductResponse,
        ) => {
          const product =
            this.mapProduct(response);

          this.product.set(product);

          this.selectedOptions.set({});

          this.loading.set(false);

          this.setProductSeo(product);
        },

        error: (
          error: unknown,
        ) => {
          console.error(
            'Load product failed:',
            error,
          );

          this.product.set(undefined);

          this.loading.set(false);

          this.setNotFoundSeo();
        },
      });
  }

  private mapProduct(
    response: ProductResponse,
  ): Product {
    return {
      id:
        response.id,

      categoryId:
        response.categoryId,

      title:
        response.name,

      slug:
        response.slug,

      price:
        response.price,

      imageUrl:
        response.imageUrl ??
        '/images/home/default-product-image.webp',

      // فعلاً Backend عمومی CategoryName برنمی‌گرداند.
      categoryName:
        '',

      // endpoint عمومی فقط Product منتشرشده را برمی‌گرداند.
      isAvailable:
        true,

      options:
        response.options ?? [],
    };
  }

  private setProductSeo(
    product: Product,
  ): void {
    this.seo.update({
      title:
        `${product.title} | فروشگاه آنلاین`,

      description:
        `خرید ${product.title} با قیمت مناسب از فروشگاه آنلاین سِوارت.`,

      canonicalUrl:
        `${SITE_URL}/products/${product.slug}`,

      imageUrl:
        product.imageUrl.startsWith(
          'http',
        )
          ? product.imageUrl
          : `${SITE_URL}${product.imageUrl}`,

      type:
        'product',
    });
  }

  private setNotFoundSeo(): void {
    this.seo.update({
      title:
        'محصول پیدا نشد | فروشگاه آنلاین',

      description:
        'محصول موردنظر شما در فروشگاه پیدا نشد.',

      canonicalUrl:
        `${SITE_URL}/products/${this.slug ?? ''}`,

      type:
        'website',
    });
  }

  sortedOptionValues(
    option: ProductOption,
  ): ProductOptionValue[] {
    return [...option.values]
      .filter(
        value =>
          value.isActive,
      )
      .sort(
        (firstValue, secondValue) =>
          firstValue.displayOrder -
          secondValue.displayOrder,
      );
  }

  selectOption(
    optionId: number,
    optionValue: ProductOptionValue,
  ): void {
    if (!optionValue.isActive) {
      return;
    }

    this.selectedOptions.update(
      currentOptions => ({
        ...currentOptions,

        [optionId]:
          optionValue,
      }),
    );
  }

  selectFromDropdown(
    option: ProductOption,
    selectedValueId: number | null,
  ): void {
    const selectedValue =
      option.values.find(
        value =>
          value.id ===
          selectedValueId,
      );

    if (!selectedValue) {
      this.removeSelectedOption(
        option.id,
      );

      return;
    }

    this.selectOption(
      option.id,
      selectedValue,
    );
  }

  removeSelectedOption(
    optionId: number,
  ): void {
    this.selectedOptions.update(
      currentOptions => {
        const updatedOptions = {
          ...currentOptions,
        };

        delete updatedOptions[
          optionId
        ];

        return updatedOptions;
      },
    );
  }

  isOptionSelected(
    optionId: number,
    valueId: number,
  ): boolean {
    return (
      this.selectedOptions()[
        optionId
      ]?.id === valueId
    );
  }

  addToCart(): void {
    const product =
      this.product();

    if (
      !product ||
      !product.isAvailable
    ) {
      return;
    }

    if (
      !this.requiredOptionsSelected()
    ) {
      this.snackBar.open(
        'لطفاً گزینه‌های ضروری محصول را انتخاب کنید.',
        'باشه',
        {
          duration: 3500,
          horizontalPosition:
            'center',
          verticalPosition:
            'bottom',
        },
      );

      return;
    }

    const selectedCartOptions =
      this.createSelectedCartOptions();

    this.cartService.add(
      product,
      selectedCartOptions,
      this.finalPrice(),
      this.quantity(),
    );

    this.snackBar.open(
      'محصول با موفقیت به سبد خرید اضافه شد.',
      'بستن',
      {
        duration: 3000,
        horizontalPosition:
          'center',
        verticalPosition:
          'bottom',
      },
    );
  }

  increaseQuantity(): void {
    this.quantity.update(quantity => quantity + 1);
  }

  decreaseQuantity(): void {
    this.quantity.update(quantity => Math.max(1, quantity - 1));
  }

  onImageError(
    event: Event,
  ): void {
    const image =
      event.target;

    if (
      !(
        image instanceof
        HTMLImageElement
      ) ||
      image.dataset[
        'fallbackApplied'
      ] === 'true'
    ) {
      return;
    }

    image.dataset[
      'fallbackApplied'
    ] = 'true';

    image.src =
      '/images/home/default-product-image.webp';
  }

  private createSelectedCartOptions():
    SelectedCartOption[] {
    const selectedOptions =
      this.selectedOptions();

    return this.sortedOptions()
      .filter(
        option =>
          Boolean(
            selectedOptions[
              option.id
            ],
          ),
      )
      .map(
        option => {
          const selectedValue =
            selectedOptions[
              option.id
            ];

          return {
            optionId:
              String(
                option.id,
              ),

            optionTitle:
              option.name,

            valueId:
              String(
                selectedValue.id,
              ),

            valueTitle:
              selectedValue.label,

            priceModifier:
              selectedValue.priceAdjustment,
          };
        },
      );
  }
}
