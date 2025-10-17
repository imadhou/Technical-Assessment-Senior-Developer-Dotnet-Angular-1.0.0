import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { API_CONFIG, Receipt } from '../models/model.index';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class ReceiptService {
  private readonly receiptEndpoint = API_CONFIG.ENDPOINTS.EXPENSES;
  private baseApi = inject(BaseApiService);
  private http = inject(HttpClient);

  uploadReceipt(file: File, expenseId: string): Observable<Receipt> {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('ExpenseId', expenseId);

    return this.baseApi.createFormDataRequest(formData, this.receiptEndpoint.RECEIPT) as Observable<Receipt>;
  }

  getReceipt(expenseId: string): Observable<Blob> {
    const url = this.baseApi.baseUrl + this.receiptEndpoint.RECEIPT_BY_ID.replace('{expenseId}', expenseId);
    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  deleteReceipt(expenseId: string): Observable<void> {
    const url = this.receiptEndpoint.RECEIPT_BY_ID.replace('{expenseId}', expenseId);
    return this.baseApi.delete<void>(url);
  }

  downloadReceipt(expenseId: string, name?: string): void {
    this.getReceipt(expenseId).subscribe(blob => {
      console.log('Received blob:', blob);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `${name || 'receipt'}`;
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }
}
