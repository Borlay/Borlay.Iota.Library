using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public class Hmac
    {
        public const int HMAC_ROUNDS = 27;
        public sbyte[] _key;

        public Hmac(string key)
        {
            this._key = Converter.GetTrits(key);
        }

        //public void AddHMAC(bundle)
        //{
        //    var curl = new Curl(HMAC_ROUNDS);
        //    var key = this._key;
        //    for (var i = 0; i < bundle.bundle.length; i++)
        //    {
        //        if (bundle.bundle[i].value > 0)
        //        {
        //            var bundleHashTrits = Converter.trits(bundle.bundle[i].bundle);
        //            var hmac = new Int8Array(243);
        //            curl.initialize();
        //            curl.absorb(key);
        //            curl.absorb(bundleHashTrits);
        //            curl.squeeze(hmac);
        //            var hmacTrytes = Converter.trytes(hmac);
        //            bundle.bundle[i].signatureMessageFragment = hmacTrytes + bundle.bundle[i].signatureMessageFragment.substring(81, 2187);
        //        }
        //    }
        //}

    }
}

/*
var HMAC_ROUNDS = 27;

function hmac(key)
{
    this._key = Converter.trits(key);
}

hmac.prototype.addHMAC = function(bundle)
{
    var curl = new Curl(HMAC_ROUNDS);
    var key = this._key;
    for (var i = 0; i < bundle.bundle.length; i++)
    {
        if (bundle.bundle[i].value > 0)
        {
            var bundleHashTrits = Converter.trits(bundle.bundle[i].bundle);
            var hmac = new Int8Array(243);
            curl.initialize();
            curl.absorb(key);
            curl.absorb(bundleHashTrits);
            curl.squeeze(hmac);
            var hmacTrytes = Converter.trytes(hmac);
            bundle.bundle[i].signatureMessageFragment = hmacTrytes + bundle.bundle[i].signatureMessageFragment.substring(81, 2187);
        }
    }
}
*/
