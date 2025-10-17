import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BaseApiService {
  public readonly baseUrl = '/api/v1';
  public http = inject(HttpClient);

  public get<T>(url: string, params?: any): Observable<T> {
    const options = {
      params: this.createParams(params)
    };

    return this.http.get<T>(`${this.baseUrl}${url}`, options);
  }

  public post<T>(url: string, body: any, params?: any): Observable<T> {
    const options = {
      params: this.createParams(params)
    };

    return this.http.post<T>(`${this.baseUrl}${url}`, body, options);
  }

  public put<T>(url: string, body: any, params?: any): Observable<T> {
    const options = {
      params: this.createParams(params)
    };

    return this.http.put<T>(`${this.baseUrl}${url}`, body, options);
  }

  public patch<T>(url: string, body: any, params?: any): Observable<T> {
    const options = {
      params: this.createParams(params)
    };

    return this.http.patch<T>(`${this.baseUrl}${url}`, body, options);
  }

  public delete<T>(url: string, params?: any): Observable<T> {
    const options = {
      params: this.createParams(params)
    };

    return this.http.delete<T>(`${this.baseUrl}${url}`, options);
  }

  public createParams(params: any): HttpParams {
    let httpParams = new HttpParams();

    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== null && value !== undefined && value !== '') {
          if (Array.isArray(value)) {
            value.forEach(item => {
              httpParams = httpParams.append(key, item.toString());
            });
          } else {
            httpParams = httpParams.set(key, value.toString());
          }
        }
      });
    }

    return httpParams;
  }

  // For file uploads - different header setup
  public createFormDataRequest(formData: FormData, url: string): Observable<any> {
    return this.http.post(`${this.baseUrl}${url}`, formData);
  }
}
