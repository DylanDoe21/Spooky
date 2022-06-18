using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CreepyCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Creepy Candle");
            Tooltip.SetDefault("Increases magic damage by 5%"
            + "\nMagic projectiles will sometimes ignite enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 24;
            Item.height = 36;
            Item.rare = 1;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.GetModPlayer<SpookyPlayer>().MagicCandle = true;
        }
    }
}