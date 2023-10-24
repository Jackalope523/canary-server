import { userSession, handleError, ratingType } from '../axios';

const apiBaseUrl = '/event';

export type eventShard = {

};

export type etchingShard = {

};

////////////////
// Event Flow //
////////////////

// Get event details
export async function getEvent(eventID: string) {
    if (!eventID) {
        return console.log('EventID is missing.');
    }

    await userSession.get('event/${eventID}')
        .then((response) => {
            console.log('Event Details:', response.data);
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

    await userSession.post(`${apiBaseUrl}/`, details)
        .then((response) => {
            console.log('Event Created:', response.data);
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

    await userSession.post(`${apiBaseUrl}/${eventID}/edit`, details)
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

    await userSession.delete(`${apiBaseUrl}/${eventID}/edit`)
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

    await userSession.post(`${apiBaseUrl}/${eventID}`)
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

    await userSession.put(`${apiBaseUrl}/${eventID}`)
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

    await userSession.post(`${apiBaseUrl}/${eventID}/report`, { hostID, report })
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

    await userSession.get(`${apiBaseUrl}/${eventID}/etchings`)
        .then((response) => {
            console.log('Event Etchings:', response.data);
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

    await userSession.post(`${apiBaseUrl}/${eventID}/etchings`, etching)
        .then((response) => {
            console.log('Etching Added to Event:', response.data);
        })
        .catch(handleError);
}

// Remove etching
export async function removeEtching(eventID: string, etchingID: string) {
    if (!eventID || !etchingID) {
        return console.log('EventID or EtchingID are missing.');
    }

    await userSession.put(`${apiBaseUrl}/${eventID}/etchings`, etchingID)
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

    await userSession.post(`${apiBaseUrl}/${eventID}/etchings/${etchingID}`, { 'Rating': rating })
        .then(() => {
            console.log('Etching Rated Successfully');
        })
        .catch(handleError);
}