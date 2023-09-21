import { StyleSheet, Text, Pressable } from 'react-native'
import React from 'react'

const Button = ({onPress, btnText, btnStyle, btnTextStyle}) => {
  return (
    <Pressable onPress={onPress} style={btnStyle}>
        <Text style={btnTextStyle}>{btnText}</Text>
    </Pressable>
  )
}

export default Button

const styles = StyleSheet.create({})