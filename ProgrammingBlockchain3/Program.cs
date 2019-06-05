using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.Protocol;
using NBitcoin.Stealth;

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

            //ExtKey masterKey = new ExtKey(); // generates a master private key
            //Console.WriteLine("Master Key: " + masterKey.ToString(Network.Main));
            //for (int i = 0; i < 5; i++) // which can be used to deterministically derive child keys by index numbers
            //{
            //    ExtKey key = masterKey.Derive((uint)i);
            //    Console.WriteLine("Key " + i + ": " + key.ToString(Network.Main));
            //}

            //ExtKey extKey = new ExtKey(); // an ExtKey contain a Key (as a property of the ExtKey object); "You can go back from a Key to an ExtKey by supplying the Key and the Chaincode to the ExtKey constructor as follows:"
            //byte[] chainCode = extKey.ChainCode; // what is a chaincode?
            //Key key2 = extKey.PrivateKey;
            //ExtKey newExtKey = new ExtKey(key2, chainCode); // so combining a ChainCode with a Key gets you the ExtKey (but how do you get the chaincode in absence of the ExtKey to pair?)

            //ExtPubKey masterPubKey = masterKey.Neuter(); // generates a master pub key--a pub key from the master private key that can derive child pub keys that match child private keys derived from the master at a given index
            //for (int i = 0; i < 5; i++)
            //{
            //    ExtPubKey pubKey = masterPubKey.Derive((uint)i);
            //    Console.WriteLine("PubKey " + i + ": " + pubKey.ToString(Network.Main));
            //}

            //ExtKey parent = new ExtKey(); // Derive key Child(1, 1) from parent in two different ways (see example on page 57)
            //ExtKey child1 = parent.Derive(1).Derive(1);
            //// or
            //ExtKey sameChild1 = parent.Derive(new KeyPath("1/1"));

            //ExtPubKey pubkey1 = masterPubKey.Derive((uint)1); // generates pub key for given child index number
            //ExtKey key1 = masterKey.Derive((uint)1); // generates a new extended key as given child index number
            //Console.WriteLine("Generated address: " + pubkey1.PubKey.GetAddress(Network.Main)); // 13r5oaSo33vRvSuqRNwSy8wRV42BrwraQq
            //Console.WriteLine("Expected address: " + key1.PrivateKey.PubKey.GetAddress(Network.Main)); // 13r5oaSo33vRvSuqRNwSy8wRV42BrwraQq

            //ExtKey ceoKey = new ExtKey();
            //Console.WriteLine("CEO: " + ceoKey.ToString(Network.Main));
            //ExtKey accountingKey = ceoKey.Derive(0, hardened: false);

            //ExtPubKey ceoPubKey = ceoKey.Neuter();

            //ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubKey); // non-hardened keys can determine parent private keys !!
            //Console.WriteLine("CEO key recovered: " + ceoKeyRecovered.ToString(Network.Main)); // same as ceoKey
            //// non-hardened keys should only be used for accounts belonging to a single point of control
            //ExtKey accountingKeyHardened = ceoKey.Derive(0, true);

            //// ExtKey ceoKeyRecovered2 = accountingKeyHardened.GetParentExtKey(ceoPubKey); // fails: "Exception: the private key is hardened so you can't get its parent"

            //// can also create hardened keys via ExtKey.Derivate(KeyPath) by using an apostrophe after a child's index:
            //var nonHardened = new KeyPath("1/2/3");
            //var hardened = new KeyPath("1/2/3'");
            //// so say the Accounting department generates one parent key for each customer, and a child key for each of the customer's payments
            //// as CEO, you want to spend the moeny on one of these addresses. here's how you'd proceed:
            //string accounting = "1'"; // hardened; can't determine CEO's master private key
            //int customerId = 5;
            //int paymentId = 50;
            //KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId); // Path: "1'/5/50"
            //ExtKey paymentKey = ceoKey.Derive(path);

            //// MNEMONIC SEED - HD KEYS (BIP 39)
            //Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve); // generates new seed phrase
            //ExtKey hdRoot = mnemonic.DeriveExtKey("my passowrd"); 
            //Console.WriteLine(mnemonic); // captain powder hollow erode lemon bomb scheme shallow kick business shove trumpet
            //Console.WriteLine(hdRoot.ToString(Network.Main));
            //// if you have mnemonic and password you can derive hdRoot key
            //mnemonic = new Mnemonic("captain powder hollow erode lemon bomb scheme shallow kick business shove trumpet", Wordlist.English);
            //hdRoot = mnemonic.DeriveExtKey("my password");

            //// DARK WALLET
            //var scanKey = new Key();
            //var spendKey = new Key();
            //BitcoinStealthAddress stealthAddress = new BitcoinStealthAddress
            //    (scanKey: scanKey.PubKey,
            //    pubKeys: new[] { spendKey.PubKey },
            //    signatureCount: 1,
            //    bitfield: null,
            //    network: Network.Main);

            //// var ephemKey = new Key();
            //Transaction transaction = new Transaction();
            //// stealthAddress.SendTo(transaction, Money.Coins(1.0m), ephemKey);
            //// Console.WriteLine(transaction);

            //// the EphemKey is an implementation detail; you can omit it and NBitcoin will generate one automatically:
            //stealthAddress.SendTo(transaction, Money.Coins(1.0m));
            //Console.WriteLine(transaction);

            //// P2WPKH Pay to Witness Public Key Hash
            //var key = new Key();
            //Console.WriteLine(key.PubKey.WitHash.ScriptPubKey); // simply use WitHash instead of Hash to get ScriptPubKey

            // MULTI SIG
            Key alice = new Key();
            Key bob = new Key();
            Key satoshi = new Key();

            //var scriptPubKey = PayToMultiSigTemplate // creating a 2 of 3 multisig
            //    .Instance
            //    .GenerateScriptPubKey(2, new[] { alice.PubKey, bob.PubKey, satoshi.PubKey });
            //Console.WriteLine(scriptPubKey);
            //// signing a multisig is more complicated than just calling Transaction.Sign. For now we will use TransactionBuilder for signing
            //var received = new Transaction(); // imagine this bitcoin held (received) in the multisig address
            //received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));
            //Coin coin = received.Outputs.AsCoins().First();
            //// create an unsigned transaction
            //BitcoinAddress matt = new Key().PubKey.GetAddress(Network.Main);
            //TransactionBuilder builder = new TransactionBuilder();
            //Transaction unsigned = builder
            //    .AddCoins(coin)
            //    .Send(matt, Money.Coins(1.0m))
            //    .BuildTransaction(sign: false);
            //// alice signs
            //Transaction aliceSigned = builder
            //    .AddCoins(coin)
            //    .AddKeys(alice)
            //    .SignTransaction(unsigned);
            //// bob signs
            //Transaction bobSigned = builder
            //    .AddCoins(coin)
            //    .AddKeys(bob)
            //    .SignTransaction(aliceSigned); // note we're signing the Tx already signed by alice

            //Transaction fullySigned = builder
            //    .AddCoins(coin)
            //    .CombineSignatures(aliceSigned, bobSigned);

            //Console.WriteLine(bobSigned); // it appears bobSigned is same as fullySigned
            //Console.WriteLine(fullySigned);
            //// let's look at the case that CombineSignatures() is needed
            //TransactionBuilder builderNew = new TransactionBuilder();
            //TransactionBuilder aliceBuilder = new TransactionBuilder();
            //TransactionBuilder bobBuilder = new TransactionBuilder();

            //Transaction unsignedNew = builderNew
            //    .AddCoins(coin)
            //    .Send(matt, Money.Coins(1.0m))
            //    .BuildTransaction(sign: false);

            //Transaction aliceSignedNew = aliceBuilder
            //    .AddCoins(coin)
            //    .AddKeys(alice)
            //    .SignTransaction(unsignedNew);

            //Transaction bobSignedNew = bobBuilder
            //    .AddCoins(coin)
            //    .AddKeys(bob)
            //    .SignTransaction(unsignedNew); // note signing unsigned tx again
            //// in this case CombineSignatures() is needed
            //Transaction fullySignedNew = builderNew.AddCoins(coin).CombineSignatures(aliceSignedNew, bobSignedNew);
            //// nowadays native Pay to MultiSig as demo'd above are never used directly

            // P2SH Pay to Script Hash
            // P2SH is an easy way to represent a scriptPubKey as a simple BitcoinScriptAddress no matter how complicated the terms of it's underlying m of n signature set up
            var paymentScript = PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new[] { alice.PubKey, bob.PubKey, satoshi.PubKey })
                .PaymentScript; // this p2sh scriptPubKey represents the hash of the multi-sig script: redeemScript.Hash.ScriptPubKey
            Console.WriteLine(paymentScript); // OP_HASH160 f6acc9ebae72d94541ef11250c996f63d841e6ed OP_EQUAL

            Script redeemScript = PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new[] { alice.PubKey, bob.PubKey, satoshi.PubKey });
            Console.WriteLine(redeemScript.Hash.GetAddress(Network.Main)); // 3LLq4yWEYCzuf5Q6mAaUcCsjEkTFj2bH8v

            // to sign a tx sent to a p2sh, have to provide the redeem script when building the Coin for the TransactionBuilder
            // imagine the multi-sig p2sh receives a tx called received
            Transaction received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScript.Hash)); // warning: tx sent to redeemScript.Hash, not redeemScript
            // when any 2 of 3 owners want to spend the tx, instead of creating a Coin, they must create a ScriptCoin
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);
            // rest of the tx generation and signing is the same as native multi-sig

        }
    }
}
