using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryChanger.Model
{

    public sealed class BundleResolveResult
    {
        public string FolderPath;        // the AssetBundles folder we found
        public string BundlePath;        // full path to the bundle we’ll load
        public string BundleFileName;    // just the file name we matched
        public bool Exists;            // did we find one?
        public AssetInformation Info;    // null if AssetInformation.json not found/parse failed
    }
}
