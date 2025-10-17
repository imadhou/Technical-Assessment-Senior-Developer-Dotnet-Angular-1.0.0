import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { createApiError } from '../models/utility-types';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError(error => {
      const apiError = createApiError(error);
      console.error('API Error:', apiError);
      return throwError(() => apiError);
    })
  );
};
