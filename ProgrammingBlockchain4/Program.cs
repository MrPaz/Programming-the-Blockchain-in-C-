using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.Protocol;
using NBitcoin.Stealth;
using NBitcoin.Crypto;

namespace ProgrammingBlockchain4
{
    class Program
    {
        static void Main(string[] args)
        {
            TransactionBuilder();
            Console.ReadLine();
        }

        static void TransactionBuilder()
        {
            // TRANSACTION BUILDER
            // the goal of transaction builder is to take Coins and Keys as input, and return back a signed or partially signed transaction
            // the TransactionBuilder will figure out what Coin to use and what to sign by itself
            // 4 steps: 1) gather the Coins to be spent 2) gather Keys that you own 3) enumerate how much money to send to what scriptPubKey 4) build and sign the transaction 5)(optional) you give the tx to somebody else to sign or continue to build it

            // create fake transaction; has a p2pkh, p2pk, and multi-sig of alice and bob
            var alice = new Key();
            var bob = new Key();

            Script aliceBob = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, alice.PubKey, bob.PubKey);
            var init = new Transaction();
            init.Outputs.Add(new TxOut(Money.Coins(1.0m), alice.PubKey.Hash)); // p2pkh
            init.Outputs.Add(new TxOut(Money.Coins(1.0m), bob.PubKey)); // p2pk
            init.Outputs.Add(new TxOut(Money.Coins(1.0m), aliceBob));

            var satoshi = new Key();

            Coin[] coins = init.Outputs.AsCoins().ToArray();
            Coin aliceCoin = coins[0];
            Coin bobCoin = coins[1];
            Coin aliceBobCoin = coins[2];

            // let's say alice wants to send 0.3btc, bob wants to send 0.2btc, and they agree to use aliceBob to send 0.5btc
            var builder = new TransactionBuilder();
            Transaction tx = builder
                                .AddCoins(aliceCoin)
                                .AddKeys(alice)
                                .Send(satoshi, Money.Coins(0.3m))
                                .SetChange(alice)
                                .Then()
                                .AddCoins(bobCoin)
                                .AddKeys(bob)
                                .Send(satoshi, Money.Coins(0.2m))
                                .SetChange(bob)
                                .Then()
                                .AddCoins(aliceBobCoin)
                                .AddKeys(alice, bob)
                                .Send(satoshi, Money.Coins(0.5m))
                                .SetChange(aliceBob)
                                .SendFees(Money.Coins(0.0001m))
                                .BuildTransaction(sign: true);

            Console.WriteLine(builder.Verify(tx)); // True

            // TransactionBuilder works the same way for p2sh, p2wsh, p2sh(p2wsh), and p2sh(p2pkh) except you need to create ScriptCoin
            init = new Transaction();
            init.Outputs.Add(new TxOut(Money.Coins(1.0m), aliceBob.Hash));
            coins = init.Outputs.AsCoins().ToArray();
            ScriptCoin aliceBobScriptCoin = coins[0].ToScriptCoin(aliceBob);

            builder = new TransactionBuilder();
            tx = builder
                    .AddCoins(aliceBobScriptCoin)
                    .AddKeys(alice, bob)
                    .Send(satoshi, Money.Coins(0.9m))
                    .SetChange(aliceBob.Hash)
                    .SendFees(Money.Coins(0.0001m))
                    .BuildTransaction(true);

            Console.WriteLine(builder.Verify(tx)); // True

            // need a ScanKey to see the StealthCoin
            Key scanKey = new Key();
            BitcoinStealthAddress darkAliceBob =
                new BitcoinStealthAddress
                    (
                        scanKey: scanKey.PubKey,
                        pubKeys: new[] { alice.PubKey, bob.PubKey },
                        signatureCount: 2,
                        bitfield: null,
                        network: Network.Main
                    );
            // someone sent this tx to darkAliceBob
            init = new Transaction();
            darkAliceBob.SendTo(init, Money.Coins(1.0m));
            // the scanner will detect the StealthCoin
            StealthCoin stealthCoin = StealthCoin.Find(init, darkAliceBob, scanKey);
            // and forward it to alice and bob who sign:
            tx = builder
                    .AddCoins(stealthCoin)
                    .AddKeys(alice, bob, scanKey) // note: you need the scanKey for spending a StealthCoin
                    .Send(satoshi, Money.Coins(0.9m))
                    .SetChange(aliceBob.Hash)
                    .SendFees(Money.Coins(0.0001m))
                    .BuildTransaction(true);
            Console.WriteLine(builder.Verify(tx));

        }
    }
}
