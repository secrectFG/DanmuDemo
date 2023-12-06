using DMDataDefine;
using DMGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace DMGame
{
    public class DMMessageHandler : Singleton<DMMessageHandler>
    {

        public bool showJsonLog = false;

        private void Start()
        {
            MessageManager.Instance.AddListener(EventNameDefine.收到弹幕消息, msg =>
            {
                HandleBiliBiliMsg(msg.Content as string);
            });
            MessageManager.Instance.AddListener(EventNameDefine.抖音消息, msg =>
            {
                HandleDYMsg(msg.Content as string);
            });
        }

        
        private void HandleBiliBiliMsg(string jsonstr)
        {
            var data = JsonConvert.DeserializeObject<WrapData>(jsonstr);
            if (showJsonLog) print($"收到B站弹幕消息 data:{data} ");
            switch (data.MsgType)
            {
                case MsgTypeEnum.Comment:
                    {
                        try
                        {
                            var obj = JObject.Parse(data.JsonData);
                            //print(data.JsonData);
                            var CommentText = obj["info"][1].ToString();
                            var UserID = obj["info"][2][0].ToObject<long>();
                            var UserName = obj["info"][2][1].ToString();
                            var isAdmin = obj["info"][2][2].ToString() == "1";
                            var isVIP = obj["info"][2][3].ToString() == "1";
                            var UserGuardLevel = obj["info"][7].ToObject<int>();

                            if (UserID == 0)
                            {
                                Debug.LogWarning("用户ID==0 UserName:" + UserName);
                                return;
                            }
                            MessageManager.Instance.SendMessage(EventNameDefine.弹幕消息_聊天,
                                content: new DMGameData.DMMessageData("B")
                                {
                                    UserId = UserID,
                                    UserName = UserName,
                                    CommentText = CommentText,
                                    MsgType = DMGameData.MsgType.聊天,
                                });
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"{data.MsgType}弹幕解析出错." + e);
                        }


                    }
                    break;
                case MsgTypeEnum.GiftSend:
                    {
                        var gift = JsonConvert.DeserializeObject<Gift>(data.JsonData).data;
                        MessageManager.Instance.SendMessage(EventNameDefine.弹幕消息_礼物,
                            content: gift);
                    }
                    break;
                case MsgTypeEnum.Interact:
                    {
                        try
                        {
                            var obj = JObject.Parse(data.JsonData);
                            var UserName = obj["data"]["uname"].ToString();
                            var UserID = obj["data"]["uid"].ToObject<long>();
                            var InteractType = (InteractTypeEnum)obj["data"]["msg_type"].ToObject<int>();
                            switch (InteractType)
                            {
                                case InteractTypeEnum.Enter:
                                    Debug.Log($"用户进入房间 UserName:{UserName} UserID:{UserID}");
                                    break;
                                case InteractTypeEnum.Follow:
                                    break;
                                case InteractTypeEnum.Share:
                                    break;
                                case InteractTypeEnum.SpecialFollow:
                                    break;
                                case InteractTypeEnum.MutualFollow:
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"{data.MsgType}弹幕解析出错." + e);
                            return;
                        }

                    }
                    break;
                //case MsgTypeEnum.Welcome:
                //    break;
                //case MsgTypeEnum.LiveStart:
                //    break;
                //case MsgTypeEnum.LiveEnd:
                //    break;
                //case MsgTypeEnum.Unknown:
                //    break;
                //case MsgTypeEnum.WelcomeGuard:
                //    break;
                //case MsgTypeEnum.GuardBuy:
                //break;
                default:
                    break;
            }
        }

        private void HandleDYMsg(string jsonstr)
        {
            try
            {
                var obj = JObject.Parse(jsonstr);
                var type = obj["type"].ToString();
                var UserId = obj["id"].ToObject<long>();
                var UserName = obj["nickname"].ToString();
                switch (type)
                {
                    case "聊天":
                        {
                            var CommentText = obj["content"].ToString();
                            var imageurl = obj["imageurl"].ToString();

                            MessageManager.Instance.SendMessage(EventNameDefine.弹幕消息_聊天,
                                new DMGameData.DMMessageData("D")
                                {
                                    UserId = UserId,
                                    UserName = UserName,
                                    CommentText = CommentText,
                                    HeadUrl = imageurl,
                                    MsgType = DMGameData.MsgType.聊天,
                                });

                        }
                        break;
                    case "进入":
                        {
                            var imageurl = obj["imageurl"].ToString();
                            Debug.Log($"进入消息 imageurl:{imageurl}");

                        }
                        break;
                    case "礼物":
                        {
                            var gift = obj["gift"];
                            var giftId = gift["giftId"].ToObject<long>();
                            var giftName = gift["giftName"].ToString();
                            var giftCount = gift["giftCount"].ToObject<int>();//repeatCount
                            var diamondCount = gift["diamondCount"].ToObject<int>();
                            MessageManager.Instance.SendMessage(EventNameDefine.弹幕消息_礼物,
                                                                new DMDataDefine.Data()
                                                                {
                                                                    giftId = giftId,
                                                                    giftName = giftName,
                                                                    price = diamondCount,
                                                                    num = giftCount,
                                                                });

                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"抖音弹幕解析出错." + e + "\n jsonstr:" + jsonstr);
            }

        }
    }
}
