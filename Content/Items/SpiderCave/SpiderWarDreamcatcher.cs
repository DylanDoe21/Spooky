using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SpiderWarDreamcatcher : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 135;
            Item.mana = 25;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
            Item.UseSound = SoundID.Item76 with { Pitch = -1f };
            Item.shoot = ModContent.ProjectileType<SpiderWarDreamcatcherWeb>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            for (int numProjs = 0; numProjs < 3; numProjs++)
            {
                Vector2 ProjectilePosition = position + new Vector2(120, 0).RotatedByRandom(360);

                Projectile.NewProjectile(source, ProjectilePosition, velocity, type, damage, knockback, player.whoAmI);
            }

			return false;
        }
    }
}