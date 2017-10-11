#! /bin/bash

cd "${0%/*}"

# Build android project

./gradlew clean assembleRelease
 
mv ./library/build/outputs/aar/library-release.aar ./library/build/outputs/aar/libvoximplant-release.aar
cp ./library/build/outputs/aar/libvoximplant-release.aar ../../package/Assets/Plugins/Android

cd -