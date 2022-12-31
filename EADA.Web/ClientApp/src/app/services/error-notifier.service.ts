import { Injectable, Optional, Inject } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { BehaviorSubject, filter, first, map, Observable, ReplaySubject, shareReplay, startWith, Subject, tap } from 'rxjs';
import { LoggerService } from './logger.service';
import { firstBy } from 'thenby';
import { ErrorNotification, HttpErrorCodes, isCritical, SESSION_EXPIRED_ERROR } from '@global/errors';
import { newId } from '@global/utils';
/**
 * The number of past errors stored in the errorIds array.
 */
const ERROR_HISTORY_COUNT = 10;
@Injectable({
  providedIn: 'root',
})
export class ErrorNotifierService {
  constructor(
    @Optional() private notifier: NotifierService,
    private logger: LoggerService
  ) {
    this.logger.log('notifier service: ', notifier);
    // subscription should live throughout the lifetime of this instance.
    this.errorHistory$.subscribe();
  }
  private errorSubject = new Subject<ErrorNotification>();
  /**
   * Internal object used to provided error history. This property should not be used directly as it may be
   * constantly mutating as errors occur. To access this data, use the {@link errorHistory$} observable.
   * Contains the recent errors that have occurred and their timestamps.
   */
  private _errorHistoryData: { [id: string]: number } = {};
  /**
   * A dictionary of the most recent errors, grouped by their {@link ErrorNotification.id IDs},
   * and set to the value of the timestamp when they occurred.
   */
  public errorHistory$ = this.errorSubject.pipe(
    map(err => err?.id),
    startWith(null),
    this.recordErrorInHistory());
  /**
   * A list of error IDs found in the current history.
   */
  public errorHistoryKeys$ = this.errorHistory$.pipe(
    map(data => Object.keys(data)),
    shareReplay(1)
  );
  public allErrors$ = this.errorSubject.asObservable();
  public criticalError$ = this.errorSubject.pipe(
    filter((e) => this.isShowableErrorType(e)),
    tap((err) => {
      this.lastCriticalErrorTimestamp = Date.now();
      this.lastCriticalError = err;
    })
  );
  private lastCriticalErrorTimestamp: number = 0;
  private lastCriticalError: ErrorNotification;
  public modelValidationError$ = this.errorSubject.pipe(
    filter((e) => e.type === HttpErrorCodes.ModelValidationError)
  );
  private clearErrorsSubject = new Subject();
  public clearErrors$ = this.clearErrorsSubject.asObservable();
  public clearErrors() {
    this.clearErrorsSubject.next(null);
  }
  /**
   * Adds an error to the history data object.
   * @returns An output observable that will emit the error IDs found in history after recording the error provided.
   */
  private recordErrorInHistory(): (
    source: Observable<string>  ) => Observable<{[id:string]:number}> {
    return (source) => source.pipe(
      tap(id => {
        // skip if error nothing
        if(!id) return;
        else if(this._errorHistoryData[id]) {
          // if already logged, then update the timestamp
          this._errorHistoryData[id] = Date.now();
          return;
        }
        // current IDs in error history
        const currentIds = Object.keys(this._errorHistoryData);
        // make room for new error
        if (currentIds.length >= ERROR_HISTORY_COUNT) {
          // get error IDs, sorted from oldest to newest
          const entries = Object
          .entries(this._errorHistoryData)
          .sort(firstBy(x => x[1], 'asc'));
          while (entries.length >= ERROR_HISTORY_COUNT) {
            const oldId = entries.pop()[0];
            delete this._errorHistoryData[oldId];
          }
        }
        // add the error to our history
        this._errorHistoryData[id] = Date.now();
      }),
      shareReplay(1),
      map(() => ({...this._errorHistoryData}))
    )
  }
  /**
   * Determines if a critical error has been reported recently.
   */
  isReported(criticalError: ErrorNotification) {
    const now = Date.now();
    if (!this.lastCriticalError || now - this.lastCriticalErrorTimestamp > 5000)
      return false;
    return this.lastCriticalError === criticalError;
  }
  /**
   * Determines if the given error should be shown to the user.
   */
  private isShowableErrorType(err: ErrorNotification): boolean {
    return !!(
      isCritical(err) &&      this.notifier &&      err.error !== SESSION_EXPIRED_ERROR &&      err.status !== 401 &&      err.status !== 403
    );
  }
  /**
   * Checks if the given error ID exists in the current error history data.
   * @param id The error ID to check for.
   * @returns A cold observable emitting the result of the check.
   */
  public errorHistoryIncludes(id:string):Observable<boolean>  /**
   * Checks if the given error ID exists in the error history and has occurred within the last {@link timeSinceErrorMilliseconds X milliseconds}.
   * @param id The error ID to check for.
   * @param timeSinceErrorMilliseconds Time since the error occurred, in milliseconds.
   * @returns A cold observable emitting the result of the check.
   */
  public errorHistoryIncludes(id:string, timeSinceErrorMilliseconds:number):Observable<boolean>  
  public errorHistoryIncludes(id:string, timeSinceErrorMilliseconds:number = null):Observable<boolean> {
    if(!timeSinceErrorMilliseconds) return this.errorHistoryKeys$.pipe(
      first(),
      map(ids => ids.includes(id)));
      const errorTime = Date.now() - timeSinceErrorMilliseconds;
      return this.errorHistory$.pipe(
        first(),
        map(data => {
          const entry = Object.entries(data).find(([key,timestamp]) => key === id && timestamp >= errorTime);
          return !!entry;
        })
      );
  }
  /**
   * Processes the given {@link err error} and triggers the workflow of events
   * that emit and track errors in the client app. If the error was emitted recently
   * (i.e., if it's ID was found in the {@link errorHistory$ history}) then the method call will be ignored.
   * @param err The error that occurred.
   * @param ignoreDuplicates When true or unsupplied (default), the error event will not be emitted if it has been
   * recently emitted for the same error.
   * @returns The error ID.
   */
  public async onError(err: ErrorNotification, ignoreDuplicates:boolean = true): Promise<string> {
    if(!err) { // bad data provided
      this.logger.warn('The "onError" function was called without any error data. No error will be recorded.');
      return '';
    }
    // ensure ID is set
    if (err && !err.id) err.id = newId();
    // check if exists in history
    const exists = await this.errorHistoryIncludes(err.id).toPromise();
    if(ignoreDuplicates && !exists) { // error not found in history, continue
      this.logger.warn('Error thrown! ', err);
      this.errorSubject.next(err);
      if (this.isShowableErrorType(err)) {
        if (err.type !== 0) {
          this.notifier.notify('error', err.error as string);
        } else {
          this.notifier.notify('error', 'An unexpected error occured.');
        }
      }
    } else {
      // error found in history; omit call
      this.logger.warn(`An error was thrown that had already been recorded recently. Error ID: ${err.id}`);
    }
    return err.id;
  }
  public showWarning(message: string): void {
    this.notifier.notify('warning', message);
  }
  public showError(message: string | Error): void {
    if (message instanceof Error)
      message = message?.message ?? 'An unexpected client-side error occurred.';
    this.notifier.notify('error', message);
  }
}