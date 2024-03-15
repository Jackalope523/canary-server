import { userSession, handleError, extractList } from '../../lib/axios';
import { eventShard, extractEventShard } from '../event/eventPigeon';

const apiBaseUrl = '/discover';

// Get personalized events in the area
export async function getPersonalizedEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/${latitude}-${longitude}-${distance}`)
        .then((response: any) => {
            console.log('Personalized Events:', response.data);

            return extractList(response.data, extractEventShard);
        })
        .catch(handleError);
}

// Get all events in the area
export async function getAllEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/all/${latitude}-${longitude}-${distance}`)
        .then((response: any) => {
            console.log('All Events:', response.data);
            
            return extractList(response.data, extractEventShard);
        })
        .catch(handleError);
}

// Update user's current position
// Ignore this method for the moment
export async function updateCurrentPosition(latitude: number, longitude: number) {
    return await userSession.post(`${apiBaseUrl}/user/${latitude}-${longitude}`)
        .then(() => {
            console.log('User Position Updated Successfully');
        })
        .catch(handleError);
}