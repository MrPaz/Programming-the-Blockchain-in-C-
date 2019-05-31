using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin.Protocol;
using System.Threading;

namespace ProgrammingBlockchain2
{
    class Program
    {
        static void Main(string[] args)
        {
            SpendBitcoin();
            Console.ReadLine();
        }

        public static void SpendBitcoin()
        {
            var bitcoinPrivateKey = new BitcoinSecret("");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();

            Console.WriteLine(bitcoinPrivateKey);
            Console.WriteLine(address);

            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine(transactionResponse.TransactionId);
            Console.WriteLine(transactionResponse.Block.Confirmations);
            // Now we have all the info needed to create transactions. The main questions now are: 'From Where?', 'To Where?', and 'How Much?'
            // From Where?
            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach(var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                    outPointToSpend = coin.Outpoint; // From Here
            }
            if (outPointToSpend == null)
                throw new Exception("TxOut doesn't contain out ScriptPubKey");
            Console.WriteLine("We want to spend {0} outpoint: {1}", outPointToSpend.N + 1, outPointToSpend);

            var transaction = new Transaction(); // Constructing the TxIn and adding it to the transaction is the answer of the 'From Where?' question
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            // To Where? Constructing the TxOut and adding it to the transaction is the answer to the remaining questions
            var hallOfMakersAddress = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            // How Much?
            TxOut hallOfMakersTxOut = new TxOut()
            {
                Value = new Money((decimal)0.001, MoneyUnit.BTC),
                ScriptPubKey = hallOfMakersAddress.ScriptPubKey
            };

            TxOut changeBackTxOut = new TxOut()
            {
                Value = new Money((decimal)0.0008, MoneyUnit.BTC),
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };

            transaction.Outputs.Add(hallOfMakersTxOut);
            transaction.Outputs.Add(changeBackTxOut);

            var hallOfMakersAmount = new Money(.001m, MoneyUnit.BTC);
            var minerFee = new Money(.0002m, MoneyUnit.BTC);
            var txInAmount = receivedCoins[(int)outPointToSpend.N].TxOut.Value;
            Money changeBackAmount = txInAmount - hallOfMakersAmount - minerFee;
            Console.WriteLine(txInAmount); // sanity check
            Console.WriteLine(hallOfMakersAmount);
            Console.WriteLine(minerFee);
            Console.WriteLine(changeBackAmount);

            //TxOut hallOfMakersTxOut = new TxOut() // already did this above
            //{
            //    Value = hallOfMakersAmount,
            //    ScriptPubKey = hallOfMakersAddress.ScriptPubKey
            //};
            //TxOut changeBackTxOut = new TxOut()
            //{
            //    Value = changeBackAmount,
            //    ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            //};

            //transaction.Outputs.Add(hallOfMakersTxOut); // already did this above
            //transaction.Outputs.Add(changeBackTxOut);

            var message = "Thank you Nicolas and Adam!!! -Paz";
            var bytes = Encoding.UTF8.GetBytes(message);
            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });

            // Exercise: Try to figure out what scriptSig will be and how to get it in our code before reading further
            //var scriptSig = transaction.SignInput(bitcoinPrivateKey, receivedCoins.SingleOrDefault(c => c.Outpoint == outPointToSpend), SigHash.All);
            var prevOutAddress = BitcoinAddress.Create("");
            transaction.Inputs[0].ScriptSig = prevOutAddress.ScriptPubKey;
            // alternatively, could use this option to fill scriptSig with the ScriptPubKey of our address:
            //transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            transaction.Sign(bitcoinPrivateKey, false);
            // broadcast transaction using QBitNinja:
            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("ErrorMessage: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaction in a block explorer:");
                Console.WriteLine(transaction.GetHash());
            }
            // broadcast using my own node:
            using (var node = Node.ConnectToLocal(network))
            {
                node.VersionHandshake(); // say hello to node
                // advertise my transaction (send just the hash)
                node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
                // send transaction
                node.SendMessage(new TxPayload(transaction));
                Thread.Sleep(500); // wait a bit
            }
        }
    }
}
