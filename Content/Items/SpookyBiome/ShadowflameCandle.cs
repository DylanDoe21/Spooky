using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
    public class ShadowflameCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Candle");
            Tooltip.SetDefault("Increases magic damage and critical strike chance by 12%"
            + "\nMagic weapons will sometimes shoot out shadowflame tentacles on use"
            + "\nMagic projectiles will inflict shadowflame on enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
			Item.value = Item.buyPrice(gold: 15);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.GetCritChance(DamageClass.Magic) += 12;
            //player.GetModPlayer<SpookyPlayer>().ShadowflameCandle = true;
        }
    }
}