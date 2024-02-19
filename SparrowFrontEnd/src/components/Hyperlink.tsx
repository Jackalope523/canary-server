import { Text } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../styles/GlobalStyles';

interface HyperlinkProps {
  text: string;
  onPress: () => void;
}

export const Hyperlink: React.FC<HyperlinkProps> = ({ text, onPress }) => {
  return (
    <Text style={globalStyles.hyperlink} onPress={onPress}>
      {text}
    </Text>
  );
};

export default Hyperlink;
