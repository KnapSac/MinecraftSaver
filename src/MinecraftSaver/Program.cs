#region

using System;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            //TODO Rewrite switch handling, shouldn't be dependent on the order in which we receive them
            bool backupMostRecentSave = false;
            bool allowOverwrite = false;
            if ( 1 <= args.Length )
            {
                backupMostRecentSave = string.Equals( args[0], "--most-recent",
                    StringComparison.OrdinalIgnoreCase );
            }

            if ( 2 <= args.Length )
            {
                allowOverwrite = string.Equals( args[1], "--force",
                    StringComparison.OrdinalIgnoreCase );
            }

            Saver saver = new Saver( backupMostRecentSave, allowOverwrite );
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