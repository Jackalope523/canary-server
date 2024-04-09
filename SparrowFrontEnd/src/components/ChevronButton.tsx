import { Pressable, StyleSheet } from 'react-native';
import React, { forwardRef, useImperativeHandle } from 'react';
import ChevronIcon from '../assets/icons/chevron-outline.svg';
import { Colors } from '../styles/ColorStyles';
import Animated, { useSharedValue, withTiming } from 'react-native-reanimated';

type ChevronButtonProps = {
  size?: number;
  color?: string;
};

export type ChevronButtonHandle = {
  rotate: () => void;
};

const ChevronButton = forwardRef<ChevronButtonHandle, ChevronButtonProps>(
  ({ size = 24, color = Colors.sparrowDark }, ref) => {
    const rotation = useSharedValue(0);

    const rotateChevron = () => {
      rotation.value = withTiming(rotation.value + 180);
    };

    useImperativeHandle(ref, () => ({
      rotate: rotateChevron,
    }));

    const animatedStyle = {
      transform: [{ rotate: `${rotation.value}deg` }],
    };

    return (
      <Animated.View style={animatedStyle}>
        <ChevronIcon height={size} width={size} fill={color} />
      </Animated.View>
    );
  },
);

export default ChevronButton;

const styles = StyleSheet.create({});
