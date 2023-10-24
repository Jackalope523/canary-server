import { userSession, handleError } from '../axios';

const apiBaseUrl = 'feed';

export type feedOptions = {
    Depth: number,
    ExclusionList: string[]
}

// Get user feed
export async function getUserFeed(options: feedOptions) {
    if (!options) {
        return console.log('Feed options are missing.');
    }

    await userSession.get(`${apiBaseUrl}/${options.Depth}`, { data: options })
        .then((response) => {
            console.log('User Feed:', response.data);
        })
        .catch(handleError);
}