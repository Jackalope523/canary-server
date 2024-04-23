import { userSession, handleError, ratingType, extractDate, extractList } from '../../lib/axios';
import { character, extractCharacter } from '../auth/accountPigeon';
import { extractUserSilhouetteManifest, userSilhouetteManifest } from '../profile/profilePigeon';
import { point, Point, Feature } from '@turf/turf';

const apiBaseUrl = '/event';

export enum eventState { upcoming, active_open, active_closed, ended };
export enum eventBond { watching, guest, arrived, left, kicked };

export type eventManifest = {
    Id: number,
    Host: userSilhouetteManifest,
    Name: string,
    Description: string,
    StartTime: Date,
    Location: Feature<Point>,
    Radius: number,
    TimeEnded?: Date,
    State: eventState,
    GroupMinimum: number,
    GroupMaximum: number,
    NumberOfGuests: number
};

export function extractEventManifest(data: any) {
    let event: eventManifest = {
        Id: data['id'],
        Host: extractUserSilhouetteManifest(data['host']),
        Name: data['name'],
        Description: data['description'],
        StartTime: extractDate(data['startTime']),
        Location: point([data['longitude'], data['latitude']]),
        Radius: data['radius'],
        TimeEnded: data['timeEnded'] ?
            extractDate(data['timeEnded']) : undefined,
        State: data['state'],
        GroupMinimum: data['groupMinimum'],
        GroupMaximum: data['groupMaximum'],
        NumberOfGuests: data['numberOfGuests']
    }

    return event;
}

export type eventHeaderManifest = {
    Id: number,
    Name: string,
    IsActive: string,
    LastActiveTime: Date,
    Latitude: number,
    Longitude: number
}

export function extractEventHeaderManifest(data: any) {
    let header: eventHeaderManifest = {
        Id: data['id'],
        Name: data['eventId'],
        IsActive: data['isActive'],
        LastActiveTime: extractDate(data['lastTimeActive']),
        Latitude: data['latitude'],
        Longitude: data['longitude']
    };

    return header;
}

export type etchingManifest = {
    Id: number,
    EventId: number,
    User: userSilhouetteManifest,
    TimeEtched: Date,
    Ratings: [Positive: number, Negative: number]
};

export function extractEtchingManifest(data: any) {
    let etching: etchingManifest = {
        Id: data['id'],
        EventId: data['eventId'],
        User: extractUserSilhouetteManifest(data['user']),
        TimeEtched: extractDate(data['timeEtched']),
        Ratings: [data['ratings']['positive'],
            data['ratings']['negative']]
    };

    return etching;
}

//////
// Event Flow
////////////////

// Get event details
export async function getEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.get('event/${eventID}')
        .then((response: any) => {
            console.log('Event Details:', response.data);

            return extractEventManifest(response.data);
        })
        .catch(handleError);
}

export type eventCreationDetails = {
    EventName: string,
    EventDescription: string,
    Latitude: number,
    Longitude: number,
    StartTime: Date,
    Radius: number,
    IsDynamic: boolean,
    GroupMinimum?: number,
    GroupMaximum?: number,
    IsOpen?: boolean
};

// Create event
export async function createEvent(details: eventCreationDetails) {
    if (!details) {
        console.log('EventDetails are missing.');
        return Promise.reject();
    }

    // TODO details.StartTime = details.StartTime.toISOString();

    return await userSession.post(`${apiBaseUrl}/`, details)
        .then((response: any) => {
            console.log('Event Created:', response.data);

            return extractEventManifest(response.data);
        })
        .catch(handleError);
}

// Edit event
export async function editEvent(eventID: number, details: eventCreationDetails) {
    if (!eventID || !details) {
        console.log('EventID or Details are missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/edit`, details)
        .then(() => {
            console.log('Event Edited Successfully');
        })
        .catch(handleError);
}

// Start event
export async function startEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${eventID}/start`)
        .then(() => {
            console.log('Event Started Successfully');
        })
        .catch(handleError);
}

// End event
export async function endEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.delete(`${apiBaseUrl}/${eventID}/edit`)
        .then(() => {
            console.log('Event Ended Successfully');
        })
        .catch(handleError);
}

// Delete event
export async function deleteEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.delete(`${apiBaseUrl}/${eventID}`)
        .then(() => {
            console.log('Event Deleted Successfully');
        })
        .catch(handleError);
}

// Watch event
export async function watchEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/watch`)
        .then(() => {
            console.log('Joined Event Successfully');
        })
        .catch(handleError);
}

