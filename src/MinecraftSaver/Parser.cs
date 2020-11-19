#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineUtils.Attributes;

#endregion

namespace MinecraftSaver
{
    internal class Parser
    {
        private const string FullSwitchIdentifier = "--";
        private const string ShortSwitchIdentifier = "-";
        private const int NoSwitchFoundLength = -1;

        private readonly string[] _args;
        private readonly IEnumerable<Type> _typesWithSwitches;

        private Config _config;

        internal Parser( string[] args )
        {
            _args = args;

            //TODO Get types through reflection
            _typesWithSwitches = new[] {typeof(Config)};
        }

        internal Config CreateConfig( )
        {
            if ( null == _config )
            {
                _config = DoCreateConfig( );
            }

            return _config;
        }

        private Config DoCreateConfig( )
        {
            IDictionary<string, PropertyInfo> switches =
                new Dictionary<string, PropertyInfo>( );
            foreach ( (CommandLineSwitch commandLineSwitch, PropertyInfo propertyInfo) in
                GetSwitchProperties( ) )
            {
                switches.Add( commandLineSwitch.Name, propertyInfo );
                switches.Add( commandLineSwitch.ShortName, propertyInfo );
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
                    string command = arg.Substring( switchLength );
                    if ( switches.TryGetValue( command,
                        out PropertyInfo propertyInfo ) )
                    {
                        //TODO Is there a more elegant way of doing this?
                        if ( typeof(bool) == propertyInfo.PropertyType )
                        {
                            propertyInfo.SetValue( config, true );
                        }
                    }
                    else
                    {
                        Console.WriteLine( $"Ignoring unrecognized switch '{arg}'" );
                    }
                }
                else
                {
                    Console.WriteLine( $"Ignoring invalid argument '{arg}'" );
                }
            }

            return config;
        }

        internal bool TryDisplayHelpMenu( )
        {
            if ( null == _config )
            {
                _config = DoCreateConfig( );
            }

            if ( _config.HelpSwitchReceived )
            {
                DisplayHelp( );
            }

            return _config.HelpSwitchReceived;
        }

        private void DisplayHelp( )
        {
            IEnumerable<(CommandLineSwitch, PropertyInfo)> switchProperties =
                GetSwitchProperties( );
            int maxNameLength = switchProperties.Max( pair => pair.Item1.Name.Length );

            foreach ( (CommandLineSwitch commandLineSwitch, PropertyInfo _) in
                GetSwitchProperties( ) )
            {
                Console.WriteLine(
                    $"--{commandLineSwitch.Name}{' '.Repeat( maxNameLength - commandLineSwitch.Name.Length )} (-{commandLineSwitch.ShortName})    {commandLineSwitch.Description}" );
            }
        }

        private IEnumerable<(CommandLineSwitch, PropertyInfo)> GetSwitchProperties( )
        {
            foreach ( Type type in _typesWithSwitches )
            {
                foreach ( PropertyInfo propertyInfo in type.GetRuntimeProperties( ) )
                {
                    foreach ( CommandLineSwitch commandLineSwitch in propertyInfo
                        .GetCustomAttributes<CommandLineSwitch>( ) )
                    {
                        yield return (commandLineSwitch, propertyInfo);
                    }
                }
            }
        }
    }
}