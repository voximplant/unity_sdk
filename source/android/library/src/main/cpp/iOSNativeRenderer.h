#import "EAGLVideoRenderer.h"
#import "LockGuard.hpp"
#import "DestroyList.h"

@class EAGLContext;


extern EAGLVideoRenderer **s_renderers;
extern Mutex *s_renderersMutex;
extern EAGLContext *s_unityContext;

extern DestroyList<EAGLVideoRenderer *> *s_destroyList;