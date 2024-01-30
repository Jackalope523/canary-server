import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';

const TestScreen = () => {
  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text
          style={[
            globalStyles.displayTextTwo,
            globalStyles.textDark,
            { textAlign: 'center' },
          ]}>
          Testing screen
        </Text>
      </View>
    </View>
  );
};

export default TestScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 16,
  },

  header: {
    margin: Spacing.lg,
  },
});
