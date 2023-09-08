import { View, Text, Image, StyleSheet } from 'react-native'
import React from 'react'
import { globalStyles } from '../../styles/Global';
import { Spacing } from '../../styles/Spacing';

// TODO rename this file later when you come up with a better name

// TEMP. example imports
const image = {uri: 'https://thenounproject.com/api/private/icons/2157361/edit/?backgroundShape=SQUARE&backgroundShapeColor=%23000000&backgroundShapeOpacity=0&exportSize=752&flipX=false&flipY=false&foregroundColor=%23000000&foregroundOpacity=1&imageFormat=png&rotation=0'};

type Props = {}

const NoNotifications = (props: Props) => {
  return (
    <View style={styles.container}>
        <Image source={image} height={300} width={300} />
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