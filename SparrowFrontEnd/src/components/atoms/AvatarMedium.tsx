import { View, Text, Image } from 'react-native';
import * as React from 'react';
import { avatarStyles } from '../../styles/Avatars';

// !! THIS COMPONENT IS CURRENTLY NOT IN USE !! //
// TODO make this into a new component in components

type Props = {};

// TEMP. avatar image
const avatarImage = {
  uri: 'https://images.unsplash.com/photo-1540569014015-19a7be504e3a?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1935&q=80',
};

const AvatarMedium = (props: Props) => {
  // Change border based on user status

  // If user is a friend and is online return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarOnline
  // If user is a friend and is online return an image styles with avatarStyles.avatarSquareMedium, avtarStyles.avatarOffline
  // Else return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarAnon

  return (
    <View>
      <Text>AvatarMedium</Text>
    </View>
  );
};

export default AvatarMedium;
