import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  if (!req.url.startsWith('/api/') || req.url.startsWith('/api/token')) {
    return next(req);
  }

  const send = (token: string | null) =>
    next(token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req);

  return from(auth.validToken()).pipe(
    switchMap((token) =>
      send(token).pipe(
        catchError((error: unknown) => {
          const expired =
            token &&
            error instanceof HttpErrorResponse &&
            (error.status === 401 || error.status === 403) &&
            error.headers.get('Token-Expired') === 'true';

          return expired
            ? from(auth.refresh()).pipe(
                switchMap((fresh) =>
                  fresh && fresh !== token ? send(fresh) : throwError(() => error),
                ),
              )
            : throwError(() => error);
        }),
      ),
    ),
  );
};
