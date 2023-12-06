import { userSession, handleError, ratingType, extractDate, extractList } from '../../lib/axios';
import { extractCharacter } from '../auth/accountPigeon';
import { etchingShard, eventShard, eventThinSlice, extractEtchingShard, extractEventShard, extractEventThinSlice } from '../event/eventPigeon';

const apiBaseUrl = '/profile';

export type userProfile = {
    Id: number,
    Name: string,
    Reputation: number,
    NumberOfFollowers: number
};

export function extractUserProfile(data: any) {
    let profile: userProfile = {
        Id: data['Id'],
        Name: data['Name'],
        Reputation: data['Reputation'],
        NumberOfFollowers: data['NumberOfFollowers']
    }

    return profile;
}

export type userSilhouette = {
    Id: number,
    Name: string
};

export function extractUserSilhouette(data: any) {
    let silhouette: userSilhouette = {
        Id: data['Id'],
        Name: data['Name']
    }

    return silhouette;
}

///////
// Profile Flow
//////////////////

// Get user profile
export async function getUserProfile(targetIdentification: number) {
    if (!targetIdentification) {
        console.log('Target identification is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${targetIdentification}`)
        .then((response: any) => {
            console.log('User Profile:', response.data);

            return extractUserProfile(response.data);
        })
        .catch(handleError);
}

// Get user nest
export async function getUserNest(targetIdentification: number) {
    if (!targetIdentification) {
        console.log('Target identification is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${targetIdentification}/nest`)
        .then((response: any) => {
            console.log('User Nest:', response.data);
            
            let events = extractList(response.data['Events'], extractEventThinSlice);
            let etchings = extractList(response.data['Etchings'], extractEtchingShard);

            return [ events, etchings ];
        })
        .catch(handleError);
}

// Rate a user
export async function rateUser(targetIdentification: number, rating: ratingType) {
    if (!targetIdentification || !rating) {
        console.log('Target identification or rating is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${targetIdentification}`, { 'Rating': rating })
        .then(() => {
            console.log('User Rated Successfully');
        })
        .catch(handleError);
}

// Get user activity
export async function getUserActivity(targetIdentification: number) {
    if (!targetIdentification) {
        console.log('Target identification is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${targetIdentification}/activity`)
        .then((response: any) => {
            console.log('User Activity:', response.data);
            
            return extractList(response.data, extractEventShard);
        })
        .catch(handleError);
}

// Get friend activity
export async function getFriendActivity() {
    return await userSession.get(`${apiBaseUrl}/activity`)
        .then((response: any) => {
            console.log('Friend Activity:', response.data);
            
            let users: userSilhouette[] = [];
            let activity: { [id: number]: eventShard[] } = {};

            for (const pair of response.data)
            {
                users.push(extractUserSilhouette(pair[0]));

                let events = extractList(pair[1], extractEventShard);

                activity[pair[0]['Id']] = events;
            }

            return [ users, activity ];
        })
        .catch(handleError);
}

// Get followed users
export async function getFollowedUsers() {
    return await userSession.get(`${apiBaseUrl}/following`)
        .then((response: any) => {
            console.log('Followed Users:', response.data);
            
            return extractList(response.data, extractUserSilhouette);
        })
        .catch(handleError);
}

// Follow a user
export async function followUser(targetID: number) {
    if (!targetID) {
        console.log('Target ID is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/following`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Followed Successfully');
        })
        .catch(handleError);
}

// Unfollow a user
export async function unfollowUser(targetID: number) {
    if (!targetID) {
        console.log('Target ID is missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/following`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Unfollowed Successfully');
        })
        .catch(handleError);
}

// Get blocked users
export async function getBlockedUsers() {
    return await userSession.get(`${apiBaseUrl}/blocked`)
        .then((response: any) => {
            console.log('Blocked Users:', response.data);
            
            return extractList(response.data, extractUserSilhouette);
        })
        .catch(handleError);
}

// Block a user
export async function blockUser(targetID: number) {
    if (!targetID) {
        console.log('Target ID is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/blocked`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Blocked Successfully');
        })
        .catch(handleError);
}

// Unblock a user
export async function unblockUser(targetID: number) {
    if (!targetID) {
        console.log('Target ID is missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/blocked`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Unblocked Successfully');
        })
        .catch(handleError);
}

export enum userReportType { rude, hate_speech, harassment, violent_behaviour,
    physical_assault, sexual_assault }

export type userReport = {
    ReportType: userReportType,
    ReportDetails: string
};

// Report a user
export async function reportUser(targetID: number, report: userReport) {
    if (!targetID || !report) {
        console.log('Target ID or report is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${targetID}/report`, report)
        .then(() => {
            console.log('User Reported Successfully');
        })
        .catch(handleError);
}