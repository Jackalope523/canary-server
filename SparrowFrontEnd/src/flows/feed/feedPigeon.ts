import axios, { AxiosHeaders } from 'axios';
import { userSession, handleError, extractDate, extractList } from '../../lib/axios';
import { etchingShard, eventEtching, eventHeader, extractEtchingShard, extractEventHeader } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

type feedOptions = {
    DepthCharge: number,
    LastDepth: number
}

type rawFeed = {
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

            let headers: eventHeader[] = [];
            let etchings: etchingShard[] = [];

            try
            {
                headers = extractList(response.data['headers'], extractEventHeader);
                etchings = extractList(response.data['etchings'], extractEtchingShard);
            }
            catch { }

            return  { Headers:headers, Etchings:etchings };
        })
        .catch(handleError);
}