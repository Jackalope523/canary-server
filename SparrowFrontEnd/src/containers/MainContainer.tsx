import * as React from 'react';
import {View, Text} from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

export default function MainContainer(){
    return (
        <View>
            <Text>Hello</Text>
            <Icon name="activity-fill" size={80} color="#bf1313" />
        </View>
    )
}