using Borlay.Iota.Library.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Tests
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void GeneratedTryteTest()
        {
            var trytes = IotaApiUtils.GenerateRandomTrytes();
            Assert.AreEqual(81, trytes.Length);
            Assert.IsTrue(ContainsChars(trytes, Constants.TryteAlphabet));
            //Assert.IsTrue(ContainsChars(Constants.TryteAlphabet, trytes)); // sometimes can throw 
        }

        private bool ContainsChars(string value, string chars)
        {
            foreach(var v in value)
            {
                if (!chars.Contains(v.ToString()))
                    return false;
            }
            return true;
        }
    }
}
