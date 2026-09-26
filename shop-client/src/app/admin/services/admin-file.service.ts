import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface UploadedFileResponse {
  url: string;
  absoluteUrl: string;
  fileName: string;
  contentType: string;
  size: number;
}

@Injectable({
  providedIn: 'root',
})
export class AdminFileService {
  private readonly http =
    inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/files`;

  uploadImage(
    file: File,
  ): Observable<UploadedFileResponse> {
    return this.upload(
      file,
      'product-images',
    );
  }

  uploadCategoryImage(
    file: File,
  ): Observable<UploadedFileResponse> {
    return this.upload(
      file,
      'category-images',
    );
  }

  private upload(
    file: File,
    endpoint: string,
  ): Observable<UploadedFileResponse> {
    const formData =
      new FormData();

    formData.append(
      'file',
      file,
      file.name,
    );

    return this.http.post<UploadedFileResponse>(
      `${this.apiUrl}/${endpoint}`,
      formData,
    );
  }
}