namespace MinecraftSaver
{
    internal class Config
    {
        [CommandLineSwitch( "most-recent",
            "Specifies whether to backup the most recently played world, without the user having to pick a world",
            "r" )]
        internal bool BackupMostRecentSave { get; private set; }

        [CommandLineSwitch( "force",
            "Specifies whether an existing backup can be overwritten without further user consent",
            "f" )]
        internal bool AllowOverwrite { get; private set; }
    }
}