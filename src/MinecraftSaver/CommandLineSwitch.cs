#region

using System;

#endregion

namespace MinecraftSaver
{
    [AttributeUsage( AttributeTargets.Property )]
    internal class CommandLineSwitch : Attribute
    {
        internal string Name { get; }
        internal string ShortName { get; }
        internal string Description { get; }

        internal CommandLineSwitch( string name, string description, string shortName = "" )
        {
            Name = name;
            Description = description;
            ShortName = shortName;
        }
    }
}