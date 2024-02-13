using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class PumpkinCarvingKit : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
			Item.height = 46;
            Item.rare = ItemRarityID.Blue;
        }
    }
}