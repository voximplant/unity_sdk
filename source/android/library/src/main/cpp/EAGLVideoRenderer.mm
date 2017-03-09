//
// Created by Aleksey Zinchenko on 07/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#ifdef IOS

#include "EAGLVideoRenderer.h"

EAGLVideoRenderer::EAGLVideoRenderer(int width, int height, EAGLSharegroup *sharegroup):
        BaseVideoRenderer(width, height),

        m_sharegroup(sharegroup)
{
    SetupRender();
}

void EAGLVideoRenderer::SetupRender() {
    m_context = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2
                                      sharegroup:m_sharegroup];
    
    [EAGLContext setCurrentContext:m_context];

    BaseVideoRenderer::SetupRender();
}

EAGLVideoRenderer::~EAGLVideoRenderer() {
    m_sharegroup = nil;
    m_context = nil;
}

EAGLContext *EAGLVideoRenderer::GetEAGLContext() {
    return m_context;
}

void EAGLVideoRenderer::Detach() {
    [EAGLContext setCurrentContext:nil];
}

void EAGLVideoRenderer::CleanupRender() {
}

bool EAGLVideoRenderer::IsActiveContextMatch() {
    return m_context == [EAGLContext currentContext];
}


#endif
