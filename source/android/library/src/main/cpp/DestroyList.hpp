//
// Created by Aleksey Zinchenko on 07/03/2017.
//

namespace std {
    template<>
    struct std::less<DestroyKey>
    {
        bool operator()(const DestroyKey& k1, const DestroyKey& k2) const
        {
            return k1.textureId < k2.textureId || k1.contextId < k2.contextId;
        }
    };
}

template <class T>
DestroyList<T>::DestroyList(): m_mutex(new Mutex()), m_map(new DestroyMap()) {

}

template <class T>
DestroyList<T>::~DestroyList() {
    delete m_mutex;
    delete m_map;
}

template <class T>
void DestroyList<T>::AddObject(void *textureId, void *contextId, T object) {
    LockGuard lock(m_mutex);

    DestroyKey key = (DestroyKey) { .contextId = contextId, .textureId = textureId };
    m_map->insert(std::make_pair(key, object));
}

template <class T>
void DestroyList<T>::DestroyObject(void *textureId, void *contextId) {
    LockGuard lock(m_mutex);

    DestroyKey key = (DestroyKey) { .contextId = contextId, .textureId = textureId };
    typedef typename std::map<DestroyKey, T>::iterator Iterator;
    const Iterator &it = m_map->find(key);
    if (it != m_map->end()) {
        delete it->second;
        m_map->erase(it);
    }
}