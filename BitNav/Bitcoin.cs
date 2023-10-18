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

                this.address = GenerateRandomBitcoinAddress(privateKey, GenerateRandomInteger());
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

        public string GetAddress()
        {
            if (randomized)
            {
                this.address = GenerateRandomBitcoinAddress(privateKey, GenerateRandomInteger());
            }

            return this.address;
        }

        private string GenerateBitcoinAddress(string privateKeyHex)
        {
            Key privateKey = Key.Parse(privateKeyHex, Network.Main);
            BitcoinAddress address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            return address.ToString();
        }

        private string GenerateRandomBitcoinAddress(string privateKeyString, int index)
        {

            Network network = Network.Main;
            Key privateKey = Key.Parse(privateKeyString, network);
            ExtKey extendedPrivateKey = new ExtKey(privateKey.ToHex());

            
            ExtKey derivedKey = extendedPrivateKey.Derive((uint)index);
            PubKey publicKey = derivedKey.PrivateKey.PubKey;
            BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, network);

            return address.ToString();
        }

        private int GenerateRandomInteger()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[4]; // 4 bytes for a 32-bit integer

                rng.GetBytes(randomBytes);

                int randomValue = BitConverter.ToInt32(randomBytes, 0);

                return Math.Abs(randomValue);
            }
        }

    }
}