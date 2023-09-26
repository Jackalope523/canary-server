import { View, TextInput, StyleSheet, Pressable } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { NavigationState, useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { StackParamList } from '../atoms/types';
import { StackNavigationProp } from '@react-navigation/stack';

const Icon = createIconSetFromFontello(fontelloConfig);

// THIS FILE IS CURRENTLY NOT IN USE

const SearchBar = () => {
  const navigation = useNavigation<NativeStackNavigationProp<StackParamList>>();

  return (
    // <View>
    //   <Pressable onPress={() => navigation.navigate('DiscoverySearch')}>
    //     <View pointerEvents='none'>
    //       <TextInput />
    //     </View>
    //   </Pressable>
    // </View>
      <View>
        <TextInput
          placeholder='Search'
          style={styles.tempTextInput}
          onPressIn={() => navigation.navigate('DiscoverySearch')}
        >
          <Icon name="search-outline"/>
        </TextInput>
      </View>
  );
};

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