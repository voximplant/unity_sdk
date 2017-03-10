//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#ifndef UNITY_BRIDGE_METALVIDEORENDERER_H
#define UNITY_BRIDGE_METALVIDEORENDERER_H

#import <objc/objc.h>
#import <Metal/Metal.h>
#import "BaseVideoRenderer.h"

class MetalVideoRenderer: public BaseVideoRenderer {
public:
    MetalVideoRenderer(int width, int height);

    id<MTLTexture> GetMetalTexture();
    void *GetMetalContext();

    void RenderBuffer(
            const uint8_t *yPlane, int yStride,
            const uint8_t *uPlane, int uStride,
            const uint8_t *vPlane, int vStride,
            int width, int height,
            int degrees) override;

private:
    id<MTLDevice> m_device;
    id<MTLTexture> m_targetTexture;
    id<MTLCommandQueue> m_commandQueue;

    id<MTLTexture> m_yTexture, m_uTexture, m_vTexture;
    id<MTLBuffer> m_indexBuffer;

    MTLRenderPassDescriptor *m_passDescriptor;
    MTLRenderPipelineDescriptor *m_pipelineDescriptor;

    void SetupRender();
    void RotateByDegrees(int degrees);
};


#endif //UNITY_BRIDGE_METALVIDEORENDERER_H
