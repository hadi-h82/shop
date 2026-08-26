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
  status: ProductStatus;

  categoryId: number;
  categoryName: string;

  imageUrl: string | null;
  displayOrder: number;
}


export interface AdminProductResponse {
  id: number;
  categoryId: number;
  name: string;
  slug: string;
  description: string | null;
  price: number;
  status: ProductStatus;
  displayOrder: number;

  images: AdminProductImageResponse[];
  options: AdminProductOptionResponse[];
}

export interface AdminProductImageResponse {
  id: number;
  url: string;
  isPrimary: boolean;
  displayOrder: number;
}

export interface AdminProductOptionResponse {
  id: number;
  productOptionDefinitionId: number;
  name: string;
  inputType: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
  values: AdminProductOptionValueResponse[];
}

export interface AdminProductOptionValueResponse {
  id: number;
  label: string;
  value: string;
  priceAdjustment: number;
  colorCode: string | null;
  isActive: boolean;
  displayOrder: number;
}

export interface UpdateAdminProductRequest {
  categoryId: number;
  name: string;
  slug: string;
  description: string | null;
  price: number;
  displayOrder: number;
}

export enum ProductStatus {
  Draft = 1,
  Published = 2,
  Archived = 3
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

getById(
  id: number
): Observable<AdminProductResponse> {
  return this.http.get<AdminProductResponse>(
    `${this.apiUrl}/${id}`
  );
}
update(
  id: number,
  request: UpdateAdminProductRequest
): Observable<void> {
  return this.http.put<void>(
    `${this.apiUrl}/${id}`,
    request
  );
}

activate(id: number): Observable<void> {
  return this.http.patch<void>(
    `${this.apiUrl}/${id}/activate`,
    {}
  );
}

deactivate(id: number): Observable<void> {
  return this.http.patch<void>(
    `${this.apiUrl}/${id}/deactivate`,
    {}
  );
}

delete(id: number): Observable<void> {
  return this.http.delete<void>(
    `${this.apiUrl}/${id}`
  );
}

}