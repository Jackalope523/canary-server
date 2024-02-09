import { Dimensions } from 'react-native';

let windowWidth: number;
let windowHeight: number;

windowWidth = Dimensions.get('window').width;
windowHeight = Dimensions.get('window').height;

export const CustomDimensions = {
  // Navigation
  navigationHeight: 50,

  windowHeight,
  windowWidth,
};
