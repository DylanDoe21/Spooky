using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class OrroboroEmbryo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Undying Embryo");
            Tooltip.SetDefault("Upon death, you will be revived back to 1 health and given embryotic revival"
            + "\nThe embryotic revival buff gives increased regeneration and defense"
            + "\nThe revival ability has a 10 minute cool down before it can activate again"
            + "\nDuring the cool down, you will gain 8% increased damage and critical strike chance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().OrroboroEmbyro = true;
        }
    }
}