import AsyncStorage from '@react-native-async-storage/async-storage';
import { userSession, handleError } from '../axios';

const apiBaseUrl = '/discover';

// Get personalized events in the area
export async function getPersonalizedEvents(latitude: number, longitude: number, distance: number) {
    await userSession.get(`${apiBaseUrl}/${latitude}-${longitude}-${distance}`)
        .then((response) => {
            console.log('Personalized Events:', response.data);

            for (const event of response.data[''])
            {
                eventList.push({
                    id: '',
                    host: { id: '', name: '' },
                    name: response.data['name'],
                    description: '',
                    type: '',
                    startTime: new Date(),
                    position: { latitude: 0, longitude: 0 },
                    numberAttendees: 0
                });
            }
        })
        .catch(handleError);
}

// Get all events in the area
export async function getAllEvents(latitude: number, longitude: number, distance: number) {
    await userSession.get(`${apiBaseUrl}/all/${latitude}-${longitude}-${distance}`)
        .then((response) => {
            console.log('All Events:', response.data);
        })
        .catch(handleError);
}

// Update user's current position
// Ignore this method for the moment
export async function updateCurrentPosition(latitude: number, longitude: number) {
    await userSession.post(`${apiBaseUrl}/user/${latitude}-${longitude}`)
        .then(() => {
            console.log('User Position Updated Successfully');
        })
        .catch(handleError);
}