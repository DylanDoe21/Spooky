using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.NPCs.Friendly
{
    public class LittleBonePot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Bone's Flower Pot");
            Tooltip.SetDefault("A magical flower pot that belongs to little bone"
            + "\nUse to summon little bone to your location");
        }

        public override void SetDefaults()
        {
            Item.consumable = false; 
            Item.noUseGraphic = true;
            Item.width = 30;
			Item.height = 24;
			Item.useTime = 35;
			Item.useAnimation = 35;
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