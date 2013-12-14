// -----------------------------------------------------------------------
// <copyright file="PlatformMap.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.IO;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Persistable physical entities of a platformer level
    /// </summary>
    public class PlatformMap
    {
        #region Constructor      
        private PlatformMap()
        {
        }

        public PlatformMap(Int16 version, string name, Platform[,] platforms, Int16 wide, Int16 high) // TODO make this private
        {
            this.Version = version;
            this.Name = name;
            this.Platforms = platforms;
            this.PlatformsWide = wide;
            this.PlatformsHigh = high;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Map version, will come in handy if this class gets changed
        /// </summary>
        public Int16 Version { get; private set; }

        /// <summary>
        /// Name of level map is associated with
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Platforms which will be drawn
        /// </summary>
        public Platform[,] Platforms { get; set; } // TODO currently need to inject

        /// <summary>
        /// First dimension of Platforms
        /// </summary>
        public Int16 PlatformsWide { get; private set; }

        /// <summary>
        /// Second dimension of Platforms
        /// </summary>
        public Int16 PlatformsHigh { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load map from binary file
        /// </summary>
        /// <param name="inFile">name of file to read</param>
        public static PlatformMap Load(string inFile, ContentManager content)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open)))
            {
                Int16 version = reader.ReadInt16();
                string levelName = reader.ReadString();
                Int16 platformsWide = reader.ReadInt16();
                Int16 platformsHigh = reader.ReadInt16();

                Platform[,] platformArray = new Platform[platformsWide, platformsHigh];

                for (int i = 0; i < platformsWide; i++)
                {
                    for (int j = 0; j < platformsHigh; j++)
                    {
                        platformArray[i, j] = Platform.Load(reader);
                        platformArray[i, j].LoadContent(content);
                    }
                }

                PlatformMap newMap = new PlatformMap(version, levelName, platformArray, platformsWide, platformsHigh);
                PlatformerLevel.Log.Info("Loaded from {0}, {1}", inFile, newMap);
                return newMap;
            }
        }

        /// <summary>
        /// Write map to binary file
        /// </summary>
        /// <param name="outFile">name of file to write</param>
        public void Save(string outFile)
        {
            // Make a backup in case we corrupt the file
            if (File.Exists(outFile))
            {
                File.Copy(outFile, outFile + ".bak", true);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                writer.Write(this.Version);
                writer.Write(this.Name);
                writer.Write(this.PlatformsWide);
                writer.Write(this.PlatformsHigh);

                for (int i = 0; i < this.PlatformsWide; i++)
                {
                    for (int j = 0; j < this.PlatformsHigh; j++)
                    {
                        this.Platforms[i, j].Save(writer);
                    }
                }
            }

            PlatformerLevel.Log.Info("Saved to {0}, {1}", outFile, this.ToString());
        }

        public override string ToString()
        {
            return string.Format("PlatformMap = {0}, Version = {1}, Size = {2} x {3}", this.Name, this.Version, this.PlatformsWide, this.PlatformsHigh);
        }
        #endregion
    }
}
