//#region imports
import React from 'react';
import { View } from 'react-native';
import { Colors } from '../styles/ColorStyles';
import { borderRadius } from '../styles/BorderStyles';
//#endregion 

interface ShadowProps {
    height: number;
    width: number;
}

const Shadow: React.FC<ShadowProps> = ({
    height,
    width
}) => {
  return (
    <View style={{
        height: height + 6,
        width: width, 
        backgroundColor: Colors.sparrowDark,
        position:'absolute',
        borderRadius: borderRadius.md
    }}/>
  );
};

export default Shadow;
