#region

using CommandLineUtils.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Local

#endregion

namespace MinecraftSaver
{
    [CommandLineInterface]
    internal class Config
    {
        [CommandLineSwitch( "most-recent",
            "Specifies whether to backup the most recently played world, without the user having to pick a world",
            "r" )]
        internal bool BackupMostRecentSave { get; private set; }

        [CommandLineSwitch( "force",
            "Specifies whether an existing backup can be overwritten without further user consent" )]
        internal bool AllowOverwrite { get; private set; }

        [CommandLineSwitch( "help",
            "Displays this help menu" )]
        internal bool HelpSwitchReceived { get; private set; }
    }
}