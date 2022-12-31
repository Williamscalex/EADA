import { HttpErrorResponse } from '@angular/common/http';
import { Type } from '@angular/core';
import { Observable, throwError, timer } from 'rxjs';
import { mergeMap, finalize } from 'rxjs/operators';
import { ErrorNotifierService } from 'src/app/services/error-notifier.service';
import { assignExcept } from '@global/utils';
import { newId } from '@global/utils';
import { FormGroup } from '@angular/forms';
import { LoggerService } from 'src/app/services/logger.service';
import { MODEL_ERROR_KEY } from './constants';
import { TreeError } from '@angular/compiler';
export const SESSION_EXPIRED_ERROR = 'session expired';
export enum HttpErrorCodes {
  Unknown = 0,
  NotFound = 404,
  SimpleErrorMessage = 470,
  EndpointError = 471,
  ModelValidationError = 472,
  ConfirmationNeeded = 473,
  OrderItemSerialNumberOverflow = 474,
  CompositeMessageResponse = 475,
  DeletedProductOnRent = 476,
  Critical = 499,
  ClientError = -1,
}
export enum EndpointErrorCategories {
  None = 0,
}
type ErrorTypeMap= {
  [HttpErrorCodes.Unknown]: ErrorNotification;
  [HttpErrorCodes.NotFound]: ErrorNotification<string>;
  [HttpErrorCodes.SimpleErrorMessage]: ErrorNotification<string>;
  [HttpErrorCodes.ModelValidationError]: ModelValidationErrorData;
  [HttpErrorCodes.EndpointError]: EndpointErrorData;
  [HttpErrorCodes.Critical]: ErrorNotification;
  [HttpErrorCodes.ConfirmationNeeded]: ErrorNotification<string>;
  [HttpErrorCodes.CompositeMessageResponse]: ErrorNotification<string>;
  [HttpErrorCodes.ClientError]: ErrorNotification<string>;
  [HttpErrorCodes.OrderItemSerialNumberOverflow]: ErrorNotification<string>;
};
export type ErrorType<TErrorCode extends keyof ErrorTypeMap> = ErrorTypeMap[TErrorCode];

  
export type ErrorTypes =
  | string
  | ModelValidationErrorData
  | EndpointErrorData
export type ErrorNotification<T extends ErrorTypes = ErrorTypes> = {
  status: number;
  type: HttpErrorCodes;
  error: T;
  id?:string;
};
export class HandledError implements ErrorNotification {
  public status:number;
  public type:HttpErrorCodes;
  public error:ErrorTypes;
  public stack?:string = '';
  public id?:string = newId();
  /**
   * An array of the standardized string messages used in this error, formatted as best as possible.
   */
  readonly messages: string[] = [];
  /**
   * Produces a new error instance containing shallow data found in the given error.
   * @param error An existing instance of {@link HandledError}.
   */
  constructor(error: HandledError);
  /**
   * Creates a new instance of {@link HandledError} using the given values.
   * @param status The HTTP status code provided by the server.
   * @param type A client-side only status code as interpretted by the client app.
   * @param error The object or string describing the error.
   * @param stack A client-side stack trace (present only on client-side errors).
   */
  constructor(
    status: number,
    type: HttpErrorCodes,
    error: ErrorTypes,
    stack?: string
  );
  constructor(
    status: number | HandledError,
    type?: HttpErrorCodes,
    error?: ErrorTypes,
    stack: string = ''
  ) {
    if (status instanceof HandledError) {
      const err = status;
      this.status = err.status;
      this.type = err.type;
      this.error = err.error;
      this.stack = err.stack;
    } else {
      this.status = status;
      this.type = type as HttpErrorCodes;
      this.error = error;
      //TODO chat gpt helped here...see here for issues with error handling
      const errorData = JSON.parse(error as string);
      if(!stack && error && 'stack' in errorData) this.stack = (error as any).stack as string;
      else if(!stack) stack = (new Error()).stack as string;
      if (typeof error === 'string') this.messages.push(error);
      else if (type === HttpErrorCodes.ModelValidationError) {
        const e = ModelValidationError.parse(error);
        // propagate the messages property to the handled error
        this.messages = e.messages;
        // set the local error object to the instantiated model validation error class
        this.error = e;
      }
    }
  }
  /**
   * The HTTP error response object that was received from the server. This property should be checked before use.
   */
  public originalResponse?: HttpErrorResponse = null;
  /**
   * Creates a new instance of {@link HandledError} from the given {@link ModelValidationError} instance.
   */
  static from(modelValidationError: ModelValidationError): HandledError {
    return new HandledError(
      modelValidationError.type,
      modelValidationError.type,
      modelValidationError
    );
  }
}
export interface ModelValidationErrorData {
  readonly type?: HttpErrorCodes;
  useExceptionMessage: boolean;
  invalidModelName: string;
  message: string;
  validationErrors: { [key: string]: string[] };
  readonly messages?: string[];
}
export const isCritical: (err: ErrorNotification) => boolean = (err) =>
  err?.status === HttpErrorCodes.Critical ||
  (err?.status !== 401 &&
    err?.status !== 403 &&
    err?.status !== 404 &&
    err?.status > 399 &&
    err?.status <= 470) ||
  err?.status === 0;
