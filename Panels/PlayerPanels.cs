using Life.DB;
using Life;
using System.Linq;
using UIPanelManager;
using Life.UI;
using Life.Network;
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
                panel.AddTabLine($"{biz.BizName}", $"{(!isOpen ? $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>fermé</color>" : $"<color={PanelManager.Colors[NotificationManager.Type.Success]}>ouvert</color>")}", -1, ui => PanelManager.NextPanel(player, ui, () => BizDetails(player, biz)));
            }

            panel.AddButton("Voir", ui => ui.SelectTab()) ;
            if (Utils.PlayerIsBizOwner(player))
            {
                panel.AddButton("Configurer", ui =>PanelManager.NextPanel(player, ui, () => BizSetup(player)));   
            }
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => MyMenu.Panels.PlayerPanels.OpenMenuPanel(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public async static void BizDetails(Player player, Bizs biz)
        {
            Characters owner = await LifeDB.FetchCharacter(biz.OwnerId);
            
            int employeesCount = await LifeDB.db.Table<Characters>()
               .Where(c => c.BizId == biz.Id)
               .CountAsync();

            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Tab).SetTitle($"{biz.BizName}");

            panel.AddTabLine($"Nom: {biz.BizName}", ui => ui.selectedTab = 0);
            panel.AddTabLine($"PDG: {(owner != null ? $"{owner.Firstname} {owner.Lastname}" : "Aucun")}", ui => ui.selectedTab = 1);
            panel.AddTabLine($"Recrutement: {(biz.IsRecruiting ? $"<color={PanelManager.Colors[NotificationManager.Type.Success]}>ouvert</color>" : $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>fermé</color>")}", ui => ui.selectedTab = 2);
            panel.AddTabLine($"Salariés: {employeesCount}", ui => ui.selectedTab = 3);

            panel.AddButton("En savoir plus", ui => PanelManager.NextPanel(player, ui, () => BizDescription(player, biz)));
            panel.AddButton("Contacter", ui => PanelManager.NextPanel(player, ui, () => BizContact(player, biz)));
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => Open(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public static void BizDescription(Player player, Bizs biz)
        {
            string description = "Aucune description";
            if (biz.Description != null)
                if(biz.Description.Length > 0) description = biz.Description;

            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Text).SetTitle($"{biz.BizName}");
            
            panel.text = $"{description}";

            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => BizDetails(player, biz)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public static void BizContact(Player player, Bizs biz)
        {
            string contact = "Aucune informations";
            if (biz.Contact != null)
                if (biz.Contact.Length > 0) contact = biz.Contact;

            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Text).SetTitle($"{biz.BizName}");

            panel.text = $"{contact}";


            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => BizDetails(player, biz)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public static void BizSetup(Player player)
        {
            Bizs biz = Nova.biz.FetchBiz(player.character.BizId);
            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Tab).SetTitle("Configuration société");

            panel.AddTabLine($"Description", ui => PanelManager.NextPanel(player, ui, () => EditBizDescription(player, biz)));
            panel.AddTabLine($"Contact", ui => PanelManager.NextPanel(player, ui, () => EditBizContact(player, biz)));
            panel.AddTabLine($"Recrutement: {(biz.IsRecruiting ? $"<color={PanelManager.Colors[NotificationManager.Type.Success]}>ouvert</color>" : $"<color={PanelManager.Colors[NotificationManager.Type.Error]}>fermé</color>")}", ui =>
            {
                biz.IsRecruiting = !biz.IsRecruiting;
                biz.Save();
                PanelManager.NextPanel(player, ui, () => BizSetup(player));
            });


            panel.AddButton("Modifier", ui => ui.SelectTab());
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => Open(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public static void EditBizDescription(Player player, Bizs biz)
        {
            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Input).SetTitle("Configuration société");

            panel.inputPlaceholder = "Description de votre société";

            panel.AddButton("Valider", ui =>
            {
                biz.Description = ui.inputText;
                biz.Save();
                PanelManager.NextPanel(player, ui, () => BizSetup(player));
            });
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => BizSetup(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }

        public static void EditBizContact(Player player, Bizs biz)
        {
            UIPanel panel = new UIPanel("MyBiz", UIPanel.PanelType.Tab).SetTitle("Configuration société");

            panel.inputPlaceholder = "Informations de contact";

            panel.AddButton("Valider", ui =>
            {
                biz.Contact = ui.inputText;
                biz.Save();
                PanelManager.NextPanel(player, ui, () => BizSetup(player));
            });
            panel.AddButton("Retour", ui => PanelManager.NextPanel(player, ui, () => BizSetup(player)));
            panel.AddButton("Fermer", ui => PanelManager.Quit(ui, player));

            player.ShowPanelUI(panel);
        }
    }
}
