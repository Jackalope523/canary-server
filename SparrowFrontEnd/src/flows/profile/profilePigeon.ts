import { userSession, handleError, ratingType, extractDate } from '../../lib/axios';
import { extractCharacter } from '../auth/accountPigeon';
import { eventShard } from '../event/eventPigeon';

const apiBaseUrl = '/profile';

export type userProfile = {
    Id: string,
    Name: string,
    Reputation: number,
    NumberOfFollowers: number
};

export type userSilhouette = {
    Id: string,
    Name: string
};

export function extractUserSilhouette(data: any) {
    let silhouette: userSilhouette = {
        Id: data['Id'],
        Name: data['Name']
    }

    return silhouette;
}

//////////////////
// Profile Flow //
//////////////////

// Get user profile
export async function getUserProfile(targetIdentification: string) {
    if (!targetIdentification) {
        console.log('Target identification is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${targetIdentification}`)
        .then((response: any) => {
            console.log('User Profile:', response.data);
            
            let profile: userProfile = {
                Id: response.data['Id'],
                Name: response.data['Name'],
                Reputation: response.data['Reputation'],
                NumberOfFollowers: response.data['NumberOfFollowers']
            }

            return profile;
        })
        .catch(handleError);
}

// Rate a user
export async function rateUser(targetIdentification: string, rating: ratingType) {
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
export async function getUserActivity(targetIdentification: string) {
    if (!targetIdentification) {
        console.log('Target identification is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${targetIdentification}/activity`)
        .then((response: any) => {
            console.log('User Activity:', response.data);
            
            let events: eventShard[] = [];

            for (const event of response.data)
            {
                events.push({
                    Id: event['Id'],
                    Host: extractUserSilhouette(event['Host']),
                    Name: event['Name'],
                    Description: event['Description'],
                    StartTime: extractDate(event['StartTime']),
                    Latitude: event['Latitude'],
                    Longitude: event['Longitude'],
                    TimeEnded: event['TimeEnded'] ?
                        extractDate(event['TimeEnded']) : undefined,
                    IsOpen: event['IsOpen'],
                    GroupMinimum: event['GroupMinimum'],
                    GroupMaximum: event['GroupMaximum'],
                    Character: extractCharacter(event['Character'])
                });
            }

            return events;
        })
        .catch(handleError);
}

// Get friend activity
export async function getFriendActivity() {
    return await userSession.get(`${apiBaseUrl}/activity`)
        .then((response: any) => {
            console.log('Friend Activity:', response.data);
            
            let users: userSilhouette[] = [];
            let activity: { [id: string]: eventShard[] } = {};

            for (const pair of response.data)
            {
                users.push(extractUserSilhouette(pair[0]));

                let events: eventShard[] = [];

                for (const event of response.data)
                {
                    events.push({
                        Id: event['Id'],
                        Host: extractUserSilhouette(event['Host']),
                        Name: event['Name'],
                        Description: event['Description'],
                        StartTime: extractDate(event['StartTime']),
                        Latitude: event['Latitude'],
                        Longitude: event['Longitude'],
                        TimeEnded: event['TimeEnded'] ?
                            extractDate(event['TimeEnded']) : undefined,
                        IsOpen: event['IsOpen'],
                        GroupMinimum: event['GroupMinimum'],
                        GroupMaximum: event['GroupMaximum'],
                        Character: extractCharacter(event['Character'])
                    });
                }

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
            
            let users: userSilhouette[] = [];

            for (const user of response.data)
            {
                users.push({
                    Id: user['Id'],
                    Name: user['Name']
                });
            }

            return users;
        })
        .catch(handleError);
}

// Follow a user
export async function followUser(targetID: string) {
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
export async function unfollowUser(targetID: string) {
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
            
            let users: userSilhouette[] = [];

            for (const user of response.data)
            {
                users.push({
                    Id: user['Id'],
                    Name: user['Name']
                });
            }

            return users;
        })
        .catch(handleError);
}

// Block a user
export async function blockUser(targetID: string) {
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
export async function unblockUser(targetID: string) {
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
export async function reportUser(targetID: string, report: userReport) {
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