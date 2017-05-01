#import <vector>

#import "EAGLVideoRenderer.h"
#import "LockGuard.hpp"
#import "DestroyList.h"
#import "IUnityGraphics.h"

@class EAGLContext;

extern std::vector<BaseVideoRenderer *> *s_renderers;
extern Mutex *s_renderersMutex;
extern EAGLContext *s_unityContext;
extern UnityGfxRenderer s_unityGFXRenderer;

extern DestroyList<BaseVideoRenderer *> *s_destroyList;