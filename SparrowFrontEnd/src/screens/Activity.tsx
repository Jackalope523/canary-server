import * as React from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { globalStyles } from '../styles/Global';

import { StyleSheet } from 'react-native';

interface ActivityScreenProps {
    navigation: string;
}

export default function ActivityScreen({ navigation }) {
    return (
        <View style={globalStyles.baseContainer}>
            <Text onPress={() => alert('This is the "Activity/Home" screen.')}>Activity/Home Screen</Text>

        </View>
    );
};

const styles = StyleSheet.create({
    containerTest: {
        padding: 20,
        flexDirection: 'row',
        justifyContent: 'space-between',
        flexWrap: 'wrap',
    }
})