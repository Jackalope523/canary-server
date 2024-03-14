import React from 'react';
import { Button, StyleSheet, View } from 'react-native';
import Animated, { useSharedValue, withSpring } from 'react-native-reanimated';

export default function CircleGrowth() {
  const width = useSharedValue(20);
  const height = useSharedValue(20);
  const borderRadius = useSharedValue(height.value / 2);

  const handlePress = () => {
    width.value = withSpring(width.value + 20);
    height.value = withSpring(height.value + 20);
    borderRadius.value = withSpring(borderRadius.value + 10);
  };

  return (
    <View style={styles.container}>
      <Animated.View
        style={{ ...styles.circle, width, height, borderRadius }}
      />
      <Button onPress={handlePress} title="Click me" />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
  },
  circle: {
    backgroundColor: '#b58df1',
    marginVertical: 40,
  },
});
