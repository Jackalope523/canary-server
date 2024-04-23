import { userSession, handleError } from './axios';

const apiBaseUrl = '/banner';

// Invite a user to Sparrow
export async function inviteUser(userPhoneNumber: string) {
    return await userSession.post(`${apiBaseUrl}/${userPhoneNumber}`)
        .then((response: any) => {
            console.log(`Invited user ${userPhoneNumber}. ${response.data}`);
        })
        .catch(handleError);
}