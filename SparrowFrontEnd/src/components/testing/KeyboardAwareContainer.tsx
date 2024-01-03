import { StyleSheet, View } from 'react-native';
import React from 'react';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';

import { globalStyles } from '../../styles/GlobalStyles';

// TODO temp component for testing
// If of no use - uninstall the react-native-keyboard-aware-scroll-view package

interface KeyboardAwareContainerProps {
  children: React.ReactNode;
}

export const KeyboardAwareContainer: React.FC<KeyboardAwareContainerProps> = ({
  children,
}) => {
  return (
    <KeyboardAwareScrollView
      resetScrollToCoords={{ x: 0, y: 0 }}
      scrollEnabled={true}
      // keyboardShouldPersistTaps="handled"
      // contentContainerStyle={[styles.container, globalStyles.baseContainer]}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}>
      {children}
    </KeyboardAwareScrollView>
  );
};

export default KeyboardAwareContainer;

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',

    // TODO Might have to use justifyContent: 'flex-end' instead for KeyboardAvoidingContainer to work
    // justifyContent: 'center',
    // paddingBottom: 48,
  },
});
