import { userSession, handleError, ratingType, extractDate, extractList } from '../../lib/axios';
import { extractCharacter } from '../auth/accountPigeon';
import { etchingShard, eventShard, eventThinSlice } from '../event/eventPigeon';

const apiBaseUrl = '/notifications';

export type noteShard = {
    NotifierId: number,
    Time: Date,
    Message: string,
    Action: string
};

export function extractNoteShard(data: any) {
    let note: noteShard = {
        NotifierId: data['NotifierId'],
        Time: extractDate(data['Time']),
        Message: data['Message'],
        Action: data['Action']
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