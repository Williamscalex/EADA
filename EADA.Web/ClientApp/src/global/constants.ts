export const DEFAULT_UOM_ID:number = 1 as const;
export const DEFAULT_PAGE_SIZE:number = 25 as const;

export const DB_FIELD_ARRAY = ['pageSize'] as const;
export type DbKeyName = typeof DB_FIELD_ARRAY[number];
export const Db_KEYS:{[k in DbKeyName]:string} = {
    pageSize: 'pageSize'
} as const;

export const ERROR_RECOURSE_MESSAGE = 'If you continue to encounter this error, please contact your system administrator for further assistance.';
export const MODEL_ERROR_KEY = 'server-validation';