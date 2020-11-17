#region

using System;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            bool backupMostRecentSave = false;
            if ( 1 <= args.Length )
            {
                backupMostRecentSave = string.Equals( args[0], "--most-recent",
                    StringComparison.OrdinalIgnoreCase );
            }

            Saver saver = new Saver( backupMostRecentSave );
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