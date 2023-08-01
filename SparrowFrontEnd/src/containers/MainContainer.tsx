import * as React from 'react';

// Colors
import { Colors } from '../styles/Colors';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

// Navigation
import { NavigationContainer } from '@react-navigation/native';
import { BottomTabBarProps, createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

// Screens
import ActivityScreen from '../screens/Activity';
import DiscoveryScreen from '../screens/Discovery';
import FeedScreen from '../screens/Feed';
import AccountScreen from '../screens/Account';
import DiscoverySearchScreen from '../screens/DiscoverySearch';
import { BottomTabParamList, StackParamList } from '../components/atoms/types';

// NEW VERSION
const Tab = createBottomTabNavigator<BottomTabParamList>();
// const Stack = createNativeStackNavigator<StackParamList>();

// const DiscoverySearchContainer = () => {
//     return (
//         <NavigationContainer>
//             <Stack.Navigator>
//                 <Stack.Screen
//                     name='DiscoverySearch'
//                     component={DiscoverySearchScreen}
//                 />
//             </Stack.Navigator>
//         </NavigationContainer>
//     );
// };

const MainContainer = () => {
    return (
        <NavigationContainer>
             <Tab.Navigator
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

                    tabBarActiveTintColor: Colors.orange400,
                    tabBarInactiveTintColor: Colors.sparrowBrown,
                    tabBarShowLabel: false,
                })}>
                
                <Tab.Screen name='Activity' component={ActivityScreen}/>
                <Tab.Screen name='Discovery' component={DiscoveryScreen}/>
                <Tab.Screen name='Feed' component={FeedScreen}/>
                <Tab.Screen name='Account' component={AccountScreen}/>

            </Tab.Navigator>
        </NavigationContainer>
    );
};

export default MainContainer

// OLD VERSION

// Screen names
// const activityName = 'Activity';
// const discoveryName = 'Discovery';
// const feedName = 'Feed';
// const accountName = 'Account';
// const DiscoverySearchName = 'DiscoverySearch';

// const Tab = createBottomTabNavigator();
// const Stack = createNativeStackNavigator();

// function MainContainer(){
//     return (
//         <NavigationContainer>
//             <Tab.Navigator
//                 initialRouteName={activityName}
//                 screenOptions={({route}) => ({
//                     tabBarIcon: ({focused, color, size}) => {
//                         let iconName;
//                         let rn = route.name;

//                         if (rn === activityName) {
//                             iconName = focused ? 'activity-fill' : 'activity-fill'
//                         } else if (rn === discoveryName) {
//                             iconName = focused ? 'discovery-fill' : 'discovery-fill'
//                         } else if (rn === feedName) {
//                             iconName = focused ? 'feed-fill' : 'feed-fill'
//                         } else if (rn === accountName) {
//                             iconName = focused ? 'account-fill' : 'account-fill'
//                         }

//                         return <Icon name={iconName} size={size} color={color}/>

//                     },

//                     tabBarActiveTintColor: Colors.orange400,
//                     tabBarInactiveTintColor: Colors.sparrowBrown,
//                     tabBarShowLabel: false,
//                 })}>
                
//                 <Tab.Screen name={activityName} component={ActivityScreen}/>
//                 <Tab.Screen name={discoveryName} component={DiscoveryScreen}/>
//                 <Tab.Screen name={feedName} component={FeedScreen}/>
//                 <Tab.Screen name={accountName} component={AccountScreen}/>

//             </Tab.Navigator>
//         </NavigationContainer>
//     );
// }

// function DiscoverySearchContainer(){
//     return (
//         <NavigationContainer>
//             <Stack.Navigator screenOptions={{headerShown: false}}>
//                 <Stack.Screen name={DiscoverySearchName} component={DiscoverySearchScreen} />
//             </Stack.Navigator>
//         </NavigationContainer>
//     );
// }

// export default MainContainer