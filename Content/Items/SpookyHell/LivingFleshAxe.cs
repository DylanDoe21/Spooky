using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshAxe : SwingWeaponBase
    {
        public override int Length => 90;
		public override int TopSize => 35;
		public override float SwingDownSpeed => 15f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;
        static bool hasHitEnemies = false;

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.crit = 10;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 12;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.25f;
        }

        public override void UseAnimation(Player player)
        {
            hasHitSomething = false;
            hasHitEnemies = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 8;

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, player.Center);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasHitEnemies)
            {
                hasHitEnemies = true;

                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<FleshAxeHitLiving>(), Item.damage, Item.knockBack, Main.myPlayer, 0, 0);
            }

            if (target.life <= target.lifeMax * 0.5)
            {
                target.takenDamageMultiplier = 1.65f;
            }

            if (hit.Crit)
            {
                target.AddBuff(ModContent.BuffType<LivingAxeBleed>(), 180);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshAxe>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}