//
// Created by Aleksey Zinchenko on 03/03/2017.
//

#ifndef VOXDEMO_LOCKGUARD_HPP
#define VOXDEMO_LOCKGUARD_HPP

#include <pthread.h>

struct Mutex {
public:
    Mutex() : m_mutex((pthread_mutex_t){ 0 }) {
        pthread_mutex_init(&m_mutex, NULL);
    }

    ~Mutex() {
        pthread_mutex_destroy(&m_mutex);
    }

    void Acquire() { pthread_mutex_lock(&m_mutex); }
    void Release() { pthread_mutex_unlock(&m_mutex); }

private:
    pthread_mutex_t m_mutex;
};

struct LockGuard {
    LockGuard(Mutex *mutex) : _ref(mutex) {
        _ref->Acquire();
    };

    ~LockGuard() {
        _ref->Release();
    }

private:
    LockGuard(const LockGuard &);

    Mutex *_ref;
};

#endif //VOXDEMO_LOCKGUARD_HPP
