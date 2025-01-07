using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 42;
			Item.mana = 8;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
            Item.width = 70;
            Item.height = 20;
            Item.useTime = 4;         
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<SpookFishronGunFire>();
			Item.shootSpeed = 8f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-22, 0);
		}
    }
}