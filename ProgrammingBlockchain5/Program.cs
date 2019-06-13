using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using NBitcoin.Protocol;
using System.Threading;

namespace ProgrammingBlockchain5
{
    class Program
    {
        static void Main(string[] args)
        {
            //DemoKeys();
            ColoredCoins();
            Console.ReadLine();
        }

        // COLORED COINS
        public static void DemoKeys()
        {
            Key key = new Key();
            PubKey pubKey = key.PubKey;
            var secret = key.GetBitcoinSecret(Network.Main);
            Console.WriteLine(secret); // L2He2yhKupreS8RoLkW6mXS5uZvSBsr3F3NBYAhbZ8jsvrWLiDbL
            Console.WriteLine(pubKey.GetAddress(Network.Main)); // 1D8P2cBtv74vXdTJLE6hbba88q4QdQVyox
            Console.WriteLine(pubKey.Hash.ScriptPubKey); // OP_DUP OP_HASH160 8506cf167fe11cc4fc9165590263a4ac830ac3ea OP_EQUALVERIFY OP_CHECKSIG
        }

        public static void ColoredCoins()
        {
            // ISSUING AN ASSET
            // In Open Asset, the Asset ID is derived from the issuers ScriptPubKey. To issue a Colored Coin, you must prove ownership of the ScriptPubKey by spending from it.
            // The coin you spend for issuing colored coins is called the "Issuance Coin" in NBitcoin. Create an issuance coin:
            // coin being used for issuing the asset:
            //{
            //    "transactionId": "eb49a599c749c82d824caf9dd69c4e359261d49bbb0b9d6dc18c59bc9214e43b",
            //    "index": 0,
            //    "value": 2000000,
            //    "scriptPubKey": "76a914c81e8e7b7ffca043b088a992795b15887c96159288ac",
            //    "redeemScript": null
            //}
            var coin = new Coin(
                fromTxHash: new uint256("eb49a599c749c82d824caf9dd69c4e359261d49bbb0b9d6dc18c59bc9214e43b"),
                fromOutputIndex: 0,
                amount: Money.Satoshis(2000000),
                scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914c81e8e7b7ffca043b088a992795b15887c96159288ac")));

            var issuance = new IssuanceCoin(coin);
            // build and sign transaction using TransactionBuilder
            var nico = BitcoinAddress.Create("15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe");
            var bookKey = new BitcoinSecret("???"); // program errors bc we don't have nico's private key/secret
            TransactionBuilder builder = new TransactionBuilder();

            var tx = builder
                .AddCoins(coin)
                .AddKeys(bookKey)
                .IssueAsset(nico, new NBitcoin.OpenAsset.AssetMoney(issuance.AssetId, quantity: 10)) // this is new part relevant to issuing colored coins
                .SendFees(Money.Coins(0.0001m))
                .SetChange(bookKey.GetAddress())
                .BuildTransaction(sign: true);

            Console.WriteLine(tx);
            // after tx verification, it's ready to be sent to the network
            Console.WriteLine(builder.Verify(tx));
            // review: broadcasting with QBitNinja:
            var client = new QBitNinjaClient(Network.Main);
            BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;

            if (!broadcastResponse.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success!");
            }
            // or broadcast with your own Bitcoin Core node:
            using (var node = Node.ConnectToLocal(Network.Main))
            {
                node.VersionHandshake();
                node.SendMessage(new InvPayload(InventoryType.MSG_TX, tx.GetHash())); // first send just the hash of your transaction
                node.SendMessage(new TxPayload(tx)); // then send signed transaction
                Thread.Sleep(500);
            }

            // Colored Coins have their own address format that only colored coin wallets understand, to prevent sending colored coins to wallets that don't support it
            nico = BitcoinAddress.Create("15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe");
            Console.WriteLine(nico.ToColoredAddress());

            // colored coin Asset ID is derived from issuer's ScriptPubKey. here is how to get the ID:
            var book = BitcoinAddress.Create("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var assetId = new AssetId(book).GetWif(Network.Main);
            Console.WriteLine(assetId);

            // TRANSFER AN ASSET
            // to send received colored coins, you need to build a ColredCoin
            // received transaction to spent:
            //{
            //    "transactionId": "fa6db7a2e478f3a8a0d1a77456ca5c9fa593e49fd0cf65c7e349e5a4cbe58842",
            //    "index": 0,
            //    "value": 600,
            //    "scriptPubKey": "76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac",
            //    "redeemScript": null,
            //    "assetId": "AVAVfLSb1KZf9tJzrUVpktjxKUXGxUTD4e",
            //    "quantity": 10
            //}
            //var coin2 = new Coin(
            //    fromTxHash: new uint256("fa6db7a2e478f3a8a0d1a77456ca5c9fa593e49fd0cf65c7e349e5a4cbe58842"),
            //    fromOutputIndex: 0,
            //    amount: Money.Satoshis(600),
            //    scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac")));
            //BitcoinAssetId assetId2 = new BitcoinAssetId("AVAVfLSb1KZf9tJzrUVpktjxKUXGxUTD4e");
            //ColoredCoin colored = coin2.ToColoredCoin(assetId2, 10);

            //// also, in this exampled, we needed another coin forFees to pay the tx fee
            //var forFees = new Coin(
            //    fromTxHash: new uint256("7f296e96ec3525511b836ace0377a9fbb723a47bdfb07c6bc3a6f2a0c23eba26"),
            //    fromOutputIndex: 0,
            //    amount: Money.Satoshis(4425000),
            //    scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac")));

            //TransactionBuilder builder2 = new TransactionBuilder();
            //var tx2 = builder2
            //    .AddCoins(colored, forFees) // note: added newly created colored coin, and coin to pay fees in this example
            //    .AddKeys(bookKey)
            //    .SendAsset(book, new AssetMoney(assetId2, 10))
            //    .SetChange(nico)
            //    .SendFees(Money.Coins(0.0001m))
            //    .BuildTransaction(true);

            //Console.WriteLine(tx2); // again, errors b/c we don't have Nico's secret and I don't feel like creating another real tx and dealing with privacy when I push this to github

            // UNIT TESTS
            // create 2 issuers silver and gold, and fake tx to give bitcoin to silver, gold, satoshi
            var gold = new Key();
            var silver = new Key();
            var goldId = gold.PubKey.ScriptPubKey.Hash.ToAssetId();
            var silverId = silver.PubKey.ScriptPubKey.Hash.ToAssetId();

            var alice = new Key();
            var bob = new Key();
            var satoshi = new Key();

            var init = new Transaction()
            {
                Outputs =
                {
                    new TxOut("1.0", gold),
                    new TxOut("1.0", silver),
                    new TxOut("1.0", satoshi)
                }
            };
            // in NBitcoin the sammary of color coin issuance and transfer is described by class ColoredTransaction
            // the trick to writing unit tests is to use an in memory IColoredTransactionRepository
            var repo = new NoSqlColoredTransactionRepository();
            // now we can put the init tx inside
            repo.Transactions.Put(init);
            // now we can get the color
            ColoredTransaction color = ColoredTransaction.FetchColors(init, repo);
            Console.WriteLine(color);
            // now we will use the coins sent to silver and gold as Issuance coins
            var issuanceCoins = init
                .Outputs
                .AsCoins()
                .Take(2)
                .Select((c, i) => new IssuanceCoin(c))
                .OfType<ICoin>()
                .ToArray();
            var sendGoldToSatoshi = new Transaction(); // use TransactionBuilder to send Gold to Satoshi, then put resulting tx in repo and print result

            var goldCoin = ColoredCoin.Find(sendGoldToSatoshi, color).FirstOrDefault();
            builder = new TransactionBuilder();
            var sendToBobAndAlice = builder
                .AddKeys(satoshi)
                .AddCoins(goldCoin)
                .SendAsset(alice, new AssetMoney(goldId, 4))
                .SetChange(satoshi)
                .BuildTransaction(true);
            // not enough funds; goldCoin input only has 600sats and need 1200 for output to transfer assets to Alice and change to satoshi; add coin
            var satoshiBtc = init.Outputs.AsCoins().Last();
            builder = new TransactionBuilder();
            var sendToAlice = builder
                .AddKeys(satoshi)
                .AddCoins(goldCoin, satoshiBtc)
                .SendAsset(alice, new AssetMoney(goldId, 4))
                .SetChange(satoshi)
                .BuildTransaction(true);
            repo.Transactions.Put(sendToAlice);
            color = ColoredTransaction.FetchColors(sendToAlice, repo);
            // see transaction and its color
            Console.WriteLine(sendToAlice);
            Console.WriteLine(color);
            // have made a unit test that emits and transfers some assets w/o any external dependencies
        }
    }
}
