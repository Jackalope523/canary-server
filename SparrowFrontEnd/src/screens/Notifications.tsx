import React, { useState } from 'react';
import { ScrollView, View, Text, FlatList, StyleSheet, Image } from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { navigationStyles } from '../styles/Navigation';
import Notification from '../components/organisms/Notification';
import { Spacing } from '../styles/Spacing';
import { SafeAreaView } from 'react-native-safe-area-context';

import { avatarStyles } from '../styles/Avatars';
import { notificationStyles } from '../styles/Notifications';
import NoNotifications from '../components/organisms/NoNotifications';

const Icon = createIconSetFromFontello(fontelloConfig);

const NotificationsScreen = ({navigation}) => {

  const eventInviteText = " has invited you to an event."
  
  // Sample avatar images
  const img1 = {uri: 'https://images.unsplash.com/photo-1632765854612-9b02b6ec2b15?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1972&q=80'}
  const img2 = {uri: 'https://images.unsplash.com/photo-1693871619509-d439714233dc?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1974&q=80'}
  const img3 = {uri: 'https://images.unsplash.com/photo-1552374196-c4e7ffc6e126?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1974&q=80'}
  const img4 = {uri: 'https://images.unsplash.com/photo-1567532939604-b6b5b0db2604?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1974&q=80'}
  const img5 = {uri: 'https://images.unsplash.com/photo-1693930450173-b78bc139e3a2?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1974&q=80'}
  
  // Sample notification dataset
  const [sampleNotifications, setSampleNotifications] = useState([
    {
      id: "1",
      uri: img1,
      name: "Patricia",
      sent: "16min ago",
    },
    {
      id: "2",
      uri: img2,
      name: "Cole",
      sent: "2h ago"
    },
    {
      id: "3",
      uri: img3,
      name: "Dipper",
      sent: "1d ago",
    },
    {
      id: "4",
      uri: img4,
      name: "Mabel",
      sent: "4d ago",
    },
    {
      id: "5",
      uri: img5,
      name: "Stanford",
      sent: "8d ago",
    },
  ]);

  React.useLayoutEffect(() => {
    navigation.setOptions({

      headerLeft: () => <Icon size={24} height={24} width={24} style={navigationStyles.topNavbar.icons} name="arrow-back-outline" onPress={() => navigation.goBack()} />,

      // headerLeftContainerStyle: {
      //   marginLeft: 24,
      // },

      headerLeftContainerStyle: {
        paddingLeft: 24,
      },

      headerTitleContainerStyle: {
        padding: 0,
        margin: 0,
        left: 0,
        right: 0,
      },

      headerShown: true,
    })
  });

  // TODO if no notifications show NoNotifications component
  // TODO use a flatlist when connecting it up to the backend

    return (
        // NEW FUNCTIONAL VERSION
        <SafeAreaView>
          <FlatList
            style={[globalStyles.baseContainer, styles.listWrapper]}
            showsVerticalScrollIndicator={false}
            ItemSeparatorComponent={() => <View style={{height: Spacing.md}} />}
            ListEmptyComponent={NoNotifications}
            keyExtractor={(item) => item.id}
            data={sampleNotifications}
            renderItem={({ item }) => (
              <View style={notificationStyles.notification}>
                <Image source={item.uri} resizeMode="cover" style={[avatarStyles.avatarSquareMedium, avatarStyles.avatarOffline]} />
                <View style={notificationStyles.notification.textWrapper}>
                  <Text style={[globalStyles.bodyTextOne, globalStyles.buttonDark]}>{item.name}{eventInviteText}</Text>
                  <Text style={[globalStyles.labelTextAsTyped, globalStyles.buttonDark]}>{item.sent}</Text>
                </View>
              </View>
            )}
          />
        </SafeAreaView>

        // OLD NON-FUNCTIONAL VERSION

        // <ScrollView>
        //   <View style={styles.container}>
        //     <View style={styles.sectionWrapper}>
        //       <Text style={[globalStyles.headingTextThree, globalStyles.buttonDark, styles.headingSpacing]}>Today</Text>
        //       <Notification />
        //       <Notification />
        //       <Notification />
        //       <Notification />
        //     </View>
        //     <View style={styles.sectionWrapper}>
        //       <Text style={[globalStyles.headingTextThree, globalStyles.buttonDark, styles.headingSpacing]}>Tomorrow</Text>
        //       <Notification />
        //       <Notification />
        //     </View>
        //     <View style={styles.sectionWrapper}>
        //       <Text style={[globalStyles.headingTextThree, globalStyles.buttonDark, styles.headingSpacing]}>Next week</Text>
        //       <Notification />
        //       <Notification />
        //     </View>
        //   </View>
        // </ScrollView>
    );
};

export default NotificationsScreen

const styles = StyleSheet.create ({
  listWrapper: {
    marginBottom: Spacing.sm,
  },
});