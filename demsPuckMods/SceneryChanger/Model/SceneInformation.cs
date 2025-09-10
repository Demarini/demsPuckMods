using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryChanger.Model
{
    public class SceneInformation
    {
        public string bundleName;   // e.g. "rinkbundle"
        public string prefabName;   // e.g. "RinkRoot"
        public string skyboxName;   // e.g. "NightSkybox"
        public bool useSceneLocally;
        public string contentKey64;
    }
}