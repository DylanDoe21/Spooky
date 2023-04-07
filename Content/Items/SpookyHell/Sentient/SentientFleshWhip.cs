using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientFleshWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 52;
            Item.height = 50;
			Item.useTime = 21;
			Item.useAnimation = 42;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 4f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            int ProjToShoot = 0;
            if (ProjToShoot == 0)
            {
                type = ModContent.ProjectileType<SentientFleshWhipProj1>();
                ProjToShoot = 1;
            }
            if (ProjToShoot == 1)
            {
                type = ModContent.ProjectileType<SentientFleshWhipProj2>();
                ProjToShoot = 0;
            }
			
			return true;
        }
    }
}