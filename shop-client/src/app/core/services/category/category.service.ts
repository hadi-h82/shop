import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { Category } from '../../models/category.model';
import { ProductResponse } from '../product/product';
import { environment } from '../../../../environments/environment';

export interface CategoryDetailsResponse {
  category: Category;
  products: ProductResponse[];
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Categories`;

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  getBySlug(slug: string): Observable<Category> {
    return this.http.get<Category>(
      `${this.apiUrl}/slug/${encodeURIComponent(slug)}`
    );
  }

  getWithProducts(
    slug: string
  ): Observable<CategoryDetailsResponse> {
    return this.http.get<CategoryDetailsResponse>(
      `${this.apiUrl}/${encodeURIComponent(slug)}/products`
    );
  }
}