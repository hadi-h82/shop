import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export enum ProductOptionInputType {
  Select = 1,
  Radio = 2,
  Color = 3,
}

export interface AdminProductOptionDefinitionResponse {
  id: number;
  name: string;
  slug: string;
  inputType: ProductOptionInputType;
  displayOrder: number;
  isActive: boolean;
}

export interface AdminProductOptionDefinitionRequest {
  name: string;
  slug: string;
  inputType: ProductOptionInputType;
  displayOrder: number;
}

@Injectable({
  providedIn: 'root',
})
export class AdminProductOptionDefinitionService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/ProductOptionDefinitions`;

  getAll(): Observable<AdminProductOptionDefinitionResponse[]> {
    return this.http.get<AdminProductOptionDefinitionResponse[]>(
      this.apiUrl,
    );
  }

  getById(
    id: number,
  ): Observable<AdminProductOptionDefinitionResponse> {
    return this.http.get<AdminProductOptionDefinitionResponse>(
      `${this.apiUrl}/${id}`,
    );
  }


  create(
  request: AdminProductOptionDefinitionRequest,
): Observable<AdminProductOptionDefinitionResponse> {
  return this.http.post<AdminProductOptionDefinitionResponse>(
    this.apiUrl,
    request,
  );
}

update(
  id: number,
  request: AdminProductOptionDefinitionRequest,
): Observable<void> {
  return this.http.put<void>(
    `${this.apiUrl}/${id}`,
    request,
  );
}

activate(id: number): Observable<void> {
  return this.http.patch<void>(
    `${this.apiUrl}/${id}/activate`,
    {},
  );
}

deactivate(id: number): Observable<void> {
  return this.http.patch<void>(
    `${this.apiUrl}/${id}/deactivate`,
    {},
  );
}
}