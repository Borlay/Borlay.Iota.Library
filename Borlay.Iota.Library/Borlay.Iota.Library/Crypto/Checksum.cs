using System;
using System.Collections.Generic;
using System.Text;

namespace Borlay.Iota.Library.Crypto
{
    public class Checksum
    {
        public static string AddChecksum(string inputValue, int checksumLength = 9)
        {
            var kerl = new Kerl();
            kerl.Initialize();

            // Address trits
            var addressTrits = Converter.GetTrits(inputValue);

            // Checksum trits
            var checksumTrits = new sbyte[Curl.HASH_LENGTH];

            // Absorb address trits
            kerl.Absorb(addressTrits, 0, addressTrits.Length);

            // Squeeze checksum trits
            kerl.Squeeze(checksumTrits, 0, Curl.HASH_LENGTH);

            // First 9 trytes as checksum
            var checksum = Converter.GetTrytes(checksumTrits).Substring(81 - checksumLength, checksumLength);
            return inputValue + checksum;
        }

        public static string RemoveChecksum(string inputValue)
        {
            return inputValue.Substring(0, 81);
        }

        public static bool IsValidChecksum(string inputValue)
        {
            var withoutChecksum = RemoveChecksum(inputValue);
            var cLen = inputValue.Length - withoutChecksum.Length;
            var withChecksum = AddChecksum(withoutChecksum, cLen);
            return inputValue == withChecksum;
        }
    }
}

/*

var addChecksum = function(inputValue, checksumLength, isAddress) {

    // checksum length is either user defined, or 9 trytes
    var checksumLength = checksumLength || 9;
var isAddress = (isAddress !== false);

// the length of the trytes to be validated
var validationLength = isAddress ? 81 : null;

var isSingleInput = inputValidator.isString(inputValue);

    // If only single address, turn it into an array
    if (isSingleInput ) inputValue = new Array(inputValue);

var inputsWithChecksum = [];

inputValue.forEach(function(thisValue) {

        // check if correct trytes
        if (!inputValidator.isTrytes(thisValue, validationLength)) {
            throw new Error("Invalid input");
        }

        var kerl = new Kerl();
kerl.initialize();

        // Address trits
        var addressTrits = Converter.trits(thisValue);

// Checksum trits
var checksumTrits = [];

// Absorb address trits
kerl.absorb(addressTrits, 0, addressTrits.length);

        // Squeeze checksum trits
        kerl.squeeze(checksumTrits, 0, Curl.HASH_LENGTH);

        // First 9 trytes as checksum
        var checksum = Converter.trytes(checksumTrits).substring(81 - checksumLength, 81);
inputsWithChecksum.push(thisValue + checksum );
    });

    if (isSingleInput) {

        return inputsWithChecksum[0];

    } else {

        return inputsWithChecksum;

    }
}

    */