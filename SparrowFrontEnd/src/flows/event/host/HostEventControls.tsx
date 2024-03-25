import { Image, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../../styles/GlobalStyles';
import { borderRadius } from '../../../styles/BorderStyles';
import { Colors } from '../../../styles/ColorStyles';
import { CustomDimensions } from '../../../styles/CustomDimensionStyles';

import tempBanner from '../../assets/images/temp/event-img-1.jpg';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../../components/Button';
import FlagMedium, { FlagType } from '../../../components/FlagMedium';

// Icons
import PersonIcon from '../../assets/icons/account-outline.svg';
import DiagonalUpArrowIcon from '../../assets/icons/arrow-up-outline-alt.svg';
import { Spacing } from '../../../styles/SpacingStyles';

interface HostEventControlsScreenProps {}

const HostEventControlsScreen = (props: HostEventControlsScreenProps) => {
  /* TODO hook up real time here - how long has an event been live for

  Similar to a stopwatch but with the stop being the terminate event button

  */
  let eventActiveFor = '00:12:34';

  const eventTitle = 'Dog Walk and Play Meetup at Central Park';

  return (
    <View style={[globalStyles.baseContainer, styles.hostEventControls]}>
      <View style={styles.top}>
        <View style={styles.status}>
          <FlagMedium type={FlagType.Live} />
          <Text
            style={[
              globalStyles.labelTextOneAsTyped,
              globalStyles.textSuccess,
            ]}>
            {eventActiveFor}
          </Text>
        </View>
        <View style={styles.event}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            {eventTitle}
          </Text>
          <Image
            source={tempBanner}
            style={styles.bannerImage}
            resizeMode="cover"
          />
        </View>
      </View>

      {/* TODO currently, visually, buttons aren't consistent with the design system but the naming partly is, once new button issue is fixed - update these */}
      {/* TODO hook up onPress */}
      <View style={styles.controls}>
        <Button
          type={ButtonType.PrimaryDark}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Manage attendees'}
          displayIcon={true}
          Icon={PersonIcon}
          onPress={null}
        />
        <Button
          type={ButtonType.SecondaryDark}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'View event page'}
          displayIcon={true}
          Icon={DiagonalUpArrowIcon}
          onPress={null}
        />
      </View>
    </View>
  );
};

export default HostEventControlsScreen;

const styles = StyleSheet.create({
  hostEventControls: {
    flex: 1,
    justifyContent: 'space-between',

    rowGap: Spacing.lg,
  },

  top: {
    rowGap: Spacing.lg,
    flex: 1,
  },

  status: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.mdsm,
  },

  event: {
    rowGap: Spacing.md,
    flex: 1,
  },

  controls: {
    rowGap: Spacing.md,
  },

  bannerImage: {
    flex: 1,
    width: '100%',

    // TODO DELETE later if not needed (this sets a height based on window height; doesn't fill the whole screen)
    // height: CustomDimensions.windowHeight / 3,

    borderRadius: borderRadius.md,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },
});
