import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { ProductOption } from '../../models/product-option.model';

export interface ProductImageResponse {
  id: number;
  url: string;
  isPrimary: boolean;
  displayOrder: number;
}

export interface ProductResponse {
  id: number;
  categoryId: number;
  categoryName: string;
  categorySlug: string;
  name: string;
  slug: string;
  description: string | null;
  price: number;
  imageUrl: string | null;
  displayOrder: number;
  images: ProductImageResponse[];
  options: ProductOption[];
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Products`;

  getByCategory(
    categorySlug: string,
  ): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(
      `${this.apiUrl}/category/${encodeURIComponent(categorySlug)}`,
    );
  }

  getBySlug(
    slug: string,
  ): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(
      `${this.apiUrl}/slug/${encodeURIComponent(slug)}`,
    );
  }
}