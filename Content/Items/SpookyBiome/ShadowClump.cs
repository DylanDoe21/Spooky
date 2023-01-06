using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using System.Linq;

namespace Spooky.Content.Items.SpookyBiome
{
    public class ShadowClump : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Clump");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.buyPrice(silver: 5);
            Item.maxStack = 999;
        }
    }
}