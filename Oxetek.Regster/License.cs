
#region Version Info
/**************************************************************** 
**************************************************************** 
* 作    者：SAM
* 邮    箱:	support@oxetek.com
* CLR 版本：4.0.30319.42000
* 创建时间：2019-12-14 15:32:09
* 当前版本：1.0.0.0
* 
* 描述说明： 
* 
* 修改历史： 
* timestamp: 1a2d740b-a030-4168-b38c-1e23c92f78c8
****************************************************************** 
* Copyright @ SAM 2019 All rights reserved**********************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxetek.Regster
{
    public class License
    {
        public string ExpiredDate { get; set; }
        public string Id { get; set; }
        public string Database { get; set; }
    }
}
