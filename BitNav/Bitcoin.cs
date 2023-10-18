using System.Security.Cryptography;
using NBitcoin;
using NBitcoin.Payment;
namespace BitNav
{
    public class Bitcoin
    {
        private string privateKey;
        private string address;
        private RandomNumberGenerator rng;
        public readonly bool randomized;
        public Bitcoin(string privateKey, bool randomizeAddress)
        {
            this.privateKey = privateKey;
            this.randomized = randomizeAddress;
            rng = RandomNumberGenerator.Create();
            if (randomizeAddress)
            {

                this.address = GenerateRandomBitcoinAddress(privateKey, 0);
            }
            else
            {
                this.address = GenerateBitcoinAddress(privateKey);
            }
        }

        public Bitcoin(string address)
        {
            this.address = address;
        }

        public static string GenerateBitcoinAddress(string privateKeyHex)
        {
            Key privateKey = Key.Parse(privateKeyHex, Network.Main);
            BitcoinAddress address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            return address.ToString();
        }

        public static string GenerateRandomBitcoinAddress(string privateKeyString, int index)
        {

            Network network = Network.Main;
            Key privateKey = Key.Parse(privateKeyString, network);
            ExtKey extendedPrivateKey = new ExtKey(privateKey.ToHex());

            
            ExtKey derivedKey = extendedPrivateKey.Derive((uint)index);
            PubKey publicKey = derivedKey.PrivateKey.PubKey;
            BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, network);

            return address.ToString();
        }


    }
}