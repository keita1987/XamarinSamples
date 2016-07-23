﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XFStopwatch.Tests
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void TestLocateNormal()
        {
            var sercice = new Service();
            ServiceLocator.Register<IService>(sercice);

            var actual = ServiceLocator.Locate<IService>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(sercice, actual);
        }

        [TestMethod]
        public void TestLocateNullValue()
        {
            ServiceLocator.Register<IService>(null);
            Assert.IsNull(ServiceLocator.Locate<IService>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLocateNotRegistered()
        {
            ServiceLocator.Locate<string>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestClear()
        {
            ServiceLocator.Register(1);
            Assert.AreEqual(1, ServiceLocator.Locate<int>());
            ServiceLocator.Clear();
            ServiceLocator.Locate<int>();
        }



        public interface IService
        {
        }

        public class Service : IService
        {
        }
    }
}
