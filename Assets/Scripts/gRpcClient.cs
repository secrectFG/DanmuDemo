using Grpc.Core;
using LiveProto;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Cysharp.Threading.Tasks;
using Channel = Grpc.Core.Channel;


public class gRpcClient : Singleton<gRpcClient>
{
    public string ip = "127.0.0.1";//目前在Lua里面设置
    public int port = 17989;//目前在Lua里面设置
    public bool showMSGLog = false;//目前在Lua里面设置
    Channel channel = null;
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    LiveMessager.LiveMessagerClient client;

    class HandleRequest
    {
        public string requestType;
    }

    class UserInfoRequest : HandleRequest
    {
        public long userid;

        public UserInfoRequest()
        {
            requestType = "GetBilibiliUserInfo";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MessageManager.Instance.AddListener(
            EventNameDefine.LUA_INIT_OK,
            msg =>
            {
                Init().Forget();
            },
            autoRemove: true
        );
    }

    async UniTask Init()
    {
        await UniTask.Yield();
        print($"grpc start {ip}:{port}");
        channel = new Channel($"{ip}:{port}", ChannelCredentials.Insecure);
        client = new LiveMessager.LiveMessagerClient(channel);
        try
        {
            await channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(3));
        }
        catch (Exception e)
        {
            Debug.LogError("GRPC连接异常：" + e);
            return;
        }

        print("grpc connected ");
        var rsp = client.JsonMsgRouter(new Empty());
        var b = await GetStream(rsp.ResponseStream);
        while (!b && Application.isPlaying)
        {
            await Task.Delay(1000);
            Debug.LogWarning("尝试重新连接");
            rsp = client.JsonMsgRouter(new Empty());
            b = await GetStream(rsp.ResponseStream);
        }
        print("grpc end ");
    }

    private void OnDestroy()
    {
        tokenSource.Cancel();
        channel?.ShutdownAsync().Wait(3);
    }

    public void Send(string type, string jsonStr)
    {
        client.HandleJsonMsgAsync(new StringMsg() { JsonStr = jsonStr, Type = type });
    }

    public async Task<Sprite> GetSpriteByUserId(long userid)
    {
        var jsonstr = JsonConvert.SerializeObject(new UserInfoRequest() { userid = userid });
        print(jsonstr);

        var faceUrl = "";
        try
        {
            var rsp = await client.HandleJsonMsgAsync(
                new StringMsg() { Type = "ClientRequestHandle", JsonStr = jsonstr }
            );
            var obj = JObject.Parse(rsp.JsonStr);
            if (obj["face"] != null)
                faceUrl = obj["face"].ToString();
            else
            {
                var err = obj["error"].ToString();
                Debug.LogWarning($"GetSpriteByUserId error. userid:{userid} error:{err}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GetSpriteByUserId paser error. userid:{userid} e:" + e);
            return null;
        }

        if (!string.IsNullOrEmpty(faceUrl))
        {
            return await GameUtils.GetSpriteByUrl(faceUrl);
        }
        else
        {
            Debug.LogWarning($"GetSpriteByUserId error. userid:{userid} faceUrl is null");
        }

        return null;
    }

    async Task<bool> GetStream(IAsyncStreamReader<StringMsg> reader)
    {
        try
        {
            while (await reader.MoveNext(tokenSource.Token))
            {
                var request = reader.Current;
                if (showMSGLog)
                {
                    Debug.Log($"gRPC:{request}");
                }
                MessageManager.Instance.SendMessage($"GRPC-{request.Type}", request.JsonStr);
            }
        }
        catch (Exception e)
        {
            if (!tokenSource.IsCancellationRequested)
            {
                Debug.LogWarning($"网络流异常中断:" + e);
                return false;
            }
        }
        return true;
    }
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
