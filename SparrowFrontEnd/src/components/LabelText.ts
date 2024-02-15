/*

Pre-made label text for TextLabel component;
Use when importing TextLabel component

*/

// Sample user data
import { SAMPLE_USER_DATA } from '../data/sampleUserData';

// TODO replace with real user data
const user = SAMPLE_USER_DATA.find((user) => user.id === '1');

export const labelText = {
  // About user
  userSince: 'User since ' + user?.userSince,
  lastSeen: 'Last seen ' + user?.lastSeen + ' ago',

  // User types
  you: 'You'.toUpperCase(),
  friend: 'Friend'.toUpperCase(),
};
