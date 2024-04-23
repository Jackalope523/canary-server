import { FlatList, Image, ImageSourcePropType, Pressable, StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { CustomDimensions } from '../styles/CustomDimensionStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

//#region Props
interface GalleryCopyProps {
  images: { media: ImageSourcePropType[] }[];
}
//#endregion

/*

Instructions:
1. changeLayout goes externally to the parent component

*/

const GalleryCopy: React.FC<GalleryCopyProps> = ({ images }) => {
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
  const image2CLFirstCol = {
    backgroundColor: 'red',
  }

  const image2CLSecondCol = {
    right: (imgSize.full + Spacing.md) / 2,
    backgroundColor: 'green',
  }

  const image3CLFirstCol = {}

  const image3CLSecondCol = {}

  const image3CLThirdCol = {}

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
  //   console.log('if this is here, the GalleryCopy isnt done yet')
  // }

  const [selectedImage, setSelectedImage] = React.useState(null);

  const handleImagePress = (image) => {
    // setSelectedImage(image);
    setSelectedImage(prevImage => (prevImage === image ? null : image));
  }

  const renderItem = ({ item, index }) => (
    // console.log('unique index:', index),
    console.log('unique item:', item),
    <Pressable onPress={() => handleImagePress(item)}>
      {/* <View style={styles.imageContainer}>
        <Image source={item} style={styles.image} />
        {selectedImage === item && (
          <View style={styles.selectedImageContainer}>
            <Image source={item} style={styles.selectedImage} />
          </View>
        )}
      </View> */}
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
              // numColumns === 1 ? oneCol : numColumns === 2 ? twoCol : threeCol
              // oneCol
              [
                /*
                
                Sketchy workaround for styling I'm experimenting with:
                
                1. if the selected image is a part of a two-column layout (numColumns === 2),
                  and if it's in the first column,
                  then apply the first column styles to the selected image (image2CLFirstCol),
                  else apply the second column styles to the selected image (image2CLSecondCol).

                2. if the selected image is a part of a three-column layout (numColumns === 3),
                  and if it's in the first column,
                  then apply the first column styles to the selected image (image3CLFirstCol),
                  else if it's in the second column,
                  then apply the second column styles to the selected image(image3CLSecondCol),
                  else apply the third column styles (image3CLThirdCol) to the selected image.

                Each layout style has a different right value, that re-centers the selected image.
                
                */

                oneCol,
                image2CLSecondCol,
              ]
            }>
            <View style={styles.user}>
              <Text>Avatar placeholder</Text>
              <Text>Name placeholder</Text>
            </View>
            <Image source={item} style={styles.image} />
          </View>
        )}
      </View>
    </Pressable>
  );

  return (
    //#region GalleryCopy
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

export default GalleryCopy

// Exported enums
//#region Exported enums
export enum GalleryCopyLayout {
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
    columnGap: Spacing.lg,
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
