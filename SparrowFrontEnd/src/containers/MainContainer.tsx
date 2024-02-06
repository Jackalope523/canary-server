import * as React from 'react';

// Styles
import { Colors } from '../styles/ColorStyles';
import { globalStyles } from '../styles/GlobalStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

// Navigation
import { NavigationContainer } from '@react-navigation/native';
import {
  BottomTabBarProps,
  createBottomTabNavigator,
} from '@react-navigation/bottom-tabs';
import { createStackNavigator } from '@react-navigation/stack';

// Screens
// Auth
import LandingScreen from '../flows/auth/Landing';
import LoginScreen from '../flows/auth/Login';
import SignupScreen from '../flows/auth/Signup';
import VerifyScreen from '../flows/auth/Verify';
import ContinueScreen from '../flows/auth/Continue';

// Quiz
import IntroScreen from '../flows/auth/survey/Intro';
import Q1Screen from '../flows/auth/survey/Q1';
import Q2Screen from '../flows/auth/survey/Q2';
import Q3Screen from '../flows/auth/survey/Q3';
import Q4Screen from '../flows/auth/survey/Q4';
import Q5Screen from '../flows/auth/survey/Q5';
import Q6Screen from '../flows/auth/survey/Q6';

// Main
import ActivityScreen from '../flows/activity/Activity';
import DiscoveryScreen from '../flows/discovery/Discovery';
import FeedScreen from '../flows/feed/Feed';
import DiscoverySearchScreen from '../flows/discovery/DiscoverySearch';
import NotificationsScreen from '../flows/activity/Notifications';
import AccountScreen from '../flows/profile/Account';
import ProfileScreen from '../flows/profile/Profile';
import TestScreen from '../flows/testing/Testing';

// More imports
import {
  BottomTabParamList,
  StackParamList,
  AuthStackParamList,
  AppStackParamList,
} from '../components/atoms/types';
import { SafeAreaProvider } from 'react-native-safe-area-context';

// TODO setup top navbars for all necessary screens
// TEMP. testing
import TopNavbarFavorite from '../components/organisms/TopNavbarFavorite';
import { StyleSheet } from 'react-native';

// v1.0.1

const AppStack = createStackNavigator<AppStackParamList>();
const AuthStack = createStackNavigator<AuthStackParamList>();
const Tab = createBottomTabNavigator<BottomTabParamList>();
const ActivityStack = createStackNavigator();
const AccountStack = createStackNavigator();

// TODO change initialRouteName back to Auth when finished with Survey

function MainContainer() {
  return (
    <SafeAreaProvider>
      <NavigationContainer>
        <AppStack.Navigator
          initialRouteName="Main"
          screenOptions={{
            headerShown: false,
            cardStyle: styles.cardContainer,
          }}>
          <AppStack.Screen name="Auth" component={Authentication} />
          <AppStack.Screen name="Survey" component={Survey} />
          <AppStack.Screen name="Main" component={Main} />
          <AppStack.Screen name="Account" component={Account} />

          <AppStack.Screen name="Testing" component={TestScreen} />
        </AppStack.Navigator>
      </NavigationContainer>
    </SafeAreaProvider>
  );
}

export default MainContainer;

function Authentication() {
  return (
    <AuthStack.Navigator
      initialRouteName="Landing"
      screenOptions={{
        headerShown: false,
        cardStyle: styles.cardContainer,
      }}>
      <AuthStack.Screen name="Landing" component={LandingScreen} />
      <AuthStack.Screen name="Login" component={LoginScreen} />
      <AuthStack.Screen name="Signup" component={SignupScreen} />
      <AuthStack.Screen name="Verify" component={VerifyScreen} />
      <AuthStack.Screen name="Continue" component={ContinueScreen} />
    </AuthStack.Navigator>
  );
}

