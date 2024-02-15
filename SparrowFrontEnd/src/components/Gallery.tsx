import { FlatList, Image, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../styles/GlobalStyles';
import TextButton, { TextButtonType, TextButtonVariant } from './TextButton';
import { SAMPLE_PAST_EVENT_DATA } from '../data/samplePastEventData';
import { CustomDimensions } from '../styles/CustomDimensionStyles';
import { Colors } from '../styles/ColorStyles';
import { Spacing } from '../styles/SpacingStyles';

// Icons
import smLayout from '../assets/icons/layout-size-small-fill.svg';
import mdLayout from '../assets/icons/layout-size-medium-fill.svg';
import lgLayout from '../assets/icons/layout-size-large-fill.svg';

// TODO fix "VirtualizedLists should never be nested inside plain ScrollViews with the same orientation - use another VirtualizedList-backed container instead."

interface GalleryProps {}

const Gallery: React.FC<GalleryProps> = () => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Layout                                     ||
  // ! ||--------------------------------------------------------------------------------||
  const [numColumns, setNumColumns] = React.useState(1);

  const changeLayout = () => {
    console.log('Change layout button pressed');

    if (numColumns === 1) {
      setNumColumns(2);
      console.log('Set from:', numColumns, 'to', numColumns + 1);
    } else if (numColumns === 2) {
      setNumColumns(3);
      console.log('Set from:', numColumns, 'to', numColumns + 1);
    } else if (numColumns === 3) {
      setNumColumns(1);
      console.log('Set from:', numColumns, 'to', numColumns - 2);
    }
  };

  // Image size
  const imgSize = {
    full: CustomDimensions.windowWidth - 24 * 2,
  };

  // Column layouts

  const oneCol = {
    width: imgSize.full,
    height: imgSize.full,
  };

  const twoCol = {
    width: imgSize.full / 2 - Spacing.md / 2,
    height: imgSize.full / 2 - Spacing.md / 2,
  };

  const threeCol = {
    width: (imgSize.full - Spacing.md * 2) / 3,
    height: (imgSize.full - Spacing.md * 2) / 3,
  };

  return (
    <View style={styles.container}>
      <View style={styles.top}>
        <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
          Gallery
        </Text>
        <View>
          <TextButton
            text="Layout"
            type={TextButtonType.Dark}
            variant={TextButtonVariant.Four}
            displayIcon={true}
            Icon={
              numColumns === 1
                ? mdLayout
                : numColumns === 2
                ? lgLayout
                : smLayout
            }
            onPress={changeLayout}
          />
        </View>
      </View>

      <FlatList
        data={SAMPLE_PAST_EVENT_DATA[0].media}
        renderItem={({ item }) => (
          <View
            style={
              numColumns === 1 ? oneCol : numColumns === 2 ? twoCol : threeCol
            }>
            <Image source={item} style={styles.image} />
          </View>
        )}
        keyExtractor={(item, index) => index.toString()}
        numColumns={numColumns}
        key={numColumns}
        ItemSeparatorComponent={
          numColumns === 1
            ? () => <View style={styles.oneColSeparator} />
            : numColumns === 2
            ? () => <View style={styles.twoColSeparator} />
            : () => <View style={styles.threeColSeparator} />
        }
        columnWrapperStyle={
          numColumns === 1
            ? false
            : numColumns === 2
            ? styles.twoColWrapper
            : styles.threeColWrapper
        }
      />
    </View>
  );
};

// Exported enums
export enum GalleryLayout {
  One,
  Two,
  Three,
}

export default Gallery;

const styles = StyleSheet.create({
  container: {},

  top: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingBottom: Spacing.md,
  },

  // Image
  image: {
    width: '100%',
    height: '100%',
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Layouts
  // One column
  oneColSeparator: {
    height: Spacing.lg,
  },

  // Two columns
  twoColWrapper: {
    columnGap: Spacing.md,
  },
  twoColSeparator: {
    height: Spacing.md,
  },

  // Three columns
  threeColWrapper: {
    columnGap: Spacing.md,
  },
  threeColSeparator: {
    height: Spacing.md,
  },
});
