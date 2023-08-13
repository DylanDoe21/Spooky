using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Biomes;
using Spooky.Content.Items.Fishing;
using Spooky.Content.NPCs.Hallucinations;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.Cemetery;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Tree;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        //misc timers
        public static float ScreenShakeAmount = 0;
        public int FlySpawnTimer = 0;
        public int MocoBoogerCharge = 0;
        public int BoogerFrenzyTime = 0;
        public int SoulDrainCharge = 0;
        public int BoneWispTimer = 0;
        public int BustlingHealTimer = 0;

        //armors
        public bool GourdSet = false;
        public bool HorsemanSet = false;
        public bool EyeArmorSet = false;
        public bool GoreArmorSet = false;

        //accessories
        public bool BustlingGlowshroom = false;
        public bool CandyBag = false;
        public bool MagicCandle = false;
        public bool CrossCharmShield = false;

        //expert accessories
        public bool FlyAmulet = false;
        public bool SpiritAmulet = false;
        public bool MocoNose = false;
        public bool OrroboroEmbyro = false;
        public bool BoneMask = false;

        //minions
        public bool SkullWisp = false;
        public bool EntityMinion = false;
        public bool TumorMinion = false;
        public bool NoseMinion = false;
        public bool Grug = false;
        public bool SoulSkull = false;
        public bool Brainy = false;

        //pets
        public bool ColumboPet = false;
        public bool CatPet = false;
        public bool FlyPet = false;
        public bool FuzzBatPet = false;
        public bool ShroomHopperPet = false;
        public bool SkullEmojiPet = false;
        public bool SkullGoopPet = false;
        public bool GhostPet = false;
        public bool ValleyNautilusPet = false;
        public bool RotGourdPet = false;
        public bool SpookySpiritPet = false;
        public bool StickyEyePet = false;
        public bool MocoPet = false;
        public bool BigBonePet = false;

        //armor glow mask stuff
        internal static readonly Dictionary<int, Texture2D> ItemGlowMask = new Dictionary<int, Texture2D>();

		internal new static void Unload() => ItemGlowMask.Clear();
		public static void AddGlowMask(int itemType, string texturePath) => ItemGlowMask[itemType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override void OnEnterWorld()
        {
            //un-hide the sun if you enter the world with the spooky mod menu enabled
            if (ModContent.GetInstance<SpookyMenu>().IsSelected)
            {
                Main.sunModY = 0;
            }
        }

        public override void ResetEffects()
        {
            //armors
            GourdSet = false;
            HorsemanSet = false;
            EyeArmorSet = false;
            GoreArmorSet = false;

            //accessories
            BustlingGlowshroom = false;
            CandyBag = false;
            MagicCandle = false;
            CrossCharmShield = false;

            //expert accessories
            FlyAmulet = false;
            SpiritAmulet = false;
            MocoNose = false;
            OrroboroEmbyro = false;
            BoneMask = false;

            //minions
            SkullWisp = false;
            EntityMinion = false;
            TumorMinion = false;
            NoseMinion = false;
            Grug = false;
            SoulSkull = false;
            Brainy = false;

            //pets
            ColumboPet = false;
            CatPet = false;
            FlyPet = false;
            FuzzBatPet = false;
            ShroomHopperPet = false;
            SkullEmojiPet = false;
            SkullGoopPet = false;
            GhostPet = false;
            ValleyNautilusPet = false;
            RotGourdPet = false;
            SpookySpiritPet = false;
            StickyEyePet = false;
            MocoPet = false;
            BigBonePet = false;
        }

        public override void ModifyScreenPosition()
        {
            float ExtraMultiplier = ModContent.GetInstance<SpookyConfig>().ScreenShakeIntensity;

            if (!Main.gameMenu && ExtraMultiplier > 0)
            {
                if (ScreenShakeAmount * ExtraMultiplier >= 0)
                {
                    ScreenShakeAmount -= 0.1f;
                }
                if (ScreenShakeAmount * ExtraMultiplier < 0)
                {
                    ScreenShakeAmount = 0;
                }
                
                Main.screenPosition += new Vector2((ScreenShakeAmount * ExtraMultiplier) * Main.rand.NextFloat(), (ScreenShakeAmount * ExtraMultiplier) * Main.rand.NextFloat());
            }
            else
            {
                ScreenShakeAmount = 0;
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
                    SoundEngine.PlaySound(SoundID.Item103, Player.Center);
                    Player.AddBuff(ModContent.BuffType<EmbryoRevival>(), 300);
                    Player.AddBuff(ModContent.BuffType<EmbryoCooldown>(), 36000);
                    Player.statLife = 1;
                    ShouldRevive = false;
                }
            }

            return ShouldRevive;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //gore armor set aura protection
            if (GoreArmorSet && Player.HasBuff(ModContent.BuffType<GoreAuraBuff>()))
            {
                modifiers.SetMaxDamage(1);
                Player.AddBuff(ModContent.BuffType<GoreAuraCooldown>(), 3600);
                SoundEngine.PlaySound(SoundID.AbigailSummon, Player.Center);

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, DustID.GemRuby, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            //gore armor set aura protection
            //copied from above because getting hit by npcs and projectiles are handled separately by tmodloader now
            if (GoreArmorSet && Player.HasBuff(ModContent.BuffType<GoreAuraBuff>()))
            {
                modifiers.SetMaxDamage(1);
                Player.AddBuff(ModContent.BuffType<GoreAuraCooldown>(), 3600);
                SoundEngine.PlaySound(SoundID.AbigailSummon, Player.Center);

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, DustID.GemRuby, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            //add fly cooldown when hit and the player has flies
            if (FlyAmulet)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<FlyCooldown>(), 1800);
                }
            }

            //spawn homing seeds when hit while wearing the spirit amulet
            if (SpiritAmulet && Main.rand.NextBool())
            {
                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(null, Player.Center.X + Main.rand.Next(-25, 25), Player.Center.Y + Main.rand.Next(-25, 25), 
                        Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), ModContent.ProjectileType<AmuletSeed>(), 30, 1, Main.myPlayer, 0, 0);
                    }
                }
            }

            //cross charm damage reduction cooldown
            if (CrossCharmShield && !Player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                Player.AddBuff(ModContent.BuffType<CrossCooldown>(), 600);

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.Center, Player.width / 2, Player.height / 2, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //hide the player's head while wearing full horseman armor
            if (HorsemanSet)
            {
                PlayerDrawLayers.Head.Hide();
            }
        }

        public override void PreUpdate()
        {
            //make player immune to the sandstorm debuff since it still applies it when you're in spooky mod biomes and theres a desert with a sandstorm happening nearby
            //because spooky mod biomes take higher priority that vanilla ones, this should not cause any issues
            if (Player.InModBiome(ModContent.GetInstance<SpookyBiome>()) || Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
            {
                Player.buffImmune[BuffID.WindPushed] = true;
            }

            if (Player.velocity == Vector2.Zero && BustlingGlowshroom)
            {
                BustlingHealTimer++;

                //dont heal the player until after they are standing still for long enough
                if (BustlingHealTimer >= 60)
                {
                    Player.AddBuff(ModContent.BuffType<BustlingGlowshroomHeal>(), 2);
                }
            }
            else
            {
                //reset the time if you move at all
                BustlingHealTimer = 0;
            }

            //bogger frenzy stuff
            //when the charge is high enough, grant the player the booger frenzy
            if (MocoBoogerCharge >= 15)
            {
                BoogerFrenzyTime++;

                //give the player the frenzy buff
                if (BoogerFrenzyTime == 1)
                {
                    Player.AddBuff(ModContent.BuffType<BoogerFrenzyBuff>(), 300);
                }

                //at the end of the frenzy, give the player the cooldown, then reset the charge and timer
                if (BoogerFrenzyTime >= 300)
                {
                    Player.AddBuff(ModContent.BuffType<BoogerFrenzyCooldown>(), 1800);
                    MocoBoogerCharge = 0;
                    BoogerFrenzyTime = 0;
                }
            }

            //spawn flies while wearing the fly amulet
            if (FlyAmulet)
            {
                //add the fly buff if the player has any flies around them
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<FlyBuff>(), 2);
                }

                //spawn flies
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] < 10)
                {
                    FlySpawnTimer++;

                    if (FlySpawnTimer == 300)
                    {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);
                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, vector.X, vector.Y,
                        ModContent.ProjectileType<SwarmFly>(), 0, 0f, Main.myPlayer, 0f, 0f);

                        FlySpawnTimer = 0;
                    }
                }
            }

            //increase endurance while you have the cross charm equipped
            if (CrossCharmShield && !Player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                Player.endurance += 0.15f;
            }

            //bone mask wisp spawning
            if (BoneMask)
            {
                //all of these calculations are just copied from vanilla's stopwatch
                //too lazy to change all the num things for now
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

                        //scale the damage based on the player's current speed
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
            if (!attempt.inLava && !attempt.inHoney)
            {
                //spooky forest catches
                if (Player.InModBiome<SpookyBiome>() || Player.InModBiome<SpookyBiomeUg>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<GourdFish>() && attempt.rare)
                    {
                        itemDrop = ModContent.ItemType<GourdFish>();

                        return;
                    }
                    if (attempt.questFish == ModContent.ItemType<ZomboidFish>() && attempt.rare)
                    {
                        itemDrop = ModContent.ItemType<ZomboidFish>();

                        return;
                    }

                    //crate
                    if (attempt.crate)
                    {
                        itemDrop = ModContent.ItemType<SpookyCrate>();

                        return;
                    }
                }

                if (Player.InModBiome<CemeteryBiome>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<SpookySpiritFish>() && attempt.rare)
                    {
                        itemDrop = ModContent.ItemType<SpookySpiritFish>();

                        return;
                    }

                    //crate
                    if (attempt.crate)
                    {
                        itemDrop = ModContent.ItemType<CatacombCrate>();

                        return;
                    }
                }

                if (Player.InModBiome<CatacombBiome>() || Player.InModBiome<CatacombBiome2>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<SpookySpiritFish>() && attempt.rare)
                    {
                        itemDrop = ModContent.ItemType<SpookySpiritFish>();

                        return;
                    }

                    //crate
                    if (attempt.crate)
                    {
                        itemDrop = ModContent.ItemType<CatacombCrate>();

                        return;
                    }
                }
            }

            //alternate blood moon enemy catches
            if (Player.InModBiome<SpookyHellBiome>())
            {
                //random blocks and junk normally fished out of the blood lake
                int[] BloodLakeItems = { ModContent.ItemType<EyeBlockItem>(), ModContent.ItemType<LivingFleshItem>(),
                ModContent.ItemType<SpookyMushItem>(), ModContent.ItemType<ValleyStoneItem>(), ModContent.ItemType<EyeSeed>() };

                itemDrop = Main.rand.Next(BloodLakeItems);

                //do not allow any other npcs to be caught in the eye valley besides the enemies below
                //this is specifically to prevent any regular blood moon fishing enemies from being caught in the blood lake if a blood moon is happening
                npcSpawn = NPCID.None;

                //quest fishes
                if (attempt.questFish == ModContent.ItemType<BoogerFish>() && attempt.rare)
                {
                    itemDrop = ModContent.ItemType<BoogerFish>();

                    return;
                }
                if (attempt.questFish == ModContent.ItemType<OrroEel>() && attempt.rare)
                {
                    itemDrop = ModContent.ItemType<OrroEel>();

                    return;
                }

                //do not allow blood lake enemy catches if any of the enemies already exist in the world
                bool BloodFishingEnemiesExist = NPC.AnyNPCs(ModContent.NPCType<ValleyFish>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyMerman>()) || 
                NPC.AnyNPCs(ModContent.NPCType<ValleySquid>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyNautilus>()) || 
                NPC.AnyNPCs(ModContent.NPCType<ValleyEelHead>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyShark>());

                if (!BloodFishingEnemiesExist)
                {
                    int ExtraChanceToFishEnemy = (Player.HeldItem.fishingPole + Player.fishingSkill) / 5;

                    //claret cephalopod
                    if (Flags.downedOrroboro && Main.rand.NextBool(25 - ExtraChanceToFishEnemy))
                    {
                        npcSpawn = ModContent.NPCType<ValleyNautilus>();

                        return;
                    }

                    //aortic eel and hemostasis beast
                    if (Main.hardMode && Main.rand.NextBool(20 - ExtraChanceToFishEnemy))
                    {
                        npcSpawn = Main.rand.NextBool() ? ModContent.NPCType<ValleyEelHead>() : ModContent.NPCType<ValleyShark>();

                        return;
                    }

                    //clot squid
                    if (Main.rand.NextBool(18 - ExtraChanceToFishEnemy))
                    {
                        npcSpawn = ModContent.NPCType<ValleySquid>();

                        return;
                    }

                    //peeper fish and flesh merfolk
                    if (Main.rand.NextBool(15 - ExtraChanceToFishEnemy))
                    {
                        npcSpawn = Main.rand.NextBool() ? ModContent.NPCType<ValleyFish>() : ModContent.NPCType<ValleyMerman>();

                        return;
                    }
                }
            }
        }
    }
}