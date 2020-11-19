#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineUtils.Attributes;
using CommandLineUtils.Collections;

#endregion

namespace CommandLineUtils.Parsing
{
    public class Parser
    {
        private const char SwitchIdentifier = '-';

        private readonly IDictionary<string, PropertyInfo> _commandToPropertyInfoMap;
        private readonly string[] _args;
        private readonly IList<string> _errorMessages;

        [MethodImpl( MethodImplOptions.NoInlining )]
        public Parser( string[] args )
        {
            _args = args;
            _commandToPropertyInfoMap = InitializeMap( Assembly.GetCallingAssembly( ) );
            _errorMessages = new List<string>( );
        }

        public bool Parse<T>( ref T instance )
        {
            bool success = true;

            IEnumerator enumerator = _args.GetEnumerator( );
            while ( enumerator.MoveNext( ) )
            {
                if ( !(enumerator.Current is string arg) )
                {
                    continue;
                }

                if ( IsCommand( arg ) )
                {
                    string command = CleanCommand( arg );
                    if ( _commandToPropertyInfoMap.TryGetValue( command,
                        out PropertyInfo propertyInfo ) )
                    {
                        //KNOWN If a boolean switch is passed, we consider it a flag so we can directly set the value to 'true'
                        if ( typeof(bool) == propertyInfo.PropertyType )
                        {
                            propertyInfo.SetValue( instance, true );
                        }

                        //ASSUME Argument to switch with parameter is passed directly after the switch
                        if ( typeof(string) == propertyInfo.PropertyType )
                        {
                            if ( enumerator.MoveNext( ) )
                            {
                                if ( null != enumerator.Current )
                                {
                                    if ( enumerator.Current is string value && !IsCommand( value ) )
                                    {
                                        propertyInfo.SetValue( instance, value );
                                        continue;
                                    }
                                }
                            }

                            success = false;
                            _errorMessages.Add( $"Missing argument for command '{command}'" );
                        }
                    }
                    else
                    {
                        success = false;
                        _errorMessages.Add( $"Ignoring unrecognized switch '{arg}'" );
                    }
                }
                else
                {
                    success = false;
                    _errorMessages.Add( $"Ignoring invalid argument '{arg}'" );
                }
            }

            return success;
        }

        public void FlushErrorMessages( )
        {
            foreach ( string errorMessage in _errorMessages )
            {
                Console.WriteLine( errorMessage );
            }
        }

        private IDictionary<string, PropertyInfo> InitializeMap( Assembly assembly )
        {
            Map<string, PropertyInfo> commandToPropertyInfoMap =
                new Map<string, PropertyInfo>( );

            foreach ( TypeInfo typeInfo in
                GetClassesWithAttribute<CommandLineInterface>( assembly ) )
            {
                foreach ( PropertyInfo propertyInfo in
                    GetPropertiesWithAttribute<CommandLineSwitch>( typeInfo ) )
                {
                    CommandLineSwitch commandLineSwitch =
                        propertyInfo.GetCustomAttribute<CommandLineSwitch>( );

                    string loweredSwitchName = commandLineSwitch.Name.ToLower( );
                    AddCommandToMap( loweredSwitchName, propertyInfo, typeInfo,
                        "Duplicate commands, defined at '{0}' and '{1}'",
                        ref commandToPropertyInfoMap );

                    string loweredShortSwitchName = commandLineSwitch.ShortName.ToLower( );
                    AddCommandToMap( loweredShortSwitchName, propertyInfo, typeInfo,
                        "Duplicate aliases, defined at '{0}' and '{1}'",
                        ref commandToPropertyInfoMap );
                }
            }

            return commandToPropertyInfoMap;
        }

        private void AddCommandToMap( string command, PropertyInfo propertyInfo, TypeInfo typeInfo,
                                      string errorMessage, ref Map<string, PropertyInfo> map )
        {
            if ( !map.Add( command,
                propertyInfo ) )
            {
                if ( map.TryGetValue( command,
                    out PropertyInfo otherPropertyInfo ) )
                {
                    TypeInfo otherTypeInfo = otherPropertyInfo.DeclaringType.GetTypeInfo( );
                    throw new ArgumentException( string.Format( errorMessage, typeInfo.FullName,
                        otherTypeInfo.FullName ) );
                }
            }
        }

        private IEnumerable<TypeInfo> GetClassesWithAttribute<T>( Assembly assembly )
            where T : Attribute
        {
            foreach ( TypeInfo typeInfo in assembly.DefinedTypes )
            {
                if ( typeInfo.HasCustomAttribute<T>( ) )
                {
                    yield return typeInfo;
                }
            }
        }

        private IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>( TypeInfo typeInfo )
            where T : Attribute
        {
            foreach ( PropertyInfo propertyInfo in typeInfo.DeclaredProperties )
            {
                if ( null != propertyInfo.GetCustomAttribute<T>( ) )
                {
                    yield return propertyInfo;
                }
            }
        }

        private bool IsCommand( string command )
        {
            return SwitchIdentifier == command[0];
        }

        private string CleanCommand( string command )
        {
            return command.TrimStart( SwitchIdentifier ).Trim( ).ToLower( );
        }
    }

    internal static class TypeInfoExtensions
    {
        internal static bool HasCustomAttribute<T>( this TypeInfo typeInfo ) where T : Attribute
        {
            return null != typeInfo.GetCustomAttribute<T>( );
        }
    }
}