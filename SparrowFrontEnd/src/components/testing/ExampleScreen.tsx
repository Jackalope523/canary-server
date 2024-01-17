import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';

// Buttons
import Button, { ButtonDisplay, ButtonSize, ButtonType } from '../Button';
import ButtonGroup from '../ButtonGroup';

const ExampleScreen = () => {
  return (
    <View style={styles.container}>
      <Text style={[globalStyles.displayTextTwo, globalStyles.textDark]}>
        Examples
      </Text>
      {/* Buttons */}
      <View style={styles.container}>
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
      </View>
    </View>
  );
};

export default ExampleScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 16,
  },
});
