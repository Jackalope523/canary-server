
import { StyleSheet } from 'react-native';

export default StyleSheet.create({
    sectionContainer: {
      marginTop: 32,
      paddingHorizontal: 24,
      justifyContent: 'flex-end',
      flex: 1,
    },
    sectionTitle: {
      fontSize: 24,
      fontWeight: '600',
    },
    sectionDescription: {
      marginTop: 8,
      fontSize: 18,
      fontWeight: '400',
    },
    inputField: {
      marginVertical: 20,
      backgroundColor: '#bbb',
    },
    errorText: {
      color: 'red',
    },
    footer: {
      height: 100,
    },
    eventSegment: {
      flex: 1,
      height: 80,
      backgroundColor: '#ddd',
      marginBottom: 10,
      marginRight: 10,
      shadowColor: '#111',
      shadowRadius: 3,
      elevation: 3,
      zIndex: 999
    },
  });