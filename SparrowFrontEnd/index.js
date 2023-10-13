/**
 * @format
 */

import {AppRegistry} from 'react-native';
import App from './App';
import {name as appName} from './app.json';

import { initialiseAxios } from './src/lib/axios';

initialiseAxios();

AppRegistry.registerComponent(appName, () => App);
