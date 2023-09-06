import { ImageBackground, Text, View } from 'react-native'
import React from 'react'
import { cardStyles } from '../../styles/Cards'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. example imports
const bgImage = {uri: 'https://images.unsplash.com/photo-1562519819-016930ada31b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=687&q=80'};

const EventCardMedium = () => {
  return (
        <View style={cardStyles.eventCardMedium}>
            <ImageBackground source={bgImage} resizeMode="cover" imageStyle={cardStyles.eventCardMedium.bgImage2}>
                <View style={cardStyles.eventCardMedium.content}>
                    <View style={cardStyles.eventCardMedium.content.topWrapper}>
                        <View style={cardStyles.eventCardMedium.content.container}>
                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                <Icon name="date-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>This Friday</Text>
                            </View>
                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                <Icon name="time-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>15:00</Text>
                            </View>
                        </View>
                        <View style={cardStyles.eventCardMedium.content.container}>
                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                <Icon name="account-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>6</Text>
                            </View>
                        </View>
                    </View>
                    <View style={cardStyles.eventCardMedium.content.container}>
                        <Text style={[globalStyles.headingTextThree, globalStyles.textDark, cardStyles.eventCardMedium.content.container.title]}>Two-on-two basketball at Venice Beach</Text>
                        <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                            <View>
                                <Icon name="location-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon} />
                            </View>
                            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>Venice Beach, Venice</Text>
                        </View>
                    </View>
                </View>
            </ImageBackground>
        </View>
  )
}

export default EventCardMedium