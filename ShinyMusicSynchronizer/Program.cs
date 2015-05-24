using System;
using System.Diagnostics.Contracts;
using System.Linq;
using clipr;
using clipr.Usage;
using LightInject;
using PortableDeviceLib;

namespace ShinyMusicSynchronizer
{
    static class Program
    {
        [ApplicationInfo(Description = "ShinyMusicSynchronizer is a tool for one-way synchronization of local media (particularly music) library to a portable MTP device.")]
        private class Options : ICommandOptions
        {
            [Verb("list-devices", "Lists the available portable MTP devices currently connected to the computer.")]
            public ListDevicesCommand.Options ListDevices { get; set; }

            [Verb("synchronize", "Synchronizes the selected portable MTP devices with computer media directory.")]
            public SynchronizeCommand.Options Synchronize { get; set; }
        }

        private class OptionsToCommandParser
        {
            private readonly Options _options;
            private readonly Action<ICommandMessage> _reportMessage;

            public Func<ListDevicesCommand> CreateListDevicesCommand { get; set; }
            public Func<SynchronizeCommand> CreateSynchronizeCommand { get; set; }

            public OptionsToCommandParser(Options options, Action<ICommandMessage> reportMessage)
            {
                Contract.Assert(options != null);
                _options = options;
                _reportMessage = reportMessage;
            }

            public ICommand CreateCommandFromOptions()
            {
                if (_options.ListDevices != null)
                    return CreateListDevicesCommand();
                if (_options.Synchronize != null)
                    return CreateSynchronizeCommand();

                var usageHelper = new AutomaticHelpGenerator<Options>();
                var usage = usageHelper.GetUsage(); //TODO uplne nefunguje
                _reportMessage(new SimpleCommandMessage(usage, CommandMessageType.Error));
                Environment.Exit(-1);
                throw new ArgumentException("Shouldn't happen. Command line options validated, but cannot create a command.");
            }
        }

        private static PortableDeviceCollection CreatePortableDeviceCollectionFromApplicationInformation(ApplicationInformation applicationInformation)
        {
            PortableDeviceCollection.CreateInstance(applicationInformation.Name, applicationInformation.MajorVersionNumber, applicationInformation.MinorVersionNumber);
            PortableDeviceCollection.Instance.AutoConnectToPortableDevice = false;
            return PortableDeviceCollection.Instance;
        }

        static void Main(string[] args)
        {
            var container = new LightInject.ServiceContainer();

            container.Register<ApplicationInformation>(c => new ApplicationInformation("ShinyMusicSynchronizer", 1, 0), new PerContainerLifetime());
            container.Register<PortableDeviceCollection>(c => CreatePortableDeviceCollectionFromApplicationInformation(c.GetInstance<ApplicationInformation>()));

            container.Register<IUserInterface, ConsoleUserInterface>(new PerContainerLifetime());
            container.Register<Action<ICommandMessage>>(c => new Action<ICommandMessage>(c.GetInstance<IUserInterface>().ReportMessage));

            container.Register<Options>(c => CliParser.StrictParse<Options>(args), new PerContainerLifetime());
            container.Register<OptionsToCommandParser, OptionsToCommandParser>();
            container.Register<ListDevicesCommand.Options>(c => c.GetInstance<Options>().ListDevices);
            container.Register<ListDevicesCommand, ListDevicesCommand>();
            container.Register<SynchronizeCommand.Options>(c => c.GetInstance<Options>().Synchronize);
            container.Register<SynchronizeCommand, SynchronizeCommand>();
            container.Register<ICommand>(c => c.GetInstance<OptionsToCommandParser>().CreateCommandFromOptions());

            var command = container.GetInstance<ICommand>();
            command.Execute();
        }
    }
}
