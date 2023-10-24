import { userSession, handleError, ratingType } from '../axios';

const apiBaseUrl = '/profile';

//////////////////
// Profile Flow //
//////////////////

// Get user profile
export async function getUserProfile(targetIdentification: string) {
    if (!targetIdentification) {
        return console.log('Target identification is missing.');
    }

    await userSession.get(`${apiBaseUrl}/${targetIdentification}`)
        .then((response) => {
            console.log('User Profile:', response.data);
            
            user = {
                name: response.data['name'],
                numberOfFollowers: response.data['numberOfFollowers'],
                reputation: response.data['reputation']
            }

            Promise.resolve();
        })
        .catch(handleError);
}

// Rate a user
export async function rateUser(targetIdentification: string, rating: ratingType) {
    if (!targetIdentification || !rating) {
        return console.log('Target identification or rating is missing.');
    }

    await userSession.post(`${apiBaseUrl}/${targetIdentification}`, { 'Rating': rating })
        .then(() => {
            console.log('User Rated Successfully');
        })
        .catch(handleError);
}

// Get user activity
export async function getUserActivity(targetIdentification: string) {
    if (!targetIdentification) {
        return console.log('Target identification is missing.');
    }

    await userSession.get(`${apiBaseUrl}/${targetIdentification}/activity`)
        .then((response) => {
            console.log('User Activity:', response.data);
        })
        .catch(handleError);
}

// Get friend activity
export async function getFriendActivity() {
    await userSession.get(`${apiBaseUrl}/activity`)
        .then((response) => {
            console.log('Friend Activity:', response.data);
        })
        .catch(handleError);
}

// Get followed users
export async function getFollowedUsers() {
    await userSession.get(`${apiBaseUrl}/following`)
        .then((response) => {
            console.log('Followed Users:', response.data);
        })
        .catch(handleError);
}

// Follow a user
export async function followUser(targetID: string) {
    if (!targetID) {
        return console.log('Target ID is missing.');
    }

    await userSession.post(`${apiBaseUrl}/following`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Followed Successfully');
        })
        .catch(handleError);
}

// Unfollow a user
export async function unfollowUser(targetID: string) {
    if (!targetID) {
        return console.log('Target ID is missing.');
    }

    await userSession.put(`${apiBaseUrl}/following`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Unfollowed Successfully');
        })
        .catch(handleError);
}

// Get blocked users
export async function getBlockedUsers() {
    await userSession.get(`${apiBaseUrl}/blocked`)
        .then((response) => {
            console.log('Blocked Users:', response.data);
        })
        .catch(handleError);
}

// Block a user
export async function blockUser(targetID: string) {
    if (!targetID) {
        return console.log('Target ID is missing.');
    }

    await userSession.post(`${apiBaseUrl}/blocked`, { 'TargetID': targetID })
        .then(() => {
            console.log('User Blocked Successfully');
        })
        .catch(handleError);
}

// Unblock a user
export async function unblockUser(targetID: string) {
    if (!targetID) {
        return console.log('Target ID is missing.');
    }

    await userSession.put(`${apiBaseUrl}/blocked`, { 'TargetID': targetID })
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
        return console.log('Target ID or report is missing.');
    }

    await userSession.post(`${apiBaseUrl}/${targetID}/report`, report)
        .then(() => {
            console.log('User Reported Successfully');
        })
        .catch(handleError);
}