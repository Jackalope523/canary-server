import { userSession, handleError, ratingType, extractDate } from '../../lib/axios';
import { extractCharacter } from '../auth/accountPigeon';
import { etchingShard, eventShard, eventThinSlice } from '../event/eventPigeon';

const apiBaseUrl = '/notifications';

export type noteShard = {
    NotifierId: number,
    Time: Date,
    Message: string,
    Action: string
};

///////
// Notification Flow
//////////////////

// Get notes for user
export async function getNotes() {
    return await userSession.get(`${apiBaseUrl}`)
        .then((response: any) => {
            let notes: noteShard[] = [];
            
            for (const note of response.data)
            {
                notes.push({
                    NotifierId: response.data['NotifierId'],
                    Time: extractDate(response.data['Time']),
                    Message: response.data['Message'],
                    Action: response.data['Action']
                });
            }

            return notes;
        })
        .catch(handleError);
}