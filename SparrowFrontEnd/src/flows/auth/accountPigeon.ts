import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { handleError, initialiseAxiosSession, extractDate, userSession } from '../../lib/axios';

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
    Id: number,
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

export function extractUserShard(data: any) {
    let user: userShard = {
        Id: data['Id'],
        PhoneNumber: data['PhoneNumber'],
        Email: data['Email'],
        Name: data['Name'],
        DateOfBirth: extractDate(data['DateOfBirth']),
        IsPhoneConfirmed: data['IsPhoneConfirmed'],
        IsEmailConfirmed: data['IsEmailConfirmed'],
        SecurityStamp: data['SecurityStamp'],
        LockoutDate: data['LockoutDate'] ?
            extractDate(data['LockoutDate']) : undefined,
        AccessTries: data['AccessTries'],
        AccountStatus: data['AccountStatus'],
        JoinDate: extractDate(data['JoinDate']),
        Reputation: data['Reputation'],
        NumberOfFollowers: data['NumberOfFollowers'],
        Character: extractCharacter(data['Character']),
    }

    return user;
}

/////////
// Account login flow
////////////////////////

export type accountCredentials = {
  PhoneNumber: string,
  Code?: string
};

// Login
export async function login(credentials: accountCredentials) {
    if (!credentials) {
        console.log('Credentials are missing.');
        return Promise.reject();
    }

    return await axios.post(`${apiBaseUrl}/login`, credentials)
    .then((response: any) => {
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
        console.log('Sign up details are missing.');
        return Promise.reject();
    }

    // TODO details.DateOfBirth = details.DateOfBirth.toISOString();
    return await axios.post(`${apiBaseUrl}/signup`, details)
    .then((response: any) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch(handleError);
}

// Verify login code
export async function verify(credentials: accountCredentials) {
    if (!credentials) {
        console.log('Credentials are missing.');
        return Promise.reject();
    }

    return await axios.post(`${apiBaseUrl}/verify`, credentials)
    .then((response: any) => {
        
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

// Resend email verification
export async function resendEmailVerification(email: string) {
    if (!email) {
        console.log('Email is missing.');
        return Promise.reject();
    }

    return await axios.post(`${apiBaseUrl}/email`, email)
    .then((response: any) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch(handleError);
}

// Logout
export async function logout() {
    return await axios.get(`${apiBaseUrl}/logout`)
    .then((response: any) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);
    })
    .catch(handleError);
}

////////
// Account use flow
//////////////////////

// Get account details
export async function getAccount() {
  return await userSession.get(`${apiBaseUrl}`)
    .then((response: any) => {
        console.log('Account Details:', response.data);
        
        return extractUserShard(response.data);
    })
    .catch(handleError);
}

export type accountDetails = {
  Name: string
};

// Modify Account
export async function modifyAccount(details: accountDetails) {
    if (!details) {
        console.log('Modified details are missing.');
        return Promise.reject();
    }

  return await userSession.post(`${apiBaseUrl}`, details)
    .then((response: any) => {
            console.log('Account modified.', response.data);
    })
    .catch(handleError);
}