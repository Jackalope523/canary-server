import axios, { AxiosHeaders } from 'axios';
import { userSession, handleError, extractDate, extractList } from '../../lib/axios';
import { etchingManifest, eventEtching, eventHeaderManifest, extractEtchingManifest, extractEventHeaderManifest } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

export type feedOptions = {
    DepthCharge: number,
    LastDepth: number
}

export type feedManifest = {
    Headers: eventHeaderManifest[],
    Etchings: etchingManifest[],
}

// Get user feed
export async function getUserFeed(options: feedOptions) : Promise<feedManifest> {
    if (!options) {
        console.log('Feed options are missing.');
        return Promise.reject();
    }
    
    return await userSession.get(`${apiBaseUrl}/${options.DepthCharge}-${options.LastDepth}`)
        .then((response: any) => {
            console.log('User Feed:', response.data);

            let headers: eventHeaderManifest[] = [];
            let etchings: etchingManifest[] = [];

            try
            {
                headers = extractList(response.data['headers'], extractEventHeaderManifest);
                etchings = extractList(response.data['etchings'], extractEtchingManifest);
            }
            catch { }

            return  { Headers:headers, Etchings:etchings };
        })
        .catch(handleError);
}