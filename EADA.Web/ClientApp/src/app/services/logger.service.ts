import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
export interface ILoggerService {
  log(message?: any, ...optionalParams: any[]): void;
}
@Injectable({
  providedIn: 'root'
})
export class LoggerService implements ILoggerService {
  /**
   * Creates a new logger instance and configures it's info function to use a specific label and color code.
   *
   * @param label The label to prefix to all info outputs.
   * @param color The color code to use on all info outputs.
   * @returns The new logger instance.
   */
  public static withInfoSettings(label:string, color:string) {
    let logger = new LoggerService();
    logger.info = logger.buildLogger(
      logger.debugLabel(label),
      color
    );
    return logger;
  }
  constructor(){
    if(!environment.production) {
      this.log = console.log.bind(this);
      this.crit = console.log.bind(this,'%c[CRIT] ','font-weight:bold;color:red;');
      this.warn = console.log.bind(this,'%c[WARN] ','font-weight:bold;color:orange;');
      this.info = console.log.bind(this,'%c[INFO] ','font-weight:bold;color:grey;');
      this.auth = console.log.bind(this,'%c[AUTH] ','font-weight:bold;color:#f87bf8;');
    } else {
      this.log = ()=>{};
      this.crit = ()=>{};
      this.warn = ()=>{};
      this.info = ()=>{};
      this.auth = ()=>{};
    }
  }
  log:(message?: any, ...optionalParams: any[])=> void;
  crit:(message?: any, ...optionalParams: any[])=> void;
  warn:(message?: any, ...optionalParams: any[])=> void;
  info:(message?: any, ...optionalParams: any[])=> void;
  auth:(message?: any, ...optionalParams: any[])=> void;
  /**
   * Constructs a new logging function with the given styling and label specifications.
   * This function is safe to use freely and will be automatically disabled in production like other native logging methods.
   * @param label The title that will be prepended to the log.
   * @param color The color of the log.
   * @returns A new logging function bound to the native "console.log" method.
   * @example
   * itemsLoaded: this.logger.buildLogger(
   *   this.logger.debugLabel('INIT ITEMS LOADED', DEBUG_ISSUE),
   *   '#092982'
   * ),
   */
  buildLogger(label:string, color:string = 'grey'):(message?: any, ...optionalParams: any[])=>void {
    let fn = !environment.production
    ? console.log.bind(this, `%c[${label}] `, `font-weight:bold;color:${color};`)
    : () => {};
    return fn;
  }
  /**
   * Creates a dynamic label you can use in logging methods.
   * @param label A phrase to display at beginning of message.
   * @param issue An optional issue number to prepend before the label.
   * @returns The full label string to use when building the logger function.
   * @example
   * itemsLoaded: this.logger.buildLogger(
   *   this.logger.debugLabel('INIT ITEMS LOADED', DEBUG_ISSUE),
   *   '#092982'
   * ),
   */
   public debugLabel(label: string, issue?: number): string {
    let value = !!issue ? `ADIM-${issue} ${label}` : label;
    return value;
  }
}