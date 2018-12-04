﻿using Microsoft.Research.SEAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SEALNetTest
{
    [TestClass]
    public class SecretKeyTests
    {
        [TestMethod]
        public void SaveLoadTest()
        {
            List<SmallModulus> coeffModulus = new List<SmallModulus>
            {
                DefaultParams.SmallMods40Bit(0)
            };
            EncryptionParameters parms = new EncryptionParameters(SchemeType.BFV)
            {
                PolyModulusDegree = 64,
                PlainModulus = new SmallModulus(1 << 6),
                CoeffModulus = coeffModulus
            };
            SEALContext context = SEALContext.Create(parms);
            KeyGenerator keygen = new KeyGenerator(context);

            SecretKey secret = keygen.SecretKey;

            Assert.AreEqual(64, secret.Data.CoeffCount);
            Assert.IsTrue(secret.Data.IsNTTForm);
            Assert.AreNotEqual(ParmsId.Zero, secret.ParmsId);

            SecretKey secret2 = new SecretKey();
            Assert.IsNotNull(secret2);
            Assert.AreEqual(0, secret2.Data.CoeffCount);
            Assert.IsFalse(secret2.Data.IsNTTForm);

            using (MemoryStream stream = new MemoryStream())
            {
                secret.Save(stream);

                stream.Seek(offset: 0, loc: SeekOrigin.Begin);

                secret2.Load(context, stream);
            }

            Assert.AreNotSame(secret, secret2);
            Assert.AreEqual(64, secret2.Data.CoeffCount);
            Assert.IsTrue(secret2.Data.IsNTTForm);
            Assert.AreNotEqual(ParmsId.Zero, secret2.ParmsId);
            Assert.AreEqual(secret.ParmsId, secret2.ParmsId);
        }
    }
}
