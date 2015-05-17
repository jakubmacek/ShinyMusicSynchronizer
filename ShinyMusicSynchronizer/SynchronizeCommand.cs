using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clipr;
using PortableDeviceLib;
using PortableDeviceLib.Model;

namespace ShinyMusicSynchronizer
{
    sealed class SynchronizeCommand : ICommand
    {
        public sealed class Options : ICommandOptions
        {
            [NamedArgument('d', "device-id", Action = ParseAction.Store, Description = "Sets the ID of MTP device to synchronize.")]
            public string DeviceId { get; set; }

            [NamedArgument('p', "root-device-path", Action = ParseAction.Store, Description = "Sets the root path of media directory on the device (eg. 'SD Card/Music').")]
            public string RootDevicePath { get; set; }

            [NamedArgument('r', "root-computer-path", Action = ParseAction.Store, Description = "Sets the root path of media directory on the computer (eg. 'C:\\Music').")]
            public string RootComputerPath { get; set; }

            [NamedArgument('m', "synchronization-marker-file-name", Action = ParseAction.Store, Description = "Sets the name of the file that marks directory for synchronization (default '.sync').")]
            public string SynchronizationMarkerFileName { get; set; }

            public Options()
            {
                SynchronizationMarkerFileName = ".sync";
                var appSettings = ConfigurationManager.AppSettings;
                DeviceId = appSettings["DeviceId"];
                RootDevicePath = appSettings["RootDevicePath"];
                RootComputerPath = appSettings["RootComputerPath"];
            }
        }

        private readonly Options _options;
        private readonly ApplicationInformation _applicationInformation;
        private readonly PortableDeviceCollection _portableDeviceCollection;
        private readonly IMessageReporter _messageReporter;

        private SynchronizedRootDirectory Root { get; set; }

        public SynchronizeCommand(Options options, PortableDeviceCollection portableDeviceCollection, IMessageReporter messageReporter, ApplicationInformation applicationInformation)
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

        private PortableDevice GetSelectedPortableDevice()
        {
            var deviceId = _options.DeviceId;
            var device = _portableDeviceCollection.Devices.FirstOrDefault(x => x.DeviceId.ToLower() == deviceId);
            if (device != null)
                return device;
            var maybeDevices = _portableDeviceCollection.Devices.Where(x => x.DeviceId.ToLower().Contains(deviceId)).ToArray();
            if (maybeDevices.Length == 1)
                return maybeDevices[0];
            else if (maybeDevices.Length == 0)
                throw new ArgumentException(string.Format("Cannot find the selected device '{0}'.", _options.DeviceId));
            else // (maybeDevices.Length > 1)
                throw new ArgumentException(string.Format("More than one device matches '{0}'.", _options.DeviceId));
        }

        public void Execute()
        {
            var device = GetSelectedPortableDevice();
            device.ConnectToDevice(_applicationInformation.Name, _applicationInformation.MajorVersionNumber, _applicationInformation.MinorVersionNumber);

            UpdateDeviceRoot(device);
            UpdateComputerRoot();

            DisplayRoot();
            Synchronize(device);

            device.Disconnect();
        }

        private void Synchronize(PortableDevice device)
        {
            var storageServiceAdapter = new StorageServicesAdapter(device);
            SynchronizeDirectory(Root, storageServiceAdapter);
        }

        private void SynchronizeDirectory(SynchronizedDirectory parentDirectory, StorageServicesAdapter storageServiceAdapter)
        {
            var operation = parentDirectory.Operation;

            if (operation == SynchronizationOperation.Copy)
            {                
                _messageReporter.ReportInformation(string.Format("Create directory {0}", parentDirectory.Path));
                DoOperation(storageServiceAdapter, parentDirectory);
            }

            foreach (var file in parentDirectory.Files.Where(file => file.Operation == SynchronizationOperation.Delete))
            {
                _messageReporter.ReportInformation(string.Format("Delete file      {0}", file.Path));
                DoOperation(storageServiceAdapter, file);
            }

            foreach (var directory in parentDirectory.Directories)
            {
                SynchronizeDirectory(directory, storageServiceAdapter);
            }

            foreach (var file in parentDirectory.Files.Where(file => file.Operation == SynchronizationOperation.Copy))
            {
                _messageReporter.ReportInformation(string.Format("Copy file        {0}", file.Path));
                DoOperation(storageServiceAdapter, file);
            }

            if (operation == SynchronizationOperation.Delete)
            {
                _messageReporter.ReportInformation(string.Format("Delete directory {0}", parentDirectory.Path));
                DoOperation(storageServiceAdapter, parentDirectory);
            }
        }

