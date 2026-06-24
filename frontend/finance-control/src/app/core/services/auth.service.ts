import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  RegisterRequest,
  TokenResponse
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';

  // Signal do usuário atual
  private _currentUser = signal<TokenResponse | null>(
    this.loadUserFromStorage()
  );

  // Computed signals — derivados automaticamente
  readonly isAuthenticated = computed(() => this._currentUser() !== null);
  readonly currentUser = computed(() => this._currentUser());

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  login(request: LoginRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/auth/login`, request)
      .pipe(
        tap(response => this.saveSession(response))
      );
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/register`, request);
  }

  logout(): void {
    const refreshToken = localStorage.getItem(this.REFRESH_KEY);

    if (refreshToken) {
      this.http.post(`${this.apiUrl}/auth/revoke`,
        { refreshToken },
        { headers: { Authorization: `Bearer ${this.getToken()}` } }
      ).subscribe({
        error: () => { } // ignora erro no logout
      });
    }

    this.clearSession();
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<TokenResponse> {
    const refreshToken = localStorage.getItem(this.REFRESH_KEY) ?? '';

    return this.http.post<TokenResponse>(
      `${this.apiUrl}/auth/refresh`,
      { refreshToken }
    ).pipe(
      tap(response => this.saveSession(response))
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private saveSession(response: TokenResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response));
    this._currentUser.set(response);
  }

  private clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem(this.USER_KEY);
    this._currentUser.set(null);
  }

  private loadUserFromStorage(): TokenResponse | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }
}