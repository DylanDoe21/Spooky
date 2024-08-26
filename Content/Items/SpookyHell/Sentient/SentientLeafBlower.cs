using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientLeafBlower : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 60;
			Item.height = 40;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<SentientLeafBlowerProj>();
			Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SentientLeafBlowerProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SentientLeafBlowerProj>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}