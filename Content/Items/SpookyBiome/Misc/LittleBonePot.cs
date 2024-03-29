using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class LittleBonePot : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
			Item.height = 24;
			Item.useTime = 35;
			Item.useAnimation = 35;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Quest;
            Item.UseSound = SoundID.Item83;
        }

        public override bool? UseItem(Player player)
        {
            //delete any existing little bones just in case
            for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && (Main.npc[k].type == ModContent.NPCType<LittleBone>() ||
                Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<LittleBoneSleeping>() ||
                Main.npc[k].type == ModContent.NPCType<LittleBonePotTransform>()))
				{
                    Main.npc[k].active = false;
                }
            }

            //spawn little bone
            NPC.NewNPC(player.GetSource_ItemUse(player.HeldItem), (int)player.Center.X, (int)player.Center.Y - 25, ModContent.NPCType<LittleBonePotTransform>());

            return true;
        }
    }
}