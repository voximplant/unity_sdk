//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#include "BaseVideoRenderer.h"

BaseVideoRenderer::BaseVideoRenderer(int width, int height):
        m_ackWidth(width), m_ackHeight(height),
        m_isInvalidated(false) {

}

void BaseVideoRenderer::Invalidate() {
    m_isInvalidated = true;
}

bool BaseVideoRenderer::IsValidForSize(int width, int height) {
    return !m_isInvalidated
            && IsActiveContextMatch()
            && m_ackWidth == width
            && m_ackHeight == height;
}

BaseVideoRenderer::~BaseVideoRenderer() {

}

bool BaseVideoRenderer::IsActiveContextMatch() {
    return true;
}
