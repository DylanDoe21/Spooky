using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class FlySmallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fly");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BlackDragonfly);
            Item.rare = ItemRarityID.Blue;
			Item.makeNPC = (short)ModContent.NPCType<FlySmall>();
            Item.bait = 10;
        }
    }
}