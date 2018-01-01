using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Confaque
{
    public class EmailClientSettingOptions
    {
        public string Name { get; set; }

        public string Server { get; set; }

        public int Port { get; set; }

        public bool IsActive { get; set; }
    }
}
