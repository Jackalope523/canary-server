import { StyleSheet, Text, TextStyle, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../styles/GlobalStyles';

export interface HighlightProps {
  type?: HighlightType;
  // size?: HighlightSize;
  children?: React.ReactNode;

  textStyle?: TextStyle[];
}

// TODO delete the switch (size), if not needed

const Highlight: React.FC<HighlightProps> = ({
  children,
  textStyle = [],
  type = null,
  // size = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||
  switch (type) {
    // Bold styles in globalStyles -> Highlights might interfere with the parent text style; monitor for issues
    case HighlightType.Dark:
      textStyle = [globalStyles.highlightDark];
      break;

    case HighlightType.Light:
      textStyle = [globalStyles.highlightLight];
      break;

    case HighlightType.Yellow:
      textStyle = [globalStyles.highlightYellow];
      break;

    case HighlightType.Orange:
      textStyle = [globalStyles.highlightOrange];
      break;

    case HighlightType.Red:
      textStyle = [globalStyles.highlightRed];
      break;

    case HighlightType.Rose:
      textStyle = [globalStyles.highlightRose];
      break;

    case HighlightType.Fuchsia:
      textStyle = [globalStyles.highlightFuchsia];
      break;

    case HighlightType.Lavender:
      textStyle = [globalStyles.highlightLavender];
      break;

    case HighlightType.Green:
      textStyle = [globalStyles.highlightGreen];
      break;

    case HighlightType.Turqoise:
      textStyle = [globalStyles.highlightTurqoise];
      break;

    case HighlightType.Picton:
      textStyle = [globalStyles.highlightPicton];
      break;

    case HighlightType.Azure:
      textStyle = [globalStyles.highlightAzure];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  // switch (size) {
  //   case HighlightSize.DisplayTextOne:
  //     textStyle = [...textStyle, globalStyles.displayTextOne];
  //     break;

  //   case HighlightSize.DisplayTextTwo:
  //     textStyle = [...textStyle, globalStyles.displayTextTwo];
  //     break;

  //   case HighlightSize.HeadingTextOne:
  //     textStyle = [...textStyle, globalStyles.headingTextOne];
  //     break;

  //   case HighlightSize.HeadingTextTwo:
  //     textStyle = [...textStyle, globalStyles.headingTextTwo];
  //     break;

  //   case HighlightSize.HeadingTextThree:
  //     textStyle = [...textStyle, globalStyles.headingTextThree];
  //     break;

  //   case HighlightSize.HeadingTextFour:
  //     textStyle = [...textStyle, globalStyles.headingTextFour];
  //     break;

  //   case HighlightSize.HeadingTextFive:
  //     textStyle = [...textStyle, globalStyles.headingTextFive];
  //     break;

  //   case HighlightSize.BodyTextOne:
  //     textStyle = [...textStyle, globalStyles.bodyTextOne];
  //     break;

  //   case HighlightSize.BodyTextTwo:
  //     textStyle = [...textStyle, globalStyles.bodyTextTwo];
  //     break;

  //   case HighlightSize.LabelTextOneAsTyped:
  //     textStyle = [...textStyle, globalStyles.labelTextOneAsTyped];
  //     break;

  //   case HighlightSize.LabelTextOneAsItalic:
  //     textStyle = [...textStyle, globalStyles.labelTextOneItalic];
  //     break;

  //   case HighlightSize.LabelTextOneTitleCase:
  //     textStyle = [...textStyle, globalStyles.labelTextOneTitlecase];
  //     break;

  //   case HighlightSize.LabelTextOneUppercase:
  //     textStyle = [...textStyle, globalStyles.labelTextOneUppercase];
  //     break;

  //   case HighlightSize.LabelTextTwoAsTyped:
  //     textStyle = [...textStyle, globalStyles.labelTextTwoAsTyped];
  //     break;

  //   case HighlightSize.LabelTextTwoItalic:
  //     textStyle = [...textStyle, globalStyles.labelTextTwoItalic];
  //     break;

  //   case HighlightSize.LabelTextTwoUppercase:
  //     textStyle = [...textStyle, globalStyles.labelTextTwoUppercase];
  //     break;
  // }

  // If the white space around {children} breaks just use {' '} wherever the text is put,
  return <Text style={textStyle}> {children} </Text>;
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum HighlightType {
  Dark,
  Light,
  Yellow,
  Orange,
  Red,
  Rose,
  Fuchsia,
  Lavender,
  Green,
  Turqoise,
  Picton,
  Azure,
}

// export enum HighlightSize {
//   DisplayTextOne,
//   DisplayTextTwo,
//   HeadingTextOne,
//   HeadingTextTwo,
//   HeadingTextThree,
//   HeadingTextFour,
//   HeadingTextFive,
//   BodyTextOne,
//   BodyTextTwo,
//   LabelTextOneAsTyped,
//   LabelTextOneAsItalic,
//   LabelTextOneTitleCase,
//   LabelTextOneUppercase,
//   LabelTextTwoAsTyped,
//   LabelTextTwoItalic,
//   LabelTextTwoUppercase,
// }

const styles = StyleSheet.create({});

export default Highlight;
