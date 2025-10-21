import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';

export default function App() {
  return (
    <View className="flex-1 items-center justify-center bg-white dark:bg-black">
      <Text className="text-2xl font-bold text-black dark:text-white">Biolinker</Text>
      <Text className="mt-2 text-base text-gray-600 dark:text-gray-300">
        React Native + Expo + NativeWind
      </Text>
      <StatusBar style="auto" />
    </View>
  );
}