        private void DoOperation(StorageServicesAdapter storageServiceAdapter, SynchronizedDirectory directory)
        {
            try
            {
                var operation = directory.Operation;
                if (operation == SynchronizationOperation.Delete)
                {
                    storageServiceAdapter.Delete(directory.ObjectOnDevice);
                    directory.IsOnDevice = false;
                    directory.ObjectOnDevice = null;
                }

                if (operation == SynchronizationOperation.Copy)
                {
                    directory.ObjectOnDevice = (PortableDeviceContainerObject)storageServiceAdapter.Mkdir(directory.Parent.ObjectOnDevice, directory.Name);
                    directory.IsOnDevice = true;
                }
            }
            catch (Exception e)
            {
                _messageReporter.ReportError(string.Format("Exception: {0}", e.Message));
            }
        }

        private void DoOperation(StorageServicesAdapter storageServiceAdapter, SynchronizedFile file)
        {
            try
            {
                var operation = file.Operation;
                if (operation == SynchronizationOperation.Delete)
                {
                    storageServiceAdapter.Delete(file.ObjectOnDevice);
                    file.IsOnDevice = false;
                    file.SizeOnDevice = 0;
                    file.ObjectOnDevice = null;
                }

                if (operation == SynchronizationOperation.Copy)
                {
                    StorageServicesAdapter.PushProgressReport pushProgressReport = (int totalBytesRead) =>
                    {
                        _messageReporter.ReportInformation(string.Format("\tCopy: {0} out of {1} = {2}%",
                            FormatFileSize(totalBytesRead),
                            FormatFileSize(file.SizeOnComputer),
                            100 * totalBytesRead / file.SizeOnComputer
                        ));
                    };
                    file.ObjectOnDevice = (PortableDeviceFileObject)storageServiceAdapter.Push(file.Parent.ObjectOnDevice, file.ObjectOnComputer.OpenRead(), file.Name, (ulong)file.SizeOnComputer, pushProgressReport);
                    file.IsOnDevice = true;
                    file.SizeOnDevice = file.SizeOnComputer;
                }
            }
            catch (Exception e)
            {
                _messageReporter.ReportError(string.Format("Exception: {0}", e.Message));
            }
        }

        private void DisplayRoot()
        {
            DisplayDirectory(Root);
        }

        private string FormatFileSize(long fileSize)
        {
            return string.Format("{0:n0} kB", fileSize / 1024);
        }

        private void DisplayDirectory(SynchronizedDirectory parentDirectory, string depthIndentation = "")
        {
            const int width = 80;

            foreach (var file in parentDirectory.Files.Where(x => x.Operation != SynchronizationOperation.Nothing))
            {
                if (file.Operation == SynchronizationOperation.Keep)
                    continue;

                _messageReporter.ReportInformation(string.Format("{0} {1} {2,-6} {3,11} {4,11}",
                    depthIndentation,
                    file.Name.PadRight(width - depthIndentation.Length),
                    file.Operation.ToString(),
                    FormatFileSize(file.SizeOnComputer),
                    FormatFileSize(file.SizeOnDevice)
                ));
            }

            foreach (var directory in parentDirectory.Directories.Where(x => x.Operation != SynchronizationOperation.Nothing))
            {
                //if (directory.Operation == SynchronizationOperation.Delete)
                //    File.WriteAllText(Path.Combine(Path.Combine(RootComputerPath, directory.Path), SynchronizationMarkerFileName), "");

                _messageReporter.ReportInformation(string.Format("{0} {1} {2,-6}",
                    depthIndentation,
                    directory.Name.PadRight(width - depthIndentation.Length),
                    directory.Operation.ToString()
                ));
                DisplayDirectory(directory, depthIndentation + "  ");
            }
        }

