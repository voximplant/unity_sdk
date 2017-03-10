//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#include "MetalVideoRenderer.h"
#import "BaseImports.h"

MetalVideoRenderer::MetalVideoRenderer(int width, int height) : BaseVideoRenderer(width, height) {
    SetupRender();
}

void MetalVideoRenderer::SetupRender() {
    m_device = MTLCreateSystemDefaultDevice();
    m_targetTexture = [m_device newTextureWithDescriptor:[MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatRGBA8Unorm
                                                                                                            width:(NSUInteger) m_ackWidth
                                                                                                           height:(NSUInteger) m_ackHeight
                                                                                                        mipmapped:false]];

    m_yTexture = [m_device newTextureWithDescriptor:[MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatR8Unorm
                                                                                                       width:(NSUInteger) m_ackWidth
                                                                                                      height:(NSUInteger) m_ackHeight
                                                                                                   mipmapped:false]];

    MTLTextureDescriptor *halfTextureDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatR8Unorm
                                                                                                     width:(NSUInteger) m_ackWidth / 2
                                                                                                    height:(NSUInteger) m_ackHeight / 2
                                                                                                 mipmapped:false];
    m_uTexture = [m_device newTextureWithDescriptor:halfTextureDescriptor];
    m_vTexture = [m_device newTextureWithDescriptor:halfTextureDescriptor];

    const uint16_t s_indices[] = { 0, 3, 2, 0, 2, 1 };
    m_indexBuffer = [m_device newBufferWithBytes:s_indices
                                          length:6 * sizeof(uint16_t)
                                         options:(MTLResourceOptions) 0];

    m_commandQueue = [m_device newCommandQueue];

    m_passDescriptor = [MTLRenderPassDescriptor renderPassDescriptor];
    m_passDescriptor.colorAttachments[0].texture = m_targetTexture;
    m_passDescriptor.colorAttachments[0].loadAction = MTLLoadActionClear;
    m_passDescriptor.colorAttachments[0].clearColor = MTLClearColorMake(1.0, 0.0, 0.0, 1.0);
    m_passDescriptor.colorAttachments[0].storeAction = MTLStoreActionStore;

    static NSString *progSrc = @""
            "#include <metal_stdlib>\n"
            "using namespace metal;\n"
            "\n"
            "struct VertexInfo {\n"
            "            float4 position [[ attribute(0) ]];\n"
            "            float2 uvPosition [[ attribute(1) ]];\n"
            "};"
            "struct VertexOut {"
            "   float4 position [[ position ]];"
            "   float2 uvPosition;"
            "};"
            "\n"
            "vertex VertexOut vertexShader(VertexInfo in [[ stage_in ]])\n"
            "{\n"
            "   VertexOut out;"
            "   out.position = in.position;"
            "   out.uvPosition = in.uvPosition;"
            "   return out;\n"
            "}\n"
            "\n"
            "constexpr sampler s = sampler(coord::normalized,\n"
            "                              address::clamp_to_edge,\n"
            "                              filter::linear);\n"
            "\n"
            "fragment half4 fragmentShader(VertexOut in [[ stage_in ]],"
            "                              texture2d<float> yPlane [[ texture(0) ]],"
            "                              texture2d<float> uPlane [[ texture(1) ]],"
            "                              texture2d<float> vPlane [[ texture(2) ]]) {"
            "   float y = yPlane.sample(s, in.uvPosition).r;"
            "   float u = uPlane.sample(s, in.uvPosition).r;"
            "   float v = vPlane.sample(s, in.uvPosition).r;"
            ""
            "   y = 1.1643 * (y - 0.0625);"
            "   u = u - 0.5;"
            "   v = v - 0.5;"
            ""
            "   return half4(y + 1.5958 * v, y - 0.39173 * u - 0.81290 * v, y + 2.017 * u, 1.0);"
            "}";

    NSError *errors;
    id <MTLLibrary> library = [m_device newLibraryWithSource:progSrc options:nil error:&errors];
    if (errors != nil) {
        vxeprintf("Failed to compiles shaders %s", [errors description].UTF8String);
    }
    id <MTLFunction> vertFunc = [library newFunctionWithName:@"vertexShader"];
    MTLVertexDescriptor *vertexDesc = [[MTLVertexDescriptor alloc] init];
    vertexDesc.attributes[0].format = MTLVertexFormatFloat3;
    vertexDesc.attributes[0].bufferIndex = 0;
    vertexDesc.attributes[0].offset = 0;
    vertexDesc.attributes[1].format = MTLVertexFormatFloat2;
    vertexDesc.attributes[1].bufferIndex = 1;
    vertexDesc.attributes[1].offset = 0;
    vertexDesc.layouts[0].stepFunction = MTLVertexStepFunctionPerVertex;
    vertexDesc.layouts[0].stride = 3 * sizeof(float);
    vertexDesc.layouts[1].stepFunction = MTLVertexStepFunctionPerVertex;
    vertexDesc.layouts[1].stride = 2 * sizeof(float);

    id <MTLFunction> fragFunc = [library newFunctionWithName:@"fragmentShader"];

    m_pipelineDescriptor = [[MTLRenderPipelineDescriptor alloc] init];
    m_pipelineDescriptor.vertexFunction = vertFunc;
    m_pipelineDescriptor.vertexDescriptor = vertexDesc;
    m_pipelineDescriptor.fragmentFunction = fragFunc;
    m_pipelineDescriptor.colorAttachments[0].pixelFormat = m_targetTexture.pixelFormat;
}

