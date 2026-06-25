import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Transaction,
  CreateTransaction,
  UpdateTransaction,
  TransactionFilter
} from '../models/transaction.model';
import { PagedResult } from '../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private readonly apiUrl = `${environment.apiUrl}/transactions`;

  constructor(private http: HttpClient) { }

  getPaged(
    pageNumber = 1,
    pageSize = 10,
    filter?: TransactionFilter
  ): Observable<PagedResult<Transaction>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (filter?.type !== undefined && filter.type !== null)
      params = params.set('type', filter.type);

    if (filter?.startDate)
      params = params.set('startDate', filter.startDate);

    if (filter?.endDate)
      params = params.set('endDate', filter.endDate);

    if (filter?.categoryId)
      params = params.set('categoryId', filter.categoryId);

    if (filter?.description)
      params = params.set('description', filter.description);

    return this.http.get<PagedResult<Transaction>>(
      `${this.apiUrl}/paged`, { params }
    );
  }

  getById(id: string): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/${id}`);
  }

  create(transaction: CreateTransaction): Observable<Transaction> {
    return this.http.post<Transaction>(this.apiUrl, transaction);
  }

  update(id: string, transaction: UpdateTransaction): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}`, transaction);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}