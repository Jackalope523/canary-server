import * as React from 'react';
import { View, Text } from 'react-native';

export default function ActivityScreen({ navigation }) {
    return (
        <View>
            <Text onPress={() => alert('This is the "Activity/Home" screen.')}>Activity/Home Screen</Text>
        </View>
    );
};