import { userSession } from '../../../lib/axios';
import { UserProfile } from '../screens/ProfileScreen';

export async function getUserProfile() {
    
    let user: UserProfile = {
        name: '',
        numberOfFollowers: 0,
        reputation: 0
    };

    await userSession.get('/account')
    .then((response) => {
        console.log(response.data);
        console.log(response.status);
        console.log(response.headers);

        user = {
            name: response.data['name'],
            numberOfFollowers: response.data['numberOfFollowers'],
            reputation: response.data['reputation']
        }
    })
    .catch((error) => {
        console.log(error.toJSON());
        if (error.response) {
            console.log(error.response.data);
            console.log(error.response.status);
            console.log(error.response.headers);
      } else if (error.request) {
            console.log(error.request);
      } else {
            console.log('Axios call failed, error', error.message);
      }

      return Promise.reject();
    });
    
    return Promise.resolve(user);
}