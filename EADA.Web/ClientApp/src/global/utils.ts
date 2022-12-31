import { HttpErrorResponse, HttpResponse, HttpResponseBase } from "@angular/common/http";

export function baseUrl():string{
    let base = '';

    if(window.location.origin){
        base = window.location.origin;
    }
    else{
        base = window.location.protocol + '//' + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
    }
    return base.replace(/\/$/,'');
};

export function getResponseBody(response: HttpResponseBase) : any{
    if(response instanceof HttpResponse){
        return response.body;
    }

    if(response instanceof HttpErrorResponse){
        return response.error || response.message || response.statusText
    }
};

export const newId: () => string = () => {
    return 'xxxxxxxxxx-xxxx-4xxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c){
        let r = (Math.random() * 16) | 0,
        v = c === 'x' ? r : (r & 0x3) | 0x8;
        return v.toString(16);
    });
};

export const assignExcept:<
Target extends object,
Source extends Partial<Target>
>(
    target: Target,
    source: Source,
    ...excludeKeys: (keyof Target | keyof Source)[]
) => void = (target, source, ...excludeKeys): void => {
    if((excludeKeys?.length ?? 0)< 1){
        Object.assign(target,source);
        return;
    }
    forin(source, (name) => {
        if(excludeKeys.indexOf(name) < 0){
            let targetIndex = Object.keys(target);
            let sourceIndex = Object.keys(source);
            targetIndex[name] = sourceIndex[name];
        }
    });
};

export const assignMatchesExcept:<
Target extends Object,
Source extends Partial<Target>
>(
    target: Target,
    source: Source,
    ...excludedKeys: (keyof Target | keyof Source)[]
) => void = (target,source, ...excludedKeys) : void => {
    const hasExclusions = !!(excludedKeys && excludedKeys.length);

    forin(source, (name) => {
        const propExcluded = (hasExclusions && excludedKeys.includes(name));
        if(!propExcluded && target.hasOwnProperty(name)){
            target[name] = source[name]; 
        }
    })
}

export const forin: <TObject extends object>(
    o: TObject,
    fn:(p:any) => void
) => void = (o,fn) => {
    for(const k in o){
        if(o.hasOwnProperty(k)){
            fn(k);
        }
    }
};