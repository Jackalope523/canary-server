import { Pressable, StyleSheet, Text, View } from 'react-native';
import React from 'react';

/*

Currently only used for PREVIOUSLY ATTENDED / PAST EVENTS

*/

// Icons
import DateIcon from '../assets/icons/date-outline.svg';
import ArrowIcon from '../assets/icons/arrow-up-outline-alt.svg';
import ChevronIcon from '../assets/icons/chevron-outline.svg';
// TODO Imports (organize later)
import { Colors } from '../styles/ColorStyles';
import ActionButton, {
  ActionButtonDisplay,
  ActionButtonSize,
  ActionButtonType,
} from './ActionButton';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { borderRadius } from '../styles/BorderStyles';
import ChevronButton, { ChevronButtonHandle } from './ChevronButton';

interface EventCardSmallAccordionProps {
  eventTitle: string;
  eventDate: string;
  onPressViewEvent: () => void;
  onPressCard: () => void;
}

const EventCardSmallAccordion: React.FC<EventCardSmallAccordionProps> = ({
  eventTitle = 'NULL',
  eventDate = 'NULL',
  onPressViewEvent,
  onPressCard,
}) => {
  const [isExpanded, setIsExpanded] = React.useState(true);
  const chevronRef = React.useRef<ChevronButtonHandle>(null);

  const handlePress = () => {
    setIsExpanded(!isExpanded);
    chevronRef.current?.rotate();
    console.log('Accordion card pressed.');
    // Notify parent component that handlePress is pressed
    onPressCard();
  };

  return (
    <View style={styles.eventCardSmallAccordion}>
      <View style={styles.top}>
        <Pressable style={styles.eventTitle} onPress={handlePress}>
          <Text
            style={[
              globalStyles.bodyTextOne,
              globalStyles.textDark,
              // styles.eventTitle,
            ]}>
            {eventTitle}
          </Text>
        </Pressable>
        {/* TODO fix ActionButton TS error */}
        <ActionButton
          Icon={ArrowIcon}
          display={ActionButtonDisplay.Contained}
          type={ActionButtonType.SecondaryLight}
          size={ActionButtonSize.Small}
          disabled={false}
          // TODO add onpress for view event button
          onPress={onPressViewEvent}
        />
      </View>
      <Pressable style={styles.bottom} onPress={handlePress}>
        <View style={styles.date}>
          <DateIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
          <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
            {eventDate}
          </Text>
        </View>
        <ChevronButton
          ref={chevronRef}
          size={24}
          color={Colors.sparrowDarkBrown}
        />
      </Pressable>
    </View>
  );
};

export default EventCardSmallAccordion;

const styles = StyleSheet.create({
  eventCardSmallAccordion: {
    padding: Spacing.md,
    borderWidth: 2,
    borderRadius: borderRadius.md,
    borderColor: Colors.sparrowDarkBrown,
    rowGap: Spacing.md,
    backgroundColor: Colors.sparrowSand,
  },

  top: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    flex: 1,
  },

  eventTitle: {
    flex: 1,
  },

  bottom: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },

  date: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },
});
