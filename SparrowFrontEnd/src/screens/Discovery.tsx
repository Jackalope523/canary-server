import * as React from 'react';
import { View, Text } from 'react-native';

export default function DiscoveryScreen({ navigation }) {
    return (
        <View>
            <Text onPress={() => navigation.navigate('Activity')}>Discovery Screen</Text>
        </View>
    );
};