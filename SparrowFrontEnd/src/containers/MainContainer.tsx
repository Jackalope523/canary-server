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
import { createNativeStackNavigator } from '@react-navigation/native-stack';

// Screens
import ActivityScreen from '../screens/Activity';
import DiscoveryScreen from '../screens/Discovery';
import FeedScreen from '../screens/Feed';
import AccountScreen from '../screens/Account';
import DiscoverySearchScreen from '../screens/DiscoverySearch';
import { BottomTabParamList, StackParamList } from '../components/atoms/types';
import { SafeAreaProvider } from 'react-native-safe-area-context';

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

                        tabBarActiveTintColor: Colors.turqoise400,
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
                    
                    <Tab.Screen name='Activity' component={ActivityScreen}/>
                    <Tab.Screen name='Discovery' component={DiscoveryScreen}/>
                    <Tab.Screen name='Feed' component={FeedScreen}/>
                    <Tab.Screen name='Account' component={AccountScreen}/>

                </Tab.Navigator>
            </NavigationContainer>
        </SafeAreaProvider>
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