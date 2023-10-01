import { Injectable } from "@angular/core";
import { BehaviorSubject, tap, shareReplay } from "rxjs";
import { LoggerService } from "./logger.service";

@Injectable({
    providedIn: 'root'
  })
  export class RefreshService {
  
    constructor(
      private logger: LoggerService
    ) { }
  
    private refreshSubject = new BehaviorSubject<void>(undefined);
  
    public refresh$ = this.refreshSubject.asObservable().pipe(
      tap(() => this.logger.warn('Refreshing all data!')),
      shareReplay(1)
    );
  
    public refreshData() {
      this.refreshSubject.next();
    }
  }