//
// Created by Aleksey Zinchenko on 07/03/2017.
//

#ifndef ANDROID_DESTROYLIST_H
#define ANDROID_DESTROYLIST_H

#include <map>
#include <memory>
#include "LockGuard.hpp"

struct DestroyKey {
    void *textureId;
    void *contextId;
};

template <class T>
class DestroyList {
public:
    DestroyList();
    ~DestroyList();

    void AddObject(void *textureId, void *contextId, T object);
    void DestroyObject(void *textureId, void *contextId);

private:
    Mutex* m_mutex;

    typedef std::map<DestroyKey, T> DestroyMap;
    DestroyMap* m_map;
};

#include "DestroyList.hpp"

#endif //ANDROID_DESTROYLIST_H
