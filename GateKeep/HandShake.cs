using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GateKeep
{
    public class HandShake
    {
        private Cloud dataVerify;

        public HandShake()
        {
            dataVerify = new Cloud();
        }

        public async Task<string> Shake()
        {
            return await dataVerify.LogIn();
        }
    }
}