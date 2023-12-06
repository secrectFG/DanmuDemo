namespace DMGame
{
    using System;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;
    public class DMGameObjectPool : ObjectPool<GameObject>
    {
        public DMGameObjectPool(GameObject prefab, int defaultCapacity = 500)
        : base(
            createFunc: () => Object.Instantiate(prefab),
            actionOnGet: (go) => go.gameObject.SetActive(true),
            actionOnRelease: (go) => go.gameObject.SetActive(false),
            actionOnDestroy: (go) => Object.Destroy(go.gameObject),
            defaultCapacity: defaultCapacity,
#if UNITY_EDITOR
            collectionCheck: true
#else
            collectionCheck: false
#endif
        )
        {
        }
    }
}