import * as React from 'react';
import { View, Text } from 'react-native';

export default function AccountScreen({ navigation }) {
    return (
        <View>
            <Text onPress={() => navigation.navigate('Home')}>Account Screen</Text>
        </View>
    );
};