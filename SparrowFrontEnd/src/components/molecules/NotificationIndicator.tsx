import { View, Text } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

const NotificationIndicator = () => {
  return (
    <View>
      <Icon name="notification-outline-alt"/>
      <Text>12</Text>
    </View>
  )
}

export default NotificationIndicator