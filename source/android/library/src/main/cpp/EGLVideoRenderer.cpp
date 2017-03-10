//
// Created by Aleksey Zinchenko on 03/03/2017.
//

#ifdef ANDROID

#include <stdio.h>
#include <cstdlib>

#include "BaseImports.h"
#include "EGLVideoRenderer.h"
#include "BaseOGLVideoRenderer.h"

EGLVideoRenderer::EGLVideoRenderer(int width, int height, EGLContext sharedContext) :
        BaseOGLVideoRenderer(width, height),
        m_display(0),
        m_context(EGL_NO_CONTEXT),

        m_sharingContext(sharedContext)
        {
            SetupEGL();
        }

bool EGLVideoRenderer::ChooseConfigAndCreateContext(EGLDisplay display) {
    EGLint numConfigs = 0;

    if (!eglGetConfigs(display, NULL, 0, &numConfigs) || numConfigs == 0) {
        return false;
    }

    EGLConfig *configs = new EGLConfig[numConfigs];

    eglGetConfigs(display, configs, numConfigs, &numConfigs);

    for (int i = 0; i < numConfigs; ++i) {
        EGLint surfaceType;
        EGLint renderableType;
        EGLint redSize;
        EGLint greenSize;
        EGLint blueSize;

        eglGetConfigAttrib(display, configs[i], EGL_SURFACE_TYPE, &surfaceType);
        eglGetConfigAttrib(display, configs[i], EGL_RENDERABLE_TYPE, &renderableType);
        eglGetConfigAttrib(display, configs[i], EGL_RED_SIZE, &redSize);
        eglGetConfigAttrib(display, configs[i], EGL_GREEN_SIZE, &greenSize);
        eglGetConfigAttrib(display, configs[i], EGL_BLUE_SIZE, &blueSize);

        int neededSurfType = EGL_PBUFFER_BIT;
        if ((surfaceType & neededSurfType) != neededSurfType)
            continue;

        if ((renderableType & EGL_OPENGL_ES2_BIT) != EGL_OPENGL_ES2_BIT)
            continue;

        if (redSize < 8 || greenSize < 8 || blueSize < 8)
            continue;

        do {
            EGLint ctxAttr[] = {
                    EGL_CONTEXT_CLIENT_VERSION, 3,
                    EGL_NONE
            };

            m_context = eglCreateContext(m_display, configs[i], m_sharingContext, ctxAttr);
            if (!testGLErrors("eglCreateContext") && m_context != EGL_NO_CONTEXT) {
                break;
            }

            // No luck with 3.0, let's try 2.0
            ctxAttr[1] = 2;
            m_context = eglCreateContext(m_display, configs[i], m_sharingContext, ctxAttr);
        } while(false);

        if (testGLErrors("eglCreateContext") || m_context == EGL_NO_CONTEXT) {
            m_context = EGL_NO_CONTEXT;
            continue;
        }

        const EGLint surfaceAttr[] = {
                EGL_WIDTH, m_ackWidth,
                EGL_HEIGHT, m_ackHeight,
                EGL_NONE
        };

        m_surface = eglCreatePbufferSurface(m_display, configs[i], surfaceAttr);
        if (testGLErrors("eglCreatePbufferSurface")) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }

        if (eglMakeCurrent(m_display, m_surface, m_surface, m_context) == EGL_FALSE) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }
        if (testGLErrors("eglMakeCurrent")) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }

        delete[] configs;
        return true;
    }

    delete[] configs;
    return false;
}

void EGLVideoRenderer::SetupEGL() {
    if (m_context != 0) {
        return;
    }

    if (eglGetCurrentContext() != EGL_NO_CONTEXT) {
        vxeprintf("Error: trying to setup context with already present OGL context");
        return;
    }

    m_display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
    EGLint major, minor;
    eglInitialize(m_display, &major, &minor);
    eglBindAPI(EGL_OPENGL_ES_API);
    testGLErrors("eglBindAPI");

    if (!ChooseConfigAndCreateContext(m_display)) {
        testGLErrors("chooseConfigAndCreateContext");
        CleanupRender();
        return;
    }

    SetupRender();
}

EGLVideoRenderer::~EGLVideoRenderer() {
    CleanupRender();
}

void EGLVideoRenderer::CleanupRender() {
    if (m_context != EGL_NO_CONTEXT) {
        eglDestroyContext(m_display, m_context);
    }
    if (m_surface != EGL_NO_SURFACE) {
        eglDestroySurface(m_display, m_surface);
    }
    eglTerminate(m_display);

    m_display = 0;
    m_context = EGL_NO_CONTEXT;
    m_surface = EGL_NO_SURFACE;
}

EGLContext EGLVideoRenderer::GetOGLContext() {
    return m_context;
}

void EGLVideoRenderer::Detach() {
    eglMakeCurrent(m_display, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
}

bool EGLVideoRenderer::IsActiveContextMatch() {
    return (m_context == eglGetCurrentContext() || m_context == EGL_NO_CONTEXT);
}

#endif
