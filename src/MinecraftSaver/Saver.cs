#region

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

#endregion

namespace MinecraftSaver
{
    internal class Saver
    {
        private const string PersonalOneDriveEnvironmentVariable = "OneDriveConsumer";
        private const string AppDataEnvironmentVariable = "AppData";
        private const int SelectMostRecentCount = 5;

        internal void CreateBackup( )
        {
            DirectoryInfo oneDriveBackupDirectory = GetOneDriveBackupDirectory( );
            DirectoryInfo saveToBackup = SelectSaveToBackup( );
            string backupFileName =
                $"{DateTime.Now:yyyyMMdd} Backup {saveToBackup.Name}.zip";
            string backupFullFileName =
                Path.Combine( oneDriveBackupDirectory.FullName, backupFileName );

            if ( File.Exists( backupFullFileName ) )
            {
                FileInfo previousBackup = new FileInfo( backupFullFileName );
                Console.WriteLine(
                    $"A backup was already created today at {previousBackup.LastWriteTimeUtc.ToLocalTime( ).ToLongTimeString( )}, do you want to overwrite this previous backup? (y)es/(n)o" );
                WritePrompt( );
                string overwriteInput = Console.ReadLine( );
                if ( string.Equals( overwriteInput, "y", StringComparison.OrdinalIgnoreCase ) ||
                     string.Equals( overwriteInput, "yes", StringComparison.OrdinalIgnoreCase ) )
                {
                    File.Delete( backupFullFileName );
                    CompressSave( saveToBackup, backupFullFileName );
                }
            }
            else
            {
                CompressSave( saveToBackup, backupFullFileName );
            }
        }

        private DirectoryInfo GetOneDriveBackupDirectory( )
        {
            string oneDriveDirectory =
                Environment.GetEnvironmentVariable( PersonalOneDriveEnvironmentVariable );
            if ( string.IsNullOrEmpty( oneDriveDirectory ) )
            {
                throw new ArgumentException(
                    "Unable to locate your personal OneDrive directory" );
            }

            return Directory.CreateDirectory( Path.Combine(
                oneDriveDirectory, "MinecraftSaver",
                "Backups" ) );
        }

        private IEnumerable<DirectoryInfo> GetMostRecentSaves( )
        {
            string appDataDirectory =
                Environment.GetEnvironmentVariable( AppDataEnvironmentVariable );
            if ( string.IsNullOrEmpty( appDataDirectory ) )
            {
                throw new ArgumentException(
                    "Unable to locate your AppData directory" );
            }

            DirectoryInfo minecraftSavesDirectory =
                new DirectoryInfo( Path.Combine( appDataDirectory, ".minecraft", "saves" ) );
            IEnumerable<DirectoryInfo> saves = minecraftSavesDirectory.EnumerateDirectories( );
            return saves.OrderByDescending( save => save.LastWriteTimeUtc )
                        .Take( SelectMostRecentCount );
        }

        private DirectoryInfo SelectSaveToBackup( )
        {
            IDictionary<int, DirectoryInfo> indexedSaves = new Dictionary<int, DirectoryInfo>( );
            int index = 0;
            int maxSaveNameLength = 0;
            foreach ( DirectoryInfo save in GetMostRecentSaves( ) )
            {
                indexedSaves.Add( ++index, save );
                maxSaveNameLength = Math.Max( maxSaveNameLength, save.Name.Length );
            }

            foreach ( KeyValuePair<int, DirectoryInfo> pair in indexedSaves )
            {
                Console.WriteLine(
                    $"{pair.Key}. '{pair.Value.Name}'{new string( ' ', maxSaveNameLength - pair.Value.Name.Length )} {pair.Value.LastWriteTimeUtc}" );
            }

            WritePrompt( );
            string indexInput = Console.ReadLine( );
            int selectedIndex;
            while ( !int.TryParse( indexInput, out selectedIndex ) || selectedIndex <= 0 ||
                    selectedIndex > SelectMostRecentCount )
            {
                Console.WriteLine( "Please enter the index of the save you want to backup" );
                WritePrompt( );
                indexInput = Console.ReadLine( );
            }

            return indexedSaves.TryGetValue( selectedIndex, out DirectoryInfo selectedSave )
                ? selectedSave
                : null;
        }

        private void CompressSave( DirectoryInfo saveToBackup, string backupFileName )
        {
            Console.WriteLine( "Creating backup..." );
            ZipFile.CreateFromDirectory( saveToBackup.FullName, backupFileName );
            Console.WriteLine( $"Created backup '{backupFileName}'" );
        }

        private void WritePrompt( )
        {
            Console.Write( "> " );
        }
    }
}