#region

using System;

#endregion

namespace MinecraftSaver
{
    internal class Program
    {
        private static void Main( )
        {
            Saver saver = new Saver( );
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