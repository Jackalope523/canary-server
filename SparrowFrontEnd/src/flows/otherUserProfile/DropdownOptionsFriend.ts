/*

Dropdown options for DropdownSelectorText component;
Type: OtherUserProfile -> Friend event status dropdown

*/

// TODO add onPress functions
const dropdownOptionsFriend = [
  {
    id: 1,
    text: 'Upcoming',
    onPress: () => console.log('Upcoming events selected'),
  },
  {
    id: 2,
    text: 'Previously attended',
    onPress: () => console.log('Past events selected'),
  },
];

export default dropdownOptionsFriend;
