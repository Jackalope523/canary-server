import { Pressable, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import Avatar, { AvatarSize } from '../Avatar';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';

import userListItemDropdownOptions from './UserListItemDropdownOptions';

interface UserListItemDropdownProps {
  image: string;
  name: string;
}

// Icons
import ChevronIcon from '../../assets/icons/chevron-outline.svg';
import { Spacing } from '../../styles/SpacingStyles';
import TextButton, { TextButtonType, TextButtonVariant } from '../TextButton';

const UserListItemDropdown: React.FC<UserListItemDropdownProps> = ({
  image,
  name = 'NULL',
}) => {
  const [showAllItems, setShowAllItems] = React.useState(false);

  const toggleShowAllItems = () => {
    setShowAllItems(!showAllItems);
  };

  return (
    <View style={styles.userListItemDropdown}>
      <Pressable style={styles.userContainer} onPress={toggleShowAllItems}>
        <View style={styles.user}>
          <Avatar size={AvatarSize.Medium} image={image} />
          <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
            {name}
          </Text>
        </View>
        <ChevronIcon
          height={24}
          width={24}
          fill={Colors.sparrowDarkBrown}
          style={
            showAllItems
              ? { transform: [{ rotate: '180deg' }] }
              : { transform: [{ rotate: '0deg' }] }
          }
        />
      </Pressable>
      {showAllItems && (
        <View style={styles.dropdownItems}>
          {userListItemDropdownOptions.map((option, index) => (
            <TextButton
              key={index}
              type={TextButtonType.Dark}
              variant={TextButtonVariant.Three}
              text={option.text}
              onPress={option.onPress}
              displayIcon={false}
            />
          ))}
        </View>
      )}
    </View>
  );
};

export default UserListItemDropdown;

const styles = StyleSheet.create({
  userListItemDropdown: {
    rowGap: Spacing.md,
  },

  userContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },

  user: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.mdsm,
  },

  dropdownItems: {
    rowGap: Spacing.sm,
  },
});
