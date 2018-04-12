#! /bin/bash
set -e

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
		   build | xcpretty

xcodebuild -workspace "unity-bridge.xcworkspace" \
		   -scheme unity-bridge\
		   -derivedDataPath build\
		   -configuration $BUILD_CONFIG\
		   ONLY_ACTIVE_ARCH=NO\
		   -sdk iphonesimulator\
		   -arch x86_64\
		   -arch i386\
		   clean\
		   build | xcpretty

PRODUCTS_PATH="build/Build/Products"

lipo -create -output "build/libunity-bridge.a" \
"$PRODUCTS_PATH/$BUILD_CONFIG-iphoneos/libunity-bridge.a" \
"$PRODUCTS_PATH/$BUILD_CONFIG-iphonesimulator/libunity-bridge.a"
 
mv ./build/libunity-bridge.a ./build/libVoxImplant-bridge.a

cp ./build/libVoxImplant-bridge.a ../../package/Assets/Plugins/iOS

# Copy framework dependencies

for deprecated in AFNetworking; do
    if [ -d "../../package/Assets/Plugins/iOS/$deprecated.framework" ]; then
		rm -rf "../../package/Assets/Plugins/iOS/$deprecated.framework"
	fi
done

rm -rf "../../package/Assets/Plugins/iOS/VoxImplant.framework"
rm -rf "../../package/Assets/Plugins/iOS/WebRTC.framework"
cp -Rf "./Pods/VoxImplantSDK/VoxImplant.framework" "../../package/Assets/Plugins/iOS"
cp -Rf "./Pods/VoxImplantWebRTC/WebRTC.framework" "../../package/Assets/Plugins/iOS"

for framework in CocoaLumberjack SocketRocket; do
    rm -rf "../../package/Assets/Plugins/iOS/$framework.framework"
	cp -Rf "./build/Build/Products/Release-iphoneos/$framework/$framework.framework" "../../package/Assets/Plugins/iOS"
done

cd -