using System;
using System.Collections.Generic;
using System.Threading;
using BM;
using CommandLine;
using UnityEngine;

namespace ET
{
	public class Init: MonoBehaviour
	{
		public static Init Instance;

		public Camera UICamera;
		public GlobalConfig GlobalConfig;
		
		private async void Awake()
		{
			Instance = this;
			
			DontDestroyOnLoad(gameObject);
			
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};
				
			Game.AddSingleton<MainThreadSynchronizationContext>();

			// 命令行参数
			string[] args = "".Split(" ");
			Parser.Default.ParseArguments<Options>(args)
				.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
				.WithParsed(Game.AddSingleton);
			
			Game.AddSingleton<TimeInfo>();
			Game.AddSingleton<Logger>().ILog = new UnityLogger();
			Game.AddSingleton<ObjectPool>();
			Game.AddSingleton<IdGenerater>();
			Game.AddSingleton<EventSystem>();
			Game.AddSingleton<TimerComponent>();
			Game.AddSingleton<CoroutineLockComponent>();
			
			ETTask.ExceptionHandler += Log.Error;

			AssetComponentConfig.DefaultBundlePackageName = "AllBundle";
			Dictionary<string, bool> updatePackageBundle = new Dictionary<string, bool>()
			{
				{AssetComponentConfig.DefaultBundlePackageName, false},
			};
			UpdateBundleDataInfo updateBundleDataInfo = await AssetComponent.CheckAllBundlePackageUpdate(updatePackageBundle);
			if (updateBundleDataInfo.NeedUpdate)
			{
				Debug.LogError("需要更新, 大小: " + updateBundleDataInfo.NeedUpdateSize);
				await AssetComponent.DownLoadUpdate(updateBundleDataInfo);
			}
			await AssetComponent.Initialize(AssetComponentConfig.DefaultBundlePackageName);

			
			Game.AddSingleton<CodeLoader>().Start();
		}

		private void Update()
		{
			Game.Update();
		}

		private void LateUpdate()
		{
			Game.LateUpdate();
			Game.FrameFinishUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Close();
		}
	}
}