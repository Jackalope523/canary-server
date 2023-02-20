import axios from 'axios';

export function initialiseAxios() {
    axios.defaults.baseURL = 'http://localhost:28721';
    axios.defaults.headers.post['Content-Type'] = 'application/json';
    axios.defaults.headers.put['Content-Type'] = 'application/json';
}
