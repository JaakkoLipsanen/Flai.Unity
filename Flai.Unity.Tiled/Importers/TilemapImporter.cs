using Flai.Diagnostics;
using Flai.Editor.Importers;
using Flai.Tilemap.Serializing;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Flai.Unity.Tiled.Importers
{
    public class TilemapImporter : AssetPostprocessor
    {
        protected static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            if (importedAssets == null)
            {
                return;
            }

            foreach (string tilemapAsset in importedAssets.Where(asset => asset != null && asset.EndsWith(".tmx")))
            {
                if (tilemapAsset == null)
                {
                    continue;  
                }

                const string TilemapPath = "Assets/Tile Maps/Maps/";
                string fileName = Path.GetFileNameWithoutExtension(tilemapAsset);
                string finalPath = Path.Combine(TilemapPath, fileName) + ".asset";

                // ensure that the directory exists
                Directory.CreateDirectory(TilemapPath);

                TmxAsset tmxAsset = TilemapLoader.LoadTilemap(tilemapAsset);
                TmxAsset previous = (TmxAsset)AssetDatabase.LoadAssetAtPath(finalPath, typeof(TmxAsset));
                if (previous != null)
                {
                    previous.CopyFrom(tmxAsset);
                    FlaiDebug.LogWithTypeTag<TilemapImporter>("Tilemap {0} updated!", fileName);
                    EditorUtility.SetDirty(previous);
                }
                else
                {
                    AssetDatabase.CreateAsset(tmxAsset, finalPath);
                    EditorUtility.SetDirty(tmxAsset);
                    FlaiDebug.LogWithTypeTag<TilemapImporter>("Tilemap {0} imported succesfully!", fileName);
                }

                TilemapImporter.SaveTilemapFile(tilemapAsset);
                AssetDatabase.DeleteAsset(tilemapAsset);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void SaveTilemapFile(string tilemapPath)
        {
            const string FolderName = "Saved .TMXs/TMX";
            const string TempFolderName = "Temp";

            string projectRootFolder = Directory.GetParent(Application.dataPath).FullName;
            string targetFolderPath = Path.Combine(projectRootFolder, FolderName);
            Directory.CreateDirectory(targetFolderPath);

            string tilemapName = Path.GetFileName(tilemapPath);
            string newFilePath = Path.Combine(targetFolderPath, tilemapName);
            if (File.Exists(newFilePath))
            {
                string tempFolder = Path.Combine(targetFolderPath, TempFolderName);
                Directory.CreateDirectory(tempFolder);

                for (int i = 1; ; i++)
                {
                    string newTempFilePath = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(tilemapName) + "_" + i + Path.GetExtension(tilemapName));
                    if (!File.Exists(newTempFilePath))
                    {
                        File.Move(newFilePath, newTempFilePath);
                        break;
                    }
                }
            }

            // copy the tilemap file
            File.Copy(tilemapPath, newFilePath, true);
        }
    }
}
