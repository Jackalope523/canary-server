import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import Avatar, { AvatarSize, AvatarStatus } from './Avatar';

interface EventCardLargeProps {}

// Icons
import FeatherIcon from '../assets/icons/feather-fill-colored.svg';

// TODO delete later after updating; TEMP. imports
import TempAvatarImage from '../assets/images/temp/host-img-1.jpg';

import { globalStyles } from '../styles/GlobalStyles';

const EventCardLarge: React.FC<EventCardLargeProps> = () => {
  return (
    <View style={styles.eventCardLarge}>
      <View style={styles.host}>
        <Avatar size={AvatarSize.Small} image={TempAvatarImage} />
        <View style={styles.hostNameContainer}>
          <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
            Host name
          </Text>
          <FeatherIcon height={24} width={24} />
        </View>
      </View>
    </View>
  );
};

export default EventCardLarge;

const styles = StyleSheet.create({
  eventCardLarge: {},
  host: {},
  hostNameContainer: {},
});
