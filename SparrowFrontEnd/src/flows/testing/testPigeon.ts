import { userSession, handleError } from '../../lib/axios';
import { userShard } from '../auth/accountPigeon';
import { eventShard } from '../event/eventPigeon';

const apiBaseUrl = '/???????';

export async function seedDatabase(users:userShard[], events:eventShard[], attendance:[number,number][], follows:[number,number][], blocks:[number,number][]) {
    return await userSession.post(`${apiBaseUrl}/???????`, 
    {
        users: users,
        events: events,
        attendance: attendance, 
        blocks: blocks, 
        follows: follows, 
    })
    .then((response: any) => 
    {
        console.log('User Profile:', response.data);
    })
    .catch(handleError);
}


