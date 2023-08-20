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
		public override float SwingDownSpeed => 12f;
		public override bool CollideWithTiles => true;
        static bool hasHitGround = false;

        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 54;
            Item.height = 48;
            Item.useTime = 42;
			Item.useAnimation = 42;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 8;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.3f;
        }

        public override void UseAnimation(Player player)
        {
            hasHitGround = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitGround)
            {
                hasHitGround = true;

                SpookyPlayer.ScreenShakeAmount = 3;

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, player.Center);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= target.lifeMax * 0.35)
            {
                target.takenDamageMultiplier = 1.35f;
            }
        }
    }
}