import AsyncStorage from '@react-native-async-storage/async-storage';
import axios from 'axios';
import { handleError, initialiseAxiosSession } from '../axios';

const apiBaseUrl = '/account';

export type accountShard = {

};

////////////////////////
// Account login flow //
////////////////////////

export type accountCredentials = {
  PhoneNumber: string,
  Code?: string
};

// Login
export async function login(credentials: accountCredentials) {
    await axios.post(`${apiBaseUrl}/login`, credentials)
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch(handleError);
}

export type signUpDetails = {
  PhoneNumber: string,
  Email: string,
  Name: string,
  DateOfBirth: Date
};

// Signup
export async function signup(details: signUpDetails) {
  // TODO details.DateOfBirth = details.DateOfBirth.toISOString();
  await axios.post(`${apiBaseUrl}/signup`, details)
  .then((response) => {
      console.log(response.data);
      console.log(response.status);
      console.log(response.headers);
  })
  .catch(handleError);
}

// Verify login code
export async function verify(credentials: accountCredentials) {
  await axios.post(`${apiBaseUrl}/verify`, credentials)
  .then((response) => {
      // Store cookie
      let tokenCookie = response.headers['set-cookie']?.[0];
      if (typeof  tokenCookie === 'string') {
        tokenCookie = tokenCookie.split(';', 2)[0];
        AsyncStorage.setItem('token', tokenCookie);
        initialiseAxiosSession(tokenCookie);
      }
  })
  .catch(handleError);
}

// Logout
export async function logout() {
  await axios.post(`${apiBaseUrl}/logout`)
  .then((response) => {
    console.log(response.data);
    console.log(response.status);
    console.log(response.headers);
  })
  .catch(handleError);
}

//////////////////////
// Account use flow //
//////////////////////

// Get account details
export async function getAccount() {
  await axios.get(`${apiBaseUrl}`)
      .then((response) => {
          console.log(response.data);
          console.log(response.status);
          console.log(response.headers);
      })
      .catch(handleError);
}

export type accountDetails = {
  Name: string
};

// Modify Account
export async function modifyAccount(name: accountDetails) {
  await axios.put(`${apiBaseUrl}`, name)
      .then((response) => {
          console.log(response.data);
          console.log(response.status);
          console.log(response.headers);
      })
      .catch(handleError);
}