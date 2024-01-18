using Life;
using UnityEngine;

namespace MyBiz
{
    public class Main : Plugin
    {
        public Main(IGameAPI api): base(api)
        {

        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            //MyMenu

            Debug.Log($"Plugin \"MyBiz\" initialisé avec succès.");
        }
    }
}