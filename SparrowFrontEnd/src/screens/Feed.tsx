import * as React from 'react';
import { View, Text } from 'react-native';

export default function FeedScreen({ navigation }) {
    return (
        <View>
            <Text onPress={() => navigation.navigate('Home')}>Feed Screen</Text>
        </View>
    );
};