function Survey() {
  return (
    <AuthStack.Navigator
      initialRouteName="Intro"
      screenOptions={{
        headerShown: false,
        cardStyle: styles.cardContainer,
      }}>
      <AuthStack.Screen name="Intro" component={IntroScreen} />
      <AuthStack.Screen name="Q1" component={Q1Screen} />
      <AuthStack.Screen name="Q2" component={Q2Screen} />
      <AuthStack.Screen name="Q3" component={Q3Screen} />
      <AuthStack.Screen name="Q4" component={Q4Screen} />
      <AuthStack.Screen name="Q5" component={Q5Screen} />
      <AuthStack.Screen name="Q6" component={Q6Screen} />
    </AuthStack.Navigator>
  );
}

// TODO If possible, set the tab navigator horizontal margin to 24, probably with tabBarStyle

function Main() {
  return (
    <Tab.Navigator
      sceneContainerStyle={globalStyles.mainContainer}
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color }) => {
          let iconName;
          let rn = route.name;

          if (rn === 'Activity') {
            iconName = focused ? 'activity-fill' : 'activity-fill';
          } else if (rn === 'Discovery') {
            iconName = focused ? 'discovery-fill' : 'discovery-fill';
          } else if (rn === 'Feed') {
            iconName = focused ? 'feed-fill' : 'feed-fill';
          } else if (rn === 'Profile') {
            iconName = focused ? 'account-fill' : 'account-fill';
          }

          return (
            <Icon
              name={iconName}
              size={40}
              height={40}
              width={40}
              color={color}
            />
          );
        },

        tabBarActiveTintColor: Colors.sparrowRed,
        tabBarInactiveTintColor: Colors.sparrowBrown,
        tabBarShowLabel: false,

        tabBarStyle: {
          height: 50,
          backgroundColor: Colors.sparrowSand,
          borderTopWidth: 2,
          borderTopColor: Colors.sparrowDarkBrown,
          paddingHorizontal: 0,
        },

        headerShown: false,
      })}>
      <Tab.Screen name="Activity" component={ActivityStackScreen} />
      <Tab.Screen name="Discovery" component={DiscoveryScreen} />
      <Tab.Screen name="Feed" component={FeedScreen} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  );
}

// TODO if you navigate from Activity to Notifications, then to another screen such as Feed, when navigating back to Activity, you will be directed to Notifiations. You should be directed back to Activity instead of last opened navigation screen.

function ActivityStackScreen() {
  return (
    <ActivityStack.Navigator
      screenOptions={() => ({
        headerShown: false,

        cardStyle: styles.cardContainer,

        headerTitleStyle: {
          fontSize: 16,
          color: Colors.sparrowDark,
        },

        // headerTitleContainerStyle: {
        //     marginHorizontal: 24,
        // },

        // headerLeftContainerStyle: {
        //     marginLeft: 0,
        // },

        headerStyle: {
          height: 50,
          backgroundColor: Colors.sparrowSand,
          borderBottomColor: Colors.sparrowDarkBrown,
          borderBottomWidth: 2,
        },

        // headerLeftContainerStyle: {
        //     marginLeft: 24,
        // },

        // headerLeftContainerStyle: {
        //     backgroundColor: Colors.red400,
        //     // marginHorizontal: 16, // between btn and title
        //     // left: 24, // moves ONLY btn from left, absolute
        // },
      })}>
      <ActivityStack.Screen name="Activity" component={ActivityScreen} />
      <ActivityStack.Screen
        name="Notifications"
        component={NotificationsScreen}
      />
    </ActivityStack.Navigator>
  );
}

function Account() {
  return (
    <AccountStack.Navigator
      initialRouteName="Account"
      screenOptions={{
        headerShown: true,
        cardStyle: styles.cardContainer,
      }}>
      <AccountStack.Screen name="Account" component={AccountScreen} />
    </AccountStack.Navigator>
  );
}

const styles = StyleSheet.create({
  cardContainer: {
    backgroundColor: Colors.sparrowSand,
    // TESTING ONLY
    // backgroundColor: Colors.fuchsia300,
  },
});
