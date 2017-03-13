//
// Created by Aleksey Zinchenko on 07/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#ifndef VOX_EAGLVIDEORENDERER_H
#define VOX_EAGLVIDEORENDERER_H
#ifdef IOS

#include "BaseImports.h"

#include "BaseOGLVideoRenderer.h"

class EAGLVideoRenderer: public BaseOGLVideoRenderer {
public:
    EAGLVideoRenderer(int width, int height, EAGLSharegroup *sharegroup);
    ~EAGLVideoRenderer();

    EAGLContext *GetEAGLContext();
    void Detach();
    bool IsActiveContextMatch();

private:
    void SetupRender();

    void CleanupRender();

    EAGLSharegroup *m_sharegroup;
    EAGLContext *m_context;
};


#endif
#endif
