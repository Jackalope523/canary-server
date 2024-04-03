import { StyleSheet, View } from 'react-native';
import React, { FC } from 'react';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';

interface LocationIndicatorProps {
  data: any[];
  selectedIndex?: number;
}

const LocationIndicator: FC<LocationIndicatorProps> = ({
  data,
  selectedIndex,
}) => {
  return (
    <View style={styles.container}>
      <View style={styles.innerContainer}>
        {data.map((_item: any, index: React.Key | null | undefined) => (
          <View
            data={data}
            key={index}
            style={[
              selectedIndex === index ? styles.selected : styles.rest,
              styles.indicator,
            ]}
          />
        ))}
      </View>
    </View>
  );
};

export default LocationIndicator;

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
  },

  innerContainer: {
    flexDirection: 'row',
    columnGap: Spacing.sm,
    position: 'absolute',
    zIndex: 2,
    bottom: 0,
    paddingVertical: 8,
    paddingHorizontal: 8,
  },

  indicator: {
    height: 8,
    width: 8,
    borderRadius: 8,
  },

  rest: {
    backgroundColor: Colors.sparrowSand,
  },

  selected: {
    // TODO this is currently set to orange500 as a visual improvement; OG was sparrowDarkBrown
    backgroundColor: Colors.orange500,
  },
});
