import axios, { AxiosInstance } from 'axios';

const API_URL = 'https://hollow.azurewebsites.net';

export function initialiseAxios() {
    axios.defaults.baseURL = API_URL;
    axios.defaults.headers.post['Content-Type'] = 'application/json';
    axios.defaults.headers.put['Content-Type'] = 'application/json';
    axios.defaults.validateStatus = (status: any) => { return status === 200 };
}

export var userSession: AxiosInstance = axios.create({
    baseURL: API_URL,
    headers: {'Content-Type': 'application/json'},
    validateStatus: (status: any) => { return status === 200 },
});

export function initialiseAxiosSession(token: string) {
    userSession = axios.create({
        baseURL: API_URL,
        headers: {'Content-Type': 'application/json'},
        validateStatus: (status: any) => { return status === 200 },
    });
}

export function handleError(error: any) {
    console.log(error.toJSON());
    if (error.response) {
        console.log(error.response.data);
        console.log(error.response.status);
        console.log(error.response.headers);
    } else if (error.request) {
        console.log(error.request);
    } else {
        console.log('Axios call failed, error', error.message);
    }

    return Promise.reject();
}

export enum ratingType { 'Positive', 'Negative', 'Remove' };

export function extractDate(data: any) {
    return new Date(data);
}

export function extractList<T>(listData: any, extractingFunction: (data: any) => T) {
    let items: T[] = [];

    for (const datum of listData)
    {
        items.push(extractingFunction(datum));
    }

    return items;
}