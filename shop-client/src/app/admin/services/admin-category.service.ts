import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category } from '../../core/models/category.model';

export interface CreateCategoryRequest {
  name: string;
  slug: string;
  description: string | null;
  imageUrl: string | null;
  displayOrder: number;
}

@Injectable({
  providedIn: 'root',
})
export class AdminCategoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Categories`;

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  create(request: CreateCategoryRequest): Observable<number> {
    return this.http.post<number>(this.apiUrl, request);
  }
}
