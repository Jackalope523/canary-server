import { View, Text, Image, StyleSheet } from 'react-native'
import React from 'react'
import { globalStyles } from '../../styles/Global';
import { Spacing } from '../../styles/Spacing';

// TODO rename this file later when you come up with a better name

type Props = {}

const NoNotifications = (props: Props) => {
  return (
    <View style={styles.container}>
        <View style={{}}>
        <Image source={require('../../assets/illustrations/temp/not-found.png')} style={[globalStyles.illustration, globalStyles.illustration.large]} />
        </View>
        <View style={styles.textWrapper}>
            <Text style={[globalStyles.headingTextThree, globalStyles.textDark, styles.textAlign]}>You don't have any notifications yet.</Text>
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark, styles.textAlign]}>We'll notify you when you get invited to events and during other occurances.</Text>
        </View>
    </View>
  )
}

export default NoNotifications

const styles = StyleSheet.create ({
    container: {
        alignItems: 'center',
        justifyContent: 'center',
    },

    textAlign: {
        textAlign: 'center',
    },

    textWrapper: {
        marginTop: Spacing.xl,
        rowGap: Spacing.sm,
    },
});