void shrFloat(float *source, int length, int distance, float *destination) {
    while (distance < 0) {
        distance += length;
    }
    for (int i = 0; i < length; i++) {
        destination[(distance+i) % length] = source[i];
    }
}

void MetalVideoRenderer::RenderBuffer(const uint8_t *yPlane, int yStride, const uint8_t *uPlane, int uStride, const uint8_t *vPlane, int vStride, int width, int height, int degrees) {
    if (degrees % 90 != 0) {
        vxeprintf("Only multiples of 90 degrees are supported");
        degrees = 0;
    }

    id<MTLCommandBuffer> buffer = [m_commandQueue commandBuffer];
    id<MTLRenderCommandEncoder> encoder = [buffer renderCommandEncoderWithDescriptor:m_passDescriptor];

    static const float posData[] = {
            // X, Y, Z
            -1, -1, 0, // Bottom Left
            1,  -1, 0, //Bottom Right
            1, 1, 0, //Top Right
            -1, 1, 0 //Top Left
    };

    const float uvPosData[] = {
            1, 0,
            0, 0,
            0, 1,
            1, 1
    };

    float rotatedUV[8];
    shrFloat((float *) uvPosData, 8, 2 * (degrees / 90), rotatedUV);

    id <MTLBuffer> posBuf = [m_device newBufferWithBytes:posData length:3 * 4 * sizeof(float) options:(MTLResourceOptions)0];
    id <MTLBuffer> uvBuf = [m_device newBufferWithBytes:rotatedUV length:4 * 2 * sizeof(float) options:(MTLResourceOptions)0];
    [encoder setVertexBuffer:posBuf offset:0 atIndex:0];
    [encoder setVertexBuffer:uvBuf offset:0 atIndex:1];

    [m_yTexture replaceRegion:MTLRegionMake2D(0, 0, (NSUInteger) width, (NSUInteger) height)
                  mipmapLevel:0
                    withBytes:yPlane
                  bytesPerRow:(NSUInteger) yStride];

    [m_uTexture replaceRegion:MTLRegionMake2D(0, 0, (NSUInteger) width / 2, (NSUInteger) height / 2)
                  mipmapLevel:0
                    withBytes:uPlane
                  bytesPerRow:(NSUInteger) uStride];

    [m_vTexture replaceRegion:MTLRegionMake2D(0, 0, (NSUInteger) width / 2, (NSUInteger) height / 2)
                  mipmapLevel:0
                    withBytes:vPlane
                  bytesPerRow:(NSUInteger) vStride];

    [encoder setFragmentTexture:m_yTexture atIndex:0];
    [encoder setFragmentTexture:m_uTexture atIndex:1];
    [encoder setFragmentTexture:m_vTexture atIndex:2];

    NSError *error = nil;
    id <MTLRenderPipelineState> pipeline = [m_device newRenderPipelineStateWithDescriptor:m_pipelineDescriptor error:&error];
    if (error != nil) {
        vxeprintf("Failed to create render pipeline state %s", [error description].UTF8String);
    }

    [encoder setRenderPipelineState:pipeline];

    [encoder drawIndexedPrimitives:MTLPrimitiveTypeTriangle
                        indexCount:6
                         indexType:MTLIndexTypeUInt16
                       indexBuffer:m_indexBuffer
                 indexBufferOffset:0];

    [encoder drawPrimitives:MTLPrimitiveTypeTriangle vertexStart:0 vertexCount:3];
    [encoder endEncoding];
    [buffer commit];
    [buffer waitUntilCompleted];
}

id <MTLTexture> MetalVideoRenderer::GetMetalTexture() {
    return m_targetTexture;
}

void *MetalVideoRenderer::GetMetalContext() {
    return (__bridge void *)m_commandQueue;
}
