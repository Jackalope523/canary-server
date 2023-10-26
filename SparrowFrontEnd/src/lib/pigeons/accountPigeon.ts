import AsyncStorage from '@react-native-async-storage/async-storage';
import axios, { AxiosResponse } from 'axios';
import { handleError, initialiseAxiosSession, extractDate } from '../axios';

const apiBaseUrl = '/account';

export enum accountStatus { active, active_no_host, active_limited, inactive_under_review, blacklisted };

export type character = {
    Extraversion: number,
    Athleticism: number,
    Chaoticness: number,
    Competitiveness: number,
    Industriousness: number,
    NightOwl: number,
    Openness: number
};

export function extractCharacter(data: any) {
    let character: character = {
        Extraversion: data['Extraversion'],
        Athleticism: data['Athleticism'],
        Chaoticness: data['Chaoticness'],
        Competitiveness: data['Competitiveness'],
        Industriousness: data['Industriousness'],
        NightOwl: data['NightOwl'],
        Openness: data['Openness'],
    }

    return character;
}

export type userShard = {
    Id: string,
    PhoneNumber: string,
    Email: string,
    Name: string,
    DateOfBirth: Date,
    IsPhoneConfirmed: boolean,
    IsEmailConfirmed: boolean,
    SecurityStamp: string,
    LockoutDate?: Date,
    AccessTries: number,
    AccountStatus: accountStatus,
    JoinDate: Date,
    Reputation: number,
    NumberOfFollowers: number,
    Character: character
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
    if (!credentials) {
        return console.log('Credentials are missing.');
    }

    return await axios.post(`${apiBaseUrl}/login`, credentials)
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
    if (!details) {
        return console.log('Sign up details are missing.');
    }

    // TODO details.DateOfBirth = details.DateOfBirth.toISOString();
    return await axios.post(`${apiBaseUrl}/signup`, details)
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch(handleError);
}

// Verify login code
export async function verify(credentials: accountCredentials) {
    if (!credentials) {
        return console.log('Credentials are missing.');
    }

    return await axios.post(`${apiBaseUrl}/verify`, credentials)
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
    return await axios.post(`${apiBaseUrl}/logout`)
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
  return await axios.get(`${apiBaseUrl}`)
    .then((response) => {
        console.log('Account Details:', response.data);
        
        let user: userShard = {
            Id: response.data['Id'],
            PhoneNumber: response.data['PhoneNumber'],
            Email: response.data['Email'],
            Name: response.data['Name'],
            DateOfBirth: extractDate(response.data['DateOfBirth']),
            IsPhoneConfirmed: response.data['IsPhoneConfirmed'],
            IsEmailConfirmed: response.data['IsEmailConfirmed'],
            SecurityStamp: response.data['SecurityStamp'],
            LockoutDate: response.data['LockoutDate'] ?
                extractDate(response.data['LockoutDate']) : undefined,
            AccessTries: response.data['AccessTries'],
            AccountStatus: response.data['AccountStatus'],
            JoinDate: extractDate(response.data['JoinDate']),
            Reputation: response.data['Reputation'],
            NumberOfFollowers: response.data['NumberOfFollowers'],
            Character: extractCharacter(response.data['Character']),
        }

        return Promise.resolve(user);
    })
    .catch(handleError);
}

export type accountDetails = {
  Name: string
};

// Modify Account
export async function modifyAccount(details: accountDetails) {
    if (!details) {
        return console.log('Modified details are missing.');
    }

  return await axios.put(`${apiBaseUrl}`, details)
    .then((response) => {
            console.log('Account modified.', response.data);
    })
    .catch(handleError);
}