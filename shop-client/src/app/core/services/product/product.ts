import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

export interface ProductResponse {
  id: number;
  categoryId: number;
  name: string;
  slug: string;
  description: string | null;
  price: number;
  imageUrl: string | null;
  displayOrder: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Products`;

  getByCategory(
    categorySlug: string
  ): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(
      `${this.apiUrl}/category/${encodeURIComponent(categorySlug)}`
    );
  }
}