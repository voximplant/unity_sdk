#ifndef VOX_BASEIMPORTS_H
#define VOX_BASEIMPORTS_H

#ifdef ANDROID

#include <android/log.h>
#define vxvprintf(...) __android_log_print(ANDROID_LOG_VERBOSE, "VOXIMPLANT", __VA_ARGS__);
#define vxeprintf(...) __android_log_print(ANDROID_LOG_ERROR, "VOXIMPLANT", __VA_ARGS__);

#include <EGL/egl.h>
#include <GLES2/gl2.h>

#elif IOS

#define vxvprintf(...) printf(__VA_ARGS__);
#define vxeprintf(...) printf(__VA_ARGS__);

#include <OpenGLES/EAGL.h>
#include <OpenGLES/gltypes.h>
#include <OpenGLES/ES2/glext.h>

#endif


#endif