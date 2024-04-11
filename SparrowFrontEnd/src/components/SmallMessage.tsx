import {
  LayoutChangeEvent,
  StyleSheet,
  Text,
  TextStyle,
  View,
} from 'react-native';
import React from 'react';
import { Colors } from '../styles/ColorStyles';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';

// Icons
import errorWarningIcon from '../assets/icons/error-fill.svg';
import infoIcon from '../assets/icons/info-fill.svg';
import successIcon from '../assets/icons/success-fill.svg';

interface SmallMessageProps {
  type: SmallMessageType;
  Icon: React.FC<any> | string | any;
  iconStyle: string;
  textStyle: TextStyle[];
  message: string;
}

export const SmallMessage: React.FC<SmallMessageProps> = ({
  type,
  Icon,
  iconStyle,
  textStyle,
  message = 'NULL',
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Layout                                     ||
  // ! ||--------------------------------------------------------------------------------||
  const [multiline, setMultiline] = React.useState(false);

  const handleTextLayout = (event: LayoutChangeEvent) => {
    const { lines } = event.nativeEvent;
    setMultiline(lines.length > 1);
  };

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||
  switch (type) {
    case SmallMessageType.Error:
      textStyle = [globalStyles.textErrorDarker, globalStyles.bodyTextTwo];
      Icon = errorWarningIcon;
      iconStyle = Colors.red400;
      break;

    case SmallMessageType.Warning:
      textStyle = [globalStyles.textWarningDarker, globalStyles.bodyTextTwo];
      Icon = errorWarningIcon;
      iconStyle = Colors.orange400;
      break;

    case SmallMessageType.Info:
      textStyle = [globalStyles.textFunctionDarker, globalStyles.bodyTextTwo];
      Icon = infoIcon;
      iconStyle = Colors.turqoise400;
      break;

    case SmallMessageType.Success:
      textStyle = [globalStyles.textSuccessDarker, globalStyles.bodyTextTwo];
      Icon = successIcon;
      iconStyle = Colors.green400;
      break;
  }

  return (
    <View
      style={[
        styles.smallMessage,
        multiline ? styles.flexStart : styles.center,
      ]}>
      <Icon height={24} width={24} fill={iconStyle} />
      <Text style={textStyle} onTextLayout={handleTextLayout}>
        {message}
      </Text>
    </View>
  );
};

export enum SmallMessageType {
  Error,
  Warning,
  Info,
  Success,
}

export default SmallMessage;

const styles = StyleSheet.create({
  smallMessage: {
    flexDirection: 'row',
    columnGap: Spacing.sm,
  },

  center: {
    alignItems: 'center',
  },

  flexStart: {
    alignItems: 'flex-start',
  },
});
