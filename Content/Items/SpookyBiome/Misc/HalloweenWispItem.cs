using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class HalloweenWispItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halloween Wisp");
            Tooltip.SetDefault("'It glows with spooky light'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 8));
			ItemID.Sets.ItemNoGravity[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 24;
            Item.useTime = 7;
			Item.useAnimation = 15; 
            Item.value = Item.buyPrice(silver: 1);
			Item.makeNPC = (short)ModContent.NPCType<HalloweenWisp>();
            Item.rare = 1; 
            Item.maxStack = 999;
        }
    }
}