export class ModelValidationError implements ModelValidationErrorData {
  static code = HttpErrorCodes.ModelValidationError;
  /**
   * The type of error.
   */
  public readonly type = ModelValidationError.code;
  useExceptionMessage: boolean = false;
  invalidModelName: string = '';
  message: string = 'The input was invalid.';
  validationErrors: { [key: string]: string[] } = {};
  /**
   * Retrieves all validation error messages, disregarding any associated property labels.
   */
  public readonly messages: string[] = [];
  public constructor(data?: Partial<ModelValidationErrorData>) {
    if (data) assignExcept(this as ModelValidationError, data, 'messages');
    // set the messages property
    const msgs = this.useExceptionMessage
      ? [this.message]
      : ([] as string[]).concat(...Object.values(this.validationErrors));
    this.messages.splice(0, this.messages.length, ...msgs);
  }
  public static parse(
    data?: Partial<ModelValidationErrorData>
  ): ModelValidationError {
    if (data instanceof ModelValidationError) return data;
    return new ModelValidationError(data);
  }
}
export interface EndpointErrorData {
  code: number;
  category: EndpointErrorCategories;
  message: string;
}
/**
 * Processes an error for use in the client app and returns a standardized {@link HandledError} instance representing the error.
 * @param err The error response from server, local error, or just a string to treat as an error message.
 * @param service An optional instance of the {@link ErrorNotifierService error notifier service}
 * that, if provided, will be used to dispatch an error notification event.
 * @returns An instance of the standardized {@link HandledError} class representing the provided error data.
 */
export const handleError: (
  err: HttpErrorResponse | string | Error,
  service?: ErrorNotifierService
) => HandledError = (err, service) => {
  let args: ErrorNotification;
  let error: HandledError;
  const errorId = (err as any)?.id ?? newId();
  let stack:string = '';
  if (err instanceof HandledError) {
    args = err;
  } else if (typeof err === 'string') {
    args = {
      type: HttpErrorCodes.SimpleErrorMessage,
      error: err,
      status: HttpErrorCodes.SimpleErrorMessage,
    };
  } else if (err instanceof Error) {
    args = {
      type: HttpErrorCodes.SimpleErrorMessage,
      error: err.message,
      status: HttpErrorCodes.ClientError
    };
    stack = err?.stack;
  } else {
    switch (err.status) {
      case HttpErrorCodes.ConfirmationNeeded:
        args = {
          status: err.status,
          error: err.error,
          type: HttpErrorCodes.ConfirmationNeeded,
        };
        break;
      case HttpErrorCodes.ModelValidationError:
        args = {
          status: err.status,
          error: err.error,
          type: HttpErrorCodes.ModelValidationError,
        };
        break;
        break;
      case HttpErrorCodes.CompositeMessageResponse:
        args = {
          status: err.status,
          error: err.message,
          type: HttpErrorCodes.SimpleErrorMessage,
        };
        break;
      case 401:
      case 403:
      case HttpErrorCodes.EndpointError:
      case HttpErrorCodes.Critical:
      case HttpErrorCodes.SimpleErrorMessage:
      case HttpErrorCodes.NotFound:
        args = {
          status: err.status,
          error: err.error,
          type: HttpErrorCodes.SimpleErrorMessage,
        };
        break;
      case HttpErrorCodes.Unknown:
      default:
        args = {
          type: HttpErrorCodes.Unknown,
          error: err?.error || err?.message || 'An unexpected error occurred.',
          status: err.status,
        };
        break;
    }
    // Unhandled errors will come back as HTML. We can try to filter those out here.
    args = checkForUnhandledServerError(args);
  }
  stack = stack
  ? stack
  : typeof (err as any)?.stack === 'string' ? (err as any).stack : null;
  error = new HandledError(args.status, args.type, args.error, stack);
  error.id = errorId;
  if (err instanceof HttpErrorResponse) error.originalResponse = err;
  if (service) service.onError(error);
  return error;
};
/**
 * Checks for any unhandled server-side errors by looking for unformatted HTML in the error message;
 * this will return the adjusted error if one is found.
 * @param err The error to check.
 * @returns The adjusted error object for unhandled server-side errors, or the given error object otherwise.
 */
