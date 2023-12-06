using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DMGame.UI;
using DMGameData;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public DMDisplay display;

    void Start()
    {
        MessageManager.Instance.AddListener(
                EventNameDefine.弹幕消息_聊天,
                msg =>
                {
                    var data = msg.Content as DMMessageData;
                    HandleDmChat(data).Forget();
                }
            );
        MessageManager.Instance.AddListener(
                EventNameDefine.弹幕消息_礼物,
                msg =>
                {
                    var data = msg.Content as DMDataDefine.Data;
                    HandleDmGift(data);
                }
            );
    }

    private async UniTask HandleDmChat(DMMessageData chatData)
        {
            var UserID = chatData.UserId;
            var CommentText = chatData.CommentText;
            var UserName = chatData.UserName;
            display.AddItem(UserName, CommentText);
            await UniTask.Yield();
        }

    private void HandleDmGift(DMDataDefine.Data gift)
        {
            Debug.Log($"收到礼物：{gift}");
        }
    
}