// Unwatch event
export async function unwatchEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}/watch`)
        .then(() => {
            console.log('Joined Event Successfully');
        })
        .catch(handleError);
}

// Join event
export async function joinEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}`)
        .then(() => {
            console.log('Joined Event Successfully');
        })
        .catch(handleError);
}

// Leave event
export async function leaveEvent(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}`)
        .then(() => {
            console.log('Left Event Successfully');
        })
        .catch(handleError);
}

// Get the guest list for an event
export async function getGuestList(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${eventID}/guests`)
    .then((response: any) => {

        let watchers: number = response.data['watchers'];
        let guestCount: number = response.data['guestCount'];
        let guests = extractList(response.data, datum => [extractUserSilhouetteManifest(datum[0]), eventBond[datum[1]]]);

        return { watchers, guestCount, guests };
    })
        .catch(handleError);
}

// Get a list of users who may be invited to an event
export async function getPotentialInvitees(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${eventID}/invite`)
        .then((response: any) => {
            return extractList(response.data, extractUserSilhouetteManifest);
        })
        .catch(handleError);
}

// Invite user to event
export async function inviteUser(eventID: number, targetID: number) {
    if (!eventID || !targetID) {
        console.log('EventID or TargetID is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/invite`, { targetID })
        .then(() => {
            console.log('Invited User Successfully');
        })
        .catch(handleError);
}

// Kick user from event
export async function kickUser(eventID: number, targetID: number) {
    if (!eventID || !targetID) {
        console.log('EventID or TargetID is missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}/guests`, { targetID })
        .then(() => {
            console.log('Kicked User Successfully');
        })
        .catch(handleError);
}

///////
// Report Flow
/////////////////

export enum eventReportType { inappropriate, spam, misleading, promotion };

export type eventReport = {
    ReportType: eventReportType,
    ReportDetails: string
};

// Report event
export async function reportEvent(eventID: number, hostID: number, report: eventReport) {
    if (!eventID || !hostID || !report) {
        console.log('EventID, HostID, or Report are missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/report`, { hostID, report })
        .then(() => {
            console.log('Event Reported Successfully');
        })
        .catch(handleError);
}

////////
// Etching Flow
//////////////////

// Get event etchings
export async function getEventEtchings(eventID: number) {
    if (!eventID) {
        console.log('EventID is missing.');
        return Promise.reject();
    }

    return await userSession.get(`${apiBaseUrl}/${eventID}/etchings`)
        .then((response: any) => {
            console.log('Event Etchings:', response.data);

            return extractList(response.data, extractEtchingManifest);
        })
        .catch(handleError);
}

export type eventEtching = {
    ImageURL: string
};

// Add etching to event
export async function etchIntoEvent(eventID: number, etching: eventEtching) {
    if (!eventID || !etching) {
        console.log('EventID or Etching are missing.');
        return Promise.reject();
    }

    var data = new FormData();
    data.append('image',
      {
         uri: etching.ImageURL,
         name:'etching.jpg',
         type:'image/jpg'
      });

    return await userSession.post(`${apiBaseUrl}/${eventID}/etchings`, data)
        .then((response: any) => {
            console.log('Etching Added to Event:', response.data);

            return extractEtchingManifest(response.data);
        })
        .catch(handleError);
}

// Remove etching
export async function removeEtching(eventID: number, etchingID: number) {
    if (!eventID || !etchingID) {
        console.log('EventID or EtchingID are missing.');
        return Promise.reject();
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}/etchings`, etchingID)
        .then(() => {
            console.log('Etching Removed Successfully');
        })
        .catch(handleError);
}

// Rate etching
export async function rateEtching(eventID: number, etchingID: number, rating: ratingType) {
    if (!eventID || !etchingID || !rating) {
        console.log('EventID, EtchingID, or Rating is missing.');
        return Promise.reject();
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/etchings/${etchingID}`, { 'Rating': rating })
        .then(() => {
            console.log('Etching Rated Successfully');
        })
        .catch(handleError);
}