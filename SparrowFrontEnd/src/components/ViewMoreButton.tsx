import { Pressable, StyleSheet, Text } from 'react-native';
import React, { FC } from 'react';
import { Spacing } from '../styles/SpacingStyles';
import { globalStyles } from '../styles/GlobalStyles';

// Icons
import Chevron from '../assets/icons/chevron-outline.svg';
import { Colors } from '../styles/ColorStyles';

type Props = {
  showAllItems: boolean;
  setShowAllItems: (showAllItems: boolean) => void;
};

const ViewMoreButton: FC<Props> = ({ showAllItems, setShowAllItems }) => {
  const onViewMore = () => {
    console.log('view more button pressed');
    setShowAllItems(!showAllItems);
  };

  return (
    <Pressable style={styles.button} onPress={onViewMore}>
      <Text style={[globalStyles.buttonTextThree, globalStyles.textDark]}>
        View more
      </Text>

      {/* TODO animate later if necessary - might get away without animating though */}
      <Chevron
        width={24}
        height={24}
        fill={Colors.sparrowDarkBrown}
        style={
          showAllItems
            ? { transform: [{ rotate: '180deg' }] }
            : { transform: [{ rotate: '0deg' }] }
        }
      />
    </Pressable>
  );
};

export default ViewMoreButton;

const styles = StyleSheet.create({
  button: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingTop: Spacing.md,
  },
});
