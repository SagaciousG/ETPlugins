using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene clientScene, string account, string password)
        {
            try
            {
                // 创建一个ETModel层的Session
                clientScene.RemoveComponent<RouterAddressComponent>();
                // 获取路由跟realmDispatcher地址
                RouterAddressComponent routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();
                if (routerAddressComponent == null)
                {
                    routerAddressComponent = clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
                    await routerAddressComponent.Init();
                    
                    clientScene.RemoveComponent<NetClientComponent>();
                    clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);
                }
                IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);
                
                var session = await RouterHelper.CreateRouterSession(clientScene, realmAddress);
                var r2CLogin = (LoginResponse) await SessionHelper.Call(session, new LoginRequest() { Account = account, Password = password });
                if (r2CLogin.Error > 0)
                {
                    return r2CLogin.Error;
                }

                clientScene.GetComponent<PlayerComponent>().Account = account;
                clientScene.GetComponent<SessionComponent>()[SessionType.Realm] = session;
                
                await EventSystem.Instance.PublishAsync(clientScene, new EventType.LoginFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return 0;
        }

        public static async ETTask<int> Register(Scene clientScene, string account, string password)
        {
            try
            {
                RegistResponse r2CRegist;
                Session session = null;
                try
                {
                    clientScene.RemoveComponent<RouterAddressComponent>();
                    var routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();
                    if (routerAddressComponent == null)
                    {
                        routerAddressComponent =
                                clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
                        await routerAddressComponent.Init();

                        clientScene.RemoveComponent<NetClientComponent>();
                        clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);
                    }

                    IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

                    session = await RouterHelper.CreateRouterSession(clientScene, realmAddress);
                    {
                        r2CRegist = (RegistResponse)await SessionHelper.Call(session, new RegistRequest() { Account = account, Password = password });
                    }
                    return r2CRegist.Error;
                }
                finally
                {
                    session?.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return 0;
        }
    }
}