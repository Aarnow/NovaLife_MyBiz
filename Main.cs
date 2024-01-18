using Life;
using Life.UI;
using System;
using UnityEngine;
using MyMenu.Entities;
using MyBiz.Panels;

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
            try
            {
                Section playerSection = new Section(Section.GetSourceName(), Section.GetSourceName(), "v1.0.0", "Aarnow");
                Action<UIPanel> playerAction = ui => PlayerPanels.Open(playerSection.GetPlayer(ui));
                playerSection.OnlyAdmin = false;
                playerSection.Line = new UITabLine(playerSection.Title, playerAction);
                playerSection.Insert(false);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            Debug.Log($"Plugin \"MyBiz\" initialisé avec succès.");
        }
    }
}