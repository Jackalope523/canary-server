import { userSession, handleError, extractDate, extractList } from './axios';
import { createWriteStream } from 'fs';
import * as stream from 'stream';
import { promisify } from 'util';

const apiBaseUrl = '/media';

const finished = promisify(stream.finished);

// Get specific etching image
export async function getEtchingImage(etchingId: number) {
    const writer = createWriteStream(`${etchingId}.png`); 

    return await userSession.get(`${apiBaseUrl}/${etchingId}`, { responseType: 'stream' })
        .then((response: any) => {
            response.data.pipe(writer);
            return finished(writer);
        })
        .catch(handleError);
}