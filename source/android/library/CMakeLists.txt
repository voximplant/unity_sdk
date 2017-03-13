# Sets the minimum version of CMake required to build your native library.
# This ensures that a certain set of CMake features is available to
# your build.

cmake_minimum_required(VERSION 3.4.1)

# Specifies a library name, specifies whether the library is STATIC or
# SHARED, and provides relative paths to the source code. You can
# define multiple libraries by adding multiple add.library() commands,
# and CMake builds them for you. When you build your app, Gradle
# automatically packages shared libraries with your APK.

add_compile_options(-x c++)
add_definitions(-DANDROID)

add_library( # Specifies the name of the library.
             VoximplantAndroidRendererPlugin

             # Sets the library as a shared library.
             SHARED

             src/main/cpp/BaseVideoRenderer.cpp
             src/main/cpp/AndroidNativeRenderer.cpp
             src/main/cpp/BaseOGLVideoRenderer.cpp
             src/main/cpp/EGLVideoRenderer.cpp)


include_directories(src/main/cpp/Unity/)
include_directories(src/main/cpp/)

target_link_libraries( VoximplantAndroidRendererPlugin

                       log
                       EGL
                       GLESv2)