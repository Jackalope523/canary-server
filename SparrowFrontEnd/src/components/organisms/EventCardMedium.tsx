import { Text, View } from 'react-native'
import React, { Component } from 'react'
import { cardStyles } from '../../styles/Cards'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

// Types
type Props = {}

const EventCardMedium = (props: Props) => {
  return (
      <View>
        <View style={cardStyles.eventCardMedium}>
            <View>
                <Icon name="date-outline"/>
                <Text>This Friday</Text>
                <Icon name="time-outline"/>
                <Text>15:00</Text>
            </View>
            {/* Replace account-fill with account-outline later - need to create an updated font file */}
            <View>
                <Icon name="account-fill"/>
                <Text>6</Text>
            </View>
            <View>
                <Text>Two-on-two basketball at Venice Beach</Text>
                <Icon name="location-outline" />
                <Text>Venice Beach, Venice</Text>
            </View>
        </View>
      </View>
  )
}