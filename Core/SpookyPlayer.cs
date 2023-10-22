using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Biomes;
using Spooky.Content.Items.Fishing;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.Cemetery;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Tree;
using Spooky.Content.Items.BossBags.Accessory;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        //misc stuff
        public static float ScreenShakeAmount = 0;
        public int FlySpawnTimer = 0;
        public int SkullFrenzyCharge = 0;
        public int MocoBoogerCharge = 0;
        public int BoogerFrenzyTime = 0;
        public int SoulDrainCharge = 0;
        public int CrossSoundTimer = 0;
        public int PandoraCuffTimer = 0;
        public int RosaryHandTimer = 0;
        public int BoneWispTimer = 0;
        public int BustlingHealTimer = 0;
        public int GizaGlassHits = 0;
        public bool RaveyardGuardsHostile;

        //armors
        public bool GourdSet = false;
        public bool HorsemanSet = false;
        public bool EyeArmorSet = false;
        public bool FlowerArmorSet = false;
        public bool GoreArmorEye = false;
        public bool GoreArmorMouth = false;
        public bool SentientCap = false;

        //accessories
        public bool BustlingGlowshroom = false;
        public bool CandyBag = false;
        public bool MagicCandle = false;
        public bool SkullAmulet = false;
        public bool CrossCharmShield = false;
        public bool PandoraChalice = false;
        public bool PandoraCross = false;
        public bool PandoraCuffs = false;
        public bool HasSpawnedCuffs = false;
        public bool PandoraRosary = false;
        public bool BackroomsCorpse = false;
        public bool CarnisFlavorEnhancer = false;
        public bool GeminiEntertainmentGame = false;
        public bool HerobrineAltar = false;
        public bool Local58Telescope = false;
        public bool MandelaCatalogueTV = false;
        public bool MonumentMythosPyramid = false;
        public bool PolybiusArcadeGame = false;
        public bool RedGodzillaCartridge = false;
        public bool RedMistClarinet = false;
        public bool SlendermanPage = false;
        public bool SmileDogPicture = false;

        //expert accessories
        public bool FlyAmulet = false;
        public bool SpiritAmulet = false;
        public bool MocoNose = false;
        public bool DaffodilHairpin = false;
        public bool OrroboroEmbyro = false;
        public bool BoneMask = false;

        //minions
        public bool SkullWisp = false;
        public bool EntityMinion = false;
        public bool RotGourdMinion = false;
        public bool TumorMinion = false;
        public bool NoseMinion = false;
        public bool Grug = false;
        public bool DaffodilHand = false;
        public bool OldHunter = false;
        public bool SoulSkull = false;
        public bool Brainy = false;

        //pets
        public bool BatPet = false;
        public bool ColumboPet = false;
        public bool CatPet = false;
        public bool FlyPet = false;
        public bool FuzzBatPet = false;
        public bool GhostPet = false;
        public bool GooSlimePet = false;
        public bool GuineaPigPet = false;
        public bool PandoraBeanPet = false;
        public bool PetscopPet = false;
        public bool PetscopMarvinPet = false;
        public bool PetscopTiaraPet = false;
        public bool ShroomHopperPet = false;
        public bool SkullEmojiPet = false;
        public bool SkullGoopPet = false;
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

        //sounds
        public static readonly SoundStyle CrossBassSound = new("Spooky/Content/Sounds/CrossBass", SoundType.Sound) { Volume = 0.7f };
        public static readonly SoundStyle CapSound1 = new("Spooky/Content/Sounds/SentientCap1", SoundType.Sound);
        public static readonly SoundStyle CapSound2 = new("Spooky/Content/Sounds/SentientCap2", SoundType.Sound);
        public static readonly SoundStyle CapSound3 = new("Spooky/Content/Sounds/SentientCap3", SoundType.Sound);

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
            FlowerArmorSet = false;
            GoreArmorEye = false;
            GoreArmorMouth = false;
            SentientCap = false;

            //accessories
            BustlingGlowshroom = false;
            CandyBag = false;
            MagicCandle = false;
            SkullAmulet = false;
            CrossCharmShield = false;
            PandoraChalice = false;
            PandoraCross = false;
            PandoraCuffs = false;
            PandoraRosary = false;
            BackroomsCorpse = false;
            CarnisFlavorEnhancer = false;
            GeminiEntertainmentGame = false;
            HerobrineAltar = false;
            Local58Telescope = false;
            MandelaCatalogueTV = false;
            MonumentMythosPyramid = false;
            PolybiusArcadeGame = false;
            RedGodzillaCartridge = false;
            RedMistClarinet = false;
            SlendermanPage = false;
            SmileDogPicture = false;

            //expert accessories
            FlyAmulet = false;
            SpiritAmulet = false;
            MocoNose = false;
            DaffodilHairpin = false;
            OrroboroEmbyro = false;
            BoneMask = false;

            //minions
            SkullWisp = false;
            EntityMinion = false;
            RotGourdMinion = false;
            TumorMinion = false;
            NoseMinion = false;
            Grug = false;
            DaffodilHand = false;
            OldHunter = false;
            SoulSkull = false;
            Brainy = false;

            //pets
            BatPet = false;
            ColumboPet = false;
            CatPet = false;
            FlyPet = false;
            FuzzBatPet = false;
            GhostPet = false;
            GooSlimePet = false;
            GuineaPigPet = false;
            PandoraBeanPet = false;
            PetscopPet = false;
            PetscopMarvinPet = false;
            PetscopTiaraPet = false;
            ShroomHopperPet = false;
            SkullEmojiPet = false;
            SkullGoopPet = false;
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

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //do not allow hotkeys to do anything if you are dead
            if (Player.dead)
            {
                return;
            }

            if (Spooky.AccessoryHotkey.JustPressed && PandoraCross && Main.myPlayer == Player.whoAmI && !Player.HasBuff(ModContent.BuffType<PandoraCrossCooldown>()))
            {
                SoundEngine.PlaySound(CrossBassSound, Player.Center);

                CrossSoundTimer = 300;
                Player.AddBuff(ModContent.BuffType<PandoraCrossCooldown>(), 2400);
            }

            if (Spooky.AccessoryHotkey.JustPressed && MandelaCatalogueTV && Main.myPlayer == Player.whoAmI && !Player.HasBuff(ModContent.BuffType<AlternateCooldown>()))
            {
                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<Alternate>(), 0, 0f, Player.whoAmI, 0f, 0f);
                Player.AddBuff(ModContent.BuffType<AlternateCooldown>(), 3600);
            }
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            if (PandoraChalice && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraChaliceOrb>()] < 6)
            {
                Projectile.NewProjectile(Player.GetSource_ItemUse(item), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<PandoraChaliceOrb>(), healValue / 2, 0, Player.whoAmI);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //snotty schnoz booger item dropping on hit
            if (MocoNose && MocoBoogerCharge < 15)
            {
                if (!Player.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()) && Main.rand.NextBool(25))
                {
                    int itemType = ModContent.ItemType<MocoNoseBooger>();
                    int newItem = Item.NewItem(target.GetSource_OnHit(target), target.Hitbox, itemType);
                    Main.item[newItem].noGrabDelay = 0;

                    if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }
            }

            //skull amulet soul spawning
            if (SkullAmulet && target.life <= 0 && !target.friendly)
            {
                Projectile.NewProjectile(target.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<SkullAmuletSoul>(), 0, 0, Player.whoAmI);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //inflict enemies with gourd decay while wearing the rotten gourd armor
            if (GourdSet && hit.DamageType == DamageClass.Melee)
            {
                if (Main.rand.NextBool(8))
                {
                    target.AddBuff(ModContent.BuffType<GourdDecay>(), 3600);
                }
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

            RaveyardGuardsHostile = false;

            return ShouldRevive;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //give daffodil hairpin cooldown
            if (DaffodilHairpin)
            {
                Player.AddBuff(ModContent.BuffType<DaffodilHairpinCooldown>(), 3600);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (MonumentMythosPyramid)
            {
                GizaGlassHits++;

                if (GizaGlassHits == 3)
                {
                    GizaGlassHits = 0;
                    
                    Player.AddBuff(ModContent.BuffType<MonumentMythosShatter>(), 300);
                    Player.AddBuff(ModContent.BuffType<MonumentMythosCooldown>(), 7200);

                    SoundEngine.PlaySound(SoundID.Shatter, Player.Center);

                    for (int numGores = 1; numGores <= 4; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(Player.GetSource_OnHurt(info.DamageSource), Player.Center, Player.velocity, ModContent.Find<ModGore>("Spooky/GizaGlassGore" + numGores).Type);
                        }
                    }
                }
            }

            if (Player.HasBuff(ModContent.BuffType<MonumentMythosShatter>()))
            {
                info.Damage *= 2;
            }

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
                        Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), Player.Center.X + Main.rand.Next(-25, 25), Player.Center.Y + Main.rand.Next(-25, 25), 
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
                    int dustEffect = Dust.NewDust(Player.position, Player.width, Player.height, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            //activate pandora rosary hands healing AI
            if (PandoraRosary && !Player.HasBuff(ModContent.BuffType<PandoraHandCooldown>()))
            {
                Player.AddBuff(ModContent.BuffType<PandoraHandCooldown>(), 720);

                for (int i = 0; i <= Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<PandoraRosaryHand>())
                    {
                        Main.projectile[i].ai[0] = 1;
                    }
                }
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //hide the player's head while wearing the full horseman armor set
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
                        ModContent.ProjectileType<SwarmFly>(), 0, 0f, Main.myPlayer);

                        FlySpawnTimer = 0;
                    }
                }
            }

            //grant the player the skull frenzy when they absorb enough souls
            if (SkullFrenzyCharge >= 20)
            {
                Player.AddBuff(ModContent.BuffType<SkullFrenzyBuff>(), 600);

                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Volume = SoundID.DD2_DarkMageSummonSkeleton.Volume * 3.5f }, Player.Center);

                for (int numDust = 0; numDust < 45; numDust++)
                {
                    int newDust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.KryptonMoss, 0f, 0f, 100, default, 1.5f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[newDust].scale = 0.5f;
                        Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }

                SkullFrenzyCharge = 0;
            }

            //when the booger charge is high enough, grant the player the booger frenzy
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

            //spawn cross sound bass projectile after pressing the hotkey
            if (CrossSoundTimer > 0)
            {
                CrossSoundTimer--;

                if (CrossSoundTimer % 12 == 2)
                {
                    //the damage for this projectile should always be the same regardless of difficulty
                    int Damage = Main.masterMode ? 30 / 4 : Main.expertMode ? 30 / 3 : 30;

                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<PandoraCrossSound>(), Damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            if (PandoraCuffs && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraCuffProj>()] < 1)
            {
                for (int i = 0; i <= Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];

                    if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 450f)
                    {
                        PandoraCuffTimer++;

                        if (PandoraCuffTimer == 900)
                        {
                            //prioritize bosses over normal enemies
                            if (NPC.boss)
                            {
                                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0,
                                ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Main.myPlayer, i);
                                
                                break;
                            }
                            else
                            {
                                Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0,
                                ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Main.myPlayer, i);

                                break;
                            }
                        }
                    }
                    else
                    {
                        PandoraCuffTimer = 0;
                    }
                }
            }
            else
            {
                PandoraCuffTimer = 0;
            }

            //spawn pandora rosary hands that circle the player
            if (PandoraRosary && !Player.HasBuff(ModContent.BuffType<PandoraHandCooldown>()) && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraRosaryHand>()] < 5)
            {
                RosaryHandTimer++;

                if (RosaryHandTimer >= 325)
                {
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<PandoraRosaryHand>(), 0, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 360));

                    RosaryHandTimer = 0;
                }
            }
            else
            {
                RosaryHandTimer = 0;
            }
            
            if (SentientCap && Main.rand.NextBool(1000))
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                    {
                        SoundEngine.PlaySound(CapSound1, Player.Center);
                        break;
                    }
                    case 1:
                    {
                        SoundEngine.PlaySound(CapSound2, Player.Center);
                        break;
                    }
                    case 2:
                    {
                        SoundEngine.PlaySound(CapSound3, Player.Center);
                        break;
                    }
                }

                CombatText.NewText(Player.getRect(), Color.DarkOrchid, Language.GetTextValue("Mods.Spooky.Dialogue.SentientCap.Dialogue" + Main.rand.Next(1, 7).ToString()), true);
            }

            //bone mask wisp spawning while running
            if (BoneMask)
            {
                //all of these calculations are just copied from vanilla's stopwatch
                //too lazy to change all the num things tbh
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
                    if (Main.rand.NextBool(3) && attempt.crate && Flags.downedRotGourd)
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
                    if (Main.rand.NextBool(5) && attempt.crate && Flags.downedSpookySpirit)
                    {
                        itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();

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
                    if (Main.rand.NextBool(5) && attempt.crate && Flags.downedSpookySpirit)
                    {
                        itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();

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