using TMPro;
using UnityEngine.UI;
using XGame;

namespace ET.Client
{
    public static partial class UILoginComponentSystem
    {
        private static void OnAwake(this UILoginComponent self)
        {
            self.loginBtn.GetComponent<XImage>().OnClick(self.OnLogin);
            self.registerBtn.GetComponent<XImage>().OnClick(self.OnRegister);
        }

        public static void OnCreate(this UILoginComponent self)
        {
            
        }
        
        public static void OnShow(this UILoginComponent self, params object[] args)
        {
        }
        
        public static void OnRemove(this UILoginComponent self)
        {
        }
        
        
        public static async void OnLogin(this UILoginComponent self)
        {
            self.tips.GetComponent<TextMeshProUGUI>().text = "";
            var result = await LoginHelper.Login(
                self.DomainScene(), 
                self.account.GetComponent<TMP_InputField>().text, 
                self.password.GetComponent<TMP_InputField>().text);
            if (result == ErrorCode.ERR_AccountOrPwNotExist)
            {
                self.tips.GetComponent<TextMeshProUGUI>().text = "账号或密码不正确";
            } 
        }
		
        public static async void OnRegister(this UILoginComponent self)
        {
            self.tips.GetComponent<TextMeshProUGUI>().text = "";
            var result = await LoginHelper.Register(
                self.DomainScene(), 
                self.account.GetComponent<TMP_InputField>().text, 
                self.password.GetComponent<TMP_InputField>().text);
            if (result == ErrorCode.ERR_AccountIsExist)
            {
                self.tips.GetComponent<TextMeshProUGUI>().text = "账号已存在";
            }
            else
            {
                self.tips.GetComponent<TextMeshProUGUI>().text = "注册成功";
            }
        }
    }
}