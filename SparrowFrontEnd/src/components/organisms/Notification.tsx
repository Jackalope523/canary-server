import { View, Text, Image } from 'react-native'
import React from 'react'
import { avatarStyles } from '../../styles/Avatars';
import { notificationStyles } from '../../styles/Notifications';
import { globalStyles } from '../../styles/Global';

// TEMP. avatar image
const avatarImage = {uri: 'https://images.unsplash.com/photo-1540569014015-19a7be504e3a?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1935&q=80'};

const Notification = () => {

  return (
    <View style={notificationStyles.notification}>
      <Image source={avatarImage} resizeMode="cover" style={[avatarStyles.avatarSquareMedium, avatarStyles.avatarOffline]} />
      <View style={notificationStyles.notification.textWrapper}>
        <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>User has invited you to a Game of Skate.</Text>
        <Text style={[globalStyles.labelTextAsTyped, globalStyles.textDark]}>4h ago</Text>
      </View>
    </View>
  )
}

export default Notification