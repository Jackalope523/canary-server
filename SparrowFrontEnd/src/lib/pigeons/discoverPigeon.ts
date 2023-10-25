import AsyncStorage from '@react-native-async-storage/async-storage';
import { userSession, handleError } from '../axios';
import { eventShard, eventThinSlice } from './eventPigeon';
import { extractUserSilhouette } from './profilePigeon';

const apiBaseUrl = '/discover';

// Get personalized events in the area
export async function getPersonalizedEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/${latitude}-${longitude}-${distance}`)
        .then((response) => {
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

            return Promise.resolve(events);
        })
        .catch(handleError);
}

// Get all events in the area
export async function getAllEvents(latitude: number, longitude: number, distance: number) {
    return await userSession.get(`${apiBaseUrl}/all/${latitude}-${longitude}-${distance}`)
        .then((response) => {
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

            return Promise.resolve(events);
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