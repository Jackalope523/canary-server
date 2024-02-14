// Image dataset
import img1 from '../assets/images/temp/host-img-1.jpg';
import img2 from '../assets/images/temp/host-img-2.jpg';
import img3 from '../assets/images/temp/host-img-3.jpg';
import img4 from '../assets/images/temp/host-img-4.jpg';
import img5 from '../assets/images/temp/host-img-5.jpg';

// User dataset
export const SAMPLE_USER_DATA = [
  {
    id: '1',
    name: 'Gale',
    location: 'Vancouver, Canada',
    friend: true,
    avatar: img1,

    userSince: '2024',
    lastSeen: '2d',

    eventsAttended: 14,
    eventsHosted: 2,
  },
  {
    id: '2',
    name: 'Beatrice',
    location: 'Ottawa, Canada',
    friend: false,
    avatar: img2,

    userSince: '2024',
    lastSeen: '12d',

    eventsAttended: 23,
    eventsHosted: 5,
  },
  {
    id: '3',
    name: 'John',
    location: 'Toronto, Canada',
    friend: false,
    avatar: img3,

    userSince: '2024',

    eventsAttended: 2,
    eventsHosted: 0,
  },
  {
    id: '4',
    name: 'Catherine',
    location: 'Lores, Canada',
    friend: true,
    avatar: img4,

    userSince: '2024',

    eventsAttended: 9,
    eventsHosted: 1,
  },
  {
    id: '5',
    name: 'Mia',
    location: 'Edmonton, Canada',
    friend: true,
    avatar: img5,

    userSince: '2024',

    eventsAttended: 3,
    eventsHosted: 8,
  },
];
