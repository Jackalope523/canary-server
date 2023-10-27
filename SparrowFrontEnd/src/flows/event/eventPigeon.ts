import { userSession, handleError, ratingType, extractDate } from '../../lib/axios';
import { character, extractCharacter } from '../auth/accountPigeon';
import { extractUserSilhouette, userSilhouette } from '../profile/profilePigeon';

const apiBaseUrl = '/event';

export type eventShard = {
    Id: string,
    Host: userSilhouette,
    Name: string,
    Description: string,
    StartTime: Date,
    Latitude: number,
    Longitude: number,
    TimeEnded?: Date,
    IsOpen: boolean,
    GroupMinimum: number,
    GroupMaximum: number,
    Character: character
};

export type eventThinSlice = {
    Id: string,
    Host: userSilhouette,
    Latitude: number,
    Longitude: number
};

export type eventHeader = {
    Id: string,
    Name: string,
    IsActive: string,
    LastActiveTime: Date
}

export type etchingShard = {
    Id: string,
    EventId: string,
    UserId: string,
    TimeEtched: Date,
    ImageURL: string,
    Ratings: [Positive: number, Negative: number]
};

////////////////
// Event Flow //
////////////////

// Get event details
export async function getEvent(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    return await userSession.get('event/${eventID}')
        .then((response: any) => {
            console.log('Event Details:', response.data);

            let event: eventShard = {
                Id: response.data['Id'],
                Host: extractUserSilhouette(response.data['Host']),
                Name: response.data['Name'],
                Description: response.data['Description'],
                StartTime: extractDate(response.data['StartTime']),
                Latitude: response.data['Latitude'],
                Longitude: response.data['Longitude'],
                TimeEnded: response.data['TimeEnded'] ?
                    extractDate(response.data['TimeEnded']) : undefined,
                IsOpen: response.data['IsOpen'],
                GroupMinimum: response.data['GroupMinimum'],
                GroupMaximum: response.data['GroupMaximum'],
                Character: extractCharacter(response.data['Character'])
            }

            return Promise.resolve(event);
        })
        .catch(handleError);
}

export type eventCreationDetails = {
    EventName: string,
    EventDescription: string,
    Latitude: number,
    Longitude: number,
    StartTime: Date,
    GroupMinimum?: number,
    GroupMaximum?: number
};

// Create event
export async function createEvent(details: eventCreationDetails) {
    if (!details) {
        return console.log('EventDetails are missing.');
    }

    // TODO details.StartTime = details.StartTime.toISOString();

    return await userSession.post(`${apiBaseUrl}/`, details)
        .then((response: any) => {
            console.log('Event Created:', response.data);

            let event: eventShard = {
                Id: response.data['Id'],
                Host: response.data['Host'],
                Name: response.data['Name'],
                Description: response.data['Description'],
                StartTime: extractDate(response.data['StartTime']),
                Latitude: response.data['Latitude'],
                Longitude: response.data['Longitude'],
                TimeEnded: response.data['TimeEnded'] ?
                    extractDate(response.data['TimeEnded']) : undefined,
                IsOpen: response.data['IsOpen'],
                GroupMinimum: response.data['GroupMinimum'],
                GroupMaximum: response.data['GroupMaximum'],
                Character: extractCharacter(response.data['Character'])
            }

            return Promise.resolve(event);
        })
        .catch(handleError);
}

export type eventEditDetails = {
    EventDescription: string,
    EventIsOpen?: boolean
};

// Edit event
export async function editEvent(eventID: string, details: eventEditDetails) {
    if (!eventID || !details) {
        return console.log('EventID or Details are missing.');
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/edit`, details)
        .then(() => {
            console.log('Event Edited Successfully');
        })
        .catch(handleError);
}

// End event
export async function endEvent(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    return await userSession.delete(`${apiBaseUrl}/${eventID}/edit`)
        .then(() => {
            console.log('Event Ended Successfully');
        })
        .catch(handleError);
}

// Join event
export async function joinEvent(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}`)
        .then(() => {
            console.log('Joined Event Successfully');
        })
        .catch(handleError);
}

// Leave event
export async function leaveEvent(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}`)
        .then(() => {
            console.log('Left Event Successfully');
        })
        .catch(handleError);
}

/////////////////
// Report Flow //
/////////////////

export enum eventReportType { inappropriate, spam, misleading, promotion };

export type eventReport = {
    ReportType: eventReportType,
    ReportDetails: string
};

// Report event
export async function reportEvent(eventID: string, hostID: string, report: eventReport) {
    if (!eventID || !hostID || !report) {
        return console.log('EventID, HostID, or Report are missing.');
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/report`, { hostID, report })
        .then(() => {
            console.log('Event Reported Successfully');
        })
        .catch(handleError);
}

//////////////////
// Etching Flow //
//////////////////

// Get event etchings
export async function getEventEtchings(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    return await userSession.get(`${apiBaseUrl}/${eventID}/etchings`)
        .then((response: any) => {
            console.log('Event Etchings:', response.data);

            let etchings: etchingShard[] = [];

            for (const etching of response.data)
            {
                etchings.push({
                    Id: etching['id'],
                    EventId: etching['EventId'],
                    UserId: etching['UserId'],
                    TimeEtched: extractDate(etching['TimeEtched']),
                    ImageURL: etching['ImageURL'],
                    Ratings: [etching['Ratings']['Positive'],
                        etching['Ratings']['Negative']]
                });
            }

            return Promise.resolve(etchings);
        })
        .catch(handleError);
}

export type eventEtching = {
    ImageURL: string
};

// Add etching to event
export async function etchIntoEvent(eventID: string, etching: eventEtching) {
    if (!eventID || !etching) {
        return console.log('EventID or Etching are missing.');
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/etchings`, etching)
        .then((response: any) => {
            console.log('Etching Added to Event:', response.data);

            let etching: etchingShard = {
                Id: response.data['Id'],
                EventId: response.data['EventId'],
                UserId: response.data['UserId'],
                TimeEtched: extractDate(response.data['TimeEtched']),
                ImageURL: response.data['ImageURL'],
                Ratings: [response.data['Ratings']['Positive'],
                    response.data['Ratings']['Negative']]
            }

            return Promise.resolve(etching);
        })
        .catch(handleError);
}

// Remove etching
export async function removeEtching(eventID: string, etchingID: string) {
    if (!eventID || !etchingID) {
        return console.log('EventID or EtchingID are missing.');
    }

    return await userSession.put(`${apiBaseUrl}/${eventID}/etchings`, etchingID)
        .then(() => {
            console.log('Etching Removed Successfully');
        })
        .catch(handleError);
}

// Rate etching
export async function rateEtching(eventID: string, etchingID: string, rating: ratingType) {
    if (!eventID || !etchingID || !rating) {
        return console.log('EventID, EtchingID, or Details are missing.');
    }

    return await userSession.post(`${apiBaseUrl}/${eventID}/etchings/${etchingID}`, { 'Rating': rating })
        .then(() => {
            console.log('Etching Rated Successfully');
        })
        .catch(handleError);
}