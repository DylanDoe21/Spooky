using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpiderCave.Misc
{
    public class OldHunterSkull : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.rare = ItemRarityID.Quest;
        }
    }
}