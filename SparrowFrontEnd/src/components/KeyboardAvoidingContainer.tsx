import {
  StyleSheet,
  SafeAreaView,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StatusBar,
  Keyboard,
} from 'react-native';
import React from 'react';
import { TouchableWithoutFeedback } from 'react-native-gesture-handler';

import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import { scrollTo } from 'react-native-reanimated';

interface KeyboardAvoidingContainerProps {
  children: React.ReactNode;
}

export const KeyboardAvoidingContainer: React.FC<
  KeyboardAvoidingContainerProps
> = ({ children }) => {
  // TODO Scroll to selected input field on focus

  return (
    <SafeAreaView style={styles.container}>
      <KeyboardAvoidingView
        style={styles.keyboardAvoidingContainer}
        // keyboardVerticalOffset={50}
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}>
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
          <ScrollView
            keyboardShouldPersistTaps="handled"
            overScrollMode="never"
            showsVerticalScrollIndicator={false}>
            {children}
          </ScrollView>
        </TouchableWithoutFeedback>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
};

export default KeyboardAvoidingContainer;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },

  keyboardAvoidingContainer: {
    flex: 1,
    backgroundColor: Colors.fuchsia200,
  },
});
