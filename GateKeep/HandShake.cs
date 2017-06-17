using System.Threading.Tasks;


namespace GateKeep
{
    public class HandShake
    {
        private Cloud dataVerify;

        public HandShake()
        {
            dataVerify = Cloud.Instance;
        }

        public async Task<string> Shake(string user, string pass)
        {
            return await dataVerify.LogIn(user, pass);
        }
    }
}