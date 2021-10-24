using Oxetek.Regster.Extentions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxetek.Regster
{
    public partial class FrmMain : Form
    {
        private static int keySize = 1024;
        public FrmMain()
        {
            InitializeComponent();
        }
        
        static byte[] EncryptData(byte[] data, string publicKey)
        {
            //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            var rsa = RSA.Create(keySize);
            RSAKeyExtensions.FromXmlString(rsa, publicKey);
            //将公钥导入到RSA对象中，准备加密；
            //rsa.FromXmlString(publicKey);
            //对数据data进行加密，并返回加密结果；
            //第二个参数用来选择Padding的格式
            //return rsa.Encrypt(data, false);
            return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }
        static byte[] DecryptData(byte[] data, string privateKey)
        {
            //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            var rsa = RSA.Create(keySize);
            RSAKeyExtensions.FromXmlString(rsa, privateKey);
            //将私钥导入RSA中，准备解密；
            //rsa.FromXmlString(privateKey);
            //对数据进行解密，并返回解密结果；
            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }


        static bool EnsureAccess(string code, string cpuid)
        {
            try
            {
                var length = Convert.ToInt32(code.Substring(0, 3));
                var data = Convert.FromBase64String(code.Substring(3));
                code = Encoding.UTF8.GetString(data);

                //客户端
                //将服务器端的sendcode 用base64解密
                var license = code.Substring(0, length);
                var key = code.Substring(length);

                //将许可证发给用户
                //var enbyte = Convert.FromBase64String(code);
                var destr = DecryptData(Convert.FromBase64String(license), key);
                var delicensestr = Encoding.UTF8.GetString(destr);
                //Console.WriteLine(delicensestr);

                //解析成license类
                var delicense = Newtonsoft.Json.JsonConvert.DeserializeObject<License>(delicensestr);

                if (delicense == null)
                {
                    //不是有效的许可证返回false
                }

                if (DateTime.Now > DateTime.Parse(delicense.ExpiredDate))
                {
                    //过期返回false
                }

                return delicense.Id.Equals(cpuid);
            }
            catch
            {
                return false;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(Guid.NewGuid().ToString("N"));

            //Scope scope = Scope.adv;
            //var sc = scope.ToString();


            var rsa = RSA.Create(keySize);
            var publicKey = RSAKeyExtensions.ToXmlString(rsa, false);

#region core

            //var publicKey = rsa.ToXmlString(false);
            //var privateKey = rsa.ToXmlString(true);
            var privateKey = RSAKeyExtensions.ToXmlString(rsa, true);
#endregion

#region .netframework框架
        /*
        //声明一个RSA算法的实例，由RSACryptoServiceProvider类型的构造函数指定了密钥长度为1024位
        //实例化RSACryptoServiceProvider后，RSACryptoServiceProvider会自动生成密钥信息。
        var rsaProvider = new RSACryptoServiceProvider(keySize);
        //将RSA算法的公钥导出到字符串PublicKey中，参数为false表示不导出私钥
        var publicKey = rsaProvider.ToXmlString(false);
        //将RSA算法的私钥导出到字符串PrivateKey中，参数为true表示导出私钥
        var privateKey = rsaProvider.ToXmlString(true);
        *
        */
#endregion

        //RESUME:

            //1、id为 mysql的uuid 获取，ba7ed0e8-6fed-11e9-a83a-fa163e1c8e05 【select @@server_uuid as uuid】
            //2、再修改数据库名为服务器的数据库名

            //Console.WriteLine("请输入id:");

            //var readkey = Console.ReadLine();
            //if (readkey.Equals("q", StringComparison.OrdinalIgnoreCase))
            //{
            //    return;
            //}
            ////生成公钥和私钥
            //var cpuid = readkey;// "BFEBFBFF000906EA";

            License license = new License()
            {
                ExpiredDate = dtTimeExpired.Value.ToString("yyyy-MM-dd"),
                Id = txtServerId.Text.Replace("-", ""),
                Database = txtDbName.Text,//"mssqlexport"
                //Database = "sdjz_uat"//aa20a923171f11eab4cb00163e0edbc0
            };
            var licenseKey = Newtonsoft.Json.JsonConvert.SerializeObject(license);
            var enstr = EncryptData(Encoding.UTF8.GetBytes(licenseKey), publicKey);
            var tostr = Convert.ToBase64String(enstr);
            var code = $"{tostr}{privateKey}";
            var enstrlength = tostr.Length.ToString().PadLeft(3, '0');

            //服务器生成了许可证
            //Console.WriteLine($"enstr:{code}");

            //再base64一次
            var sendcode = $"{enstrlength}{Convert.ToBase64String(Encoding.UTF8.GetBytes(code))}";


            txtCode.Text = sendcode;
            //Console.WriteLine($"code:{ sendcode}");
            //var eq = EnsureAccess(sendcode, cpuid);

            //goto RESUME;
        }
    }
}
