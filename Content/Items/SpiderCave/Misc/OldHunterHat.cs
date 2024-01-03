using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpiderCave.Misc
{
    public class OldHunterHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 14;
            Item.rare = ItemRarityID.Quest;
        }
    }
}