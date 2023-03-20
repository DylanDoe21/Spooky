using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Biomes;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        public static int ShakeTimer = 0;
        public static float ScreenShakeAmount = 0;

        public int flySpawnTimer = 0;
        public int BoneWispTimer = 0;
        public int MocoBoogerCharge = 0;
        public int BoogerFrenzyTime = 0;

        //armors
        public bool SpookySet = false;
        public bool EyeArmorSet = false;
        public bool GoreArmorSet = false;

        //accessories
        public bool CandyBag = false;
        public bool MagicCandle = false;
        public bool ShadowflameCandle = false; 
        public bool CrossCharmShield = false;

        //expert accessories
        public bool FlyAmulet = false;
        public bool MocoNose = false;
        public bool OrroboroEmbyro = false;
        public bool BoneMask = false;

        //minions
        public bool SkullWisp = false;
        public bool EntityMinion = false;
        public bool TumorMinion = false;
        public bool NoseMinion = false;
        public bool SoulSkull = false;
        public bool Brainy = false;

        //pets
        public bool FuzzBatPet = false;
        public bool SkullEmojiPet = false;
        public bool GhostPet = false;
        public bool RotGourdPet = false;
        public bool SpookySpiritPet = false;
        public bool MocoPet = false;
        public bool BigBonePet = false;

        public override void ResetEffects()
        {
            //armors
            SpookySet = false;
            EyeArmorSet = false;
            GoreArmorSet = false;

            //accessories
            CandyBag = false;
            MagicCandle = false;
            CrossCharmShield = false;
            ShadowflameCandle = false; 

            //expert accessories
            FlyAmulet = false;
            MocoNose = false;
            OrroboroEmbyro = false;
            BoneMask = false;

            //minions
            SkullWisp = false;
            EntityMinion = false;
            TumorMinion = false;
            NoseMinion = false;
            SoulSkull = false;
            Brainy = false;

            //pets
            FuzzBatPet = false;
            SkullEmojiPet = false;
            GhostPet = false;
            RotGourdPet = false;
            SpookySpiritPet = false;
            MocoPet = false;
            BigBonePet = false;
        }

        public override void ModifyScreenPosition()
        {
            if (!Main.gameMenu && ModContent.GetInstance<SpookyConfig>().ScreenShakeEnabled)
            {
                ShakeTimer++;
                if (ScreenShakeAmount >= 0 && ShakeTimer >= 5)
                {
                    ScreenShakeAmount -= 0.1f;
                }
                if (ScreenShakeAmount < 0)
                {
                    ScreenShakeAmount = 0;
                }
                Main.screenPosition += new Vector2(ScreenShakeAmount * Main.rand.NextFloat(), ScreenShakeAmount * Main.rand.NextFloat());
            }
            else
            {
                ScreenShakeAmount = 0;
                ShakeTimer = 0;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool ShouldRevive = true;

            //embryo revive ability
            if (Player.statLife <= 0)
            {
                if (OrroboroEmbyro && !Player.HasBuff(ModContent.BuffType<EmbryoCooldown>()))
                {
                    Player.AddBuff(ModContent.BuffType<EmbryoRevival>(), 300);
                    Player.AddBuff(ModContent.BuffType<EmbryoCooldown>(), 36000);
                    Player.statLife = 1;
                    SoundEngine.PlaySound(SoundID.Item103, Player.position);
                    ShouldRevive = false;
                }
            }

            return ShouldRevive;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (FlyAmulet)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<FlyCooldown>(), 1800);
                }
            }

            if (GoreArmorSet && Player.HasBuff(ModContent.BuffType<GoreAuraBuff>()))
            {
                damage = 0;
                quiet = true;
                Player.AddBuff(ModContent.BuffType<GoreAuraCooldown>(), 3600);
                SoundEngine.PlaySound(SoundID.AbigailSummon, Player.Center);
                Player.statLife = Player.statLife;

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            if (CrossCharmShield && !Player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                Player.AddBuff(ModContent.BuffType<CrossCooldown>(), 600);

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            return true;
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (SpookySet)
            {
                PlayerDrawLayers.Head.Hide();
            }
        }

        public override void PreUpdate()
        {
            if (Player.InModBiome(ModContent.GetInstance<SpookyBiome>()) || Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
            {
                Player.buffImmune[BuffID.WindPushed] = true;
            }

            if (MocoBoogerCharge >= 15)
            {
                BoogerFrenzyTime++;

                if (BoogerFrenzyTime == 1)
                {
                    Player.AddBuff(ModContent.BuffType<BoogerFrenzyBuff>(), 300);
                }

                if (BoogerFrenzyTime >= 300)
                {
                    Player.AddBuff(ModContent.BuffType<BoogerFrenzyCooldown>(), 1800);
                    MocoBoogerCharge = 0;
                    BoogerFrenzyTime = 0;
                }
            }

            //spawn flies with the fly amulet
            if (FlyAmulet)
            {
                //add the buff if the player has any flies around them
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<FlyBuff>(), 2);
                }

                //spawn flies
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] < 10)
                {
                    flySpawnTimer++;

                    if (flySpawnTimer == 300)
                    {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);
                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, vector.X, vector.Y,
                        ModContent.ProjectileType<SwarmFly>(), 0, 0f, Main.myPlayer, 0f, 0f);

                        flySpawnTimer = 0;
                    }
                }
            }

            if (CrossCharmShield && !Player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                Player.endurance += 0.05f;
            }

            if (BoneMask)
            {
                //all of these formulas are just copied from vanilla's stopwatch
                Vector2 vector = Player.velocity + Player.instantMovementAccumulatedThisFrame;

                if (Player.mount.Active && Player.mount.IsConsideredASlimeMount && Player.velocity != Vector2.Zero && !Player.SlimeDontHyperJump)
                {
                    vector += Player.velocity;
                }

                Player.speedSlice[0] = vector.Length();

                int num15 = (int)(1f + vector.Length() * 6f);
                if (num15 > Player.speedSlice.Length)
                {
                    num15 = Player.speedSlice.Length;
                }

                float num16 = 0f;
                for (int num17 = num15 - 1; num17 > 0; num17--)
                {
                    Player.speedSlice[num17] = Player.speedSlice[num17 - 1];
                }

                Player.speedSlice[0] = vector.Length();
                for (int m = 0; m < Player.speedSlice.Length; m++)
                {
                    if (m < num15)
                    {
                        num16 += Player.speedSlice[m];
                    }
                    else
                    {
                        Player.speedSlice[m] = num16 / (float)num15;
                    }
                }

                num16 /= num15;
                int num18 = 42240;
                int num19 = 216000;
                float num20 = num16 * (float)num19 / (float)num18;

                //do not shoot skulls under 20mph (basically if you are not moving fast enough)
                if (num20 >= 20)
                {
                    BoneWispTimer++;

                    if (BoneWispTimer >= 180 / (num20 / 10))
                    {
                        SoundEngine.PlaySound(SoundID.Item8, Player.Center);

                        Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                        Vector2 newVelocity = Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));

                        int damage = 80 + ((int)num20 / 3);

                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, newVelocity.X, newVelocity.Y,
                        ModContent.ProjectileType<BoneMaskWisp>(), damage, 0f, Main.myPlayer, 0f, 0f);

                        BoneWispTimer = 0;
                    }
                }
                else
                {
                    BoneWispTimer = 0;
                }
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;
            bool inSpookyBiome = Player.InModBiome<SpookyBiome>() || Player.InModBiome<SpookyBiomeUg>();
            bool inCatacombArea = Player.InModBiome<CemeteryBiome>() || Player.InModBiome<CatacombBiome>();

            if (inWater && attempt.crate)
            {
                if (!attempt.legendary && !attempt.veryrare && attempt.rare)
                {
                    if (inSpookyBiome)
                    {
                        sonar.Text = "Spooky Crate";
                        sonar.Color = Color.Orange;
                        sonar.Velocity = Vector2.Zero;
                        sonar.DurationInFrames = 300;

                        itemDrop = ModContent.ItemType<SpookyCrate>();
                    }

                    if (inCatacombArea)
                    {
                        sonar.Text = "Skull Crate";
                        sonar.Color = Color.Yellow;
                        sonar.Velocity = Vector2.Zero;
                        sonar.DurationInFrames = 300;

                        itemDrop = ModContent.ItemType<CatacombCrate>();
                    }
                }
            }
        }
    }

    public class CrossCharmShield : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().CrossCharmShield && !drawInfo.drawPlayer.HasBuff(ModContent.BuffType<CrossCooldown>());
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Items/Catacomb/CrossCharmDraw").Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Main.EntitySpriteDraw(tex, new Vector2(drawInfo.drawPlayer.Center.X, drawInfo.drawPlayer.Center.Y - 45)  - Main.screenPosition, null, Color.White, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0);
        }
    }

    public class GoreAura : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !drawInfo.drawPlayer.HasBuff<GoreAuraCooldown>() && drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().GoreArmorSet;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex;

            if (!drawInfo.drawPlayer.armorEffectDrawOutlines && !drawInfo.drawPlayer.armorEffectDrawShadow)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Armor/GoreAuraEffect2").Value;
            }
            else
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Armor/GoreAuraEffect").Value;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Lerp(Color.Transparent, new Color(220, 20, 60, 100), fade), Color.Transparent, fade);

            Main.EntitySpriteDraw(tex, drawInfo.drawPlayer.MountedCenter - Main.screenPosition, null, color, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0);
        }
    }
}