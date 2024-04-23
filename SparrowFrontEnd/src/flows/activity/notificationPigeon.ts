import { userSession, handleError, ratingType, extractDate, extractList } from '../../lib/axios';
import { extractCharacter } from '../auth/accountPigeon';
import { etchingManifest, eventManifest } from '../event/eventPigeon';

const apiBaseUrl = '/notifications';

export type noteManifest = {
    NotifierId: number,
    Time: Date,
    Message: string,
    Action: string
};

export function extractNoteShard(data: any) {
    let note: noteManifest = {
        NotifierId: data['notifierId'],
        Time: extractDate(data['time']),
        Message: data['message'],
        Action: data['action']
    }

    return note;
}

///////
// Notification Flow
//////////////////

// Get notes for user
export async function getNotes() {
    return await userSession.get(`${apiBaseUrl}`)
        .then((response: any) => {
            return extractList(response.data, extractNoteShard);
        })
        .catch(handleError);
}