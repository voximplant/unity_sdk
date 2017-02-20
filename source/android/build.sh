#! /bin/bash

cd "${0%/*}"

# Build android project

gradle clean assembleRelease
 
cp ./library/build/outputs/aar/library-release.aar ../../package/Assets/Plugins/Android

cd -