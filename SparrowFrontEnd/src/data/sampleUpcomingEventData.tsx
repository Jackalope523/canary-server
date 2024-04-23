// Image dataset
import e1img1 from '../assets/images/temp/event-img-1.jpg';
import e1img2 from '../assets/images/temp/event-img-1.2.jpg';
import e1img3 from '../assets/images/temp/event-img-1.3.jpg';

import e2img1 from '../assets/images/temp/event-img-2.jpg';

import e3img1 from '../assets/images/temp/event-img-3.jpg';
import e4img1 from '../assets/images/temp/event-img-4.jpg';

import e5img1 from '../assets/images/temp/event-img-5.jpg';
import e5img2 from '../assets/images/temp/event-img-15.2.jpg';
import e5img3 from '../assets/images/temp/event-img-15.jpg';

import e6img1 from '../assets/images/temp/event-img-6.jpg';

import e7img1 from '../assets/images/temp/event-img-7.jpg';

import e8img1 from '../assets/images/temp/event-img-8.jpg';
import e8img2 from '../assets/images/temp/event-img-14.2.jpg';

import e9img1 from '../assets/images/temp/event-img-9.jpg';
import e10img1 from '../assets/images/temp/event-img-10.jpg';

// Event dataset
export const SAMPLEEVENTDATA = [
  {
    id: 1,
    host: 'Robert',
    date: 'Today',
    time: '12:00',
    attendees: 16,
    attendeesFriends: 3,
    title: 'Downhill MTB Competition 2023',
    description:
      "We're back again for another downhill mountain biking competition at Owl's Head. There are several rewards for winners of the competition. 1st place - $500 and a SCOTT Voltage YZ downhill mountain bike, 2nd place - $350 LOCALBIKES store gift card, 3rd place - $100 LOCALBIKES store gift card.",
    location: "Owl's Head, Vale Perkins",
    uri: e1img1,
  },

  {
    id: 2,
    host: 'Alice',
    date: 'Today',
    time: '22:00',
    attendees: 6,
    attendeesFriends: 0,
    title: 'The Spookiest Night of the Year - Prepare Your Costumes!',
    description:
      "I'm hosting a Halloween party at my house. Everyone from the neighborhood is welcome to join. Please show up dressed up in a costume that matches the Halloween theme. Food will be provided but you're welcome to bring food and drinks to the get-together. We have planned a movie night and karaoke. If that spikes your interest, come and join the event.",
    location: '38 Deer Street, Potton Boulevard, Potton',
    uri: e2img1,
  },

  {
    id: 3,
    host: 'Lily',
    date: 'This Wednesday',
    time: '01:00',
    attendees: 13,
    attendeesFriends: 0,
    title: 'Backyard Bonfire Bash Under the Autumn Stars',
    description:
      "Join me for a cozy evening around the bonfire in my backyard. We'll roast marshmallows, share stories, and enjoy the warmth of the fire while gazing at the brilliant autumn stars. Feel free to bring your favorite snacks or drinks. It's a casual gathering to celebrate the beauty of autumn and good company.",
    location: '123 Maple Street, Mansonville',
    uri: e3img1,
  },

  {
    id: 4,
    host: 'David',
    date: 'This Friday',
    time: '16:30',
    attendees: 4,
    attendeesFriends: 1,
    title: 'Jam Session in the Park: Spontaneous Musical Gathering',
    description:
      "Calling all musicians and music enthusiasts! I'll be at Central Park with my guitar, and I invite you to bring your instruments, your voice, or just your love for music. Let's create some spontaneous melodies and enjoy the fresh air together in the heart of the city.",
    location: 'Central Park',
    uri: e4img1,
  },

  {
    id: 5,
    host: 'Emily',
    date: 'This Saturtday',
    time: '12:30',
    attendees: 9,
    attendeesFriends: 2,
    title: 'Hike and Sunrise Breakfast Adventure at Pine Ridge Trail',
    description:
      "Rise and shine for a beautiful sunrise hike! We'll meet at the trailhead of Pine Ridge and make our way to the summit to catch the first light of the day. Afterward, I'll prepare a simple but delicious breakfast for all attendees. It's an early adventure you won't want to miss on the scenic Pine Ridge Trail. Feel free to bring dogs on the walk.",
    location: 'Pine Ridge Trail, Trailhead Parking Lot',
    uri: e5img1,
  },

  {
    id: 6,
    host: 'Sophia',
    date: 'This Sunday',
    time: '09:00',
    attendees: 27,
    attendeesFriends: 0,
    title: 'Beach Cleanup and Picnic',
    description:
      "Join us for a day of eco-friendly fun at Oceanview Beach. We'll start the morning with a beach cleanup, doing our part to protect the environment and maintain the pristine beauty of the coastline. After our collective effort to make the beach even more stunning, we'll gather for a delightful picnic right by the shore. Feel free to bring your favorite picnic dishes, relax in the soft sands, and enjoy the fresh sea breeze. This event is not only about making a positive impact but also about coming together as a community and celebrating the natural splendor of the coast.",
    location: 'Oceanview Beach, Beachfront Park',
    uri: e6img1,
  },

  {
    id: 7,
    host: 'Oliver',
    date: 'This Sunday',
    time: '21:00',
    attendees: 13,
    attendeesFriends: 3,
    title: 'Stargazing Camping Night in the Wilderness',
    description:
      "Escape the city lights for a night of stargazing in the tranquil wilderness. We'll set up telescopes and guides to explore the night sky, taking in the mesmerizing beauty of distant galaxies and constellations. No city lights mean incredible celestial views, and you'll have the opportunity to learn about the mysteries of the universe from our expert astronomers. Bring your camping gear for an overnight stay, share stories around the campfire, and bond with fellow stargazers. This event promises to be an unforgettable journey into the cosmos, deepening your appreciation for the wonders of the night sky.",
    location: 'Dark Sky Reserve, Camping Area',
    uri: e7img1,
  },

  {
    id: 8,
    host: 'Ethan',
    date: 'Next Monday',
    time: '18:30',
    attendees: 16,
    attendeesfriends: 5,
    title: 'Game Night Extravaganza',
    description:
      "Calling all board game enthusiasts, card sharks, and anyone up for a night of friendly competition and laughter! Join us for a Game Night Extravaganza at our cozy apartment. We'll have a variety of games, from classics like Monopoly and Scrabble to newer favorites like Codenames and Catan. Feel free to bring your own games too if you'd like. We'll provide some snacks and refreshments, but you're welcome to bring your favorite game-time treats to share. It's the perfect opportunity to test your strategic skills, engage in some good-natured rivalry, and make new friends. Whether you're a seasoned gamer or a newbie, everyone is welcome to join the fun. Get ready for a night of board games, card games, and endless entertainment.",
    location: '123 Elm Street, Apartment 4A',
    uri: e8img1,
  },

  {
    id: 9,
    host: 'Mia',
    date: 'Next Monday',
    time: '15:00',
    attendees: 23,
    attendeesfriends: 7,
    title: 'Skateboarding Meetup & Mini Ramp Jam',
    description:
      "Calling all skateboarders and skating enthusiasts, it's time to shred the ramps and showcase your skills at our Skateboarding Meetup & Mini Ramp Jam! Join us at Central Skate Park for an epic afternoon of tricks, flips, and thrilling rides. Our event will feature a mini ramp jam session, where you can show off your best moves and learn from fellow skaters. Whether you're a beginner or a seasoned pro, everyone is welcome to participate. The atmosphere will be friendly, and there will be plenty of opportunities to connect with the skating community.",
    location: 'Central Skate Park',
    uri: e9img1,
  },

  {
    id: 10,
    host: 'Noah',
    date: 'Next Tuesday',
    time: '17:00',
    attendees: 34,
    attendeesFriends: 5,
    title: 'Summer Soiree',
    description:
      "Get ready to kick off the summer season in style and join us at the picturesque Sandy Beachfront for an unforgettable day of sun, sea, and celebration. This beach party is all about good vibes, great company, and the perfect mix of relaxation and fun. We'll have a variety of beach games, from beach volleyball to sandcastle competitions, so bring your A-game.",
    location: 'Sandy Beachfront',
    uri: e10img1,
  },
];
