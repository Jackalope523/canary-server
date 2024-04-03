import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { SAMPLE_USER_DATA } from '../../data/sampleUserData';
import AvatarStackScroll, {
  AvatarType,
} from '../../components/AvatarStackScroll';
import { AvatarSize } from '../../components/Avatar';
import { Spacing } from '../../styles/SpacingStyles';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';

// Icons
import ShareIcon from '../../assets/icons/share-outline.svg';

interface ShareScreenProps {}

const ShareScreen = (props: ShareScreenProps) => {
  return (
    <View>
      <Text style={[globalStyles.textDark, { backgroundColor: 'red' }]}>
        Insert header component here
      </Text>
      <View style={styles.shareScreen}>
        <View style={styles.inviteFriends}>
          <Text
            style={[
              globalStyles.textDark,
              globalStyles.headingTextThree,
              styles.inviteFriendsHeading,
            ]}>
            Invite friends
          </Text>
          <AvatarStackScroll
            avatars={SAMPLE_USER_DATA.map((user) => user.avatar)}
            size={AvatarSize.Large}
            type={AvatarType.Beside}
            onPress={null}
          />
        </View>
        <View style={styles.shareOnSocials}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            Share on socials
          </Text>
          {/* TODO modify buttons to have an aligned to left alignment, when implemented */}
          {/* TODO hook up onPress */}
          <View>
            <Button
              type={ButtonType.SecondaryLight}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Share on Instagram'}
              displayIcon={true}
              Icon={ShareIcon}
              onPress={null}
            />
            <Button
              type={ButtonType.SecondaryLight}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Share on Facebook'}
              displayIcon={true}
              Icon={ShareIcon}
              onPress={null}
            />
            <Button
              type={ButtonType.SecondaryLight}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Share on X (Twitter)'}
              displayIcon={true}
              Icon={ShareIcon}
              onPress={null}
            />
            <Button
              type={ButtonType.SecondaryLight}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Share on WhatsApp'}
              displayIcon={true}
              Icon={ShareIcon}
              onPress={null}
            />
          </View>
        </View>
      </View>
    </View>
  );
};

export default ShareScreen;

const styles = StyleSheet.create({
  shareScreen: {
    rowGap: Spacing.xl,
    paddingTop: Spacing.lg,
  },

  inviteFriends: {
    rowGap: Spacing.md,
  },

  inviteFriendsHeading: {
    paddingLeft: Spacing.lg,
  },

  shareOnSocials: {
    rowGap: Spacing.md,
    paddingHorizontal: Spacing.lg,
  },
});
