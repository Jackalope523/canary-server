import {
  StyleSheet,
  SafeAreaView,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StatusBar,
} from 'react-native';
import React from 'react';
import { Spacing } from '../styles/SpacingStyles';

interface KeyboardAvoidingContainerProps {
  children: React.ReactNode;
}

export const KeyboardAvoidingContainer: React.FC<
  KeyboardAvoidingContainerProps
> = ({ children }) => {
  return (
    <SafeAreaView>
      <KeyboardAvoidingView
        style={styles.container}
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}>
        {children}
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
};

export default KeyboardAvoidingContainer;

const styles = StyleSheet.create({
  container: {},
});
