import axios, { AxiosInstance } from 'axios';

const API_URL = 'http://localhost:28721';

export function initialiseAxios() {
    axios.defaults.baseURL = API_URL;
    axios.defaults.headers.post['Content-Type'] = 'application/json';
    axios.defaults.headers.put['Content-Type'] = 'application/json';
    axios.defaults.validateStatus = (status) => { return status === 200 };
}

export var userSession: AxiosInstance;

export function initialiseAxiosSession(token: string) {
    userSession = axios.create({
        baseURL: API_URL,
        headers: {'Content-Type': 'application/json'},
        validateStatus: (status) => { return status === 200 },
    });
}
