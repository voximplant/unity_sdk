#! /bin/bash
set -e

cd "${0%/*}"

# Build android project

./gradlew clean assembleRelease
 
mv ./library/build/outputs/aar/library-release.aar ./library/build/outputs/aar/libvoximplant-release.aar

# Stripping unsupported architectures
zip -d ./library/build/outputs/aar/libvoximplant-release.aar jni/{x86_64,arm64}*

cp ./library/build/outputs/aar/libvoximplant-release.aar ../../package/Assets/Plugins/Android

cd -