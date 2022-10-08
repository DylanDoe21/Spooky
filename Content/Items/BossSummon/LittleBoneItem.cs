using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Items.BossSummon
{
    public class LittleBoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Bone");
            Tooltip.SetDefault("Hey it's me, I'm in your inventory!"
            + "\nJust release me whenever you need help or if you want to talk!");
        }

        public override void SetDefaults()
        {
            Item.consumable = true; 
            Item.noUseGraphic = true;
            Item.favorited = true;
            Item.width = 26;
			Item.height = 32;
			Item.useTime = 30;
			Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange = 1000000;
        }

        public override bool? UseItem(Player player)
        {
            //delete any existing little bones just in case
            for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<LittleBone>()) 
				{
                    Main.npc[k].active = false;
                }
            }

            //spawn little bone
            NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y - 25, ModContent.NPCType<LittleBone>());

            return true;
        }
    }
}