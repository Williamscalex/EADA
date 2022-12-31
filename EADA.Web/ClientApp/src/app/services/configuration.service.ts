import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import * as utils from '@global/utils';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  constructor() { }

  // **Specify default config here**
  public static readonly defaultLanguage: string = 'en';
  public static readonly defaultHomeUrl: string = '/';
  public static baseUrl:string = environment.baseUrl || utils.baseUrl();

  public readonly baseUrl: string = environment.baseUrl;
  public homeUrl = environment.homeUrl || ConfigurationService.defaultHomeUrl;
  public language = ConfigurationService.defaultLanguage;

  private onConfigurationImported: Subject<boolean> = new Subject<boolean>();
  configurationImported$ = this.onConfigurationImported.asObservable();

  public clearLocalChanges(){

  };
  public import(jsonValue: string){
    this.clearLocalChanges();

    this.onConfigurationImported.next(true);
  };
}
