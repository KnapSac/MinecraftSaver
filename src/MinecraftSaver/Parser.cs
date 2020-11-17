#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace MinecraftSaver
{
    internal class Parser
    {
        private const string FullSwitchIdentifier = "--";
        private const string ShortSwitchIdentifier = "-";
        private const int NoSwitchFoundLength = -1;

        private readonly string[] _args;

        internal Parser( string[] args )
        {
            _args = args;
        }

        internal Config CreateConfig( )
        {
            IDictionary<string, PropertyInfo> switches =
                new Dictionary<string, PropertyInfo>( );
            foreach ( PropertyInfo propertyInfo in typeof(Config).GetRuntimeProperties( ) )
            {
                foreach ( CommandLineSwitch commandLineSwitch in propertyInfo
                    .GetCustomAttributes<CommandLineSwitch>( ) )
                {
                    switches.Add( commandLineSwitch.Name, propertyInfo );
                    switches.Add( commandLineSwitch.ShortName, propertyInfo );
                }
            }

            Config config = new Config( );
            foreach ( string arg in _args )
            {
                int switchLength = arg.StartsWith( FullSwitchIdentifier )
                    ? FullSwitchIdentifier.Length
                    : arg.StartsWith( ShortSwitchIdentifier )
                        ? ShortSwitchIdentifier.Length
                        : NoSwitchFoundLength;

                if ( NoSwitchFoundLength != switchLength )
                {
                    if ( switches.TryGetValue( arg.Substring( switchLength ),
                        out PropertyInfo propertyInfo ) )
                    {
                        propertyInfo.SetValue( config, true );
                    }
                }
                else
                {
                    Console.WriteLine( $"Ignoring unrecognized switch '{arg}'" );
                }
            }

            return config;
        }
    }
}