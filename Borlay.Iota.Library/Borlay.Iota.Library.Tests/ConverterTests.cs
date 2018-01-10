using System;
using System.Collections.Generic;
using System.Text;
using Borlay.Iota.Library.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Borlay.Iota.Library.Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void ConvertAsciiToTrytes()
        {
            Assert.AreEqual(Converter.AsciiToTrytes("HELLOWORLD"), "RBOBVBVBYBFCYBACVBNB");
        }

        [TestMethod]
        public void ConvertTrytesToAscii()
        {
            Assert.AreEqual(Converter.TrytesToAscii("RBOBVBVBYBFCYBACVBNB"),"HELLOWORLD");
        }
    }
}
