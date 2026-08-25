import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';



export interface CreateAdminProductRequest {
  categoryId: number;
  name: string;
  slug: string;
  description: string | null;
  price: number;
  displayOrder: number;
  options: CreateAdminProductOptionRequest[];
}

export interface CreateAdminProductOptionRequest {
  productOptionDefinitionId: number;
  isRequired: boolean;
  displayOrder: number;
  values: CreateAdminProductOptionValueRequest[];
}

export interface CreateAdminProductOptionValueRequest {
  label: string;
  value: string;
  priceAdjustment: number;
  colorCode: string | null;
  displayOrder: number;
}


export interface AdminProductListItem {
  id: number;
  name: string;
  slug: string;
  price: number;
  status: number;

  categoryId: number;
  categoryName: string;

  imageUrl: string | null;
  displayOrder: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminProductService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Products`;

  getAll(): Observable<AdminProductListItem[]> {
    return this.http.get<AdminProductListItem[]>(
      this.apiUrl
    );
  }

create(request: CreateAdminProductRequest) {
  return this.http.post<{ id: number }>(
    this.apiUrl,
    request
  );

}
}