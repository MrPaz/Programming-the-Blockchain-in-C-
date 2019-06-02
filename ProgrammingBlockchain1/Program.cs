using NBitcoin;
using System;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingBlockchain1
{
    class Program
    {
        static void Main(string[] args)
        {        
            BitcoinPractice();
            Console.ReadLine();
        }

        public static void BitcoinPractice()
        {
            Key privateKey = new Key();

            //PubKey publicKey = privateKey.PubKey;
            //Console.WriteLine(publicKey);

            //Console.WriteLine(publicKey.GetAddress(Network.Main));
            //Console.WriteLine(publicKey.GetAddress(Network.TestNet));

            //var publicKeyHash = publicKey.Hash;
            //Console.WriteLine(publicKeyHash);

            //var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
            //var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
            //Console.WriteLine(mainNetAddress);
            //Console.WriteLine(testNetAddress);

            //var publicKeyHash2 = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");  // I don't get what's going on here; where does this string come from, what does KeyId do??

            //var mainNetAddress2 = publicKeyHash2.GetAddress(Network.Main);
            //var testNetAddress2 = publicKeyHash2.GetAddress(Network.TestNet);
            //Console.WriteLine(mainNetAddress2.ScriptPubKey);
            //Console.WriteLine(testNetAddress2.ScriptPubKey);

            //var paymentScript = publicKeyHash2.ScriptPubKey;
            //var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main); // going backwards from script and network to get address
            //Console.WriteLine(sameMainNetAddress);
            //Console.WriteLine(mainNetAddress2 == sameMainNetAddress);

            //var samePublicKeyHash = (KeyId)paymentScript.GetDestination(); // going backwards to get same result as line 32 from the script
            //Console.WriteLine(publicKeyHash2 == samePublicKeyHash);
            //var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main); // 
            //Console.WriteLine(mainNetAddress2 == sameMainNetAddress2);

            //BitcoinSecret mainNetPrivateKey = privateKey.GetBitcoinSecret(Network.Main); // generate WIF Wallet Import Format of privKey
            //Console.WriteLine(mainNetPrivateKey);
            //bool wifIsBitcoinSecret = mainNetPrivateKey == privateKey.GetWif(Network.Main);
            //Console.WriteLine(wifIsBitcoinSecret);


            //// Exercise:
            //// 1. Generate a private key on mainnet and note it
            //Key myKey = new Key();
            //BitcoinSecret myKeyWIF = myKey.GetBitcoinSecret(Network.Main);
            //Console.WriteLine(myKeyWIF); // saved offline so it isn't pushed to github

            //// 2. Get the corresponding address
            //PubKey myPubKey = myKey.PubKey;
            //var myBitcoinAddress = myPubKey.GetAddress(Network.Main);
            //Console.WriteLine(myBitcoinAddress);
      
            //// or...
            //var myPubKeyHash = myPubKey.Hash;
            //var myBitcoinAddress2 = myPubKeyHash.GetAddress(Network.Main);
            //Console.WriteLine(myBitcoinAddress2); // saved offline for privacy

            // 3. Send bitcoins to it. As much as you can afford to lose, so it will keep you focused and motivated to get them back 
            // during the following lessons.
            // DONE!!!

            // Transaction tx = new Transaction("");
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);
            var transactionId = NBitcoin.uint256.Parse(""); // remove tx id and hex before push
            GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            NBitcoin.Transaction transaction = transactionResponse.Transaction;

            Console.WriteLine(transactionResponse.TransactionId);
            Console.WriteLine(transaction.GetHash());

            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            foreach(var coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount; // amount of each output in the tx to the address we created above (RECEIVED COINS)

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.TxOut.ScriptPubKey;
                Console.WriteLine(paymentScript); // ScriptPubKey of each output in the tx
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address); // address of each output in the tx    
            }

            // Exercise: Write out the same info about the SPENT COINS using QBitNinja's GetTransactionResponse class
            List<ICoin> spentCoins = transactionResponse.SpentCoins;
            foreach (var spentCoin in spentCoins)
            {
                Money amountSpent = (Money)spentCoin.Amount;

                Console.WriteLine(amountSpent.ToDecimal(MoneyUnit.BTC));
                var paymentScript = spentCoin.TxOut.ScriptPubKey; // not sure if this is working... doesn't look like a script, see below
                Console.WriteLine(paymentScript); // 0 367003b...  // ??? '[index of output from prev tx spent in current tx] [hash of prev tx]' == 'Outpoint' ???
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address); // 
            }

            var outputs = transaction.Outputs;  // this does same thing as ReceivedCoins foreach loop above
            foreach(TxOut output in outputs)
            {
                Money amount = output.Value;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = output.ScriptPubKey;
                Console.WriteLine(paymentScript);
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
            }

            var inputs = transaction.Inputs;
            foreach(TxIn input in inputs)
            {
                OutPoint previousOutpoint = input.PrevOut;
                Console.WriteLine(previousOutpoint.Hash);
                Console.WriteLine(previousOutpoint.N);
            }

            Money twentyOneBitcoin = new Money(21, MoneyUnit.BTC);
            var scriptPubKey = transaction.Outputs.First().ScriptPubKey;
            TxOut txOut = new TxOut(twentyOneBitcoin, scriptPubKey);

            OutPoint firstOutPoint = receivedCoins.First().Outpoint;
            Console.WriteLine(firstOutPoint.Hash);
            Console.WriteLine(firstOutPoint.N);

            Console.WriteLine(transaction.Inputs.Count);

            OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;
            var firstPreviousTransaction = client.GetTransaction(firstPreviousOutPoint.Hash).Result.Transaction;
            Console.WriteLine(firstPreviousTransaction.IsCoinBase);

            Money spentAmount = Money.Zero;
            foreach(var spentCoin in spentCoins)
            {
                spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
            }
            Console.WriteLine(spentAmount.ToDecimal(MoneyUnit.BTC));

            // Exercise: Get the total received amount
            Money receivedAmount = Money.Zero;
            foreach(var receivedCoin in receivedCoins)
            {
                receivedAmount = (Money)receivedCoin.Amount.Add(receivedAmount);
            }
            Console.WriteLine(receivedAmount.ToDecimal(MoneyUnit.BTC));

            var fee = transaction.GetFee(spentCoins.ToArray());
            Console.WriteLine(fee);
            Console.WriteLine(fee == (spentAmount - receivedAmount)); // True


        }
    }
}
