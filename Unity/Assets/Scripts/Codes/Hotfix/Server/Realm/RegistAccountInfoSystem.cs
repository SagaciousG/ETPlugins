namespace ET.Server
{
    public static class RegistAccountInfoSystem
    {
        [ObjectSystem]
        public class RegistAccountInfoAwakeSystem  : AwakeSystem<AccountInfo, string, string>
        {
            protected override void Awake(AccountInfo self, string a, string b)
            {
                self.account = a;
                self.password = b;
                self.createTicks = TimeHelper.ServerNow();
                self.Id = IdGenerater.Instance.GenerateId();
            }
        }
    }
}