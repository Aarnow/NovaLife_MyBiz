using Life.DB;
using Life;
using System.Linq;
using UIPanelManager;
using Life.UI;
using Life.Network;
using UnityEngine;
using System.Collections.Generic;

namespace MyBiz.Panels
{
    abstract class PlayerPanels
    {
        public static void Open(Player player)
        {
            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.TabPrice).SetTitle("Répertoire des sociétés");

            foreach (Bizs biz in Nova.biz.bizs)
            {
                bool isOpen = false;
                List<Player> players = Nova.server.GetAllInGamePlayers().Where(p => p.biz.Id == biz.Id).ToList();
                foreach (Player currPlayer in players)
                {
                    if (currPlayer.serviceMetier)
                    {
                        isOpen = true;
                        break;
                    }
                }
                panel.AddTabLine($"{biz.BizName}", $"{(!isOpen ? $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>fermé</color>" : $"<color={PanelManager.Colors[NotificationManager.Type.Success]}>ouvert</color>")}", -1, ui => Debug.Log("open biz"));
            }

            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => MyMenu.Panels.PlayerPanels.OpenMenuPanel(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }
    }
}
