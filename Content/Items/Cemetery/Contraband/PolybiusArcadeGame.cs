using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class PolybiusArcadeGame : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().PolybiusArcadeGame = true;

            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PolybiusSwirl>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 0f, ModContent.ProjectileType<PolybiusSwirl>(), 25, 0f, player.whoAmI);
			}
        }
    }
}