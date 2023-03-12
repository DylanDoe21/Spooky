using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class LittleSpiderItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Spider");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BlackDragonfly);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<LittleSpider>();
            Item.bait = 10;
        }
    }
}