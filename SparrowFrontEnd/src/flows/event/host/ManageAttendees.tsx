import { Image, ScrollView, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../../styles/GlobalStyles';

// TEMP. data
import { SAMPLE_USER_DATA } from '../../../data/sampleUserData';
import Avatar, { AvatarSize } from '../../../components/Avatar';
import { Spacing } from '../../../styles/SpacingStyles';
import UserListItemDropdown from '../../../components/event/UserListItemDropdown';

interface ManageAttendeesScreenProps {}

const ManageAttendeesScreen = (props: ManageAttendeesScreenProps) => {
  return (
    <View>
      <Text style={[globalStyles.textDark, { backgroundColor: 'red' }]}>
        Insert header component here
      </Text>
      <View style={styles.manageAttendees}>
        <View style={styles.inviteFriends}>
          <Text
            style={[
              globalStyles.textDark,
              globalStyles.headingTextThree,
              styles.inviteFriendsHeading,
            ]}>
            Invite friends
          </Text>
          {/* TODO make AvatarListItem into a component */}
          {/* TODO come up with a better design */}
          <ScrollView
            horizontal={true}
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.inviteFriendsInner}>
            {SAMPLE_USER_DATA.map((user) => (
              <View key={user.id} style={styles.avatarListItem}>
                <Avatar size={AvatarSize.Large} image={user.avatar} />
                {/* <Text
                style={[globalStyles.textDark, globalStyles.headingTextFive]}>
                {user.name}
              </Text> */}
              </View>
            ))}
          </ScrollView>
        </View>
        <View style={styles.attendees}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            Attendees
          </Text>
          {/* TODO hook up the onPress to go to a user's profile */}
          <View style={styles.attendeesInner}>
            {SAMPLE_USER_DATA.map((user) => (
              <UserListItemDropdown
                key={user.id}
                image={user.avatar}
                name={user.name}
              />
            ))}
          </View>
        </View>
      </View>
    </View>
  );
};

export default ManageAttendeesScreen;

const styles = StyleSheet.create({
  manageAttendees: {
    paddingTop: Spacing.lg,
    rowGap: Spacing.xl,
  },

  inviteFriends: {
    rowGap: Spacing.md,
  },

  inviteFriendsHeading: {
    paddingLeft: Spacing.lg,
  },

  inviteFriendsInner: {
    columnGap: Spacing.md,
    paddingHorizontal: Spacing.lg,
  },

  avatarListItem: {
    alignItems: 'center',
    rowGap: Spacing.xs,
  },

  attendees: {
    rowGap: Spacing.md,
    paddingHorizontal: Spacing.lg,
    paddingBottom: Spacing.lg,
  },

  attendeesInner: {
    rowGap: Spacing.md,
  },
});
