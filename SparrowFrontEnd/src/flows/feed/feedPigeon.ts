import { userSession, handleError, extractDate, extractList } from '../../lib/axios';
import { etchingShard, eventHeader, extractEtchingShard, extractEventHeader } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

export type feedOptions = {
    Depth: number,
    ExclusionList: number[]
}

// Get user feed
export async function getUserFeed(options: feedOptions) {
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

            return [ depth, headers, etchings ];
        })
        .catch(handleError);
}