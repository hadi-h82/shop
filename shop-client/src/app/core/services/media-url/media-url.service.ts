import {
  Injectable,
} from '@angular/core';

import {
  environment,
} from '../../../../environments/environment';

export const DEFAULT_PRODUCT_IMAGE =
  '/images/home/default-product-image.webp';

@Injectable({
  providedIn: 'root',
})
export class MediaUrlService {
  private readonly apiOrigin =
    environment.apiUrl.replace(
      /\/api\/?$/i,
      '',
    );

  resolve(
    imageUrl: string | null | undefined,
    fallbackUrl = DEFAULT_PRODUCT_IMAGE,
  ): string {
    const normalizedValue =
      imageUrl?.trim();

    if (!normalizedValue) {
      return fallbackUrl;
    }

    /*
     * URL کامل؛ نیازی به تغییر ندارد.
     */
    if (
      normalizedValue.startsWith(
        'http://',
      ) ||
      normalizedValue.startsWith(
        'https://',
      )
    ) {
      return normalizedValue;
    }

    const normalizedImageUrl =
      normalizedValue.startsWith('/')
        ? normalizedValue
        : `/${normalizedValue}`;

    /*
     * تصویر آپلودشده روی Backend
     */
    if (
      normalizedImageUrl.startsWith(
        '/uploads/',
      )
    ) {
      return (
        `${this.apiOrigin}${normalizedImageUrl}`
      );
    }

    /*
     * تصویر موجود در public فرانت
     */
    return normalizedImageUrl;
  }

  applyFallback(
    event: Event,
    fallbackUrl = DEFAULT_PRODUCT_IMAGE,
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
      fallbackUrl;
  }
}