using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Blooms;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Core
{
    public class ItemGlobal : GlobalItem
    {
        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Volume = 0.75f, Pitch = 0.9f };

        public static Item ActiveItem(Player player) => Main.mouseItem.IsAir ? player.HeldItem : Main.mouseItem;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }

            //manually handle daffodils music box recording if any of her themes are playing, since music boxes cant be assigned more than one song
            if (item.type == ItemID.MusicBox && Main.rand.NextBool(540) && (Main.curMusic == MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro1") || Main.curMusic == MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro2")))
            {
                SoundEngine.PlaySound(SoundID.Item166, player.Center);
                item.ChangeItemType(ModContent.ItemType<DaffodilBox>());
            }

            base.UpdateAccessory(item, player, hideVisual);
        }

		public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
		{
			//halve crit chance with the poker pineapple bloom
			if (player.GetModPlayer<BloomBuffsPlayer>().SummerPineapple)
			{
				crit *= 0.5f;
			}
		}

		public override bool CanUseItem(Item item, Player player)
        {
            //dont allow placing blocks and disable the rod of discord in the catacombs
            if (player.HasBuff(ModContent.BuffType<CatacombDebuff>()) && ((item.createTile > 0 && !TileID.Sets.Torch[item.createTile]) || item.type == ItemID.RodofDiscord))
            {
                return false;
            }

            return base.CanUseItem(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            //make items shoot boogers when the snotty schnoz is at full charge
            if (player.GetModPlayer<SpookyPlayer>().MocoNose && player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge >= 15)
            {
                //if the item in question shoots no projectile, or shoots a projectile and has a shoot speed of zero, then manually set the velocity for the booger projectiles
                if (item.damage > 0 && item.pick <= 0 && item.hammer <= 0 && item.axe <= 0 && item.mountType <= 0 && (item.shoot <= 0 || (item.shoot > 0 && item.shootSpeed == 0)))
                {
                    SoundEngine.PlaySound(SneezeSound, player.Center);

                    SpookyPlayer.ScreenShakeAmount = 8;

                    float mouseXDist = Main.mouseX + Main.screenPosition.X;
                    float mouseYDist = Main.mouseY + Main.screenPosition.Y;
                
                    Vector2 Velocity = player.Center - new Vector2(mouseXDist, mouseYDist);
					Velocity.Normalize();
					Velocity *= -12;

                    for (int numProjectiles = 0; numProjectiles <= 12; numProjectiles++)
                    {
                        Projectile.NewProjectile(null, player.Center, Velocity + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)), 
                        ModContent.ProjectileType<MocoNoseSnot>(), item.damage + 20, item.knockBack, player.whoAmI);
                    }

                    player.AddBuff(ModContent.BuffType<SnottySchnozCooldown>(), 1800);
                }
            }

			//items shoot out 3 monkey orchid shurikens if you have the monkey shruiken bloom buff
			if (player.GetModPlayer<BloomBuffsPlayer>().SpringOrchid)
			{
				if (item.damage > 0 && item.pick <= 0 && item.hammer <= 0 && item.axe <= 0 && item.mountType <= 0)
				{
					int Chance = item.shoot > 0 ? 7 : 85;

					if (Main.rand.NextBool(Chance))
					{
                        float DivideAmount = 1.5f;

						for (int numProjectiles = 0; numProjectiles <= 2; numProjectiles++)
						{
							float mouseXDist = Main.mouseX + Main.screenPosition.X + Main.rand.Next(-30, 30);
							float mouseYDist = Main.mouseY + Main.screenPosition.Y + Main.rand.Next(-30, 30);

							Vector2 ShootSpeed = player.Center - new Vector2(mouseXDist, mouseYDist);
							ShootSpeed.Normalize();
							ShootSpeed *= -10;

							Projectile.NewProjectile(null, player.Center, ShootSpeed, ModContent.ProjectileType<MonkeyOrchidShuriken>(), item.damage / (int)DivideAmount, item.knockBack, player.whoAmI);
						}
					}
				}
			}

			//items shoot out lemon bombs with the summer lemon bloom buff
			if (player.GetModPlayer<BloomBuffsPlayer>().SummerLemon && player.GetModPlayer<BloomBuffsPlayer>().SummerLemonDelay <= 0)
			{
				if (item.damage > 0 && item.pick <= 0 && item.hammer <= 0 && item.axe <= 0 && item.mountType <= 0)
				{
					int Chance1 = item.shoot > 0 ? 7 : 85;
					int Chance2 = item.shoot > 0 ? 2 : 3;

					if (Main.rand.NextBool(Chance1) || (player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot > 0 && Main.rand.NextBool(Chance2)))
					{
						float mouseXDist = Main.mouseX + Main.screenPosition.X;
						float mouseYDist = Main.mouseY + Main.screenPosition.Y;

						for (int numProjectiles = 0; numProjectiles <= player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot; numProjectiles++)
						{
							Vector2 ShootSpeed = new Vector2(mouseXDist, mouseYDist) - player.Center;
							ShootSpeed.Normalize();
							ShootSpeed.X *= 15 + Main.rand.NextFloat(-5f, 5f);
                            ShootSpeed.Y *= 15 + Main.rand.NextFloat(-5f, 5f);

							Projectile.NewProjectile(null, player.Center, ShootSpeed, ModContent.ProjectileType<BouncyLemon>(), item.damage, item.knockBack, player.whoAmI);
						}

						player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot++;

                        //if you reach a combo of 4 lemons, set it back to 1 and give the player a short delay before they can shoot lemons again
						if (player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot >= 4)
						{
                            player.GetModPlayer<BloomBuffsPlayer>().SummerLemonDelay = 240;
							player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot = 0;
						}
					}
					else
					{
						player.GetModPlayer<BloomBuffsPlayer>().SummerLemonsShot = 0;
					}
				}
			}

			//shoot out a kidney stone with the stoned kidney
			if (player.GetModPlayer<SpookyPlayer>().StonedKidney && player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge >= 7.5f)
			{
				//if the item in question shoots no projectile, or shoots a projectile and has a shoot speed of zero, then manually set the velocity for the booger projectiles
				if (item.damage > 0 && item.pick <= 0 && item.hammer <= 0 && item.axe <= 0 && item.mountType <= 0 && (item.shoot <= 0 || (item.shoot > 0 && item.shootSpeed == 0)))
				{
					float mouseXDist = Main.mouseX + Main.screenPosition.X;
					float mouseYDist = Main.mouseY + Main.screenPosition.Y;

					Vector2 Velocity = player.Center - new Vector2(mouseXDist, mouseYDist);
					Velocity.Normalize();
					Velocity *= -25;

					for (int numProjectiles = 0; numProjectiles <= 5; numProjectiles++)
					{
						Projectile.NewProjectile(null, player.Center, Velocity + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)),
						ModContent.ProjectileType<KidneyRock>(), 150, item.knockBack, player.whoAmI);
					}

					player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge = 0f;
				}
			}

			return base.UseItem(item, player);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //make items shoot boogers when the snotty schnoz is at full charge
            if (player.GetModPlayer<SpookyPlayer>().MocoNose && player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge >= 15)
            {
				if (item.damage > 0 && item.shootSpeed != 0)
				{
					SoundEngine.PlaySound(SneezeSound, player.Center);

                    SpookyPlayer.ScreenShakeAmount = 8;

                    for (int numProjectiles = 0; numProjectiles <= 12; numProjectiles++)
                    {
                        Projectile.NewProjectile(null, player.Center, velocity * 2f + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)), 
                        ModContent.ProjectileType<MocoNoseSnot>(), item.damage + 40, item.knockBack, player.whoAmI);
                    }

                    player.AddBuff(ModContent.BuffType<SnottySchnozCooldown>(), 1800);
                }
            }

			if (player.GetModPlayer<SpookyPlayer>().StonedKidney && player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge >= 7.5f)
			{
				if (item.damage > 0 && item.shootSpeed != 0)
				{
					for (int numProjectiles = 0; numProjectiles <= 5; numProjectiles++)
					{
						Projectile.NewProjectile(null, player.Center, velocity * 2 + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)),
						ModContent.ProjectileType<KidneyRock>(), 150, item.knockBack, player.whoAmI);
					}

					player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge = 0f;
				}
			}

			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

		public override bool WingUpdate(int wings, Player player, bool inUse)
		{
			//increase timer for spawning dandelion herd projectiles while flying
			if (player.GetModPlayer<BloomBuffsPlayer>().DandelionHerd && inUse && player.wingTime > 0)
			{
				player.GetModPlayer<BloomBuffsPlayer>().DandelionHerdTimer++;
			}

			//increase timer for spawning dandelion herd projectiles while flying
			if (player.GetModPlayer<BloomBuffsPlayer>().DandelionMapleSeed && inUse && player.wingTime > 0)
			{
				player.GetModPlayer<BloomBuffsPlayer>().DandelionMapleSeedTimer++;
			}

			return base.WingUpdate(wings, player, inUse);
		}
	}
}