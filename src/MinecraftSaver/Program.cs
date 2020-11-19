#region

using System;
using CommandLineUtils.Parsing;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            Parser parser = new Parser( args );
            Config config = new Config( );

            if ( !parser.Parse( ref config ) )
            {
                parser.FlushErrorMessages( );
                return;
            }

            Saver saver = new Saver( config );

            try
            {
                saver.CreateBackup( );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error during backup creation: {ex.Message}" );
            }
        }
    }
}