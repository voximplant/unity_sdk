#! /bin/bash
set -e

cd "${0%/*}"

# Build android project

./gradlew clean assembleDebug
 
mv ./library/build/outputs/aar/library-debug.aar ./library/build/outputs/aar/libvoximplant-debug.aar
cp ./library/build/outputs/aar/libvoximplant-debug.aar ../../package/Assets/Plugins/Android

cd -