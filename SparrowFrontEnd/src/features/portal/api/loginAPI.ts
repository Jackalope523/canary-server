import axios from 'axios';
import { initialiseAxiosSession } from '../../../lib/axios';

export async function login(phoneNumber: string) {
    await axios.post('/account/login', { 'phoneNumber': phoneNumber })
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch((error) => {
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
    });
}

export async function verify(phoneNumber: string, code: string) {
    await axios.post('/account/verify', { 'phoneNumber': phoneNumber, 'code': code })
    .then((response) => {
        // Store cookie
        // initialiseAxiosSession();
    })
    .catch((error) => {
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
    });
}