        private void UpdateComputerRoot()
        {
            var container = new DirectoryInfo(_options.RootComputerPath);
            ExploreComputerDirectory(container, Root);
        }

        private bool ExploreComputerDirectory(DirectoryInfo container, SynchronizedDirectory parentDirectory)
        {
            var shouldBeSynchronized = container.GetFiles(_options.SynchronizationMarkerFileName).Length > 0;

            if (shouldBeSynchronized)
            {
                foreach (var child in container.GetFiles())
                {
                    if (child.Name == _options.SynchronizationMarkerFileName)
                        continue;
                    var file = parentDirectory.Files.FirstOrDefault(x => x.Name == child.Name);
                    if (file == null)
                    {
                        file = new SynchronizedFile();
                        file.Parent = parentDirectory;
                        file.Name = child.Name;
                        file.ComputePath();
                        parentDirectory.Files.Add(file);
                    }
                    file.IsOnComputer = true;
                    file.SizeOnComputer = child.Length;
                    file.ObjectOnComputer = child;
                }
            }

            foreach (var child in container.GetDirectories())
            {
                var directory = parentDirectory.Directories.FirstOrDefault(x => x.Name == child.Name);
                if (directory == null)
                {
                    directory = new SynchronizedDirectory();
                    directory.Parent = parentDirectory;
                    directory.Name = child.Name;
                    directory.ComputePath();
                    parentDirectory.Directories.Add(directory);
                }

                var childShouldBeSynchronized = ExploreComputerDirectory(child, directory);
                shouldBeSynchronized = shouldBeSynchronized || childShouldBeSynchronized;
            }

            parentDirectory.IsOnComputer = shouldBeSynchronized;
            parentDirectory.ObjectOnComputer = container;
            return shouldBeSynchronized;
        }

        private void UpdateDeviceRoot(PortableDevice device)
        {
            Root = new SynchronizedRootDirectory();
            device.RefreshContent(new RecursivePathEnumerateHelper(_options.RootDevicePath + "/.*"));
            ExploreDeviceDirectory(device.Content, Root, _options.RootDevicePath.Count(x => x == '/') + 1);
        }

        private void ExploreDeviceDirectory(PortableDeviceContainerObject container, SynchronizedDirectory parentDirectory, int remaingDepthToIgnore)
        {
            if (remaingDepthToIgnore == 0)
                parentDirectory.ObjectOnDevice = container;

            foreach (var child in container.Childs)
            {
                var fileObject = child as PortableDeviceFileObject;
                if (fileObject != null)
                {
                    if (remaingDepthToIgnore > 0)
                        continue;

                    var file = new SynchronizedFile();
                    file.Parent = parentDirectory;
                    file.Name = fileObject.FileName;
                    file.ComputePath();
                    file.SizeOnDevice = fileObject.Size;
                    file.IsOnDevice = true;
                    file.ObjectOnDevice = fileObject;

                    parentDirectory.Files.Add(file);
                }
                else
                {
                    var containerObject = child as PortableDeviceContainerObject;
                    if (containerObject != null)
                    {
                        if (remaingDepthToIgnore > 0)
                            ExploreDeviceDirectory(containerObject, parentDirectory, remaingDepthToIgnore - 1);
                        else
                        {
                            var directory = new SynchronizedDirectory();
                            directory.Parent = parentDirectory;
                            directory.Name = containerObject.Name;
                            directory.ComputePath();
                            directory.IsOnDevice = true;
                            directory.ObjectOnDevice = containerObject;

                            ExploreDeviceDirectory(containerObject, directory, remaingDepthToIgnore - 1);

                            parentDirectory.Directories.Add(directory);
                        }
                    }
                }
            }
        }
    }
}
