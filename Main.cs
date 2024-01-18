using Life;
using Life.UI;
using System;
using UnityEngine;
using MyMenu.Entities;
using Life.Network;
using UIPanelManager;
using Life.DB;
using System.Collections.Generic;
using System.Linq;

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
                Action<UIPanel> playerAction = ui => Open(playerSection.GetPlayer(ui));
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

        public void Open(Player player)
        {
            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.TabPrice).SetTitle("Répertoire des sociétés");

            foreach(Bizs biz in Nova.biz.bizs)
            {
                bool isOpen = false;
                List<Player> players = Nova.server.GetAllInGamePlayers().Where(p => p.biz.Id == biz.Id).ToList();
                foreach(Player currPlayer in players)
                {
                    if (currPlayer.serviceMetier)
                    {
                        isOpen = true;
                        break;
                    }
                }

                panel.AddTabLine($"{biz.BizName}", $"{(!isOpen ? $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>fermé</color>": $"<color={PanelManager.Colors[NotificationManager.Type.Success]}>ouvert</color>")}", -1, ui => Debug.Log("open biz"));
            }

            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }
    }
}