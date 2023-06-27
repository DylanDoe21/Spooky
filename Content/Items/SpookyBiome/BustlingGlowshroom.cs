using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class BustlingGlowshroom : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().BustlingGlowshroom = true;
        }
    }
}