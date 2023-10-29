import { userSession, handleError, extractDate } from '../../lib/axios';
import { etchingShard, eventHeader } from '../event/eventPigeon';

const apiBaseUrl = '/feed';

export type feedOptions = {
    Depth: number,
    ExclusionList: string[]
}

// Get user feed
export async function getUserFeed(options: feedOptions) {
    if (!options) {
        return console.log('Feed options are missing.');
    }

    return await userSession.get(`${apiBaseUrl}/${options.Depth}`, { data: options })
        .then((response: any) => {
            console.log('User Feed:', response.data);

            let depth: number = response.data['Depth'];

            let headers: eventHeader[] = [];

            for (const header of response.data['Headers'])
            {
                headers.push({
                    Id: header['Id'],
                    Name: header['EventId'],
                    IsActive: header['IsActive'],
                    LastActiveTime: extractDate(header['LastTimeActive'])
                });
            }

            let etchings: etchingShard[] = [];

            for (const etching of response.data['Etchings'])
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

            return [ depth, headers, etchings ];
        })
        .catch(handleError);
}