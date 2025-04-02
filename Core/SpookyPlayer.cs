using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Fishing;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.NPCs.Boss.SpookFishron;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.Cemetery;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Tree;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        //armors
        public bool GourdSet = false;
        public bool RootSet = false;
        public bool SpiderSet = false;
        public bool SpiderSpeed = false;
        public bool HorsemanSet = false;
        public bool EyeArmorSet = false;
        public bool FlowerArmorSet = false;
        public bool GoreArmorEye = false;
        public bool GoreArmorMouth = false;
        public bool SentientCap = false;

        //accessories
        public bool BustlingGlowshroom = false;
        public bool CandyBag = false;
		public bool CandyBagJustHit = false;
        public bool MagicCandle = false;
        public bool SkullAmulet = false;
        public bool CrossCharmShield = false;
        public bool PandoraChalice = false;
        public bool PandoraCross = false;
        public bool PandoraCuffs = false;
        public bool HasSpawnedCuffs = false;
        public bool PandoraRosary = false;
        public bool HunterScarf = false;
        public bool AnalogHorrorTape = false;
        public bool CreepyPasta = false;
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
		public bool GiantEar = false;
		public bool GooChompers = false;
		public bool PeptoStomach = false;
		public bool SmokerLung = false;
		public bool StonedKidney = false;
		public bool VeinChain = false;
		public bool GhostBookBlue = false;
		public bool GhostBookGreen = false;
		public bool GhostBookRed = false;
		public bool MagicEyeOrb = false;
		public bool SewingThread = false;
		public bool StitchedCloak = false;

		//expert accessories
		public bool FlyAmulet = false;
        public bool SpiritAmulet = false;
        public bool MocoNose = false;
        public bool DaffodilHairpin = false;
        public bool OrroboroEmbyro = false;
        public bool BoneMask = false;

        //pets
        public bool ColumboPet = false;
        public bool CatPet = false;
        public bool FlyPet = false;
        public bool GhostPet = false;
        public bool InchwormPet = false;
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
        public bool OrroboroPet = false;
        public bool BeePet = false;
        public bool FuzzBatPet  = false;
        public bool PuttyPet = false;
        public bool RatPet = false;
        public bool ZombieCultistPet = false;
        public bool SinisterSnailPet = false;

        //misc bools
        public bool EatenByGooSlug = false;
        public bool RaveyardGuardsHostile = false;
        public bool WhipSpiderAggression = false;
        public bool SpiderGrottoCompass = false;
        public bool EyeValleyCompass = false;
        public bool NoseCultistDisguise1 = false;
		public bool NoseCultistDisguise2 = false;
		public bool NoseBlessingBuff = false;

		//misc timers
		public static float ScreenShakeAmount = 0;
        public float SpiderStealthAlpha = 0f;
		public float StonedKidneyCharge = 0f;
		public int SpiderSpeedTimer = 0;
        public int FlySpawnTimer = 0;
        public int SkullFrenzyCharge = 0;
        public int MocoBoogerCharge = 0;
        public int SoulDrainCharge = 0;
        public int CrossSoundTimer = 0;
        public int PandoraCuffTimer = 0;
        public int RosaryHandTimer = 0;
        public int BoneWispTimer = 0;
        public int BustlingHealTimer = 0;
        public int GizaGlassHits = 0;
        public int SlendermanPageDelay = 0;
        public int CarnisSporeSpawnTimer = 0;
        public int RedMistNoteSpawnDelay = 0;
        public int RedGodzillaCartridgeSpawnDelay = 0;
        public int GeminiMockerySpawnTimer = 0;
        public int GooSlugEatCooldown = 0;
		public int RootHealCooldown = 0;
		public int MagicEyeOrbHits = 0;
        public int CandyBagCooldown = 0;
        public int PotionSicknessLatteTimer = 0;

		//dashing stuff
		public const int dashDown = 0;
		public const int dashUp = 1;
		public const int dashRight = 2;
		public const int dashLeft = 3;
		public int dashCooldown = 600;
		public int dashDuration = 15;
		public float dashVelocityX = 15f;
		public float dashVelocityY = 18f;
		public int dashDir = -1;
		public int dashDelay = 0;
		public int dashTimer = 0;

		public Vector2 MocoUITopLeft = new Vector2(Main.screenWidth / 2 + (Main.screenWidth / 12), Main.screenHeight / 1.5f);
		public Vector2 KidneyUITopLeft = new Vector2(Main.screenWidth / 2 + (Main.screenWidth / 12), Main.screenHeight / 1.5f);

		private static Asset<Texture2D> SentientLeafBlowerBackTex;

		//sounds
		public static readonly SoundStyle CrossBassSound = new("Spooky/Content/Sounds/CrossBass", SoundType.Sound) { Volume = 0.7f };
        public static readonly SoundStyle ClarinetSound = new("Spooky/Content/Sounds/Clarinet", SoundType.Sound) { Volume = 0.7f, PitchVariance = 0.6f };
        public static readonly SoundStyle CapSound1 = new("Spooky/Content/Sounds/SentientCap1", SoundType.Sound);
        public static readonly SoundStyle CapSound2 = new("Spooky/Content/Sounds/SentientCap2", SoundType.Sound);
        public static readonly SoundStyle CapSound3 = new("Spooky/Content/Sounds/SentientCap3", SoundType.Sound);

		public override void SaveData(TagCompound tag)
		{
			tag["MocoUITopLeft"] = MocoUITopLeft;
			tag["KidneyUITopLeft"] = KidneyUITopLeft;
		}
		public override void LoadData(TagCompound tag)
		{
			MocoUITopLeft = tag.Get<Vector2>("MocoUITopLeft");
			KidneyUITopLeft = tag.Get<Vector2>("KidneyUITopLeft");
		}

		public override void OnEnterWorld()
        {
            //un-hide the sun if you enter the world with the spooky mod menu enabled since it hides the sun offscreen
            if (ModContent.GetInstance<SpookyMenu>().IsSelected)
            {
                Main.sunModY = 0;
            }
        }

        public override void ResetEffects()
        {
            //armors
            GourdSet = false;
            RootSet = false;
            SpiderSet = false;
            SpiderSpeed = false;
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
            HunterScarf = false;
            AnalogHorrorTape = false;
            CreepyPasta = false;
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
			GiantEar = false;
			GooChompers = false;
			PeptoStomach = false;
			SmokerLung = false;
			StonedKidney = false;
			VeinChain = false;
			GhostBookBlue = false;
			GhostBookGreen = false;
			GhostBookRed = false;
			MagicEyeOrb = false;
			SewingThread = false;
			StitchedCloak = false;

			//expert accessories
			FlyAmulet = false;
            SpiritAmulet = false;
            MocoNose = false;
            DaffodilHairpin = false;
            OrroboroEmbyro = false;
            BoneMask = false;

            //pets
            ColumboPet = false;
            CatPet = false;
            FlyPet = false;
            GhostPet = false;
            InchwormPet = false;
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
            OrroboroPet = false;
            BeePet = false;
            FuzzBatPet  = false;
            PuttyPet = false;
            RatPet = false;
            ZombieCultistPet = false;
            SinisterSnailPet = false;

            //misc bools
            WhipSpiderAggression = false;
            EatenByGooSlug = false;
            SpiderGrottoCompass = false;
            EyeValleyCompass = false;
            NoseCultistDisguise1 = false;
			NoseCultistDisguise2 = false;
			NoseBlessingBuff = false;

			//dashing stuff
			if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[dashRight] < 15)
			{
				dashDir = dashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[dashLeft] < 15)
			{
				dashDir = dashLeft;
			}
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[dashUp] < 15)
			{
				dashDir = dashUp;
			}
			else if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[dashDown] < 15)
			{
				dashDir = dashDown;
			}
			else
			{
				dashDir = -1;
			}
		}

        public override void ModifyScreenPosition()
        {
            float ExtraMultiplier = ModContent.GetInstance<SpookyConfig>().ScreenShakeIntensity;

            if (!Main.gameMenu && ExtraMultiplier > 0)
            {
                PunchCameraModifier modifier = new PunchCameraModifier(Player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), ScreenShakeAmount * ExtraMultiplier, 1f, 1, 550f, null);
				Main.instance.CameraModifiers.Add(modifier);

                if (ScreenShakeAmount * ExtraMultiplier >= 0)
                {
                    ScreenShakeAmount -= 0.2f;
                }
                if (ScreenShakeAmount * ExtraMultiplier < 0)
                {
                    ScreenShakeAmount = 0;
                }
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

            //handle everything when the accessory hotkey is pressed
            if (Spooky.AccessoryHotkey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                //create sound with the pandora cross
                if (PandoraCross && !Player.HasBuff(ModContent.BuffType<PandoraCrossCooldown>()))
                {
                    SoundEngine.PlaySound(CrossBassSound, Player.Center);

                    CrossSoundTimer = 300;
                    Player.AddBuff(ModContent.BuffType<PandoraCrossCooldown>(), 2400);
                }

                //spawn alternate with the analog tv
                if (MandelaCatalogueTV && !Player.HasBuff(ModContent.BuffType<AlternateCooldown>()))
                {
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<Alternate>(), 0, 0f, Player.whoAmI, 0f, 0f);
                    Player.AddBuff(ModContent.BuffType<AlternateCooldown>(), 3600);
                }

                //spawn lightning with the herobrine altar
                if (HerobrineAltar && !Player.HasBuff(ModContent.BuffType<HerobrineAltarCooldown>()))
                {
                    SoundEngine.PlaySound(SoundID.Thunder with { Pitch = -0.5f }, Player.Center);

                    SpookyPlayer.ScreenShakeAmount = 10;

                    Vector2 ShootSpeed = new Vector2(Player.Center.X, Player.Center.Y - Main.screenHeight) - Main.MouseWorld;
                    ShootSpeed.Normalize();
                    ShootSpeed *= -100f;

                    //each lighting bolt is set to deal 1 damage, the actual final damage is handled in the projectile itself
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y - Main.screenHeight, ShootSpeed.X, ShootSpeed.Y,
                    ModContent.ProjectileType<HerobrineLightning>(), 1, 0f, Player.whoAmI, ShootSpeed.ToRotation(), 100);

                    Main.NewLightning();

                    Player.AddBuff(ModContent.BuffType<HerobrineAltarCooldown>(), 7200);
                }

				//spawn a stationary smoke cloud with the smoker lung
				if (SmokerLung && !Player.HasBuff(ModContent.BuffType<SmokerLungCooldown>()))
				{
					SoundEngine.PlaySound(SoundID.NPCHit27 with { Pitch = -1.2f }, Player.Center);

					Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<CoughSmokeCloud>(), 50, 0f, Player.whoAmI);

					Player.AddBuff(ModContent.BuffType<SmokerLungCooldown>(), 3600);
				}
            }

            //handle everything when they armor bonus hotkey is pressed
            if (Spooky.ArmorBonusHotkey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                //flower armor setbonus
                if (FlowerArmorSet && !Player.HasBuff(ModContent.BuffType<FlowerArmorCooldown>()))
                {
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Player.Center);

                    for (int numProjectiles = 0; numProjectiles < 12; numProjectiles++)
                    {
                        Projectile.NewProjectile(null, Player.Center.X + Main.rand.Next(-30, 30), 
                        Player.Center.Y + Main.rand.Next(-30, 30), 0, 0, ModContent.ProjectileType<FlowerArmorPollen>(), 55, 2f, Player.whoAmI);
                    }

                    Player.AddBuff(ModContent.BuffType<FlowerArmorCooldown>(), 1800);
                }

                //spider stealth
                if (SpiderSet && !Player.HasBuff(ModContent.BuffType<SpiderStealthCooldown>()))
                {
                    Player.AddBuff(ModContent.BuffType<SpiderArmorStealth>(), 600);
                    Player.AddBuff(ModContent.BuffType<SpiderStealthCooldown>(), 7200);
                }
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
            if (target.active && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                //inflict enemies with gourd decay while wearing the rotten gourd armor
                if (GourdSet && hit.DamageType == DamageClass.Melee)
                {
                    if (Main.rand.NextBool(8))
                    {
                        target.AddBuff(ModContent.BuffType<GourdDecay>(), Main.rand.Next(600, 1200));
                    }
                }

                //spawn eyes when hitting enemies with whips with the living flesh armor
                if (EyeArmorSet && hit.DamageType == DamageClass.SummonMeleeSpeed && Main.rand.NextBool(5))
                {
                    Vector2 SpawnPosition = target.Center + new Vector2(0, 85).RotatedByRandom(360);

                    for (int numDusts = 0; numDusts < 10; numDusts++)
                    {                                                                                  
                        int dust = Dust.NewDust(SpawnPosition, 20, 20, DustID.Blood, 0f, -2f, 0, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    }

                    Projectile.NewProjectile(target.GetSource_OnHit(target), SpawnPosition, Vector2.Zero, 
                    ModContent.ProjectileType<LivingFleshEye>(), damageDone / 2, hit.Knockback, Player.whoAmI, 0, target.whoAmI);
                }

                //drop booger charge item when hitting an enemy while wearing the snotty schnoz
                if (MocoNose && MocoBoogerCharge < 15 && !Player.HasBuff(ModContent.BuffType<SnottySchnozCooldown>()) && Main.rand.NextBool(12))
                {
                    int itemType = ModContent.ItemType<MocoNoseBooger>();
                    int newItem = Item.NewItem(target.GetSource_OnHit(target), target.Hitbox, itemType);
                    Main.item[newItem].noGrabDelay = 0;

                    if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }

                //spawn souls when you kill an enemy while wearing the skull amulet
                if (SkullAmulet && target.life <= 0 && !target.friendly)
                {
                    Projectile.NewProjectile(target.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<SkullAmuletSoul>(), 0, 0, Player.whoAmI);
                }

                //spawn an orbiting note on critical hits with the clarinet
                if (RedMistClarinet && RedMistNoteSpawnDelay <= 0 && hit.Crit && Main.rand.NextBool())
                {
                    SoundEngine.PlaySound(ClarinetSound, Player.Center);

                    RedMistNoteSpawnDelay = 120;

                    //dont cap the damage if the player has the combined creepypasta accessory
                    int damage = CreepyPasta ? hit.Damage : (hit.Damage >= 70 ? 70 : hit.Damage);

                    Projectile.NewProjectile(target.GetSource_OnHit(target), Player.Center, Vector2.Zero, ModContent.ProjectileType<RedMistNote>(), damage, hit.Knockback, Player.whoAmI, 0, 0, Main.rand.Next(0, 2));
                }

                //rarely spawn the red face when hitting an enemy
                if (RedGodzillaCartridge && RedGodzillaCartridgeSpawnDelay <= 0 && Main.rand.NextBool(50))
                {
                    RedGodzillaCartridgeSpawnDelay = 360;

                    //dont spawn a red apparition if one already exists
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RedFace>()] <= 0)
                    {
                        Vector2 SpawnPosition = target.Center + new Vector2(0, 85).RotatedByRandom(360);

                        Projectile.NewProjectile(target.GetSource_OnHit(target), SpawnPosition, Vector2.Zero, ModContent.ProjectileType<RedFace>(), damageDone * 5, hit.Knockback, Player.whoAmI, 0, target.whoAmI);
                    }
                }

                //inflict enemies with stomach ache debuff with the pepto stomach
                if (PeptoStomach && !target.boss && !target.IsTechnicallyBoss() && Main.rand.NextBool(20))
                {
                    target.AddBuff(ModContent.BuffType<PeptoDebuff>(), int.MaxValue);
                }

                //attach a chain to an enemy with the vein chain
                if (VeinChain && Main.rand.NextBool(10) && target.active && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type] && Vector2.Distance(Player.Center, target.Center) <= 370f)
                {
                    int MaxChains = Player.statLife < (Player.statLifeMax / 4) ? 1 : (Player.statLife < (Player.statLifeMax / 2) ? 2 : 3);

                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<VeinChainProj>()] < MaxChains && !target.GetGlobalNPC<NPCGlobal>().HasVeinChainAttached)
                    {
                        Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<VeinChainProj>(), 35, 0, Player.whoAmI, 0, 0, target.whoAmI);
                        target.GetGlobalNPC<NPCGlobal>().HasVeinChainAttached = true;
                    }
                }

                //spawn goo jaws on enemies when you hit them with the goo chompers
                if (GooChompers && Main.rand.NextBool(15) && !target.GetGlobalNPC<NPCGlobal>().HasGooChompterAttached)
                {
                    int Damage = ItemGlobal.ActiveItem(Player).damage > 50 ? ItemGlobal.ActiveItem(Player).damage : 50;

                    Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<GooChomperProj>(), Damage, 0, Player.whoAmI, target.whoAmI);
                    target.GetGlobalNPC<NPCGlobal>().HasGooChompterAttached = true;
                }

                //if the player has the nose blessing buff and hits an npc with the nose blessing debuff
                if (NoseBlessingBuff && Main.rand.NextBool(10) && Player.ownedProjectileCounts[ModContent.ProjectileType<SnotBlessingOrbiter>()] < 10 && !target.HasBuff(ModContent.BuffType<NoseBlessingDebuffCooldown>()))
                {
                    if (!target.HasBuff(ModContent.BuffType<NoseBlessingDebuff>()))
                    {
                        target.AddBuff(ModContent.BuffType<NoseBlessingDebuff>(), 360);
                    }
                    
                    if (target.HasBuff(ModContent.BuffType<NoseBlessingDebuff>()))
                    {
                        int distance = Main.rand.Next(0, 360);

                        Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<SnotBlessingOrbiter>(), damageDone * 2, 3, Player.whoAmI, target.whoAmI, distance);
                    }
                }

                //make candy bag shoot out homing candy when an enemy is hit with a summon item or whip
                if (CandyBag && (hit.DamageType == DamageClass.Summon || hit.DamageType == DamageClass.SummonMeleeSpeed))
                { 
                    CandyBagJustHit = true;
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //give daffodil hairpin cooldown when you get hit with the petal barrier
            if (DaffodilHairpin)
            {
                Player.AddBuff(ModContent.BuffType<DaffodilHairpinCooldown>(), 3600);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            //set maximum damage cap based on difficulties due to damage scaling
            int damageToActivateSpeedBoost1 = Main.masterMode ? 100 : Main.expertMode ? 70 : 40;
            int damageToActivateSpeedBoost2 = Main.masterMode ? 150 : Main.expertMode ? 100 : 50;

            //give players the spider armor speed boosts when they get hit by a strong enough attack
            if (SpiderSpeed && info.Damage >= damageToActivateSpeedBoost1)
            {
                SpiderSpeedTimer = 45;
            }

            //give the player monument mythos shatter and cooldown if they get hit 3 times
            if (MonumentMythosPyramid)
            {
                GizaGlassHits++;

                if (GizaGlassHits == 3)
                {
                    SoundEngine.PlaySound(SoundID.Shatter, Player.Center);
                    
                    Player.AddBuff(ModContent.BuffType<MonumentMythosShatter>(), 600);
                    Player.AddBuff(ModContent.BuffType<MonumentMythosCooldown>(), 3600);

                    for (int numGores = 1; numGores <= 4; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(Player.GetSource_OnHurt(info.DamageSource), Player.Center, Player.velocity, ModContent.Find<ModGore>("Spooky/GizaGlassGore" + numGores).Type);
                        }
                    }

                    GizaGlassHits = 0;
                }
            }

            //player takes twice as much damage with the monument mythos shatter debuff
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
                        Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), ModContent.ProjectileType<AmuletSeed>(), 20, 0, Main.myPlayer);
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

			//if the player gets hit too many times with the glass eye then give the player the glass eye cooldown
			if (MagicEyeOrb)
			{
				MagicEyeOrbHits++;

				if (MagicEyeOrbHits == 5)
				{
					Player.AddBuff(ModContent.BuffType<GlassEyeCooldown>(), 1800);

					MagicEyeOrbHits = 0;
				}
			}
        }

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			RaveyardGuardsHostile = false;
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
                    Player.immuneTime += 60;
                    Player.statLife = 1;
                    ShouldRevive = false;
                }
            }

            return ShouldRevive;
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //hide the player's head while wearing the full horseman armor set
            if (HorsemanSet)
            {
                PlayerDrawLayers.Head.Hide();
            }

            //do not draw anything if the player is being eaten by a goo slug
			if (EatenByGooSlug)
			{
				drawInfo.drawPlayer.frozen = true;

				foreach (var layer in PlayerDrawLayerLoader.Layers)
				{
					layer.Hide();
				}
			}
        }

        public override void PreUpdate()
        {
            //decrease spider armor speed boost time
            if (SpiderSpeedTimer > 0)
            {
                SpiderSpeedTimer--;
            }

            //decrease slenderman page delay
            if (SlendermanPageDelay > 0)
            {
                SlendermanPageDelay--;
            }

            //decrease red mist note spawning delay
            if (RedMistNoteSpawnDelay > 0)
            {
                RedMistNoteSpawnDelay--;
            }

            if (RedGodzillaCartridgeSpawnDelay > 0)
            {
                RedGodzillaCartridgeSpawnDelay--;
            }

            //set skeleton bouncer hositility to false if no raveyard is happening
            if (!Flags.RaveyardHappening)
            {
                RaveyardGuardsHostile = false;
            }

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
                        Vector2 randomVelocity = Vector2.UnitY.RotatedByRandom(1.5f) * new Vector2(5f, 3f);

                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, randomVelocity.X, 
                        randomVelocity.Y, ModContent.ProjectileType<SwarmFly>(), 0, 0f, Main.myPlayer);

                        FlySpawnTimer = 0;
                    }
                }
            }

            //grant the player the skull frenzy when they absorb enough souls
            if (SkullFrenzyCharge >= 10)
            {
                Player.AddBuff(ModContent.BuffType<SkullFrenzyBuff>(), 600);

                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Volume = SoundID.DD2_DarkMageSummonSkeleton.Volume * 3.5f }, Player.Center);

                for (int numDust = 0; numDust < 45; numDust++)
                {
                    int newDust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.KryptonMoss, 0f, 0f, 100, default, 1.5f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].noGravity = true;
                }

                SkullFrenzyCharge = 0;
            }

            //spawn cross sound bass projectile after pressing the hotkey
            if (CrossSoundTimer > 0)
            {
                CrossSoundTimer--;

                if (CrossSoundTimer % 12 == 2)
                {
                    //the damage for this projectile should always be the same regardless of difficulty
                    int Damage = Main.masterMode ? 30 / 4 : Main.expertMode ? 30 / 3 : 30;

                    Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCrossSound>(), Damage, 0f, Main.myPlayer);
                }
            }

            //spawn pandora cuff to attach to the chosen enemy
            if (PandoraCuffs && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraCuffProj>()] < 1)
            {
                for (int i = 0; i <= Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];

                    if (NPC.active && !NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 450f)
                    {
                        PandoraCuffTimer++;

                        if (PandoraCuffTimer == 900)
                        {
                            //prioritize bosses over normal enemies
                            if (NPC.boss)
                            {
                                Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Main.myPlayer, i);
                                
                                break;
                            }
                            else
                            {
                                Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Main.myPlayer, i);

                                break;
                            }
                        }
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

            //inflict enemies with the hunter mark debuff with the old hunter's scarf
            if (HunterScarf)
            {
                for (int i = 0; i <= Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];

                    if (NPC.active && !NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 450f)
                    {
                        NPC.AddBuff(ModContent.BuffType<HunterScarfMark>(), 5);
                    }
                }
            }

            //spawn nature's mockery on the ground with the lethal omen accessory
            if (GeminiEntertainmentGame && Player.ownedProjectileCounts[ModContent.ProjectileType<NaturesMockery>()] < 1)
            {
                GeminiMockerySpawnTimer++;

                if (GeminiMockerySpawnTimer >= 420)
                {
                    Vector2 center = new Vector2(Player.Center.X, Player.Center.Y + Player.height / 4);
                    center.X += Main.rand.Next(-125, 126);
                    int numtries = 0;
                    int x = (int)(center.X / 16);
                    int y = (int)(center.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) &&
                    Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        center.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
                    {
                        numtries++;
                        y--;
                        center.Y = y * 16;
                    }

                    int NewProj = Projectile.NewProjectile(null, center.X, center.Y, 0, -0.3f, ModContent.ProjectileType<NaturesMockery>(), 0, 0, Main.myPlayer, 0, 0, 4);
                    Main.projectile[NewProj].frame = AnalogHorrorTape ? 4 : Main.rand.Next(0, 4);

                    GeminiMockerySpawnTimer = 0;
                }
            }
            else
            {
                GeminiMockerySpawnTimer = 0;
            }

			//spawn spores with the vita carnis flavor enhancer
            if (CarnisFlavorEnhancer)
            {
                CarnisSporeSpawnTimer++;

                if (PlayerSpeed(Player) >= 10)
                {
                    CarnisSporeSpawnTimer++;

                    if (CarnisSporeSpawnTimer >= 30)
                    {
                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<FoodEnhancerSpore>(), 0, 0f, Main.myPlayer);
                        CarnisSporeSpawnTimer = 0;
                    }
                }
                else
                {
                    CarnisSporeSpawnTimer = 0;
                }
            }

			//shoot skulls with big bones expert item
            if (BoneMask)
            {
                //do not shoot skulls under 20mph (basically if you are not moving fast enough)
                if (PlayerSpeed(Player) >= 20)
                {
                    BoneWispTimer++;

                    if (BoneWispTimer >= 180 / (PlayerSpeed(Player) / 10))
                    {
                        SoundEngine.PlaySound(SoundID.Item8, Player.Center);

                        Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                        Vector2 newVelocity = Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));

                        //scale the damage based on the player's current speed
                        int damage = 80 + ((int)PlayerSpeed(Player) / 3);

                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, newVelocity.X, newVelocity.Y,
                        ModContent.ProjectileType<BoneMaskWisp>(), damage, 0f, Main.myPlayer);

                        BoneWispTimer = 0;
                    }
                }
                else
                {
                    BoneWispTimer = 0;
                }
            }

			//handle stoned kidney charge for the UI
			if (StonedKidney)
			{
                bool PlayerHoldingWeapon = ItemGlobal.ActiveItem(Player).damage > 0 && ItemGlobal.ActiveItem(Player).pick <= 0 && ItemGlobal.ActiveItem(Player).hammer <= 0 && 
			    ItemGlobal.ActiveItem(Player).axe <= 0 && ItemGlobal.ActiveItem(Player).mountType <= 0;

				if ((!Player.controlUseItem || !PlayerHoldingWeapon) && StonedKidneyCharge < 7.5f)
				{
					StonedKidneyCharge += 0.05f;
				}
			}
			else
			{
				StonedKidneyCharge = 0;
			}

            //sentient cap random dialogue
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

			//handle cooldowns
            if (RootHealCooldown > 0)
            {
                RootHealCooldown--;
            }
            if (GooSlugEatCooldown > 0)
            {
                GooSlugEatCooldown--;
            }
            if (CandyBagCooldown > 0)
            {
                CandyBagCooldown--;
            }

			//set candy bag hit to false if you dont have the candy bag
			if (!CandyBag)
			{
				CandyBagJustHit = false;
			}
        }

        public override void PostUpdateMiscEffects()
		{
			if (PotionSicknessLatteTimer > 0)
			{
				PotionSicknessLatteTimer--;
			}
			if (PotionSicknessLatteTimer == 1)
			{
				int Duration = Player.pStone ? (int)(3600 * 0.75) : 3600;
				Player.AddBuff(BuffID.PotionSickness, Duration);
			}
		}

		public override void PreUpdateMovement()
		{
			// If the player can use our dash and has double tapped in a direction, then apply the dash
			if (CanUseDash())
			{
				Vector2 newVelocity = Player.velocity;

				switch (dashDir)
				{
					case dashLeft when Player.velocity.X > -dashVelocityX:
					case dashRight when Player.velocity.X < dashVelocityX:
					{
						float dashDirection = dashDir == dashRight ? 1 : -1;
						newVelocity.X = dashDirection * dashVelocityX;
						break;
					}
					case dashUp when Player.velocity.Y > -dashVelocityY:
					case dashDown when Player.velocity.Y < dashVelocityY:
					{
						float dashDirection = dashDir == dashDown ? 1 : -1;
						newVelocity.Y = dashDirection * dashVelocityY;
						break;
					}
					default:
					{
						return;
					}
				}

				SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Player.position);

				dashDelay = dashCooldown;
				dashTimer = dashDuration;

				Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<StitchedCloakWeb>(), 35, 0f, Main.myPlayer);

				Player.AddBuff(ModContent.BuffType<StitchedCloakCooldown>(), dashDelay);

				Player.velocity = newVelocity;
			}

			if (dashDelay > 0)
			{
				dashDelay--;
			}

			if (dashTimer > 0)
			{
				dashTimer--;

				int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Web, 0, 0, default, default, Main.rand.NextFloat(0.75f, 1.5f));
				Main.dust[dust].velocity *= 0;
			}
		}

		//check if you can dash
		private bool CanUseDash()
		{
			return StitchedCloak && Player.dashType == 0 && !Player.setSolar && dashDir != -1 && dashDelay == 0 && !Player.mount.Active;
		}

		public override void PostUpdateRunSpeeds()
        {
            if (SpiderSpeedTimer > 0)
            {
                Player.maxRunSpeed += 5f;
                Player.runAcceleration += 0.075f;
            }

			if (StitchedCloak)
			{
				Player.runAcceleration += 0.015f;
			}

            if (Player.HasBuff(ModContent.BuffType<GooseberryBoostBuff>()))
            {
                Player.maxRunSpeed += 3f;
                Player.runAcceleration += 0.015f;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff(ModContent.BuffType<SpiderArmorStealth>()))
            {
                if (SpiderStealthAlpha < 0.8f)
                {
                    SpiderStealthAlpha += 0.02f;
                }
            }
            else
            {
                if (SpiderStealthAlpha > 0f)
                {
                    SpiderStealthAlpha -= 0.02f;
                }
            }

            if (SpiderStealthAlpha > 0f)
            {
                r *= 1f - (SpiderStealthAlpha * 0.75f);
                g *= 1f - (SpiderStealthAlpha * 0.5f);
                b *= 1f - (SpiderStealthAlpha * 0.75f);
                a *= 1f - (SpiderStealthAlpha * 0.5f);
            }
        }

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}

			if (!drawInfo.drawPlayer.frozen && !drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.wet)
			{
				if (ItemGlobal.ActiveItem(drawInfo.drawPlayer).type == ModContent.ItemType<SentientLeafBlower>())
				{
					SentientLeafBlowerBackTex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Sentient/SentientLeafBlowerBack");

					SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

					int xOffset = 10;

					DrawData PlayerBack = new DrawData(SentientLeafBlowerBackTex.Value,
					new Vector2((int)(drawInfo.drawPlayer.position.X - Main.screenPosition.X + (drawInfo.drawPlayer.width / 2) - (xOffset * drawInfo.drawPlayer.direction)) - 4f * drawInfo.drawPlayer.direction, (int)(drawInfo.drawPlayer.position.Y - Main.screenPosition.Y + (drawInfo.drawPlayer.height / 2) + 2f * drawInfo.drawPlayer.gravDir - 8f * drawInfo.drawPlayer.gravDir + drawInfo.drawPlayer.gfxOffY)),
					new Rectangle(0, 0, SentientLeafBlowerBackTex.Width(), SentientLeafBlowerBackTex.Height()),
					drawInfo.colorArmorBody,
					drawInfo.drawPlayer.bodyRotation,
					new Vector2(SentientLeafBlowerBackTex.Width() / 2, SentientLeafBlowerBackTex.Height() / 2),
					1f, 
					spriteEffects, 
					0);

					PlayerBack.shader = 0;
					drawInfo.DrawDataCache.Add(PlayerBack);
				}
			}
		}


		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (!attempt.inLava && !attempt.inHoney)
            {
                if (Player.ZoneBeach && (Main.pumpkinMoon || Main.snowMoon) && attempt.playerFishingConditions.BaitItemType == ModContent.ItemType<SinisterSnailItem>())
				{
					npcSpawn = ModContent.NPCType<SpookFishron>();
					return;
				}

                //spooky forest catches
                if (Player.InModBiome<SpookyBiome>() || Player.InModBiome<SpookyBiomeUg>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<GourdFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<GourdFish>();

						return;
                    }
                    if (attempt.questFish == ModContent.ItemType<ZomboidFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<ZomboidFish>();

						return;
                    }

					//crate
					if (attempt.uncommon && attempt.crate)
					{
						itemDrop = ModContent.ItemType<SpookyCrate>();
                    }
                }

                if (Player.InModBiome<CemeteryBiome>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<SpookySpiritFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<SpookySpiritFish>();
                    }

					//crate
					if (attempt.uncommon && attempt.crate && Flags.downedSpookySpirit)
					{
						itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();
					}
				}

                if (Player.InModBiome<CatacombBiome>() || Player.InModBiome<CatacombBiome2>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<HibiscusFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<HibiscusFish>();
                    }

                    //crate
                    if (attempt.uncommon && attempt.crate)
                    {
                        itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();
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
                if (attempt.questFish == ModContent.ItemType<BoogerFish>() && attempt.uncommon)
				{
                    itemDrop = ModContent.ItemType<BoogerFish>();

                    return;
                }
                if (attempt.questFish == ModContent.ItemType<OrroEel>() && attempt.uncommon)
				{
                    itemDrop = ModContent.ItemType<OrroEel>();

                    return;
                }

				//crate
				if (attempt.uncommon && attempt.crate)
				{
					itemDrop = Main.hardMode ? ModContent.ItemType<SpookyHellCrate2>() : ModContent.ItemType<SpookyHellCrate>();
				}

				//do not allow blood lake enemy catches if any of the enemies already exist in the world
				bool BloodFishingEnemiesExist = NPC.AnyNPCs(ModContent.NPCType<ValleyFish>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyMerman>()) || 
                NPC.AnyNPCs(ModContent.NPCType<ValleySquid>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyNautilus>()) || 
                NPC.AnyNPCs(ModContent.NPCType<ValleyEelHead>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyShark>());

                if (!BloodFishingEnemiesExist && ItemGlobal.ActiveItem(Player).type == ModContent.ItemType<SentientChumCaster>())
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

        public static float PlayerSpeed(Player Player)
        {
            //all of these calculations are just copied from vanilla's stopwatch
            //too lazy to change all the num things tbh
            Vector2 SpeedVector = Player.velocity + Player.instantMovementAccumulatedThisFrame;

            if (Player.mount.Active && Player.mount.IsConsideredASlimeMount && Player.velocity != Vector2.Zero && !Player.SlimeDontHyperJump)
            {
                SpeedVector += Player.velocity;
            }

            Player.speedSlice[0] = SpeedVector.Length();

            int num15 = (int)(1f + SpeedVector.Length() * 6f);
            if (num15 > Player.speedSlice.Length)
            {
                num15 = Player.speedSlice.Length;
            }

            float num16 = 0f;
            for (int num17 = num15 - 1; num17 > 0; num17--)
            {
                Player.speedSlice[num17] = Player.speedSlice[num17 - 1];
            }

            Player.speedSlice[0] = SpeedVector.Length();
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

            return num20;
        }
    }
}