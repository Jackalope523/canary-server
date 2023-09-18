import { View, Text, Image } from 'react-native'
import React from 'react'
import { avatarStyles } from '../../styles/Avatars';

// !! THIS COMPONENT IS CURRENTLY NOT IN USE !!

type Props = {}

// TEMP. avatar image
const avatarImage = {uri: 'https://images.unsplash.com/photo-1540569014015-19a7be504e3a?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1935&q=80'};

const AvatarMedium = (props: Props) => {
    // TODO wire it up and test
    // Change border based on user status
    // Don't know if this will work, don't know how to test
    if (userIsFriend) {
        if (userIsOnline) {
            return <Image source={avatarImage} resizeMode="cover" style={[avatarStyles.avatarSquareMedium, avatarStyles.avatarOnline]} />;
        }

        else {
            return <Image source={avatarImage} resizeMode="cover" style={[avatarStyles.avatarSquareMedium, avatarStyles.avatarOffline]} />;
        }
    }

    else {
        return <Image source={avatarImage} resizeMode="cover" style={[avatarStyles.avatarSquareMedium, avatarStyles.avatarAnon]} />;
    }

//   return (
//     <View>
//       <Text>AvatarMedium</Text>
//     </View>
//   )
}

export default AvatarMedium