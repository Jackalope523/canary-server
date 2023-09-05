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
import ActivityScreen from '../screens/Activity';
import DiscoveryScreen from '../screens/Discovery';
import FeedScreen from '../screens/Feed';
import AccountScreen from '../screens/Account';
import DiscoverySearchScreen from '../screens/DiscoverySearch';
import NotificationsScreen from '../screens/Notifications';

// More imports
import { BottomTabParamList, StackParamList } from '../components/atoms/types';
import { SafeAreaProvider } from 'react-native-safe-area-context';

// TEMP. testing
import TopNavbarFavorite from '../components/organisms/TopNavbarFavorite';

// v1.0.1

const ActivityStack = createStackNavigator();

// TODO if you navigate from Activity to Notifications, then to another screen such as Feed, when navigating back to Activity, you will be directed to Notifiations. You should be directed back to Activity instead of last opened navigation screen.

function ActivityStackScreen () {
    return (
        <ActivityStack.Navigator
            screenOptions={() => ({
                // headerShown: false,

                headerTitleStyle: {
                    fontSize: 16,
                    color: Colors.sparrowDark,
                },

                // TODO set margin to 24
                headerTitleContainerStyle: {
                    marginHorizontal: 24,
                },

                headerLeftContainerStyle: {
                    marginLeft: 0,
                },

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

const Tab = createBottomTabNavigator<BottomTabParamList>();

// TODO If possible, set the tab navigator horizontal margin to 24, probably with tabBarStyle

const MainContainer = () => {
    return (
        <SafeAreaProvider>
            <NavigationContainer>
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
                            } else if (rn === 'Account') {
                                iconName = focused ? 'account-fill' : 'account-fill'
                            }

                            return <Icon name={iconName} size={size} color={color}/>

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

                        // headerShown: false,
                    })}>
                    
                    <Tab.Screen
                        name='Activity'
                        component={ActivityStackScreen}/>
                    <Tab.Screen name='Discovery' component={DiscoveryScreen}/>
                    <Tab.Screen name='Feed' component={FeedScreen}/>
                    <Tab.Screen name='Account' component={AccountScreen}/>

                </Tab.Navigator>
            </NavigationContainer>
        </SafeAreaProvider>
    );
};

export default MainContainer

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