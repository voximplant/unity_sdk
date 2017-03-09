//
// Created by Aleksey Zinchenko on 07/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#ifndef UNITY_BRIDGE_EAGLVIDEORENDERER_H
#define UNITY_BRIDGE_EAGLVIDEORENDERER_H
#ifdef IOS

#include "BaseImports.h"

#include "BaseVideoRenderer.h"

class EAGLVideoRenderer: public BaseVideoRenderer {
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
#endif //UNITY_BRIDGE_EAGLVIDEORENDERER_H
