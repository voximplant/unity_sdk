#!/bin/sh
set -e

if [ -z "$ANDROID_HOME" ] || [ -z "$ANDROID_NDK_HOME" ]; then
    echo "Android SDK location not found. Define location with an ANDROID_HOME and ANDROID_NDK_HOME environment variables."
    exit 1
fi

# Building Android native library
source/android/buildDebug.sh

if [[ `uname` == 'Darwin' ]]
then

    # Building iOS native library
    cd source/ios/
    pod install
    . build.sh

fi
