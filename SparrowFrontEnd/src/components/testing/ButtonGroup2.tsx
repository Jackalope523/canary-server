import * as React from 'react';
import { StyleSheet, View } from 'react-native';
import { Spacing } from '../../styles/SpacingStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

/*

THIS COMPONENT IS A WORK IN PROGRESS

This ButtonGroup component differs from the reulgar ButtonGroup component by
being able to use any <Button /> component as a child

*/

// Types
interface ButtonGroup2Props {
  children: React.ReactNode;
}

export const ButtonGroup2: React.FC<ButtonGroup2Props> = ({ children }) => {
  const [selectedId, setSelectedId] = React.useState(0);

  return <View style={styles.container}>{children}</View>;
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    gap: Spacing.md,
  },
});

export default ButtonGroup2;
