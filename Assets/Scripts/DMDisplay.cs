namespace DMGame.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class DMDisplay : MonoBehaviour
    {
        public DMDisplayItem template;
        // public int maxItemCount = 10;

        public float moveItemAnimeTime = 0.5f;

        float itemHeight = 0;
        // Vector3 templatePos;
        Vector2 offestMin;
        Vector2 offestMax;

        DMGameObjectPool itemPool;
        List<DMDisplayItem> items = new List<DMDisplayItem>();

        bool isItemsMoving = false;

        DMDisplayItem lastItem = null;

        struct ItemData
        {
            public string userName;
            public string message;
            public string time;
        }

        List<ItemData> itemTempDatas = new List<ItemData>();

        float height = 0;

        void Awake()
        {
            template.gameObject.SetActive(false);
            var rt = template.GetComponent<RectTransform>();
            itemHeight = rt.rect.height;
            // templatePos = template.transform.localPosition;
            offestMin = rt.offsetMin;
            offestMax = rt.offsetMax;
            height = GetComponent<RectTransform>().rect.height;
            itemPool = new DMGameObjectPool(template.gameObject);
        }

        public void AddItem(string userName, string message, string time = null)
        {
            if (isItemsMoving)
            {
                itemTempDatas.Add(new ItemData() { userName = userName, message = message, time = time });
                if (itemTempDatas.Count > 10000)
                {
                    itemTempDatas.RemoveAt(0);
                    Debug.LogWarning("itemTempDatas.Count > 10000");
                }
                return;
            }
            // var item = Instantiate(template);
            var itemObj = itemPool.Get();
            var item = itemObj.GetComponent<DMDisplayItem>();
            item.UserNameAdapter.text = userName;
            item.MessageAdapter.text = message;
            if (time == null)
                time = System.DateTime.Now.ToString("[HH:mm:ss]");
            item.TimeAdapter.text = time;
            item.transform.SetParent(template.transform.parent);
            item.transform.localScale = Vector3.one;
            var rt = item.GetComponent<RectTransform>();
            rt.offsetMin = offestMin;
            rt.offsetMax = offestMax;
            items.Add(item);
            // if (items.Count > maxItemCount)
            // {
            //     var firstItem = items[0];
            //     items.RemoveAt(0);
            //     itemPool.Release(firstItem.gameObject);
            // }
            while (items.Count * itemHeight > height + itemHeight*2)
            {
                var firstItem = items[0];
                items.RemoveAt(0);
                itemPool.Release(firstItem.gameObject);
            }

            if (lastItem != null)
                item.transform.localPosition = lastItem.transform.localPosition + new Vector3(0, -itemHeight, 0);

            lastItem = item;
            MoveItems().Forget();
        }

        async UniTask MoveItems()
        {
            while (isItemsMoving)
            {
                await UniTask.NextFrame();
            }
            isItemsMoving = true;
            Tweener tweener = null;
            foreach (var item in items)
            {
                var itemTrans = item.transform;
                tweener = itemTrans.DOLocalMoveY(itemTrans.localPosition.y + itemHeight, moveItemAnimeTime).SetEase(Ease.InOutQuad);
            }
            await tweener;
            isItemsMoving = false;
        }

        private void Update()
        {
            if (itemTempDatas.Count > 0 && !isItemsMoving)
            {
                var data = itemTempDatas[0];
                itemTempDatas.RemoveAt(0);
                AddItem(data.userName, data.message, data.time);
            }

        }

        int counter = 0;
        private void OnGUI()
        {
            if (GUILayout.Button("AddItem"))
            {
                counter++;
                AddItem("UserName" + counter, "Message");
            }
        }
    }
}