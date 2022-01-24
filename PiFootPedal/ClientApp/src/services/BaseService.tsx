export default class BaseService {

    protected get<T>(url: string) : Promise<T>{
        return fetch(url).then(res => {
            if(!res.ok){
                throw new Error(res.statusText);
            }
            return res.json() as Promise<T>;
        });
    }

    protected postData<TRequest, TResponse>(url: string, data: TRequest | null = null) : Promise<TResponse>{
        return fetch(url, {
            method: 'post',
            mode: "cors",
            body: JSON.stringify(data),
            headers: { 'Content-Type': 'application/json' }
        }).then(res => {
            if(!res.ok){
                throw new Error(res.statusText);
            }
            return res.json() as Promise<TResponse>;
        });
    }

    protected post<T>(url: string) : Promise<T>{
        return this.postData<null, T>(url);
    }
}