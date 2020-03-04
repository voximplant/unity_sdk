/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIMetalRenderer.h"
#import <Metal/Metal.h>

@interface VIMetalRenderer ()

@property(assign, nonatomic) CGSize frameSize;

@property(strong, nonatomic) id <MTLDevice> device;
@property(strong, nonatomic) id <MTLTexture> targetTexture;
@property(strong, nonatomic) id <MTLCommandQueue> commandQueue;

@property(strong, nonatomic) id <MTLTexture> yTexture;
@property(strong, nonatomic) id <MTLTexture> uTexture;
@property(strong, nonatomic) id <MTLTexture> vTexture;
@property(strong, nonatomic) id <MTLBuffer> indexBuffer;

@property(strong, nonatomic) MTLRenderPassDescriptor *passDescriptor;
@property(strong, nonatomic) MTLRenderPipelineDescriptor *pipelineDescriptor;

@end

@implementation VIMetalRenderer

- (void)setupRenderer:(CGSize)frameSize {
    if (CGSizeEqualToSize(frameSize, self.frameSize)) return;

    _frameSize = frameSize;

    _device = MTLCreateSystemDefaultDevice();

    _targetTexture = [_device newTextureWithDescriptor:[MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatRGBA8Unorm
                                                                                                          width:static_cast<NSUInteger>(_frameSize.width)
                                                                                                         height:static_cast<NSUInteger>(_frameSize.height)
                                                                                                      mipmapped:NO]];

    _yTexture = [_device newTextureWithDescriptor:[MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatR8Unorm
                                                                                                     width:static_cast<NSUInteger>(_frameSize.width)
                                                                                                    height:static_cast<NSUInteger>(_frameSize.height)
                                                                                                 mipmapped:NO]];

    MTLTextureDescriptor *halfTextureDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatR8Unorm
                                                                                                     width:static_cast<NSUInteger>(_frameSize.width / 2)
                                                                                                    height:static_cast<NSUInteger>(_frameSize.height / 2)
                                                                                                 mipmapped:NO];

    _uTexture = [_device newTextureWithDescriptor:halfTextureDescriptor];
    _vTexture = [_device newTextureWithDescriptor:halfTextureDescriptor];

    const uint16_t s_indices[] = {0, 3, 2, 0, 2, 1};
    _indexBuffer = [_device newBufferWithBytes:s_indices length:6 * sizeof(uint16_t) options:static_cast<MTLResourceOptions>(0)];

    _commandQueue = [_device newCommandQueue];

    _passDescriptor = [MTLRenderPassDescriptor renderPassDescriptor];
    _passDescriptor.colorAttachments[0].texture = _targetTexture;
    _passDescriptor.colorAttachments[0].loadAction = MTLLoadActionClear;
    _passDescriptor.colorAttachments[0].clearColor = MTLClearColorMake(1.0, 0.0, 0.0, 1.0);
    _passDescriptor.colorAttachments[0].storeAction = MTLStoreActionStore;

    static NSString *shader = @""
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
    id <MTLLibrary> library = [_device newLibraryWithSource:shader options:nil error:&errors];
    if (errors != nil) {
        NSLog(@"Failed to compiles shaders %@", [errors description]);
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

    _pipelineDescriptor = [[MTLRenderPipelineDescriptor alloc] init];
    _pipelineDescriptor.vertexFunction = vertFunc;
    _pipelineDescriptor.vertexDescriptor = vertexDesc;
    _pipelineDescriptor.fragmentFunction = fragFunc;
    _pipelineDescriptor.colorAttachments[0].pixelFormat = _targetTexture.pixelFormat;
}

void shrFloat(float *source, int length, int distance, float *destination) {
    while (distance < 0) {
        distance += length;
    }
    for (int i = 0; i < length; i++) {
        destination[(distance + i) % length] = source[i];
    }
}

- (void)renderBuffer:(id <RTCI420Buffer>)frameBuffer rotation:(RTCVideoRotation)rotation {
    if (rotation % 90 != 0) {
        NSLog(@"Only multiples of 90 degrees are supported");
        rotation = RTCVideoRotation_0;
    }

    id <MTLCommandBuffer> buffer = [_commandQueue commandBuffer];
    id <MTLRenderCommandEncoder> encoder = [buffer renderCommandEncoderWithDescriptor:_passDescriptor];

    static const float posData[] = {
            // X, Y, Z
            -1, -1, 0, // Bottom Left
            1, -1, 0, //Bottom Right
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
    shrFloat((float *) uvPosData, 8, -2 * (rotation / 90), rotatedUV);

    id <MTLBuffer> posBuf = [_device newBufferWithBytes:posData length:3 * 4 * sizeof(float) options:(MTLResourceOptions) 0];
    id <MTLBuffer> uvBuf = [_device newBufferWithBytes:rotatedUV length:4 * 2 * sizeof(float) options:(MTLResourceOptions) 0];
    [encoder setVertexBuffer:posBuf offset:0 atIndex:0];
    [encoder setVertexBuffer:uvBuf offset:0 atIndex:1];

    [_yTexture replaceRegion:MTLRegionMake2D(0, 0, static_cast<NSUInteger>(frameBuffer.width), static_cast<NSUInteger>(frameBuffer.height))
                 mipmapLevel:0
                   withBytes:frameBuffer.dataY
                 bytesPerRow:static_cast<NSUInteger>(frameBuffer.strideY)];

    [_uTexture replaceRegion:MTLRegionMake2D(0, 0, static_cast<NSUInteger>(frameBuffer.width / 2), static_cast<NSUInteger>(frameBuffer.height / 2))
                 mipmapLevel:0
                   withBytes:frameBuffer.dataU
                 bytesPerRow:static_cast<NSUInteger>(frameBuffer.strideU)];

    [_vTexture replaceRegion:MTLRegionMake2D(0, 0, static_cast<NSUInteger>(frameBuffer.width / 2), static_cast<NSUInteger>(frameBuffer.height / 2))
                 mipmapLevel:0
                   withBytes:frameBuffer.dataV
                 bytesPerRow:static_cast<NSUInteger>(frameBuffer.strideV)];

    [encoder setFragmentTexture:_yTexture atIndex:0];
    [encoder setFragmentTexture:_uTexture atIndex:1];
    [encoder setFragmentTexture:_vTexture atIndex:2];

    NSError *error = nil;
    id <MTLRenderPipelineState> pipeline = [_device newRenderPipelineStateWithDescriptor:_pipelineDescriptor error:&error];
    if (error != nil) {
        NSLog(@"Failed to create render pipeline state %@", [error description]);
    }

    [encoder setRenderPipelineState:pipeline];

    [encoder drawIndexedPrimitives:MTLPrimitiveTypeTriangle
                        indexCount:6
                         indexType:MTLIndexTypeUInt16
                       indexBuffer:_indexBuffer
                 indexBufferOffset:0];

    [encoder drawPrimitives:MTLPrimitiveTypeTriangle vertexStart:0 vertexCount:3];
    [encoder endEncoding];
    [buffer commit];
    [buffer waitUntilCompleted];
}

- (long long)texture {
    return reinterpret_cast<long long int>(_targetTexture);
}

@end
