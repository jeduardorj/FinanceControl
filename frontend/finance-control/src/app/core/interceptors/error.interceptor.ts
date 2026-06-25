import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { throwError, catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado.';

      if (error.error?.detail) {
        errorMessage = error.error.detail;
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      } else if (Array.isArray(error.error)) {
        errorMessage = error.error.join(', ');
      }

      return throwError(() => ({
        status: error.status,
        message: errorMessage
      }));
    })
  );
};