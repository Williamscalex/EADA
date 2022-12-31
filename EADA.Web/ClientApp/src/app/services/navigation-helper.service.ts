import { Injectable } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { ReplaySubject, Subject } from "rxjs";
import { filter, map } from "rxjs/operators";

@Injectable({providedIn: 'root'})
export class NavigationHelperService {
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute
    ){
        this. router.events
        .pipe(
            filter(e => e instanceof NavigationEnd),
            map(() => this.activatedRoute),
            map((route) => {
                while(route.firstChild) route = route.firstChild;
                return route;
            }),
            filter(route => route.outlet === 'primary')
        )
        .subscribe(route => this.primaryNavEndSubject.next(route));
    }

    private primaryNavEndSubject = new Subject<ActivatedRoute>();

    private connectionErrorSubject = new ReplaySubject(1);
    connectionError$ = this.connectionErrorSubject.asObservable();

    reportConnectionError(){
        this.connectionErrorSubject.next;
    }

}