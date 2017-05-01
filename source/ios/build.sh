#! /bin/bash

cd "${0%/*}"

# Build and copy pods library and our bridge

xcodebuild -workspace "unity-bridge.xcworkspace" \
		   -scheme unity-bridge\
		   -derivedDataPath build\
		   -configuration Release\
		   ONLY_ACTIVE_ARCH=NO\
		   -sdk iphoneos\
		   clean\
		   build

xcodebuild -workspace "unity-bridge.xcworkspace" \
		   -scheme unity-bridge\
		   -derivedDataPath build\
		   -configuration Release\
		   ONLY_ACTIVE_ARCH=NO\
		   -sdk iphonesimulator\
		   -arch x86_64\
		   -arch i386\
		   -arch armv7\
		   -arch armv7s\
		   -arch arm64\
		   clean\
		   build

PRODUCTS_PATH="build/Build/Products"

lipo -create -output "build/libunity-bridge.a" "$PRODUCTS_PATH/Release-iphoneos/libunity-bridge.a" "$PRODUCTS_PATH/Release-iphonesimulator/libunity-bridge.a"

lipo -create -output "build/libPods-unity-bridge.a" "$PRODUCTS_PATH/Release-iphoneos/libPods-unity-bridge.a" "$PRODUCTS_PATH/Release-iphonesimulator/libPods-unity-bridge.a"
 
mv ./build/libunity-bridge.a ./build/libVoxImplant-bridge.a

cp ./build/libVoxImplant-bridge.a ../../package/Assets/Plugins/iOS

# Copy framework dependencies

cp -R ./Pods/VoxImplantWebRTC/WebRTC.framework ../../package/Assets/Plugins/iOS
cp -R ./Pods/VoxImplantSDK/VoxImplant.framework ../../package/Assets/Plugins/iOS

cd -