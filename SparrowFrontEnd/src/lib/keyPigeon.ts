import { userSession, handleError } from './axios';

const apiBaseUrl = '/keys';

// Get specific etching image
export async function getSecret(secret: string) {
    return await userSession.get(`${apiBaseUrl}/${secret}`)
        .then((response: any) => {
            console.log(response.data);
        })
        .catch(handleError);
}