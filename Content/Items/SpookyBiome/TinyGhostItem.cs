using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class TinyGhostItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Bunny);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<TinyGhost1>();
        }
    }
}