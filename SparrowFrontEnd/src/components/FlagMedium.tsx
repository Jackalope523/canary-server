import { StyleSheet, Text, TextStyle, View, ViewStyle } from 'react-native';
import React from 'react';
import { Spacing } from '../styles/SpacingStyles';
import { FlagStyles } from '../styles/FlagStyles';
import { globalStyles } from '../styles/GlobalStyles';

/*

TODO test if this component time variable works with StartingLate and StartingSoon flag types
or/and hook up to back-end

*/

interface FlagMediumProps {
  flagText?: string | number;
  flagTextStyle: TextStyle[];
  time?: number | string;

  type: FlagType;
  flagStyle: ViewStyle[];
}

const FlagMedium: React.FC<FlagMediumProps> = ({
  flagTextStyle,
  flagText,
  time,
  type = null,
  flagStyle,
}) => {
  switch (type) {
    /*
    
    TODO this should accept a "how many days until event is live" timer,
    for ex. any of the following options - 1d, 2d, 3d, 4d, 5d, 6d, 7d;
    with 7d being the maximum value and 1d being the minimum value
    
    */
    case FlagType.StartingLate: {
      flagStyle = [FlagStyles.startingLate];
      flagText = `In ${time}`;
      flagTextStyle = [globalStyles.textDark, globalStyles.labelTextOneAsTyped];
      break;
    }

    // TODO this should accept a countdown timer hh:mm:ss
    case FlagType.StartingSoon: {
      flagStyle = [FlagStyles.startingSoon];
      flagText = `In ${time}`;
      flagTextStyle = [globalStyles.textDark, globalStyles.labelTextOneAsTyped];
      break;
    }

    /*
    
        TODO note to myself (Anna) - might want to look into the * symbol and test it out on mobile
        as it seems to look different than in Figma, maybe use an SVG instead of a text symbol

    */
    case FlagType.Live: {
      flagStyle = [FlagStyles.live];
      flagText = `${'*'}live${'*'}`;
      flagTextStyle = [
        globalStyles.textDark,
        globalStyles.labelTextOneUppercase,
      ];
      break;
    }
    case FlagType.Terminated: {
      flagStyle = [FlagStyles.terminated];
      flagText = `${'*'}terminated${'*'}`;
      flagTextStyle = [
        globalStyles.textDark,
        globalStyles.labelTextOneUppercase,
      ];
      break;
    }
  }

  return (
    <View style={[styles.flag, flagStyle]}>
      <Text style={flagTextStyle}>{flagText}</Text>
    </View>
  );
};

export default FlagMedium;

// Exported enums
export enum FlagType {
  StartingLate,
  StartingSoon,
  Live,
  Terminated,
}

const styles = StyleSheet.create({
  flag: {
    paddingHorizontal: 12,
    paddingVertical: Spacing.xs,
    borderRadius: 100,
    alignSelf: 'flex-start',
  },
});
