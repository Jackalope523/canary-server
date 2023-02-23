import { userSession } from '../../../lib/axios';
import { EventModel } from '../../../components/EventSegment';

export async function getNearbyEvents() {
    
    let eventList: EventModel[] = [];

    await userSession.get('/discover/0-0-0')
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);

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
    .catch((error) => {
        console.log(error.toJSON());
        if (error.response) {
            console.log(error.response.data);
            console.log(error.response.status);
            console.log(error.response.headers);
      } else if (error.request) {
            console.log(error.request);
      } else {
            console.log('Axios call failed, error', error.message);
      }

      return Promise.reject();
    });
    
    return Promise.resolve(eventList);
}