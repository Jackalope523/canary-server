import * as React from 'react';
import { TextInput, StyleSheet, Text, View, Pressable } from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import TextInputSmall, { InputType } from '../TextInputSmall';
import { Spacing } from '../../styles/SpacingStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface DateOfBirthInputProps {}

export const DateOfBirthInput: React.FC<DateOfBirthInputProps> = ({}) => {
  const [Day, setDay] = React.useState('');
  const [Year, setYear] = React.useState('');

  return (
    <View style={styles.container}>
      <TextInputSmall
        type={InputType.Day}
        label="Day"
        value={Day}
        onChangeText={setDay}
        inputMode="numeric"
        maxLength={2}
      />
      <TextInputSmall
        type={InputType.Year}
        label="Year"
        value={Year}
        onChangeText={setYear}
        inputMode="numeric"
        maxLength={4}
      />
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
});
