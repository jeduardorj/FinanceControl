import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSummary } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) { }

  getSummary(month?: number, year?: number): Observable<DashboardSummary> {
    let params = new HttpParams();

    if (month) params = params.set('month', month);
    if (year) params = params.set('year', year);

    return this.http.get<DashboardSummary>(this.apiUrl, { params });
  }
}