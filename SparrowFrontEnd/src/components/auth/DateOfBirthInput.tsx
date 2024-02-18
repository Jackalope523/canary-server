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

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface DateOfBirthInputProps {
  day: string;
  month: string;
  year: string;

  validDay: boolean;
  validMonth: boolean;
  validYear: boolean;

  setValidDay: React.Dispatch<React.SetStateAction<boolean>>;
  setValidMonth: React.Dispatch<React.SetStateAction<boolean>>;
  setValidYear: React.Dispatch<React.SetStateAction<boolean>>;

  setDay: React.Dispatch<React.SetStateAction<string>>;
  setMonth: React.Dispatch<React.SetStateAction<string>>;
  setYear: React.Dispatch<React.SetStateAction<string>>;
}

export const DateOfBirthInput: React.FC<DateOfBirthInputProps> = ({
  day,
  month,
  year,

  validDay,
  validMonth,
  validYear,

  setValidDay,
  setValidMonth,
  setValidYear,

  setDay,
  setMonth,
  setYear
}) => {
  // TODO for the DAY and YEAR text inputs, might want to disable the clear text button

  return (
    <View style={styles.container}>
      <View style={{ flex: 1 }}>
        <TextInputSmall
          type={InputType.Day}
          valid={validDay}
          setValid={setValidDay}
          label="Day"
          text={day}
          setText={setDay}
          inputMode="numeric"
          maxLength={2}
          clearButton={false}
        />
      </View>
      <Dropdown
        label="Month"
        data={MONTHS}
        valid={validMonth}
        setValid={setValidMonth}
        value={month}
        setValue={setMonth}
        dropdownContentAlignment={styles.dropdownContentAlignment}
        containerFlexValue={styles.containerFlexValue}
      />
      <View style={{ flex: 1 }}>
        <TextInputSmall
          type={InputType.Year}
          label="Year"
          valid={validYear}
          setValid={setValidYear}
          text={year}
          setText={setYear}
          inputMode="numeric"
          maxLength={4}
          clearButton={false}
        />
      </View>
    </View>
  );
};

export default DateOfBirthInput;

const styles = StyleSheet.create({
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
