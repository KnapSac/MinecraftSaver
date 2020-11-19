#region

using System;

#endregion

namespace CommandLineUtils.Attributes
{
    [AttributeUsage( AttributeTargets.Property )]
    public class CommandLineSwitch : Attribute
    {
        public string Name { get; }
        public string ShortName { get; }
        public string Description { get; }

        public CommandLineSwitch( string name, string description ) : this( name, description,
            name[0].ToString( ) )
        {
        }

        public CommandLineSwitch( string name, string description, string shortName )
        {
            Name = name;
            Description = description;
            ShortName = shortName;
        }
    }
}