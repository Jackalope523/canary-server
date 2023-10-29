import * as React from 'react';

// Styles
import { Colors } from '../styles/Colors';
import { globalStyles } from '../styles/Global';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

// Navigation
import { NavigationContainer } from '@react-navigation/native';
import { BottomTabBarProps, createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createStackNavigator } from '@react-navigation/stack';

// Screens
import LandingScreen from '../flows/auth/Landing';
import LoginScreen from '../flows/auth/Login';
import SignupScreen from '../flows/auth/Signup';
import VerifyScreen from '../flows/auth/Verify';
import ContinueScreen from '../flows/auth/Continue';

import ActivityScreen from '../flows/activity/Activity';
import DiscoveryScreen from '../flows/discovery/Discovery';
import FeedScreen from '../flows/feed/Feed';
import DiscoverySearchScreen from '../flows/discovery/DiscoverySearch';

import NotificationsScreen from '../flows/activity/Notifications';

import AccountScreen from '../flows/profile/Account';
import ProfileScreen from '../flows/profile/Profile';

// More imports
import { BottomTabParamList, StackParamList, AuthStackParamList, AppStackParamList } from '../components/atoms/types';
import { SafeAreaProvider } from 'react-native-safe-area-context';

// TEMP. testing
import TopNavbarFavorite from '../components/organisms/TopNavbarFavorite';

// v1.0.1

const AppStack = createStackNavigator<AppStackParamList>();
const AuthStack = createStackNavigator<AuthStackParamList>();
const Tab = createBottomTabNavigator<BottomTabParamList>();
const ActivityStack = createStackNavigator();
const AccountStack = createStackNavigator();

function MainContainer () {
    return (
        <SafeAreaProvider>
            <NavigationContainer>
                <AppStack.Navigator initialRouteName="Auth">
                    <AppStack.Screen name="Auth" component={Authentication}
                    options={{headerShown: true}}
                    />
                    <AppStack.Screen name="Main" component={Main}
                    options={{headerShown: false}}
                    />
                    <AppStack.Screen name="Account" component={Account}
                    options={{headerShown: false}}
                    />
                </AppStack.Navigator>
            </NavigationContainer>
        </SafeAreaProvider>
    );
};

export default MainContainer;


function Authentication () {
    return(
        <AuthStack.Navigator initialRouteName="Landing">
            <AuthStack.Screen name="Landing" component={LandingScreen} />
            <AuthStack.Screen name="Login" component={LoginScreen} />
            <AuthStack.Screen name="Signup" component={SignupScreen} />
            <AuthStack.Screen name="Verify" component={VerifyScreen} />
            <AuthStack.Screen name="Continue" component={ContinueScreen} />
        </AuthStack.Navigator>
    );
}

// TODO If possible, set the tab navigator horizontal margin to 24, probably with tabBarStyle

function Main () {
    return(
        <Tab.Navigator
            sceneContainerStyle={globalStyles.mainContainer}
            screenOptions={({route}) => ({

                tabBarIcon: ({focused, color, size}) => {
                    let iconName;
                    let rn = route.name;

                    if (rn === 'Activity') {
                        iconName = focused ? 'activity-fill' : 'activity-fill'
                    } else if (rn === 'Discovery') {
                        iconName = focused ? 'discovery-fill' : 'discovery-fill'
                    } else if (rn === 'Feed') {
                        iconName = focused ? 'feed-fill' : 'feed-fill'
                    } else if (rn === 'Profile') {
                        iconName = focused ? 'account-fill' : 'account-fill'
                    }

                    return <Icon name={iconName} size={40} height={40} width={40} color={color}/>

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
            
            <Tab.Screen name='Activity' component={ActivityStackScreen} />
            <Tab.Screen name='Discovery' component={DiscoveryScreen} />
            <Tab.Screen name='Feed' component={FeedScreen} />
            <Tab.Screen name='Profile' component={ProfileScreen} />
        </Tab.Navigator>
    );
}

// TODO if you navigate from Activity to Notifications, then to another screen such as Feed, when navigating back to Activity, you will be directed to Notifiations. You should be directed back to Activity instead of last opened navigation screen.

function ActivityStackScreen () {
    return (
        <ActivityStack.Navigator
            screenOptions={() => ({
                headerShown: false,

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
            <ActivityStack.Screen name="Notifications" component={NotificationsScreen} />
        </ActivityStack.Navigator>
    );
}

function Account () {
    return(
        <AccountStack.Navigator initialRouteName="Account" screenOptions={{headerShown: true}}>
            <AccountStack.Screen name="Account" component={AccountScreen} />
        </AccountStack.Navigator>
    );
}

// ------- END ------------

// v1.0.0

// const Tab = createBottomTabNavigator<BottomTabParamList>();

// // TODO If possible, set the tab navigator horizontal margin to 24, probably with tabBarStyle

// const MainContainer = () => {
//     return (
//         <SafeAreaProvider>
//             <NavigationContainer>
//                 <Tab.Navigator
//                     sceneContainerStyle={globalStyles.mainContainer}
//                     screenOptions={({route}) => ({

//                         tabBarIcon: ({focused, color, size}) => {
//                             let iconName;
//                             let rn = route.name;

//                             if (rn === 'Activity') {
//                                 iconName = focused ? 'activity-fill' : 'activity-fill'
//                             } else if (rn === 'Discovery') {
//                                 iconName = focused ? 'discovery-fill' : 'discovery-fill'
//                             } else if (rn === 'Feed') {
//                                 iconName = focused ? 'feed-fill' : 'feed-fill'
//                             } else if (rn === 'Account') {
//                                 iconName = focused ? 'account-fill' : 'account-fill'
//                             }

//                             return <Icon name={iconName} size={size} color={color}/>

//                         },

//                         tabBarActiveTintColor: Colors.turqoise400,
//                         tabBarInactiveTintColor: Colors.sparrowBrown,
//                         tabBarShowLabel: false,
                        
//                         tabBarStyle: {
//                             height: 50,
//                             backgroundColor: Colors.sparrowSand,
//                             borderTopWidth: 2,
//                             borderTopColor: Colors.sparrowDarkBrown,
//                             paddingHorizontal: 0,
//                         },

//                         headerShown: false,
//                     })}>
                    
//                     <Tab.Screen name='Activity' component={ActivityScreen}/>
//                     <Tab.Screen name='Discovery' component={DiscoveryScreen}/>
//                     <Tab.Screen name='Feed' component={FeedScreen}/>
//                     <Tab.Screen name='Account' component={AccountScreen}/>

//                 </Tab.Navigator>
//             </NavigationContainer>
//         </SafeAreaProvider>
//     );
// };

// export default MainContainer