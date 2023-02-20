import axios from 'axios';

export async function login(phoneNumber: string) {
    await axios.post('/account/login', { 'phoneNumber': phoneNumber })
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
        if (response.status === 200) {
            return true;
        }
        return false;
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

export async function verifyLogin(phoneNumber: string, code: string) {
    await axios.post('/account/verify', { 'phoneNumber':phoneNumber, 'code': code })
    .then((response) => {
        if (response.status === 200) {
            return true;
        }
        return false;
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