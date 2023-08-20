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
		public override float SwingDownSpeed => 12f;
		public override bool CollideWithTiles => true;
        static bool hasHitGround = false;

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
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.25f;
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

                SpookyPlayer.ScreenShakeAmount = 5;

                //play both sounds on top of each other because its cool
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, player.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath21, player.Center);

                for (int numProjectiles = 0; numProjectiles < 10; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X + (player.direction == 1 ? 120 + (Item.scale * 2) : -120 + (-Item.scale * 2)), 
                        player.Center.Y, Main.rand.Next(-5, 6), Main.rand.Next(-15, -10), ModContent.ProjectileType<LivingFleshAxeEye>(), Item.damage, 2f, Main.myPlayer);
                    }
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= target.lifeMax * 0.5)
            {
                target.takenDamageMultiplier = 1.65f;
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