using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class EyeGoblinTongue : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
			Item.height = 34;
            Item.rare = ItemRarityID.Quest;
        }
    }
}