import { ImageSourcePropType, Pressable, StyleSheet, Text, View } from 'react-native';
import React, { FC } from 'react';
import EventCardSmall, { EventStatus } from '../EventCardSmall';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import Gallery from '../Gallery';
import { globalStyles } from '../../styles/GlobalStyles';

// TODO make sure this component isn't broken after merging into Profile branch (if gallery is fixed there)

// Icons
import LayoutMediumIcon from '../../assets/icons/layout-size-medium-fill-alt.svg'
import LayoutLargeIcon from '../../assets/icons/layout-size-large-fill.svg'
import { Colors } from '../../styles/ColorStyles';

type GallerySectionProps = {
  onPress: () => void;
  images: { media: ImageSourcePropType[] }[];
  posterAvatar: ImageSourcePropType;
  posterName: string;
};

const GallerySection: FC<GallerySectionProps> = ({
  onPress,
  images,
  posterAvatar,
  posterName,
}) => {

  return (
    <View style={styles.gallerySection}>
      <View style={styles.galleryControls}>
        <Text style={[globalStyles.textDark, globalStyles.headingTextFour]}>Gallery</Text>
        {/* TODO add functionality to the layout pressable once Gallery component is fixed; add layoutLargeIcon as well */}
        <Pressable onPress={null}>
          <LayoutMediumIcon width={24} height={24} fill={Colors.sparrowDarkBrown} />
        </Pressable>
      </View>
      <Gallery images={images} posterAvatar={posterAvatar} posterName={posterName} />
    </View>
  );
};

export default GallerySection;

const styles = StyleSheet.create({
  gallerySection: {
    flex: 1,
    rowGap: Spacing.md,
  },

  galleryControls: {
    justifyContent: 'space-between',
    flexDirection: 'row',
    alignItems: 'center',
  },
});
