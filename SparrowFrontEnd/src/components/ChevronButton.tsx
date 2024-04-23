import { Pressable, StyleSheet } from 'react-native';
import React, { forwardRef, useImperativeHandle } from 'react';
import ChevronIcon from '../assets/icons/chevron-outline.svg';
import { Colors } from '../styles/ColorStyles';
import Animated, { useAnimatedStyle, useSharedValue, withTiming } from 'react-native-reanimated';
import { Duration } from '../utils/AnimationUtils';

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
      rotation.value = withTiming(rotation.value === 0 ? 180 : 0, { duration: Duration.medium });
    };

    // TODO apparently this is not the best method and useImperativeHandle shouldn't be used - implement a better method
    useImperativeHandle(ref, () => ({
      rotate: rotateChevron,
    }));

    const animatedStyle = useAnimatedStyle(() => ({
      transform: [{ rotate: `${rotation.value}deg` }],
    }));

    return (
      <Animated.View style={animatedStyle}>
        <ChevronIcon height={size} width={size} fill={color} />
      </Animated.View>
    );
  },
);

export default ChevronButton;

const styles = StyleSheet.create({});
