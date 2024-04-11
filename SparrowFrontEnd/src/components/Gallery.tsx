import { FlatList, Image, ImageSourcePropType, Pressable, StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { CustomDimensions } from '../styles/CustomDimensionStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import Avatar, { AvatarSize } from './Avatar';
import { globalStyles } from '../styles/GlobalStyles';

// Icons
import FeatherIcon from '../assets/icons/feather-fill-colored.svg'
import BirdIcon from '../assets/icons/bird-fill-colored.svg'

//#region Props
interface GalleryProps {
  images: { media: ImageSourcePropType[] }[];
  posterAvatar: ImageSourcePropType;
  posterName: string;
}
//#endregion

/*

Instructions:
1. changeLayout goes externally to the parent component

*/

const Gallery: React.FC<GalleryProps> = ({
  images,
  posterAvatar = "NULL",
  posterName = "NULL",
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Layout                                     ||
  // ! ||--------------------------------------------------------------------------------||
  //#region Layout

  /*   const [numColumns, setNumColumns] = React.useState(
      images[0].media.length <= 1 ? 1 : images[0].media.length === 2 ? 2 : 3,
    ); */

  const [numColumns, setNumColumns] = React.useState(2);

  // Control the number of items displayed based on the layout
  const displayedData = numColumns === 1
    ? images[0].media.slice(0, 2)
    : numColumns === 2
      ? images[0].media.slice(0, 4)
      : numColumns === 3
        ? images[0].media.slice(0, 6)
        : [];

  // Handle layout change
  const changeLayout = () => {
    console.log('Change layout button pressed');

    if (numColumns === 1) {
      setNumColumns(2);

      console.log('Set from:', numColumns, 'to', numColumns + 1);
    } else if (numColumns === 2) {
      setNumColumns(3);

      console.log('Set from:', numColumns, 'to', numColumns + 1);
    } else if (numColumns === 3) {
      setNumColumns(1);

      console.log('Set from:', numColumns, 'to', numColumns - 2);
    }
  };

  /*   const changeLayout = () => {
      console.log('Change layout button pressed');
  
      if (numColumns === 1) {
        setNumColumns(2);
  
        console.log('Set from:', numColumns, 'to', numColumns + 1);
      } else if (numColumns === 2) {
        setNumColumns(3);
  
        console.log('Set from:', numColumns, 'to', numColumns + 1);
      } else if (numColumns === 3) {
        setNumColumns(1);
  
        console.log('Set from:', numColumns, 'to', numColumns - 2);
      }
    }; */

  // Image size
  const imgSize = {
    full: CustomDimensions.windowWidth - 24 * 2,
  };

  // Column layouts
  const oneCol = {
    width: imgSize.full,
    height: imgSize.full,
  };

  const twoCol = {
    width: imgSize.full / 2 - Spacing.md / 2,
    height: imgSize.full / 2 - Spacing.md / 2,
  };

  const threeCol = {
    width: (imgSize.full - Spacing.md * 2) / 3,
    height: (imgSize.full - Spacing.md * 2) / 3,
  };
  //#endregion

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                  Select image                                  ||
  // ! ||--------------------------------------------------------------------------------||
  // Two column layout styles
  // TODO remove if not used
  const image2CLFirstCol = {
    width: imgSize.full,
    height: imgSize.full,
    backgroundColor: 'red',
  }

  const image2CLSecondCol = {
    right: (imgSize.full + Spacing.md) / 2,
    width: imgSize.full,
    height: imgSize.full,
    backgroundColor: 'green',
  }

  const image3CLFirstCol = {
    width: imgSize.full,
    height: imgSize.full,
    backgroundColor: 'red',
  }

  const image3CLSecondCol = {
    width: imgSize.full,
    height: imgSize.full,
    backgroundColor: 'green',
  }

  const image3CLThirdCol = {
    width: imgSize.full,
    height: imgSize.full,
    backgroundColor: 'blue',
  }

  /*
  
  TODO: Implement a way to select an image and display it in a larger view, like in the prototype
  WHEN AN ITEM IS SELECTED:
  1. Selected item should have a style added that makes it darker (by 75%, #00000)
  2. A copy of the selected item should be made and displayed in a larger view (1 column layout),
      right below the original selected item.
  3. A copy of the selected item should also show the selected item's user (user who posted the image) and their avatar
  4. The original select item needs to be able to be deselected, by clicking on it again,
      which would also remove the copy of the selected item.
 
  */

  // const selectItem = () => {
  //   console.log('if this is here, the gallery isnt done yet')
  // }

  const [selectedImage, setSelectedImage] = React.useState(null);

  const handleImagePress = (image) => {
    // setSelectedImage(image);
    setSelectedImage(prevImage => (prevImage === image ? null : image));
  }

  // TODO remove these variables after testing and hook up to back-end data
  var you = true;
  var friend = false;
  var anon = false;

  const renderItem = ({ item, index }) => {
    return (
      <Pressable onPress={() => handleImagePress(item)}>
        <View>
          <View
            style={
              numColumns === 1 ? oneCol : numColumns === 2 ? twoCol : threeCol
            }>
            <Image source={item} style={styles.image} />
          </View>
          {selectedImage === item && (
            <View
              style={
                oneCol
              }>
              <View style={styles.user}>
                <Avatar size={AvatarSize.Small} image={posterAvatar} />
                <View style={styles.userInner}>
                  <Text style={[globalStyles.headingTextThree, globalStyles.textDark]}>{posterName}</Text>
                  {you ?
                    <BirdIcon height={24} width={24} />
                    : friend ?
                      <FeatherIcon height={24} width={24} />
                      : null}
                </View>
              </View>
              <Image source={item} style={styles.image} />
            </View>
          )}
        </View>
      </Pressable>
    );
  };

  return (
    //#region Gallery
    <View style={styles.container} >
      <FlatList
        data={displayedData}
        renderItem={renderItem}

        // renderItem={({ item }) => (
        //   <View
        //     style={
        //       numColumns === 1 ? oneCol : numColumns === 2 ? twoCol : threeCol
        //     }>
        //     <Image source={item} style={styles.image} />
        //   </View>
        // )}
        keyExtractor={(item, index) => index.toString()}
        numColumns={numColumns}
        key={numColumns}
        ItemSeparatorComponent={
          numColumns === 1
            ? () => <View style={styles.oneColSeparator} />
            : numColumns === 2
              ? () => <View style={styles.twoColSeparator} />
              : () => <View style={styles.threeColSeparator} />
        }
        columnWrapperStyle={
          numColumns === 1
            ? false
            : numColumns === 2
              ? styles.twoColWrapper
              : styles.threeColWrapper
        }
      />
    </View >
    //#endregion
  )
}

export default Gallery

// Exported enums
//#region Exported enums
export enum GalleryLayout {
  One,
  Two,
  Three,
}
//#endregion

//#region Styles
const styles = StyleSheet.create({
  container: {},

  top: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingBottom: Spacing.md,
  },

  // TODO modify to match Figma
  user: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.mdsm,
    paddingVertical: Spacing.md,
  },

  userInner: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.xs,
  },

  // Image
  image: {
    width: '100%',
    height: '100%',
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  selectedImageContainer: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    width: 300,
    height: 300,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },

  selectedImage: {
    // opacity: 0.5,
    width: '100%',
    height: '100%',
  },

  // Layouts
  // One column
  oneColSeparator: {
    height: Spacing.lg,
  },

  // Two columns
  twoColWrapper: {
    columnGap: Spacing.md,
  },
  twoColSeparator: {
    height: Spacing.md,
  },

  // Three columns
  threeColWrapper: {
    columnGap: Spacing.md,
  },
  threeColSeparator: {
    height: Spacing.md,
  },
})
//#endregion
