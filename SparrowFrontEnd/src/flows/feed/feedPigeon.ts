import axios, { AxiosHeaders } from 'axios';
import { userSession, handleError, extractDate, extractList } from '../../lib/axios';
import { etchingShard, eventEtching, eventHeader, extractEtchingShard, extractEventHeader } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

type feedOptions = {
    DepthCharge: number,
    LastDepth: number
}

type rawFeed = {
    Depth: number,
    Headers: eventHeader[],
    Etchings: etchingShard[],

}

// Get user feed
export async function getUserFeed(options: feedOptions) : Promise<rawFeed> {
    if (!options) {
        console.log('Feed options are missing.');
        return Promise.reject();
    }
    
    return await userSession.get(`${apiBaseUrl}/${options.DepthCharge}-${options.LastDepth}`)
        .then((response: any) => {
            console.log('User Feed:', response.data);

            let depth: number = response.data['depth'];
            let headers = extractList(response.data['headers'], extractEventHeader);
            let etchings = extractList(response.data['etchings'], extractEtchingShard);

            return  { Depth:depth, Headers:headers, Etchings:etchings };
        })
        .catch(handleError);
}