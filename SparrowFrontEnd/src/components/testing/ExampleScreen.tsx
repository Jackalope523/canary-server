import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';

// Buttons
import Button, { ButtonDisplay, ButtonSize, ButtonType } from '../Button';
import ButtonGroup from '../ButtonGroup';

// Input fields
import DateOfBirthInput from '../auth/DateOfBirthInput';
import DateOfBirthInput2 from '../auth/DateOfBirthInput2';
import RadioButton from '../RadioButton';

const ExampleScreen = () => {
  return (
    <View style={styles.container}>
      {/* Header */}

      <View style={styles.header}>
        <Text style={[globalStyles.displayTextTwo, globalStyles.textDark]}>
          Examples
        </Text>
        <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
          Keep sections you don't want to test commented out for better
          visibility.
        </Text>
      </View>

      {/* Buttons */}

      {/* <View style={styles.container}>
        <Button
          type={ButtonType.Warning}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Warning Button'}
          onPress={() => {}}
        />
        <Button
          type={ButtonType.Warning}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Disabled Success Button'}
          onPress={() => {}}
          disabled
        />
        <ButtonGroup
          buttonText={['One', 'Two', 'Three']}
          onSelect={(item) => console.log(item)}
        />
      </View> */}

      {/* Input fields */}

      {/* <View style={styles.container}>
        <DateOfBirthInput />
        <DateOfBirthInput2 />
      </View> */}

      {/* Selectors */}
      {/* <View style={styles.container}>
        <RadioButton
          onPress={(item) => console.log(item)}
          buttonText={[
            'Radio button one',
            'Radio button two',
            'Radio button three',
          ]}
        />
      </View> */}
    </View>
  );
};

export default ExampleScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 16,
    paddingBottom: 24,
  },

  header: {
    paddingBottom: 8,
  },
});
