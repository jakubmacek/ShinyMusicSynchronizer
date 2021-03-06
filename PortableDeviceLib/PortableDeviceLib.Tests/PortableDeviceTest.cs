// <copyright file="PortableDeviceTest.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>਍ഀ
using System;਍ഀ
using Microsoft.Pex.Framework;਍ഀ
using Microsoft.Pex.Framework.Validation;਍ഀ
using Microsoft.VisualStudio.TestTools.UnitTesting;਍ഀ
using PortableDeviceLib;਍ഀ
਍ഀ
namespace PortableDeviceLib਍ഀ
{਍ഀ
    /// <summary>This class contains parameterized unit tests for PortableDevice</summary>਍ഀ
    [PexClass(typeof(PortableDevice))]਍ഀ
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]਍ഀ
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]਍ഀ
    [TestClass]਍ഀ
    public partial class PortableDeviceTest਍ഀ
    {਍ഀ
        /// <summary>Test stub for ConnectToDevice(String, Single, Single)</summary>਍ഀ
        [PexMethod]਍ഀ
        public void ConnectToDevice(਍ഀ
            [PexAssumeUnderTest]PortableDevice target,਍ഀ
            string appName,਍ഀ
            float majorVersionNumber,਍ഀ
            float minorVersionNumber਍ഀ
        )਍ഀ
        {਍ഀ
            // TODO: add assertions to method PortableDeviceTest.ConnectToDevice(PortableDevice, String, Single, Single)਍ഀ
            target.ConnectToDevice(appName, majorVersionNumber, minorVersionNumber);਍ഀ
        }਍ഀ
਍ഀ
        /// <summary>Test stub for .ctor(String)</summary>਍ഀ
        [PexMethod]਍ഀ
        public PortableDevice Constructor(string deviceId)਍ഀ
        {਍ഀ
            // TODO: add assertions to method PortableDeviceTest.Constructor(String)਍ഀ
            PortableDevice target = new PortableDevice(deviceId);਍ഀ
            return target;਍ഀ
        }਍ഀ
਍ഀ
        /// <summary>Test stub for DeviceId</summary>਍ഀ
        [PexMethod]਍ഀ
        public void DeviceIdGet([PexAssumeUnderTest]PortableDevice target)਍ഀ
        {਍ഀ
            // TODO: add assertions to method PortableDeviceTest.DeviceIdGet(PortableDevice)਍ഀ
            string result = target.DeviceId;਍ഀ
        }਍ഀ
਍ഀ
        /// <summary>Test stub for IsConnected</summary>਍ഀ
        [PexMethod]਍ഀ
        public void IsConnectedGet([PexAssumeUnderTest]PortableDevice target)਍ഀ
        {਍ഀ
            // TODO: add assertions to method PortableDeviceTest.IsConnectedGet(PortableDevice)਍ഀ
            bool result = target.IsConnected;਍ഀ
        }਍ഀ
    }਍ഀ
}਍ഀ
