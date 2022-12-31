import { Injectable, Optional } from '@angular/core';
import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse,
  HttpEvent,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of, empty, Subject, throwError, from } from 'rxjs';
import { NavigationHelperService } from '../../services/navigation-helper.service';
import {
  catchError,
  retryWhen,
  map,
  switchMap,
  mergeMap,
} from 'rxjs/operators';
import { genericRetryStrategy, handleError, SESSION_EXPIRED_ERROR } from '@global/errors';
import { ErrorNotifierService } from '../../services/error-notifier.service';
import { LoggerService } from '../../services/logger.service';
@Injectable({
  providedIn: 'root',
})
export abstract class EndpointBaseService {
  protected navigationHelper: NavigationHelperService;
  private taskPauser: Subject<any>;
  constructor(
    protected http: HttpClient,
    navigationHelper: NavigationHelperService,

    @Optional() protected errorNotifier: ErrorNotifierService = null,
    @Optional() protected logger: LoggerService = null
  ) {
    this.navigationHelper = navigationHelper;
  }
  private handleConnectionError(err: HttpErrorResponse): Observable<any> {
    this.logger?.auth('CHECK: Connection Error?', {err, status:err.status});
    if (err.status === 0) {
      this.navigationHelper.reportConnectionError();
    }
    return throwError(err);
  }

  protected deleteObj<T>(url: string, body?: object): Observable<T> {
    this.logger?.log('Preparing to delete object @ URL: ' + url, body);
    if (body) {
      const options = {
        headers: new HttpHeaders({
          'Content-Type': 'application/json',
        }),
        body,
      };
      return (this.http.delete<T>(url, options) as Observable<T>).pipe(
        catchError((err) => this.handleConnectionError(err))
      );
    } else {
      return this.http.delete<T>(url).pipe(
        catchError((err) => this.handleConnectionError(err))
      );
    }
  }
  protected postJson<T>(url: string): Observable<T>;
  protected postJson<T>(url: string, data: any): Observable<T>;
  protected postJson<T>(url: string, data?: any): Observable<T> {
    if (data !== null && data !== undefined) {
      return this.http
        .post<T>(url, data, {
          headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        })
        .pipe(
          catchError((err) => this.handleConnectionError(err)));
    } else {
      return this.http
        .post<T>(url, {
          headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        })
        .pipe(
          retryWhen(genericRetryStrategy({ includedStatusCodes: [0] })),
          catchError((err) => this.handleConnectionError(err))
        );
    }
  }
  protected get<T>(url: string): Observable<T> {
    return this.http.get<T>(url).pipe(
      catchError((err) => this.handleConnectionError(err))
    );
  }


  /**
   * Creates an error handler to use for endpoints.
   * @param autoShowCritical [false] - When true, this will cause the notifier to display messages from critical errors at the top of the app window.
   * @param options Use this object provide advanced configurations, including a default value to return from handler if an error is thrown.
   * @param options.default The default value to return from handler if an error is thrown.
   */
  protected buildEndpointErrorHandler(autoShowCritical?: boolean):(err:any)=>Observable<any>;
  protected buildEndpointErrorHandler(options?: {
    autoShowCritical?: boolean;
    defaultValue?: any;
  }):(err:any)=>Observable<any>;
  protected buildEndpointErrorHandler(
    args?: { autoShowCritical?: boolean; defaultValue?: any } | boolean
  ):(err:any)=>Observable<any> {
    const opts = {
      autoShowCritical:
        typeof args === 'boolean' ? args : args?.autoShowCritical ?? false,
      defaultValue:
        typeof args === 'object' && args?.defaultValue !== undefined
          ? args.defaultValue
          : undefined,
    };
    // if no default value was provided
    if (opts.defaultValue === undefined) {
      return ((err: any) => {
        if (opts.autoShowCritical) throw handleError(err, this.errorNotifier);
        throw handleError(err);
      }).bind(this);
    }
    // if a default value was provided
    return ((err: any) => {
      if (opts.autoShowCritical) {
        handleError(err, this.errorNotifier);
      } else {
        handleError(err);
      }
      return of(opts.defaultValue);
    }).bind(this);
  }
  private pauseTask(continuation: () => Observable<any>) {
    if (!this.taskPauser) {
      this.taskPauser = new Subject();
    }
    return this.taskPauser.pipe(
      switchMap((continueOp) => {
        return continueOp ? continuation() : throwError(SESSION_EXPIRED_ERROR);
      })
    );
  }
  private resumeTasks(continueOp: boolean) {
    setTimeout(() => {
      if (this.taskPauser) {
        this.taskPauser.next(continueOp);
        this.taskPauser.complete();
        this.taskPauser = null;
      }
    });
  }
}