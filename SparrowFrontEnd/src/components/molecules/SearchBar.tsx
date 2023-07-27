import { View, TextInput, StyleSheet } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

type Props = {}

const SearchBar = (props: Props) => {
  return (
      <View style={styles.tempTextInput}>
        <Icon name="search-outline" />
        <TextInput placeholder='Search' />
      </View>
  )
}

export default SearchBar

const styles = StyleSheet.create ({
    tempTextInput: {
        backgroundColor: 'lightgrey',
        borderWidth: 2,
        marginHorizontal: 22,
        marginVertical: 16,
        paddingHorizontal: 16,
    }
})