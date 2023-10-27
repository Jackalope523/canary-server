import React, {useState} from 'react';
import {View, StyleSheet, Text} from 'react-native';
import {GestureHandlerRootView} from 'react-native-gesture-handler';

// Testing components
import {Spacing} from '../styles/Spacing';
import {Colors} from '../styles/Colors';
import RangeSelector from '../components/atoms/testing/RangeSelector';

// Icons font
import {createIconSetFromFontello} from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

import SingleValueSelector from '../components/atoms/testing/SingleValueSelector';
import RangeSlider from '../components/atoms/testing/RangeSlider';
import SingleValueSlider from '../components/atoms/SingleValueSlider';

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
    <View style={styles.container}>
      <SingleValueSlider />
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
  colorBlack: {color: 'black'},

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
