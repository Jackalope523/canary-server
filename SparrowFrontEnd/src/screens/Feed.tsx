import * as React from 'react';
import {View, StyleSheet} from 'react-native';

// Testing components
import {Spacing} from '../styles/Spacing';
import {Colors} from '../styles/Colors';
import RangeSelector from '../components/atoms/testing/RangeSelector';

// Icons font
import {createIconSetFromFontello} from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import SingleValueSelector from '../components/atoms/testing/SingleValueSelector';

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

  return (
    <View style={styles.container}>
      <RangeSelector min={1} max={50} steps={1} />
      <SingleValueSelector />
    </View>
  );
};

export default FeedScreen;

const styles = StyleSheet.create({
  // TEMP. styles
  container: {
    gap: Spacing.xl,
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    // marginTop: 125,
  },

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

  // TODO DELETE THIS
  eventCardContainer: {
    marginHorizontal: Spacing.lg,
    flexDirection: 'row',
    columnGap: Spacing.md,
  },
});
