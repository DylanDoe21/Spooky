using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Biomes;

namespace Spooky.Content.Items.Catacomb
{
    public class RustyRing : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 60;
            player.lifeRegen += 1;

            if (player.ZoneDungeon || player.InModBiome(ModContent.GetInstance<CatacombBiome>()) || player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
            {
				player.findTreasure = true;
			}
        }
    }
}