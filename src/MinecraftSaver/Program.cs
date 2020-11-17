#region

using System;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( string[] args )
        {
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