import React, { useState } from 'react';
import { View, StyleSheet, Text } from 'react-native';
import {
  GestureHandlerRootView,
  ScrollView,
} from 'react-native-gesture-handler';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';

// Testing components
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';

import SingleValueSlider from '../../components/slider/SingleValueSlider';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';

import SearchFilter from '../../components/organisms/SearchFilter';

const Icon = createIconSetFromFontello(fontelloConfig);

const FeedScreen = () => {
  // Activity component testing
  // TODO delete this code below that sets text as overflowing if it exceeds 2 lines. We're not using that anymore.
  // TODO update for Discovery
  // If textWrapper text exceeds 2 lines, align items to flex-start
  // const [isTextOverflowing, setIsTextOverflowing] = useState(false);

  // const handleTextLayout = (event) => {
  //     const { lines } = event.nativeEvent;

  //     setIsTextOverflowing(lines.length > 2);
  // };

  const MIN_DEFAULT = 10;
  const MAX_DEFAULT = 500;
  const [minValue, setMinValue] = useState(MIN_DEFAULT);
  const [maxValue, setMaxValue] = useState(MAX_DEFAULT);

  return ( 
    <View>
      <SearchFilter />
      <View style={styles.container}>
        <SingleValueSlider />

        <Text>Button Contained</Text>
        <Button
          type={ButtonType.PrimaryDark}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="I MADE A BUTTON!"
          btnIcon="settings-outline"
        />
        <Button
          type={ButtonType.SecondaryDark}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />
        <Button
          type={ButtonType.Tertiary}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />
        <Button
          type={ButtonType.Warning}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />
        <Button
          type={ButtonType.Error}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />
        <Button
          type={ButtonType.Function}
          size={ButtonSize.Large}
          display={ButtonDisplay.Contained}
          text="Another one"
        />

        <Text>Button Full</Text>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Small}
          display={ButtonDisplay.Full}
          text="Small Button"
        />
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text="Medium Button"
        />
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Large}
          display={ButtonDisplay.Full}
          text="Large Button"
        />
        <Button
          type={ButtonType.Success}
          size={ButtonSize.ExtraSmall}
          display={ButtonDisplay.Full}
          text="Extra Small Button"
        />
      </View>
    </View>
    // <GestureHandlerRootView style={{flex: 1}}>
    //   <View style={styles.container}>
    //     <View style={styles.contentContainer}>
    //       <View style={styles.content}>
    //         <Text style={styles.text}>Price Slider</Text>
    //         <RangeSlider
    //           sliderWidth={300}
    //           min={MIN_DEFAULT}
    //           max={MAX_DEFAULT}
    //           step={10}
    //           onValueChange={range => {
    //             setMinValue(range.min);
    //             setMaxValue(range.max);
    //           }}
    //         />
    //         <View style={styles.tableContainer}>
    //           <View style={{marginBottom: 20}}>
    //             <Text style={styles.colorBlack}>Min Price</Text>
    //             <View style={styles.table}>
    //               <Text style={styles.colorBlack}>${minValue}</Text>
    //             </View>
    //           </View>
    //           <View>
    //             <Text style={styles.colorBlack}>Max Price</Text>
    //             <View style={styles.table}>
    //               <Text style={styles.colorBlack}>${maxValue}</Text>
    //             </View>
    //           </View>
    //         </View>
    //       </View>
    //     </View>
    //   </View>
    // </GestureHandlerRootView>
  );
};

export default FeedScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#EBECF2',
  },
  contentContainer: {
    width: '90%',
    height: 300,
    backgroundColor: 'white',
    borderRadius: 25,
  },
  content: {
    paddingHorizontal: 16,
    paddingVertical: 16,
    flex: 1,
    justifyContent: 'space-between',
  },
  text: {
    color: 'black',
    fontSize: 20,
  },
  tableContainer: {
    flexDirection: 'column',
    justifyContent: 'space-between',
  },
  table: {
    borderColor: '#EBECF2',
    borderWidth: 1,
    padding: 10,
    marginTop: 5,
    borderRadius: 5,
  },
  colorBlack: { color: 'black' },

  // TEMP. styles
  // container: {
  //   gap: Spacing.xl,
  //   flex: 1,
  //   alignItems: 'center',
  //   justifyContent: 'center',
  //   // marginTop: 125,
  // },

  // contentContainer: {
  //     width: '90%',
  //     height: 300,
  // },

  // other styles
  headingText: {
    color: Colors.sparrowDark,
    marginBottom: Spacing.md,
    marginLeft: Spacing.lg,
  },

  mainWrapper: {
    paddingBottom: Spacing.lg,
  },

  topWrapper: {
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },

  notificationWrapper: {
    alignItems: 'flex-end',
  },
});
