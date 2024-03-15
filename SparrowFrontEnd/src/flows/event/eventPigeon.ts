import { userSession, handleError, ratingType, extractDate, extractList } from '../../lib/axios';
import { character, extractCharacter } from '../auth/accountPigeon';
import { extractUserSilhouette, userSilhouette } from '../profile/profilePigeon';
import { point, Point } from '@turf/helpers';

const apiBaseUrl = '/event';

export enum eventState { upcoming, active_open, active_closed, ended };

export type eventShard = {
    Id: number,
    Host: userSilhouette,
    Name: string,
    Description: string,
    StartTime: Date,
    Latitude: number,
    Longitude: number,
    TimeEnded?: Date,
    State: eventState,
    GroupMinimum: number,
    GroupMaximum: number,
    Character: character,
    Radius: number,
    IsDynamic: boolean,
    NumberOfGuests: number
};

export function extractEventShard(data: any) {
    let event: eventShard = {
        Id: data['Id'],
        Host: extractUserSilhouette(data['Host']),
        Name: data['Name'],
        Description: data['Description'],
        StartTime: extractDate(data['StartTime']),
        Latitude: data['Latitude'],
        Longitude: data['Longitude'],
        TimeEnded: data['TimeEnded'] ?
            extractDate(data['TimeEnded']) : undefined,
        State: data['State'],
        GroupMinimum: data['GroupMinimum'],
        GroupMaximum: data['GroupMaximum'],
        Character: extractCharacter(data['Character']),
        Radius: data['Radius'],
        IsDynamic: data['IsDynamic'],
        NumberOfGuests: data['NumberOfGuests']
    }

    return event;
}

export type eventHeader = {
    Id: number,
    Name: string,
    IsActive: string,
    LastActiveTime: Date,
    Latitude: number,
    Longitude: number
}

export function extractEventHeader(data: any) {
    let header: eventHeader = {
        Id: data['Id'],
        Name: data['EventId'],
        IsActive: data['IsActive'],
        LastActiveTime: extractDate(data['LastTimeActive']),
        Latitude: data['Latitude'],
        Longitude: data['Longitude']
    };

    return header;
}

export type etchingShard = {
    Id: number,
    EventId: number,
    User: userSilhouette,
    TimeEtched: Date,
    ImageURL: string,
    Ratings: [Positive: number, Negative: number],
    IsHidden: boolean
};

export function extractEtchingShard(data: any) {
    let etching: etchingShard = {
        Id: data['id'],
        EventId: data['EventId'],
        User: extractUserSilhouette(data['User']),
        TimeEtched: extractDate(data['TimeEtched']),
        ImageURL: data['ImageURL'],
        Ratings: [data['Ratings']['Positive'],
            data['Ratings']['Negative']],
        IsHidden: data['IsHidden']
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

            return extractEventShard(response.data);
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

            return extractEventShard(response.data);
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
        return extractList(response.data, extractUserSilhouette);
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
            return extractList(response.data, extractUserSilhouette);
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

            return extractList(response.data, extractEtchingShard);
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

    return await userSession.post(`${apiBaseUrl}/${eventID}/etchings`, etching)
        .then((response: any) => {
            console.log('Etching Added to Event:', response.data);

            return extractEtchingShard(response.data);
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