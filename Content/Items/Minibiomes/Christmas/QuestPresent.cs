using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
	public class QuestPresent1 : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
			Item.consumable = true;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.rare = ItemRarityID.Quest;
		}

		public override bool? UseItem(Player player)
		{
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.friendly && npc.townNPC && npc.Distance(player.Center) <= 200f)
				{
					if (!Flags.KrampusQuest1)
					{
						Flags.KrampusQuest1 = true;
					}
					else if (Flags.KrampusQuest1 && !Flags.KrampusQuest2)
					{
						Flags.KrampusQuest2 = true;
					}
					else if (Flags.KrampusQuest2 && !Flags.KrampusQuest3)
					{
						Flags.KrampusQuest3 = true;
					}
					else if (Flags.KrampusQuest3 && !Flags.KrampusQuest4)
					{
						Flags.KrampusQuest4 = true;
					}
					else if (Flags.KrampusQuest4 && !Flags.KrampusQuest5)
					{
						Flags.KrampusQuest5 = true;
					}

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					return true;
				}
			}

			return false;
		}
	}

	public class QuestPresent2 : QuestPresent1
	{
	}

	public class QuestPresent3 : QuestPresent1
	{
	}
}