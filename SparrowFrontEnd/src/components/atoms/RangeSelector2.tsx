import { StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { GestureHandlerRootView } from 'react-native-gesture-handler'
import { Colors } from '../../styles/Colors'

// Range Selector v2

const RangeSelector2 = () => {
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
        <View>
            <Text>RangeSelector2</Text>
        </View>
    </GestureHandlerRootView>
  )
}

export default RangeSelector2

const styles = StyleSheet.create({
})