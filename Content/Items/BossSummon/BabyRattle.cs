using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Hallucinations;
 
namespace Spooky.Content.Items.BossSummon
{
	public class BabyRattle : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.crit = 5;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
			Item.width = 64;          
			Item.height = 60;         
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.scale = 1.2f;
		}

		public override bool CanUseItem(Player player)
        {
            if (!Flags.encounteredBaby && !NPC.AnyNPCs(ModContent.NPCType<TheBaby>()))
            {
                return true;
            }

			if (Flags.encounteredBaby)
			{
				return true;
			}

            return false;
        }

		public override bool? UseItem(Player player)
        {
            if (!Flags.encounteredBaby)
            {
                int type = ModContent.NPCType<TheBaby>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else 
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }
	}
}
