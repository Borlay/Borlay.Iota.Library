using Borlay.Iota.Library.Models;
using Borlay.Iota.Library.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Tests
{
    [TestClass]
    public class IotaTests
    {
        public const string TestSeed = "VP9MTUWYVYWJRGYYEQXPLRCYKM9KJYZPFEGYNDJTLGDEHRJDOLIFCBMSTFRBTZVTIWJVVFQFI9YZMY99B";
        public const string TestSeedForConfirm = "VP9MTUWYVYWJRGYYEMXPLRCYKM9KJYZPFEGYNKJTLGDEHRJDOLIFABMSTFRBTZUTIWJVVFQFI9YZMY99B";

        public const string TestSeed2 = "VP9MTUWYVYPJRGYYEQNPLRCYKM9KJYZPFEGYNDJTLGDEHRJDOLIFCBMSTFRBTZVTIWJVVFQFI9YZMY99B";


        [TestMethod]
        public async Task GetIotaAddressTest()
        {
            var api = CreateIotaClient();

            var address = await api.GetAddress(TestSeed, 1);
            Assert.IsNotNull(address);
            Assert.AreEqual(1, address.Index);
            Assert.AreEqual(0, address.Balance);
            Assert.IsTrue(address.TransactionCount == 0);
        }

        [TestMethod]
        public async Task GetManyIotaAddressesTest()
        {
            var api = CreateIotaClient();

            var address = await api.GetAddresses(TestSeed, 0, 100, CancellationToken.None);
            Assert.IsNotNull(address);
            Assert.AreEqual(100, address.Length);
        }

        [TestMethod]
        public async Task GetIotaAddressesIndexNotZeroTest()
        {
            var api = CreateIotaClient();

            var address = await api.GetAddresses(TestSeed, 2, 5, CancellationToken.None);
            Assert.IsNotNull(address);
            Assert.AreEqual(5, address.Length);
            Assert.AreEqual(2, address.Min(a => a.Index));
            Assert.AreEqual(6, address.Max(a => a.Index));
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task GetIotaAddressesCancelTest()
        {
            var api = CreateIotaClient();

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var address = await api.GetAddresses(TestSeed, 0, 1000, cts.Token);
        }

        [TestMethod]
        public async Task TrytesAttachToTangleTest()
        {
            string intx = "MESSAGETEST9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999UDQKJREDBZVHWYU9LIENNUWNKXWDIDXEIJMXKZJAUPVKIEAFMYTDRQWZWXMMYIPBLZRGJSXRWFVECUCEG999999999999999999999999999TAGTEST99999999999999999999KMOHSJIHE999999999999999999SZ9HCGDIBH9OGGCOXLGBUAUNCBUYEIR9CNJMDMHGYRQPXFUSDAN9MTHVCQMLRDAHSPWKEXMABTTDVKNMX999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999";
            string outtx = "MESSAGETEST9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999UDQKJREDBZVHWYU9LIENNUWNKXWDIDXEIJMXKZJAUPVKIEAFMYTDRQWZWXMMYIPBLZRGJSXRWFVECUCEG999999999999999999999999999TAGTEST99999999999999999999KMOHSJIHE999999999999999999SZ9HCGDIBH9OGGCOXLGBUAUNCBUYEIR9CNJMDMHGYRQPXFUSDAN9MTHVCQMLRDAHSPWKEXMABTTDVKNMXIQOWKUOXEOUTTWDHUII9VZDWCEQZOWDXXXSEXKQXGCKAMUVZ9YKDAA9MGZFCR9GBTKVHPTXSIZRO99999M9AAEGOSOFMJWCSPEDGQQEZMRBLRAGKKLLBBYWWDZXSYUEHIBAOZJFIHPEVFNEWLYSUCADQBTZPI99999AOGPYULRSZNBHLDQTQTZORY9MRPMZXCWYJKFNUBILFMERGCOXOQKGRJQMP9VGXQICSBAQZUKZBNOTHHKR";
            string trunk = "IQOWKUOXEOUTTWDHUII9VZDWCEQZOWDXXXSEXKQXGCKAMUVZ9YKDAA9MGZFCR9GBTKVHPTXSIZRO99999";
            string branch = "M9AAEGOSOFMJWCSPEDGQQEZMRBLRAGKKLLBBYWWDZXSYUEHIBAOZJFIHPEVFNEWLYSUCADQBTZPI99999";

            var api = CreateIotaClient();
            var trytes = new string[] { outtx };
            var trytesResult = await api.IriApi.AttachToTangle(trytes, trunk, branch, CancellationToken.None);

            Assert.AreEqual(outtx, trytesResult.First());
        }

        [TestMethod]
        public async Task TrytesAttachToTangle2Test()
        {
            string intx = "COGEFAUCMKPCCPXKSWDIUXRDMXYMLUQIOZGTTLLZXRXBCPETQULHNCFDKPJPKTUHRGZFWAWUBMXRZSDTWGIHDLJYRVN9TLJWWDRWALERYLBSYMGYHSJPO9ZFBFMUSAUSCRUBNKTLNRDMYUZKLQPQDCBNTSZSGYLCEQTVRHNALINLYGGYRUFPZPJPAQBORRNG9VXMUNN9PLEASIYTIMMHQHYBCRGDSGGPBRSDRUJYXQYBDJPXXUEBU9QPLMTAQTTZOLOAQI9HYOHO9XXIWMKBHRCVPLCKUTKGOMMJDIRKBLVOAYWQGFHZRBU9ANNZRPGWFMMJSATKWHXCOLFRPXICO9NNMORXJNJEWLOQBATKJPHEMUOYAWGUZH9VKYBC9VCBNTGGHRSORCEYSLORCYVAE9DBIHZVWMJHUFHPOCNSYLVSWTOUXIHRW9QVQFKOVL9COHEQQNWGRWXXIKSOSNWLDSRWQDFCSHVTPTDLDXADDOEDUANAKBHVVCHMREOMRBBNUIWBKWEBKDUCGGBJICCH9DIMNJHFYFMAELFDTMRGQYBAZHOIERHPOOGMSMHZOWUZLKCETKKGDBWEYXQUSFEAFYJJKKHBNWLVKJFVT9WVPYVEJUZELSCDUWUQYDZDCNDVRMEVQIYKLSVQHQMTR9MU9ABMCBQEUBCPMVEDJMKOEHMYNXVYXNOAITMYGNHORTRFYNJEXDWTMLGPIXQVRCFHCOQXDBXVEGMMOIMCEOKIPLENWWHTIERSTNTHODQDDXUUZCPSUEDBVWYUTZTVGIAYKFZLOXSOVCLHTVESARNLBB9FTKEXSYBNESCEZNBGBILAOLG9CZVCNSEERFKWXWDUSQYRPDSJGCB9VIFEZCZTUYMRWUSKFCZOP9SEMJYJNWLOVLCQGMEMQNIO9LNEEYHJBKXMR9HXSHEHGOVKMFHEGNASYVXITOUCKLWP9CEKSPYIFVRDDM9DONOCSUIOKC9DMGSQHBHYCOSNJSGIYNXPNCFFFSZKDRJOOCL9DSDBXREVNHBIE9WUKCXHADEPPSEKKKBEFJ9AZALBBYDFAXLVIUYIBZOXJCUSIGTUPUFACUDIGOWTVEGWEZYWUOZOXYYAMCCQLHGMPQNLRNODKODTXCIGCDQVWDFVWAAI9ZJJCB9HSNBHOXESFTWPLMZHQRIHCDIFECNVQDRWWIPEIZNURGSYALJIRKMYZGMNEZYBSRWSWNKYWIKMJMQXUZZPJQBOLHAYFGFNJEK9ZAURLVXQUDNSSXFMFFXLLAZJQJMXTRJXLLFHYRVWARVPKGGTUQCCEETQY9HYCKZWWVSQHUKTGQMWXAXJNFJJ9XBJV9JCLGDHZWFTQTEINGGKI99IWMSYCOKTXVONQYXDZKBVWXUI9IWVJKSQLDUPZTRXVLECJEEBW9VDWZBQFOHZSOQULKKQVPRHEYOPRODJAQRB9ZYNMKIHTHQXUDALEYZ9WBMDKYBXTTGEKHXOGORPRDLVOIGTYDJLTTKEBCUSCOUUJJSTSZIEXDPKOILBMZVLXUYRBRUEBF9EUYSSYLQCNBW99XTJRIQHEXAPDTSKKTEQOWKFLNNVXSWQK9MGBWYJABTJXSLJPVWCDEEXXGNOWJLSNMILAJKBMNFRTNHSJ9NSWSBESQNSUKI9AZQJBJXUTWMX9DEFRUNYIXAJAOQTXIKPCLJPNMICXQCGWJFAHNIGSDPVUZKTVBIUHQWAHCGQFVTPJLPDYSKPHD9QVSUZFCNFVKUXARYA9ONFWIFAZRPMDFKNPNAHQZLFGXZCTDUGJQ99PADJ9FWVGKBRFYECDETIQHGDTTGXVRVQCNBPJTDGTMUYZJNBAPFFFMNUVWGXQUQGEQKFZIUEAQDMIFXHQERZYMGEKHXSTMOCKSVIQGKYPFCVHOPDQYHGFDHTMKC9XMNKYZBIOQPVU9GRJIZIOVAOZVFOEINTBHGOXAEDWMSBQJWGJWNKLSJJITMUSJFTDRIUUCGJCTJINQRIQLUPDRSU9OBGWOHUSUGMDVZLMUXSPWUYSDNGQQCYITIKWXWMPVF9RHCEJIYZQUYM9PBXFEWODSKOMKXKHYSRHM9PVDHQIZKXHSJGLHXZWGIOAHEGFMJOXZPETBBXFBXRWHNVLMXQ9KEVBSXOVRABWLDFEQNHMXGAGXRAXVJL9QRKRGLBQQIJB9IWMENHAQJGRYQKPWCHICGJZXKE9GSUDXZYUAPLHAKAHYHDXNPHENTERYMMBQOPSQIDENXKLKCEYCPVTZQLEEJVYJZV9BWU999999999999999999999999999XFGE99999999999999999999999CLJMMWD99999999999A99999999VECNDLC9KX9YJCVWEVSVWQ9YPIATRAVGMBHTAGKMLEYIFOVHPHTRBMWCTLEIQPOHKQAJGN9OCVPDKTDPUSCPPYKCIEEHMARSMMXFSTEYAUBHOZKZJBLIUXUWQPBWTDHJHAOEDBZNGBBTVJLRMVWTVHCKFLIVP99999ZKQRBCTRVNWEJZOVQOJFACQYDBLTLYTNL9RHICEZGMLFRRHZEHHPXYNQXPMORPXOVGGMBPTVQPDB99999XVTQHMCABUCBNNQNFZKRHPEBZA9YEUSCLVJXJBW9GCPYESKEW9OSD9XXVKWAERTILMSELVNYDSOHYVUZO";
            string outtx = "COGEFAUCMKPCCPXKSWDIUXRDMXYMLUQIOZGTTLLZXRXBCPETQULHNCFDKPJPKTUHRGZFWAWUBMXRZSDTWGIHDLJYRVN9TLJWWDRWALERYLBSYMGYHSJPO9ZFBFMUSAUSCRUBNKTLNRDMYUZKLQPQDCBNTSZSGYLCEQTVRHNALINLYGGYRUFPZPJPAQBORRNG9VXMUNN9PLEASIYTIMMHQHYBCRGDSGGPBRSDRUJYXQYBDJPXXUEBU9QPLMTAQTTZOLOAQI9HYOHO9XXIWMKBHRCVPLCKUTKGOMMJDIRKBLVOAYWQGFHZRBU9ANNZRPGWFMMJSATKWHXCOLFRPXICO9NNMORXJNJEWLOQBATKJPHEMUOYAWGUZH9VKYBC9VCBNTGGHRSORCEYSLORCYVAE9DBIHZVWMJHUFHPOCNSYLVSWTOUXIHRW9QVQFKOVL9COHEQQNWGRWXXIKSOSNWLDSRWQDFCSHVTPTDLDXADDOEDUANAKBHVVCHMREOMRBBNUIWBKWEBKDUCGGBJICCH9DIMNJHFYFMAELFDTMRGQYBAZHOIERHPOOGMSMHZOWUZLKCETKKGDBWEYXQUSFEAFYJJKKHBNWLVKJFVT9WVPYVEJUZELSCDUWUQYDZDCNDVRMEVQIYKLSVQHQMTR9MU9ABMCBQEUBCPMVEDJMKOEHMYNXVYXNOAITMYGNHORTRFYNJEXDWTMLGPIXQVRCFHCOQXDBXVEGMMOIMCEOKIPLENWWHTIERSTNTHODQDDXUUZCPSUEDBVWYUTZTVGIAYKFZLOXSOVCLHTVESARNLBB9FTKEXSYBNESCEZNBGBILAOLG9CZVCNSEERFKWXWDUSQYRPDSJGCB9VIFEZCZTUYMRWUSKFCZOP9SEMJYJNWLOVLCQGMEMQNIO9LNEEYHJBKXMR9HXSHEHGOVKMFHEGNASYVXITOUCKLWP9CEKSPYIFVRDDM9DONOCSUIOKC9DMGSQHBHYCOSNJSGIYNXPNCFFFSZKDRJOOCL9DSDBXREVNHBIE9WUKCXHADEPPSEKKKBEFJ9AZALBBYDFAXLVIUYIBZOXJCUSIGTUPUFACUDIGOWTVEGWEZYWUOZOXYYAMCCQLHGMPQNLRNODKODTXCIGCDQVWDFVWAAI9ZJJCB9HSNBHOXESFTWPLMZHQRIHCDIFECNVQDRWWIPEIZNURGSYALJIRKMYZGMNEZYBSRWSWNKYWIKMJMQXUZZPJQBOLHAYFGFNJEK9ZAURLVXQUDNSSXFMFFXLLAZJQJMXTRJXLLFHYRVWARVPKGGTUQCCEETQY9HYCKZWWVSQHUKTGQMWXAXJNFJJ9XBJV9JCLGDHZWFTQTEINGGKI99IWMSYCOKTXVONQYXDZKBVWXUI9IWVJKSQLDUPZTRXVLECJEEBW9VDWZBQFOHZSOQULKKQVPRHEYOPRODJAQRB9ZYNMKIHTHQXUDALEYZ9WBMDKYBXTTGEKHXOGORPRDLVOIGTYDJLTTKEBCUSCOUUJJSTSZIEXDPKOILBMZVLXUYRBRUEBF9EUYSSYLQCNBW99XTJRIQHEXAPDTSKKTEQOWKFLNNVXSWQK9MGBWYJABTJXSLJPVWCDEEXXGNOWJLSNMILAJKBMNFRTNHSJ9NSWSBESQNSUKI9AZQJBJXUTWMX9DEFRUNYIXAJAOQTXIKPCLJPNMICXQCGWJFAHNIGSDPVUZKTVBIUHQWAHCGQFVTPJLPDYSKPHD9QVSUZFCNFVKUXARYA9ONFWIFAZRPMDFKNPNAHQZLFGXZCTDUGJQ99PADJ9FWVGKBRFYECDETIQHGDTTGXVRVQCNBPJTDGTMUYZJNBAPFFFMNUVWGXQUQGEQKFZIUEAQDMIFXHQERZYMGEKHXSTMOCKSVIQGKYPFCVHOPDQYHGFDHTMKC9XMNKYZBIOQPVU9GRJIZIOVAOZVFOEINTBHGOXAEDWMSBQJWGJWNKLSJJITMUSJFTDRIUUCGJCTJINQRIQLUPDRSU9OBGWOHUSUGMDVZLMUXSPWUYSDNGQQCYITIKWXWMPVF9RHCEJIYZQUYM9PBXFEWODSKOMKXKHYSRHM9PVDHQIZKXHSJGLHXZWGIOAHEGFMJOXZPETBBXFBXRWHNVLMXQ9KEVBSXOVRABWLDFEQNHMXGAGXRAXVJL9QRKRGLBQQIJB9IWMENHAQJGRYQKPWCHICGJZXKE9GSUDXZYUAPLHAKAHYHDXNPHENTERYMMBQOPSQIDENXKLKCEYCPVTZQLEEJVYJZV9BWU999999999999999999999999999XFGE99999999999999999999999CLJMMWD99999999999A99999999VECNDLC9KX9YJCVWEVSVWQ9YPIATRAVGMBHTAGKMLEYIFOVHPHTRBMWCTLEIQPOHKQAJGN9OCVPDKTDPUSCPPYKCIEEHMARSMMXFSTEYAUBHOZKZJBLIUXUWQPBWTDHJHAOEDBZNGBBTVJLRMVWTVHCKFLIVP99999ZKQRBCTRVNWEJZOVQOJFACQYDBLTLYTNL9RHICEZGMLFRRHZEHHPXYNQXPMORPXOVGGMBPTVQPDB99999XVTQHMCABUCBNNQNFZKRHPEBZA9YEUSCLVJXJBW9GCPYESKEW9OSD9XXVKWAERTILMSELVNYDSOHYVUZO";
            string trunk = "SCPPYKCIEEHMARSMMXFSTEYAUBHOZKZJBLIUXUWQPBWTDHJHAOEDBZNGBBTVJLRMVWTVHCKFLIVP99999";
            string branch = "ZKQRBCTRVNWEJZOVQOJFACQYDBLTLYTNL9RHICEZGMLFRRHZEHHPXYNQXPMORPXOVGGMBPTVQPDB99999";

            var api = CreateIotaClient();
            var trytes = new string[] { outtx };
            var trytesResult = await api.IriApi.AttachToTangle(trytes, trunk, branch, CancellationToken.None);

            Assert.AreEqual(outtx, trytesResult.First());
        }


        [TestMethod]
        public async Task SendEmptyTransferTest()
        {
            var api = CreateIotaClient();

            //10000000
            var address = await api.GetAddress(TestSeed2, 1);

            var transfer = new TransferItem()
            {
                Address = address.Address,
                Value = 0,
                Message = "MESSAGETEST",
                Tag = "TAGTEST"
            };

            var transactionItem = await api.SendTransfer(transfer, CancellationToken.None);

            //var transactions = transfer.CreateTransactions();
            //var trytes = transactions.GetTrytes();

            //var trytesResult = await api.SendTrytes(trytes, CancellationToken.None);
            ////var trytesResult2 = await api.SendTrytes(trytesResult, CancellationToken.None);

            //var broadcastUrls = BroadcastUrls();
            //await broadcastUrls.ParallelAsync(async u =>
            //{
            //    await Task.Yield();
            //    await api.BroadcastAndStore(trytesResult);
            //});

            //var transactionItem = new TransactionItem(trytesResult[0]);
            //var transactionHash = transactionItem.Hash;

            ////foreach (var i = 100; int < >)
            //var addresses = await api.GetAddresses(TestSeedForConfirm, 100, 100, CancellationToken.None);

            //List<Task> tasks = new List<Task>();

            //var count = 0;

            //foreach(var adr in addresses)
            //{
            //    var task = AproveTransaction(adr.Address, transactionHash);
            //    //task.ContinueWith(t => count++);
            //    tasks.Add(task);
            //}
            //await Task.WhenAll(tasks);
        }


        [TestMethod]
        public async Task SendEmptyTransferWithPowTest()
        {
            var api = CreateIotaClient();
            var address = await api.GetAddress(TestSeed2, 20);
            var transfer = new TransferItem()
            {
                Address = address.Address,
                Value = 0,
                Message = "THEMESSAGETEST",
                Tag = "THETAGTEST"
            };

            while (true)
            {
                try
                {
                    Console.WriteLine("Do pow and send");
                    CancellationTokenSource cts = new CancellationTokenSource();

                    var transactions = transfer.CreateTransactions();
                    var trytesToPow = transactions.GetTrytes().Single();
                    var toApprove = await api.IriApi.GetTransactionsToApprove(9);
                    var diver = new PowDiver();
                    cts.CancelAfter(15000); //
                    var trytesToSend = await diver.DoPow(trytesToPow.SetApproveTransactions(toApprove.TrunkTransaction, toApprove.BranchTransaction), 15, cts.Token);
                    //Thread.Sleep(200000);
                    await api.IriApi.BroadcastTransactions(trytesToSend);
                    await api.IriApi.StoreTransactions(trytesToSend);

                    var transaction = new TransactionItem(trytesToSend);

                    break;
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
            }
        }

        [TestMethod]
        public async Task SendVeryEmptyTransactionTest()
        {
            var api = CreateIotaClient();

            var emptyAddress = IotaUtils.GenerateRandomTrytes(81); // "".Pad(81);

            var transfer = new TransferItem()
            {
                Address = emptyAddress,
                Value = 0,
                Message = null,
                Tag = null
            };

            while (true)
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();

                    var transactions = transfer.CreateTransactions();
                    var trytesToPow = transactions.GetTrytes().Single();
                    var toApprove = await api.IriApi.GetTransactionsToApprove(9);
                    var diver = new PowDiver();
                    //cts.CancelAfter(15000); 
                    var trunk = toApprove.TrunkTransaction;
                    var branch = toApprove.BranchTransaction;


                    var trytesToSend = await diver.DoPow(trytesToPow.SetApproveTransactions(trunk, branch), 15, cts.Token);

                    await api.IriApi.BroadcastTransactions(trytesToSend);
                    await api.IriApi.StoreTransactions(trytesToSend);

                    var transaction = new TransactionItem(trytesToSend);
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
            }
        }

        [TestMethod]
        public async Task ApproveAnyTransactionFast()
        {
            // "EWIMRMDVLWJOS9BCMZCOLHJJIJUMCHVSEQSNPHHMMY9OCZFSJ9L9DBCLVZXCPYUUKYQWOEPYVLWG99999"

            // "LIIAMJACMXDCWTFRRZMRBZHBGDRACVIJGEFZSPCUISBDEFLZSWB9QBXACPCC9VIUAWSJRUOFLPBC99999";

            string tranHash = "JWVFWNKBOIH9SVOZENCADOUJSQWOLVTZWTPBFJIBSJAUEMHYYJKDRYHCK99HOKHDXBAHHGTOA9PW99999";

            Stopwatch watch = Stopwatch.StartNew();

            var task1 = ApproveTransaction(tranHash, ApproveTransactionType.Trunk);
            var task2 = ApproveTransaction(tranHash, ApproveTransactionType.Branch);

            var tran1 = await task1;
            var tran2 = await task2;

            //var task1 = await Enumerable.Range(0, 8).ParallelAnyAsync(async
            //    (i) => await ApproveTransaction(tranHash, ApproveTransactionType.Trunk));

            //var task2 = await Enumerable.Range(0, 8).ParallelAnyAsync(async
            //    (i) => await ApproveTransaction(tranHash, ApproveTransactionType.Branch));

            //var tran1 = await task1;
            //var tran2 = await task2;

            watch.Stop();

        }

        public enum ApproveTransactionType
        {
            Trunk,
            Branch
        }



        public async Task<string> ApproveTransaction(string transactionHash, ApproveTransactionType approveType)
        {
            var api = CreateIotaClient();
            //var address = await api.GetAddress(TestSeed2, 8);

            var emptyAddress = IotaUtils.GenerateRandomTrytes(81); // "".Pad(81);

            var transfer = new TransferItem()
            {
                Address = emptyAddress,
                Value = 0,
                Message = "",
                Tag = ""
            };

            while (true)
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();

                    var transactions = transfer.CreateTransactions();
                    var trytesToPow = transactions.GetTrytes().Single();
                    var toApprove = await api.IriApi.GetTransactionsToApprove(9);
                    var diver = new PowDiver();
                    cts.CancelAfter(20000);
                    var trunk = toApprove.TrunkTransaction;
                    var branch = toApprove.BranchTransaction;

                    if (approveType == ApproveTransactionType.Trunk)
                        trunk = transactionHash;
                    else
                        branch = transactionHash;

                    var trytesToSend = await diver.DoPow(trytesToPow.SetApproveTransactions(trunk, branch), 15, cts.Token);

                    await api.IriApi.BroadcastTransactions(trytesToSend);
                    await api.IriApi.StoreTransactions(trytesToSend);

                    var transaction = new TransactionItem(trytesToSend);

                    return transaction.Hash;
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
            }
        }

        public Task<string> Rebroadcast(string trytes)
        {
            return Rebroadcast(trytes, CancellationToken.None);
        }

        public async Task<string> Rebroadcast(string trytes, CancellationToken cancellationToken)
        {
            var api = CreateIotaClient();

            while (true)
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    cancellationToken.Register(() => cts.Cancel());

                    var toApprove = await api.IriApi.GetTransactionsToApprove(9);
                    var diver = new PowDiver();
                    cts.CancelAfter(15000);

                    var trunk = toApprove.TrunkTransaction;
                    var branch = toApprove.BranchTransaction;



                    trytes = trytes.SetApproveBranch(trunk);
                    var trytesToSend = await diver.DoPow(trytes, 15, cts.Token);

                    if (cts.IsCancellationRequested)
                        continue;

                    await api.IriApi.BroadcastTransactions(trytesToSend);
                    await api.IriApi.StoreTransactions(trytesToSend);

                    var transaction = new TransactionItem(trytesToSend);

                    return transaction.Hash;
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw;

                    continue;
                }
            }
        }


        [TestMethod]
        public async Task ParallelAsyncTest()
        {
            var list = new ConcurrentBag<bool>();
            var range = Enumerable.Range(0, 1000);
            await range.ParallelAsync(async u =>
            {
                await TaskIota.Yield().ConfigureAwait(false);
                //await Task.Yield();
                Thread.Sleep(100);
                list.Add(true);
            });

            Assert.AreEqual(range.Count(), list.Count());
        }


        [TestMethod]
        public async Task DoPowAndSendTransactionWithValueTest()
        {
            var api = CreateIotaClient();
            var addresses = await api.GetAddresses(TestSeed, 104, 3, CancellationToken.None);
            var transfer = new TransferItem()
            {
                Address = addresses[1].Address,
                Value = 10000000,
                Message = "MESSAGETESTVALUEPOW",
                Tag = "TAGTESTVALUEPOW"
            };

            while (true)
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();

                    var transactionItems = transfer.CreateTransactions(addresses[2].Address, addresses.First());
                    var transactionTrytes = transactionItems.GetTrytes();
                    var toApprove = await api.IriApi.GetTransactionsToApprove(9);

                    var trunk = toApprove.TrunkTransaction;
                    var branch = toApprove.BranchTransaction;

                    var trytesToSend = await transactionTrytes.DoPow(trunk, branch, 15, cts.Token);

                    await api.IriApi.BroadcastTransactions(trytesToSend);
                    await api.IriApi.StoreTransactions(trytesToSend);

                    var firstTrytes = trytesToSend.Last();

                    var transactionItem = new TransactionItem(firstTrytes);
                    var rebroadcastTransactionHash = await Rebroadcast(firstTrytes);

                    break;
                }
                catch (AggregateException)
                {
                    continue;
                }
            }
        }


        [TestMethod]
        public async Task GetIotaAddressHasTransactionTest()
        {
            var api = CreateIotaClient();

            var address = await api.GetAddress(TestSeed.ToUpper(), 100);
            Assert.IsNotNull(address);
            Assert.AreEqual(100, address.Index);
            Assert.AreEqual(0, address.Balance);
            Assert.IsTrue(address.TransactionCount > 0);
        }

        [TestMethod]
        public async Task SendTransferFromEmptyBalanceWithoudCheckTest()
        {
            var api = CreateIotaClient();

            var address1 = await api.GetAddress(TestSeed, 51);
            var address2 = await api.GetAddress(TestSeed, 52);
            var address3 = await api.GetAddress(TestSeed, 53);

            address2.Balance = 20;

            await api.AttachTransferWithoutRenewBalance(new TransferItem()
            {
                Address = address1.Address,
                Value = 10,
                Message = "MESSAGETEST",
                Tag = "TAGTEST"
            }, new AddressItem[]
            {
                address2
            }, address3.Address, CancellationToken.None);
        }

        [TestMethod]
        public async Task GetIotaAddressHasTransaction2Test()
        {
            var api = CreateIotaClient();

            var address = await api.GetAddress("randomseedopalaskdjf".ToUpper(), 0);
            Assert.IsNotNull(address);
            Assert.AreEqual(0, address.Index);
            Assert.AreEqual(0, address.Balance);
            Assert.IsTrue(address.TransactionCount > 0);
        }

        private IotaApi CreateIotaClient()
        {
            // "http://iota.bitfinex.com:80"
            // "http://node.iotawallet.info:14265"
            // "http://node.deviceproof.org:14265"
            // "http://88.198.230.98:14265"
            // "http://iota.digits.blue:14265"
            return new IotaApi("http://eugeneoldisoft.iotasupport.com:14265");
        }

        private IEnumerable<string> BroadcastUrls()
        {
            yield return "http://iota.bitfinex.com:80";
            yield return "http://node.iotawallet.info:14265";
            yield return "http://node.deviceproof.org:14265";
            yield return "http://88.198.230.98:14265";
            yield return "http://eugeneoldisoft.iotasupport.com:14265";
            yield return "http://eugene.iota.community:14265";
            yield return "http://mainnet.necropaz.com:14500";
            yield return "http://iotatoken.nl:14265";
            yield return "http://wallets.iotamexico.com:80";
        }
    }

    public static class ParallelExtensions
    {
        public static async Task ParallelAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            var tasks = new List<Task>();
            foreach (var e in enumerable)
            {
                var task = action(e);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        public static async Task<TResult> ParallelAnyAsync<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> action)
        {
            var tasks = new List<Task<TResult>>();
            foreach (var e in enumerable)
            {
                var task = action(e);
                tasks.Add(task);
            }
            var t = await Task.WhenAny<TResult>(tasks);
            return await t;
        }
    }
}
