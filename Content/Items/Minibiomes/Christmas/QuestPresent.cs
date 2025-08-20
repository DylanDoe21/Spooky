using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
	public class QuestPresent1 : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
			Item.consumable = true;
            Item.noUseGraphic = true;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Quest;
		}

		public override bool CanUseItem(Player player)
        {
			if (Flags.KrampusQuestGiven)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.friendly && npc.townNPC && (!npc.immortal && !npc.dontTakeDamage) && npc.Distance(player.Center) <= 100f)
					{
						return true;
					}
				}
			}

			return false;
		}

		public override bool? UseItem(Player player)
        {
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.friendly && npc.townNPC && (!npc.immortal && !npc.dontTakeDamage) && npc.Distance(player.Center) <= 100f)
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

					Flags.KrampusQuestGiven = false;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					int frame = 0;
					if (Type == ModContent.ItemType<QuestPresent2>())
					{
						frame = 1;
					}
					if (Type == ModContent.ItemType<QuestPresent3>())
					{
						frame = 2;
					}

					int Offset = npc.direction == -1 ? -15 : 15;

					if (player.ownedProjectileCounts[ModContent.ProjectileType<QuestPresentSpawner>()] < 1)
					{
						Projectile.NewProjectile(null, npc.Center + new Vector2(Offset, 5), Vector2.Zero,
						ModContent.ProjectileType<QuestPresentSpawner>(), 0, 0f, player.whoAmI, ai1: frame, ai2: npc.whoAmI);
					}
				}
			}

			return true;
		}
	}

	public class QuestPresent2 : QuestPresent1
	{
	}

	public class QuestPresent3 : QuestPresent1
	{
	}

	public class QuestPresentLittleEye : QuestPresent1
	{
		public override bool CanUseItem(Player player)
        {
			if (Flags.KrampusQuestGiven)
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<LittleEye>() && npc.Distance(player.Center) <= 100f)
					{
						return true;
					}
				}
			}

			return false;
		}

		public override bool? UseItem(Player player)
        {
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<LittleEye>() && npc.Distance(player.Center) <= 100f)
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

					Flags.KrampusQuestGiven = false;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					int frame = 3;

					if (player.ownedProjectileCounts[ModContent.ProjectileType<QuestPresentSpawner>()] < 1)
					{
						Projectile.NewProjectile(null, npc.Center + new Vector2(35, 5), Vector2.Zero,
						ModContent.ProjectileType<QuestPresentSpawner>(), 0, 0f, player.whoAmI, ai1: frame, ai2: npc.whoAmI);
					}
				}
			}

			return true;
		}
	}
}