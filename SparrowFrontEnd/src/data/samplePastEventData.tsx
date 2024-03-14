// Image dataset
// Events
import e1img1 from '../assets/images/temp/event-img-11.jpg';
import e1img2 from '../assets/images/temp/event-img-11.2.jpg';
import e1img3 from '../assets/images/temp/event-img-11.3.jpg';

import e2img1 from '../assets/images/temp/event-img-12.jpg';
import e2img2 from '../assets/images/temp/event-img-12.2.jpg';

import e3img1 from '../assets/images/temp/event-img-13.jpg';

import e4img1 from '../assets/images/temp/event-img-14.jpg';
import e4img2 from '../assets/images/temp/event-img-14.2.jpg';

import e5img1 from '../assets/images/temp/event-img-15.jpg';
import e5img2 from '../assets/images/temp/event-img-15.2.jpg';

// Hosts (users)
import h1img from '../assets/images/temp/host-img-1.jpg';
import h2img from '../assets/images/temp/host-img-2.jpg';
import h3img from '../assets/images/temp/host-img-3.jpg';
import h4img from '../assets/images/temp/host-img-4.jpg';
import h5img from '../assets/images/temp/host-img-5.jpg';

// Event dataset (past events)
// Status can either be live or time until event turns live
export const SAMPLE_PAST_EVENT_DATA = [
  {
    id: '1',
    status: 'live',
    time: 'live',
    host: 'Liam',
    avatar: h1img,
    attendees: ['Gale, ', 'Mia'],
    leftoverAttendeeCount: 2,
    title: 'Peak Shredder Showdown',
    description:
      "Get ready to carve through the powder and showcase your snowboarding skills. This adrenaline-packed event brings together snowboarders of all levels for a thrilling day of tricks, jumps and high-flying stunts. Whether you're a seasoned pro or just starting out, join us for a day of snowy excitement and friendly competition.",
    location: 'Frosty Ridge Terrain Park',
    likeCount: 3,
    media: [e1img1, e1img2, e1img3],
  },

  {
    id: '2',
    status: 'live',
    time: 'live',
    host: 'Ava',
    avatar: h2img,
    attendees: ['Monica, ', 'Dylan'],
    leftoverAttendeeCount: 6,
    title: 'Golden Hour Photography Workshop',
    description:
      "Join us for a golden hour photography workshop at the beautiful Golden Gate Park. We'll be exploring the park's stunning landscapes and capturing the magic of the golden hour. Whether you're a beginner or a seasoned photographer, this workshop is a great opportunity to learn new skills, meet fellow photography enthusiasts and capture some breathtaking shots.",
    location: 'Golden Gate Park',
    likeCount: 25,
    media: [e2img1, e2img2],
  },

  {
    id: '3',
    status: 'passed',
    time: '12m',
    host: 'Ruben',
    avatar: h3img,
    attendees: ['Sonya, ', 'Lauren'],
    leftoverAttendeeCount: 13,
    title: 'Come join a friendly soccer match and meet the locals!',
    description:
      'We are a group of locals who play soccer every Sunday. We are looking for new players to join us. We play for fun and to meet new people. All skill levels are welcome!',
    location: 'Fairview Park Soccer Field',
    likeCount: 19,
    media: [e3img1],
  },
  {
    id: '4',
    status: 'passed',
    time: '3h',
    host: 'Ksenia',
    avatar: h4img,
    attendees: ['Mason, ', 'Stephen'],
    leftoverAttendeeCount: 17,
    title: "Roll, Strateize, Conquer: Board Game Night at Mason's",
    description:
      'Join us for a night of board games, card games, and endless entertainment. Whether you are a seasoned gamer or a newbie, everyone is welcome to join the fun. Get ready for a night of board games, card games, and endless entertainment.',
    location: "Mason's Cafe, 63 Broadleaf street",
    likeCount: 4,
    media: [e4img1, e4img2],
  },
  {
    id: '5',
    status: 'passed',
    time: '18h',
    host: 'Clara',
    avatar: h5img,
    attendees: ['Gordon, ', 'Stephanie'],
    leftoverAttendeeCount: 23,
    title: 'Slow hike for nature lovers of all ages',
    description:
      'Join us for a slow hike through the beautiful Pine Ridge Trail. We will be taking our time to enjoy the scenery and take in the fresh air. This hike is perfect for nature lovers of all ages. Feel free to bring your dogs along for the walk.',
    location: 'Pine Ridge Trail, Trailhead Parking Lot',
    likeCount: 38,
    media: [e5img1, e5img2],
  },
];