const checkForUnhandledServerError = (err: ErrorNotification) =>
  typeof err.error === 'string' && err.error.startsWith('<!DOCTYPE')
    ? { ...err, error: 'An unexpected error occurred.' }
    : err;
export const genericRetryStrategy =
  ({
    maxRetryAttempts = 3,
    scalingDuration = 1000,
    includedStatusCodes = [],
  }: {
    maxRetryAttempts?: number;
    scalingDuration?: number;
    includedStatusCodes?: number[];
  } = {}) =>
  (attempts: Observable<any>) => {
    return attempts.pipe(
      mergeMap((error, i) => {
        const retryAttempt = i + 1;
        // if maximum number of retries have been met
        // or response is a status code we don't wish to retry, throw error
        if (
          retryAttempt > maxRetryAttempts ||
          includedStatusCodes.findIndex((e) => e === error.status) < 0
        ) {
          return throwError(error);
        }
        console.log(
          `Attempt ${retryAttempt}: retrying in ${
            retryAttempt * scalingDuration
          }ms`
        );
        // retry after 1s, 2s, etc...
        return timer(retryAttempt * scalingDuration);
      }),
      finalize(() => {})
    );
  };
  /**
   * Executes the {@link AbstractControl.setErrors setErrors} method for any controls with a name matching a key found in the
   * given {@link ModelValidationError error's} {@link ModelValidationError.validationErrors validationErrors} array.
   * The value of the error will be an array of messages returned from the server.
   * @param form The form containing the controls that correspond to the given {@link error}.
   * @param error The {@link ModelValidationError model validation error} returned by the server for the input provided by the
   * given {@link form}. If the key links to a nested control, then that nested control will have the errors.
   * Alternatively, a {@link HandledError} can be provided here and nothing will occur if the error
   * does not contain a valid {@link ModelValidationError} in its error property.
   * @example
   * orderForm = new FormGroup({
   *   address: new FormGroup({ street: new FormControl() })
   * });
   *
   * orderService.updateOrder(orderForm.value).pipe(
   *   catchError((err:HandledError) => )
   * );
   */
export const addModelValidationErrorsToForm:
<T extends ModelValidationError|HandledError>(form:FormGroup, error:T, logger?:LoggerService) => T
 = <T extends ModelValidationError|HandledError>(form:FormGroup, error:T, logger?:LoggerService) => {
  if(!form || !error) throw new Error('The form and error must be provided when adding model validation errors to a form.');
  const errorToUse = error instanceof ModelValidationError
  ? error
  : (error as any)?.error instanceof ModelValidationError
  ? (error as any).error
  : null;
  if(!errorToUse) return error;
  Object.entries(errorToUse.validationErrors).forEach(([name,messages]) => {
    const control = form.get(name);
    logger?.info(`Processing validation errors for control \"${name}\". Found control:`, control);
    if(!control || !(messages as any)?.length) return;
    control.setErrors({
      [MODEL_ERROR_KEY]:messages
    });
  });
  return error;
}