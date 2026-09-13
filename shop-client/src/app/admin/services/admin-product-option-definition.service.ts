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
}