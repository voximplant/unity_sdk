#! /bin/bash

cd "${0%/*}"

# Build and copy pods library and our bridge

BUILD_CONFIG=Release

xcodebuild -workspace "unity-bridge.xcworkspace" \
		   -scheme unity-bridge\
		   -derivedDataPath build\
		   -configuration $BUILD_CONFIG\
		   ONLY_ACTIVE_ARCH=NO\
		   -sdk iphoneos\
		   clean\
		   build

xcodebuild -workspace "unity-bridge.xcworkspace" \
		   -scheme unity-bridge\
		   -derivedDataPath build\
		   -configuration $BUILD_CONFIG\
		   ONLY_ACTIVE_ARCH=NO\
		   -sdk iphonesimulator\
		   -arch x86_64\
		   -arch i386\
		   clean\
		   build

PRODUCTS_PATH="build/Build/Products"

lipo -create -output "build/libunity-bridge.a" \
"$PRODUCTS_PATH/$BUILD_CONFIG-iphoneos/libunity-bridge.a" \
"$PRODUCTS_PATH/$BUILD_CONFIG-iphonesimulator/libunity-bridge.a"
 
mv ./build/libunity-bridge.a ./build/libVoxImplant-bridge.a

cp ./build/libVoxImplant-bridge.a ../../package/Assets/Plugins/iOS

# Copy framework dependencies

cp -Rf ./Pods/VoxImplantWebRTC/WebRTC.framework ../../package/Assets/Plugins/iOS
cp -Rf ./Pods/VoxImplantSDK/VoxImplant.framework ../../package/Assets/Plugins/iOS

cd -