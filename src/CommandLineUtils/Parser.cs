#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLineUtils.Attributes;

#endregion

namespace CommandLineUtils
{
    public class Parser
    {
        [MethodImpl( MethodImplOptions.NoInlining )]
        public void Parse( )
        {
            Assembly caller = Assembly.GetCallingAssembly( );

            foreach ( TypeInfo typeInfo in GetClassesWithAttribute<CommandLineInterface>( caller ) )
            {
                Console.WriteLine( typeInfo.FullName );

                foreach ( PropertyInfo propertyInfo in
                    GetPropertiesWithAttribute<CommandLineSwitch>( typeInfo ) )
                {
                    Console.WriteLine( propertyInfo.Name );
                }
            }
        }

        private IEnumerable<TypeInfo> GetClassesWithAttribute<T>( Assembly assembly )
            where T : Attribute
        {
            foreach ( TypeInfo typeInfo in assembly.DefinedTypes )
            {
                if ( null != typeInfo.GetCustomAttribute<T>( ) )
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
    }
}