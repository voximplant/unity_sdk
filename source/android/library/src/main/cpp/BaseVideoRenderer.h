//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#ifndef UNITY_BRIDGE_BASEVIDEORENDERER_H
#define UNITY_BRIDGE_BASEVIDEORENDERER_H

#import "BaseImports.h"

class BaseVideoRenderer {
public:
    BaseVideoRenderer(int width, int height);
    virtual ~BaseVideoRenderer();

    virtual void Invalidate();
    bool IsValidForSize(int width, int height);

    virtual bool IsActiveContextMatch();

    virtual void RenderBuffer(
            const uint8_t *yPlane, int yStride,
            const uint8_t *uPlane, int uStride,
            const uint8_t *vPlane, int vStride,
            int width, int height,
            int degrees) = 0;

protected:
    bool m_isInvalidated;
    int m_ackWidth, m_ackHeight;

private:
};


#endif //UNITY_BRIDGE_BASEVIDEORENDERER_H
