import * as React from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { globalStyles } from '../styles/Global';

import { StyleSheet } from 'react-native';

export default function ActivityScreen({ navigation }) {
    return (
        <View style={globalStyles.baseContainer}>
            <Text onPress={() => alert('This is the "Activity/Home" screen.')}>Activity/Home Screen</Text>

            <View style={styles.containerTest}>

            <TouchableOpacity style={[globalStyles.textButtonLarge, globalStyles.textButtonPrimary]}>
                <Text style={globalStyles.textButtonLargeText}>Test Button</Text>
            </TouchableOpacity>

            <TouchableOpacity style={globalStyles.textButtonLarge}>
                <Text>Test Button two</Text>
            </TouchableOpacity>

            </View>

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