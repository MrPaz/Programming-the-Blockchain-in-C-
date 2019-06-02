using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.Protocol;

namespace ProgrammingBlockchain3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Authentication();
            KeyGenerationAndEncyption();
            Console.ReadLine();
        }

        public static void Authentication()
        {
            var message = "Nicolas Dorier Book Funding Address";
            var address = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var signature = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";

            bool NicolasOwnsPrivateKeyOfBookAddress = address.VerifyMessage(message, signature);
            Console.WriteLine(NicolasOwnsPrivateKeyOfBookAddress); // True
        }

        public static void KeyGenerationAndEncyption()
        {
            //var privateKey = new Key();
            //var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
            //Console.WriteLine(bitcoinPrivateKey); // KyYVLzTSmfhdPYinq3K5AoNiWwJK531hceK9tt9RRW7KjdJVDQvc

            //BitcoinEncryptedSecret encryptedBitcoinPrivateKey = bitcoinPrivateKey.Encrypt("password");
            //Console.WriteLine(encryptedBitcoinPrivateKey); // 6PYMKmzYzptv6RboNbqWyBNwx51cxkMBKPaUUufTR8HogT2sWhwubx6sQJ

            //var decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret("password");
            //Console.WriteLine(decryptedBitcoinPrivateKey); // KyYVLzTSmfhdPYinq3K5AoNiWwJK531hceK9tt9RRW7KjdJVDQvc

            var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            var encryptedKey = encryptedKeyResult.EncryptedKey;
            var confirmationCode = encryptedKeyResult.ConfirmationCode;
            Console.WriteLine(confirmationCode.Check("my secret", generatedAddress)); // True
            var bitcoinPrivateKey2 = encryptedKey.GetSecret("my secret");
            Console.WriteLine(bitcoinPrivateKey2.GetAddress() == generatedAddress); // True
            Console.WriteLine(bitcoinPrivateKey2); // L1tKFXfnUAN1v76DG1sy83JMX18M1ctkGPZXDbBACUhKokFE1VeZ

        }
    }
}
