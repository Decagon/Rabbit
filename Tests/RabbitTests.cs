﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit;
using Rabbit.Auth;

namespace Rabbit.Tests
{
    [TestClass]
    public class RabbitTests
    {
        [TestMethod]
        public void NullAuthenticationTest()
        {
            ArgumentNullException expectedException = null;

            try
            {
                var authResult = Rabbit.GetAuthType(null, null);
            }

            catch (ArgumentNullException ex)
            {
                expectedException = ex;
            }

            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public void RegularAuthenticationTest()
        {
            var authResult = Rabbit.GetAuthType("test@email.com", "testpassword");

            Assert.AreEqual(AuthType.Regular, authResult);
        }

        [TestMethod]
        public void FacebookAuthenticationTest()
        {
            var authResult = Rabbit.GetAuthType(null, "TcsL8qdSwvzDqudWGfYEGF5RrBLBSanHW78t5Z87ngKzUDdhCE4Jbtq6Vrwk2vuVS8WW2RYT54hwFxchWywLLvQaUyA2k2fanfAs");

            Assert.AreEqual(AuthType.Facebook, authResult);
        }

        [TestMethod]
        public void KongregateAuthenticationTest()
        {
            var authResult = Rabbit.GetAuthType("123456", "969ad76abf19b5c3e5917321e659abe6d8d6f3aba73e5158863c3b4159c00366");

            Assert.AreEqual(AuthType.Kongregate, authResult);
        }

        [TestMethod]
        public void ArmorGamesAuthenticationTest()
        {
            var authResult = Rabbit.GetAuthType("ee6ab941d09050400c4f916dbb47aad8", "23b9a5088c3c940f82945b6f3df80abc");

            Assert.AreEqual(AuthType.ArmorGames, authResult);
        }
    }
}
