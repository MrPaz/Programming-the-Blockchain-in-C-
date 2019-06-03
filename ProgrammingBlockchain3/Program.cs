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

            //var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);
            //EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            //var generatedAddress = encryptedKeyResult.GeneratedAddress;
            //var encryptedKey = encryptedKeyResult.EncryptedKey;
            //var confirmationCode = encryptedKeyResult.ConfirmationCode;
            //Console.WriteLine(confirmationCode.Check("my secret", generatedAddress)); // True
            //var bitcoinPrivateKey2 = encryptedKey.GetSecret("my secret");
            //Console.WriteLine(bitcoinPrivateKey2.GetAddress(ScriptPubKeyType.Legacy) == generatedAddress); // True  // add ScriptPubKeyType.Legacy as argument to GetAddress to get rid of warning message re: GetAddress() being obsolete
            //Console.WriteLine(bitcoinPrivateKey2); // L1tKFXfnUAN1v76DG1sy83JMX18M1ctkGPZXDbBACUhKokFE1VeZ

            ExtKey masterKey = new ExtKey(); // generates a master private key
            Console.WriteLine("Master Key: " + masterKey.ToString(Network.Main));
            for (int i = 0; i < 5; i++) // which can be used to deterministically derive child keys
            {
                ExtKey key = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + ": " + key.ToString(Network.Main));
            }

            ExtKey extKey = new ExtKey(); // an ExtKey contain a Key (as a property of the ExtKey object); "You can go back from a Key to an ExtKey by supplying the Key and the Chaincode to the ExtKey constructor as follows:"
            byte[] chainCode = extKey.ChainCode; // what is a chaincode?
            Key key2 = extKey.PrivateKey;
            ExtKey newExtKey = new ExtKey(key2, chainCode); // so combining a ChainCode with a Key gets you the ExtKey (but how do you get the chaincode in absence of the ExtKey to pair?)

            ExtPubKey masterPubKey = masterKey.Neuter(); // generates a master pub key--a pub key from the master private key that can derive child pub keys that match child private keys derived from the master at a given index
            for (int i = 0; i < 5; i++)
            {
                ExtPubKey pubKey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + ": " + pubKey.ToString(Network.Main));
            }

            ExtKey parent = new ExtKey(); // Derive key Child(1, 1) from parent in two different ways (see example on page 57)
            ExtKey child1 = parent.Derive(1).Derive(1);
            // or
            ExtKey sameChild1 = parent.Derive(new KeyPath("1/1"));

            ExtPubKey pubkey1 = masterPubKey.Derive((uint)1); // generates pub key for given child index number
            ExtKey key1 = masterKey.Derive((uint)1); // generates a new extended key as given child index number
            Console.WriteLine("Generated address: " + pubkey1.PubKey.GetAddress(Network.Main)); // 13r5oaSo33vRvSuqRNwSy8wRV42BrwraQq
            Console.WriteLine("Expected address: " + key1.PrivateKey.PubKey.GetAddress(Network.Main)); // 13r5oaSo33vRvSuqRNwSy8wRV42BrwraQq

            ExtKey ceoKey = new ExtKey();
            Console.WriteLine("CEO: " + ceoKey.ToString(Network.Main));
            ExtKey accountingKey = ceoKey.Derive(0, hardened: false);

            ExtPubKey ceoPubKey = ceoKey.Neuter();

            ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubKey); // non-hardened keys can determine parent private keys !!
            Console.WriteLine("CEO key recovered: " + ceoKeyRecovered.ToString(Network.Main)); // same as ceoKey
            // non-hardened keys should only be used for accounts belonging to a single point of control
            ExtKey accountingKeyHardened = ceoKey.Derive(0, true);

            // ExtKey ceoKeyRecovered2 = accountingKeyHardened.GetParentExtKey(ceoPubKey); // fails: "Exception: the private key is hardened so you can't get its parent"

            // can also create hardened keys via ExtKey.Derivate(KeyPath) by using an apostrophe after a child's index:
            var nonHardened = new KeyPath("1/2/3");
            var hardened = new KeyPath("1/2/3'");
            // so say the Accounting department generates one parent key for each customer, and a child key for each of the customer's payments
            // as CEO, you want to spend the moeny on one of these addresses. here's how you'd proceed:
            string accounting = "1'"; // hardened; can't determine CEO's master private key
            int customerId = 5;
            int paymentId = 50;
            KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId); // Path: "1'/5/50"
            ExtKey paymentKey = ceoKey.Derive(path);
        }
    }
}
