#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineUtils.Attributes;

#endregion

namespace CommandLineUtils.Parsing
{
    public class Parser
    {
        private const char SwitchIdentifier = '-';

        private readonly IDictionary<string, PropertyInfo> _commandToPropertyInfoMap;
        private readonly string[] _args;

        [MethodImpl( MethodImplOptions.NoInlining )]
        public Parser( string[] args )
        {
            _args = args;
            _commandToPropertyInfoMap = InitializeMap( Assembly.GetCallingAssembly( ) );
        }

        public T Parse<T>( ) where T : new( )
        {
            T instance = new T( );

            foreach ( string arg in _args )
            {
                if ( SwitchIdentifier == arg[0] )
                {
                    string command = CleanCommand( arg );
                    if ( _commandToPropertyInfoMap.TryGetValue( command,
                        out PropertyInfo propertyInfo ) )
                    {
                        //TODO Is there a more elegant way of doing this?
                        if ( typeof(bool) == propertyInfo.PropertyType )
                        {
                            propertyInfo.SetValue( instance, true );
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

            return instance;
        }

        private IDictionary<string, PropertyInfo> InitializeMap( Assembly assembly )
        {
            IDictionary<string, PropertyInfo> commandToPropertyInfoMap =
                new Dictionary<string, PropertyInfo>( );

            foreach ( TypeInfo typeInfo in
                GetClassesWithAttribute<CommandLineInterface>( assembly ) )
            {
                foreach ( PropertyInfo propertyInfo in
                    GetPropertiesWithAttribute<CommandLineSwitch>( typeInfo ) )
                {
                    CommandLineSwitch commandLineSwitch =
                        propertyInfo.GetCustomAttribute<CommandLineSwitch>( );

                    commandToPropertyInfoMap.Add( commandLineSwitch.Name.ToLower( ), propertyInfo );
                    commandToPropertyInfoMap.Add( commandLineSwitch.ShortName.ToLower( ),
                        propertyInfo );
                }
            }

            return commandToPropertyInfoMap;
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