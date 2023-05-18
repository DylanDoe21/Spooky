using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshAxe : SwingWeaponBase
    {
        public override int Length => 50;
		public override int TopSize => 25;
		public override float SwingDownSpeed => 13f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;
        static bool hasHitEnemies = false;

        public override void SetDefaults()
        {
            Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 54;
            Item.height = 50;
            Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 8;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.3f;
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

                SpookyPlayer.ScreenShakeAmount = 5;

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, player.Center);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasHitEnemies)
            {
                hasHitEnemies = true;

                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<FleshAxeHit>(), Item.damage, Item.knockBack, Main.myPlayer, 0, 0);
            }

            if (target.life <= target.lifeMax * 0.35)
            {
                target.takenDamageMultiplier = 1.2f;
            }
        }
    }
}