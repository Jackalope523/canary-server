import * as React from 'react';
import { ScrollView, View, Text, FlatList, StyleSheet } from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { navigationStyles } from '../styles/Navigation';
import Notification from '../components/organisms/Notification';
import { Spacing } from '../styles/Spacing';

const Icon = createIconSetFromFontello(fontelloConfig);

const NotificationsScreen = ({navigation}) => {

  React.useLayoutEffect(() => {
    navigation.setOptions({

      headerLeft: () => <Icon size={24} height={24} width={24} style={navigationStyles.topNavbar.icons} name="arrow-back-outline" onPress={() => navigation.goBack()} />,

      // headerLeftContainerStyle: {
      //   marginLeft: 24,
      // },
    })
  })

  // TODO if no notifications show NoNotifications component
  // TODO use a flatlist when connecting it up to the backend

    return (
        <ScrollView>
          <View style={styles.container}>
            <View style={styles.sectionWrapper}>
              <Text style={[globalStyles.headingTextThree, globalStyles.textDark, styles.headingSpacing]}>Today</Text>
              <Notification />
              <Notification />
              <Notification />
              <Notification />
            </View>
            <View style={styles.sectionWrapper}>
              <Text style={[globalStyles.headingTextThree, globalStyles.textDark, styles.headingSpacing]}>Tomorrow</Text>
              <Notification />
              <Notification />
            </View>
            <View style={styles.sectionWrapper}>
              <Text style={[globalStyles.headingTextThree, globalStyles.textDark, styles.headingSpacing]}>Next week</Text>
              <Notification />
              <Notification />
            </View>
          </View>
        </ScrollView>
    );
};

export default NotificationsScreen

const styles = StyleSheet.create ({
  container: {
    marginVertical: Spacing.md,
    marginHorizontal: Spacing.lg,
  },

  sectionWrapper: {
    marginVertical: Spacing.sm,
  },

  headingSpacing: {
    marginBottom: Spacing.md,
  },
});