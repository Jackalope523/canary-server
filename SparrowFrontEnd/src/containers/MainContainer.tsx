import * as React from 'react';

// Colors
import Colors from '../styles/Colors';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

// Navigation
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

// Screens
import ActivityScreen from '../screens/Activity';
import DiscoveryScreen from '../screens/Discovery';
import FeedScreen from '../screens/Feed';
import AccountScreen from '../screens/Account';
import { StyleSheet } from 'react-native';

// Screen names
const activityName = 'Activity';
const discoveryName = 'Discovery';
const feedName = 'Feed';
const accountName = 'Account';

const Tab = createBottomTabNavigator();

export default function MainContainer(){
    return (
        <NavigationContainer>
            <Tab.Navigator
                initialRouteName={activityName}
                screenOptions={({route}) => ({
                    tabBarIcon: ({focused, color, size}) => {
                        let iconName;
                        let rn = route.name;

                        if (rn === activityName) {
                            iconName = focused ? 'activity-fill' : 'activity-fill'
                        } else if (rn === discoveryName) {
                            iconName = focused ? 'discovery-fill' : 'discovery-fill'
                        } else if (rn === feedName) {
                            iconName = focused ? 'feed-fill' : 'feed-fill'
                        } else if (rn === accountName) {
                            iconName = focused ? 'account-fill' : 'account-fill'
                        }

                        return <Icon name={iconName} size={size} color={color}/>

                    },

                    tabBarActiveTintColor: Colors.orange400,
                    tabBarInactiveTintColor: Colors.sparrowBrown,
                    tabBarShowLabel: false,
                })}>
                
                <Tab.Screen name={activityName} component={ActivityScreen}/>
                <Tab.Screen name={discoveryName} component={DiscoveryScreen}/>
                <Tab.Screen name={feedName} component={FeedScreen}/>
                <Tab.Screen name={accountName} component={AccountScreen}/>

            </Tab.Navigator>
        </NavigationContainer>
    );
}

// Icon styling
const styles = StyleSheet.create({
    icon: {
        color: 'black',
    },
    iconFocused: {
        color: 'red',
    }
})