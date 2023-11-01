import { userSession, handleError } from '../../lib/axios';
import { eventShard, eventThinSlice } from '../event/eventPigeon';
import { extractUserSilhouette } from '../profile/profilePigeon';

const apiBaseUrl = '/discover';

// Get personalized events in the area
export async function getPersonalizedEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/${latitude}-${longitude}-${distance}`)
        .then((response: any) => {
            console.log('Personalized Events:', response.data);

            let events: eventThinSlice[] = [];

            for (const event of response.data)
            {
                events.push({
                    Id: event['Id'],
                    Host: extractUserSilhouette(event['Host']),
                    Latitude: event['Latitude'],
                    Longitude: event['Longitude']
                });
            }

            return events;
        })
        .catch(handleError);
}

// Get all events in the area
export async function getAllEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/all/${latitude}-${longitude}-${distance}`)
        .then((response: any) => {
            console.log('All Events:', response.data);
            
            let events: eventThinSlice[] = [];

            for (const event of response.data)
            {
                events.push({
                    Id: event['Id'],
                    Host: extractUserSilhouette(event['Host']),
                    Latitude: event['Latitude'],
                    Longitude: event['Longitude']
                });
            }

            return events;
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