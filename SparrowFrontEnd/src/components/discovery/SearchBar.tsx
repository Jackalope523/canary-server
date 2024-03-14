import { View, TextInput, StyleSheet, Pressable } from 'react-native';
import * as React from 'react';
import { NavigationState, useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { StackParamList } from '../atoms/types';
import { StackNavigationProp } from '@react-navigation/stack';

// Icons
import SearchOutline from '../../assets/icons/search-outline.svg';
import { Colors } from '../../styles/ColorStyles';

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
        placeholder="Search"
        style={styles.tempTextInput}
        onPressIn={() => navigation.navigate('DiscoverySearch')}>
        <SearchOutline height={24} width={24} fill={Colors.sparrowDarkBrown} />
      </TextInput>
    </View>
  );
};

export default SearchBar;

const styles = StyleSheet.create({
  tempTextInput: {
    backgroundColor: 'lightgrey',
    borderWidth: 2,
    marginHorizontal: 22,
    marginVertical: 16,
    paddingHorizontal: 16,
  },
});
