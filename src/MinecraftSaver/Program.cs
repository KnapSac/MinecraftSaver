#region

using System;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            CommandLineUtils.Parser p = new CommandLineUtils.Parser( );
            p.Parse( );

            Console.ReadKey( );
            return;

            Parser parser = new Parser( args );
            if ( parser.TryDisplayHelpMenu( ) )
            {
                return;
            }

            Saver saver = new Saver( parser.CreateConfig( ) );

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