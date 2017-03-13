//
// Created by Aleksey Zinchenko on 03/03/2017.
//

#ifndef VOX_EGLVIDEORENDERER_H
#define VOX_EGLVIDEORENDERER_H
#ifdef ANDROID

#include <EGL/egl.h>
#include <GLES2/gl2.h>

#include "LockGuard.hpp"
#include "BaseOGLVideoRenderer.h"

class EGLVideoRenderer: public BaseOGLVideoRenderer {
public:
    EGLVideoRenderer(int width, int height, EGLContext sharedContext);
    ~EGLVideoRenderer();

    EGLContext GetOGLContext();
    void Detach();

private:
    EGLDisplay m_display;
    EGLSurface m_surface;
    EGLContext m_context;

    EGLContext m_sharingContext;

    void SetupEGL();
    void CleanupRender();

    bool IsActiveContextMatch();

    bool ChooseConfigAndCreateContext(EGLDisplay display);
};

#endif
#endif