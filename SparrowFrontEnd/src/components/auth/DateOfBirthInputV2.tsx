import * as React from 'react';
import { TextInput, StyleSheet, Text, View, Pressable } from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import TextInputSmall, { InputType } from '../TextInputSmall';
import { Spacing } from '../../styles/SpacingStyles';
import Dropdown from '../Dropdown';
import { MONTHS } from '../../data/auth/months';

const Icon = createIconSetFromFontello(fontelloConfig);

/*

Use this input field component (DateOfBirthInput2) instead of DateOfBirthInput, in case DateOfBirthInput
fails after user testing. This input is pretty solid UX-wise and should fit on all screens.

*/

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface DateOfBirthInputV2Props {}

export const DateOfBirthInputV2: React.FC<DateOfBirthInputV2Props> = ({}) => {
  const [validDay, setValidDay] = React.useState(false);
  const [validYear, setValidYear] = React.useState(false);
  const [Day, setDay] = React.useState('');
  const [Year, setYear] = React.useState('');

  // TODO for the DAY and YEAR text inputs, might want to disable the clear text button

  return (
    <View style={styles.container}>
      <View style={styles.inputWrapper}>
        <TextInputSmall
          type={InputType.Day}
          label="Day"
          valid={validDay}
          setValid={setValidDay}
          text={Day}
          setText={setDay}
          inputMode="numeric"
          maxLength={2}
          clearButton={false}
        />
      </View>

      {/* TODO replace with Month instead of Day as an input type */}
      <View style={styles.inputWrapper}>
        <TextInputSmall
          type={InputType.Day}
          label="Month"
          valid={validDay}
          setValid={setValidDay}
          text={Day}
          setText={setDay}
          inputMode="numeric"
          maxLength={2}
          clearButton={false}
        />
      </View>

      <View style={styles.inputWrapper}>
        <TextInputSmall
          type={InputType.Year}
          label="Year"
          valid={validYear}
          setValid={setValidYear}
          text={Year}
          setText={setYear}
          inputMode="numeric"
          maxLength={4}
          clearButton={false}
        />
      </View>
    </View>
  );
};

export default DateOfBirthInputV2;

const styles = StyleSheet.create({
  inputWrapper: {
    flex: 1,
  },

  container: {
    maxWidth: '100%',
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  dropdownContentAlignment: {
    alignSelf: 'center',
  },

  // This still doesn't solve the issue with dropdown styling
  // - pushes the icon component outside the dropdown
  containerFlexValue: {
    flex: 1.5,
  },
});
