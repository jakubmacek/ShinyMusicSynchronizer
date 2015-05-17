﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableDeviceLib;

namespace ShinyMusicSynchronizer
{
    sealed class ListDevicesCommand : ICommand
    {
        public sealed class Options : ICommandOptions
        {
        }

        private readonly Options _options;
        private readonly ApplicationInformation _applicationInformation;
        private readonly PortableDeviceCollection _portableDeviceCollection;
        private readonly IMessageReporter _messageReporter;

        public ListDevicesCommand(Options options, PortableDeviceCollection portableDeviceCollection, IMessageReporter messageReporter, ApplicationInformation applicationInformation)
        {
            Contract.Assert(options != null);
            Contract.Assert(applicationInformation != null);
            Contract.Assert(portableDeviceCollection != null);
            Contract.Assert(messageReporter != null);

            _options = options;
            _applicationInformation = applicationInformation;
            _portableDeviceCollection = portableDeviceCollection;
            _messageReporter = messageReporter;
        }

        public void Execute()
        {
            foreach (var device in _portableDeviceCollection.Devices)
            {
                device.ConnectToDevice(_applicationInformation.Name, _applicationInformation.MajorVersionNumber, _applicationInformation.MinorVersionNumber);
                var message = string.Format("Device: {0}\n\tModel: {1}\n\tFriendly name: {2}\n", device.DeviceId, device.Model, device.FriendlyName);
                _messageReporter.ReportInformation(message);
            }
        }
    }
}
