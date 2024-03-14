import { userSession, handleError, extractDate, extractList } from '../../lib/axios';
import { etchingShard, eventEtching, eventHeader, extractEtchingShard, extractEventHeader } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

type feedOptions = {
    Depth: number,
    ExclusionList: number[]
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

    return await userSession.get(`${apiBaseUrl}/${options.Depth}`, { data: options })
        .then((response: any) => {
            console.log('User Feed:', response.data);

            let depth: number = response.data['Depth'];
            let headers = extractList(response.data['Headers'], extractEventHeader);
            let etchings = extractList(response.data['Etchings'], extractEtchingShard);

            return  { Depth:depth, Headers:headers, Etchings:etchings };
        })
        .catch(handleError);
}