using Microsoft.Xna.Framework;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Boss.SpookFishron.Projectiles;
using Spooky.Content.Projectiles.Blooms;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Spooky.Core
{
    //separate modplayer for all of the bloom buffs since there is a ton of them and i dont feel like cluttering SpookyPlayer more than it already is
    public class BloomBuffsPlayer : ModPlayer
    {
        //list of strings for each buff slot
        //each consumable bloom adds its name to a slot in this list of strings, and then each bonus is applied if that string is in the list
        //also used in the bloom UI so that it can draw each respective buff icon on it
        public string[] BloomBuffSlots = new string[4];

        //durations for each buff slot
        public int Duration1 = 0;
        public int Duration2 = 0;
        public int Duration3 = 0;
        public int Duration4 = 0;

        public float UITransparency = 0f;

		//bools for each edible bloom
        public bool FallGourd = false;
        public bool FallSoulPumpkin = false;
        public bool FallWaterGourd = false;
        public bool FallZucchini = false;
        public bool WinterBlackberry = false;
        public bool WinterBlueberry = false;
        public bool WinterGooseberry = false;
        public bool WinterStrawberry = false;
        public bool SpringHeartFlower = false;
        public bool SpringIris = false;
        public bool SpringOrchid = false;
        public bool SpringRose = false;
        public bool SummerLemon = false;
        public bool SummerOrange = false;
        public bool SummerPineapple = false;
        public bool SummerSunflower = false;
		public bool VegetableCauliflower = false;
		public bool VegetableEggplantPaint = false;
		public bool VegetablePepper = false;
		public bool VegetableRomanesco = false;
		public bool DandelionHerd = false;
        public bool DandelionMapleSeed = false;
		public bool DandelionTumbleweed = false;
		public bool Dragonfruit = false;
		public bool SeaBarnacle = false;
		public bool SeaCucumber = false;
		public bool SeaSponge = false;
		public bool SeaUrchin = false;
		public bool FossilBlackPepper = false;
		public bool FossilDutchmanPipe = false;
		public bool FossilMagnolia = false;
		public bool FossilProtea = false;
		public bool CemeteryLily = false;
		public bool CemeteryMarigold = false;
		public bool CemeteryPoppy = false;
		public bool CemeteryRose = false;

		//slot unlocks
		public bool UnlockedSlot3 = false;
		public bool UnlockedSlot4 = false;

		//misc stuff
		public int FallSoulPumpkinTimer = 0;
		public int FallZucchiniTimer = 0;
        public int WinterGooseberryHits = 0;
		public int WinterStrawberryTimer = 0;
        public int SpringIrisTimer = 0;
        public int SummerLemonsShot = 0;
        public int SummerLemonDelay = 0;
		public int DandelionHerdTimer = 0;
		public int DandelionMapleSeedTimer = 0;
		public int DragonFruitTimer = 0;
		public int DragonfruitStacks = 0;
		public int CemeteryLilyRevives = 0;
		public int FossilBlackPepperTimer = 0;
		public int FossilBlackPepperStacks = 0;
		public int CemeteryMarigoldTimer = 0;
		public bool FossilProteaSlammed = false;

		//accessories
		public bool Wormy = false;
		public bool FarmerGlove = false;
		public bool TheMask = false;
		public bool DaylightSavings = false;

		//UI default position
		public Vector2 BloomUIPos = new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 20f * Main.UIScale);

		public static readonly SoundStyle SpongeSound = new("Spooky/Content/Sounds/SpongeAbsorb", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SaveData(TagCompound tag)
        {
            tag[nameof(BloomUIPos)] = BloomUIPos;

            tag["BloomBuffSlot1"] = BloomBuffSlots[0];
            tag["BloomBuffSlot2"] = BloomBuffSlots[1];
            tag["BloomBuffSlot3"] = BloomBuffSlots[2];
            tag["BloomBuffSlot4"] = BloomBuffSlots[3];

			tag["Duration1"] = Duration1;
			tag["Duration2"] = Duration2;
			tag["Duration3"] = Duration3;
			tag["Duration4"] = Duration4;
            tag["DragonfruitStacks"] = DragonfruitStacks;
			tag["CemeteryLilyRevives"] = CemeteryLilyRevives;

			if (UnlockedSlot3) tag["UnlockedSlot3"] = true;
            if (UnlockedSlot4) tag["UnlockedSlot4"] = true;
        }
        
        public override void LoadData(TagCompound tag)
        {
			if (tag.ContainsKey(nameof(BloomUIPos)))
			{
				BloomUIPos = tag.Get<Vector2>(nameof(BloomUIPos));
			}

			BloomBuffSlots[0] = tag.Get<string>("BloomBuffSlot1");
			BloomBuffSlots[1] = tag.Get<string>("BloomBuffSlot2");
			BloomBuffSlots[2] = tag.Get<string>("BloomBuffSlot3");
			BloomBuffSlots[3] = tag.Get<string>("BloomBuffSlot4");

			Duration1 = tag.Get<int>("Duration1");
			Duration2 = tag.Get<int>("Duration2");
			Duration3 = tag.Get<int>("Duration3");
			Duration4 = tag.Get<int>("Duration4");
            DragonfruitStacks = tag.Get<int>("DragonfruitStacks");
			CemeteryLilyRevives = tag.Get<int>("CemeteryLilyRevives");

			UnlockedSlot3 = tag.ContainsKey("UnlockedSlot3");
            UnlockedSlot4 = tag.ContainsKey("UnlockedSlot4");
        }

		public override void ResetEffects()
        {
			Wormy = false;
			FarmerGlove = false;
			TheMask = false;
			DaylightSavings = false;
		}

        //global bool used for each individual bloom item so that they cannot be eaten if all of your slots are filled
        public bool CanConsumeFruit(string BuffName)
        {
            //if the player eats a bloom they already have active, allow it to be used so its duration can be added to its exsiting buff duration
            if (BloomBuffSlots.Contains(BuffName))
            {
                return true;
            }
            //conditions to account for when the player still has locked bloom slots
            else
            {
                //if every single buff slot is filled and both slot 3 and slot 4 are locked, dont allow the player to consume any new blooms
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && !UnlockedSlot3 && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled and slot 4 is locked, dont allow the player to consume any new blooms
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled, dont allow the player to consume any new blooms
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && BloomBuffSlots[3] != string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        //when the player consumes a bloom, add that blooms name to a buff list slot and set its duration in that specific slot
        public void AddBuffToList(string BuffName, int Duration)
        {
            int TimeMultiplier = 1;
            
            //winter blackberry has a chance to cause blooms besides itself to have double the durations
            if (WinterBlackberry && BuffName != "WinterBlackberry" && Main.rand.NextBool(5))
            {
                TimeMultiplier = 2;
            }

            //if the player consumes a bloom they already have, then add that blooms base duration to the existing bloom buffs duration
            //also cap out the maximum duration for every single bloom buff at 72000 (which is 20 minutes in real time)
            if (BloomBuffSlots.Contains(BuffName))
            {
                if (BloomBuffSlots[0] == BuffName)
                {
                    Duration1 += Duration * TimeMultiplier;

                    if (Duration1 > 72000)
                    {
                        Duration1 = 72000;
                    }
                }
                else if (BloomBuffSlots[1] == BuffName)
                {
                    Duration2 += Duration * TimeMultiplier;

                    if (Duration2 > 72000)
                    {
                        Duration2 = 72000;
                    }
                }
                else if (BloomBuffSlots[2] == BuffName && UnlockedSlot3)
                {
                    Duration3 += Duration * TimeMultiplier;

                    if (Duration3 > 72000)
                    {
                        Duration3 = 72000;
                    }
                }
                else if (BloomBuffSlots[3] == BuffName && UnlockedSlot4)
                {
                    Duration4 += Duration * TimeMultiplier;

                    if (Duration4 > 72000)
                    {
                        Duration4 = 72000;
                    }
                }

                return;
            }

            //if the buff is not already in the players slots, then add the buff to the list by checking each slot to see if its open, and if it is add that buff to that slot
            //only attempt to check beyond the second slot when the player has each unlockable slot unlocked
            if (BloomBuffSlots[0] == string.Empty)
            {
                BloomBuffSlots[0] = BuffName;
                Duration1 = Duration * TimeMultiplier;
            }
            else if (BloomBuffSlots[1] == string.Empty)
            {
                BloomBuffSlots[1] = BuffName;
                Duration2 = Duration * TimeMultiplier;
            }
            else if (BloomBuffSlots[2] == string.Empty && UnlockedSlot3)
            {
                BloomBuffSlots[2] = BuffName;
                Duration3 = Duration * TimeMultiplier;
            }
            else if (BloomBuffSlots[3] == string.Empty && UnlockedSlot4)
            {
                BloomBuffSlots[3] = BuffName;
                Duration4 = Duration * TimeMultiplier;
            }
        }

        //manually set the bools for each bonus if the list of buffs contains that buff name
        public void GivePlayerBloomBonus()
        {
            FallGourd = BloomBuffSlots.Contains("FallGourd");
            FallSoulPumpkin = BloomBuffSlots.Contains("FallSoulPumpkin");
            FallWaterGourd = BloomBuffSlots.Contains("FallWaterGourd");
            FallZucchini = BloomBuffSlots.Contains("FallZucchini");
            WinterBlackberry = BloomBuffSlots.Contains("WinterBlackberry");
            WinterBlueberry = BloomBuffSlots.Contains("WinterBlueberry");
            WinterGooseberry = BloomBuffSlots.Contains("WinterGooseberry");
            WinterStrawberry = BloomBuffSlots.Contains("WinterStrawberry");
            SpringHeartFlower = BloomBuffSlots.Contains("SpringHeartFlower");
            SpringIris = BloomBuffSlots.Contains("SpringIris");
            SpringOrchid = BloomBuffSlots.Contains("SpringOrchid");
            SpringRose = BloomBuffSlots.Contains("SpringRose");
            SummerLemon = BloomBuffSlots.Contains("SummerLemon");
            SummerOrange = BloomBuffSlots.Contains("SummerOrange");
			SummerPineapple = BloomBuffSlots.Contains("SummerPineapple");
			SummerSunflower = BloomBuffSlots.Contains("SummerSunflower");
			VegetableCauliflower = BloomBuffSlots.Contains("VegetableCauliflower");
			VegetableEggplantPaint = BloomBuffSlots.Contains("VegetableEggplantPaint");
			VegetablePepper = BloomBuffSlots.Contains("VegetablePepper");
			VegetableRomanesco = BloomBuffSlots.Contains("VegetableRomanesco");
			DandelionHerd = BloomBuffSlots.Contains("DandelionHerd");
            DandelionMapleSeed = BloomBuffSlots.Contains("DandelionMapleSeed");
            DandelionTumbleweed = BloomBuffSlots.Contains("DandelionTumbleweed");
            Dragonfruit = BloomBuffSlots.Contains("Dragonfruit");
			SeaBarnacle = BloomBuffSlots.Contains("SeaBarnacle");
			SeaCucumber = BloomBuffSlots.Contains("SeaCucumber");
			SeaSponge = BloomBuffSlots.Contains("SeaSponge");
			SeaUrchin = BloomBuffSlots.Contains("SeaUrchin");
			FossilBlackPepper = BloomBuffSlots.Contains("FossilBlackPepper");
			FossilDutchmanPipe = BloomBuffSlots.Contains("FossilDutchmanPipe");
			FossilMagnolia = BloomBuffSlots.Contains("FossilMagnolia");
			FossilProtea = BloomBuffSlots.Contains("FossilProtea");
			CemeteryLily = BloomBuffSlots.Contains("CemeteryLily");
			CemeteryMarigold = BloomBuffSlots.Contains("CemeteryMarigold");
			CemeteryPoppy = BloomBuffSlots.Contains("CemeteryPoppy");
			CemeteryRose = BloomBuffSlots.Contains("CemeteryRose");
        }

        //handler for the buffs duration decreasing over time and setting each buff slot back to blank if the duration of that buff slot runs out
        public void HandleBloomBuffDuration()
        {
            if (Duration1 > 0)
            {
                Duration1--;
            }
            else
            {
				if (DaylightSavings && Main.rand.NextBool(5))
				{
					Duration1 = 7200;
				}
				else
				{
                	BloomBuffSlots[0] = string.Empty;
				}
            }

            if (Duration2 > 0)
            {
                Duration2--;
            }
            else
            {
				if (DaylightSavings && Main.rand.NextBool(5))
				{
					Duration2 = 7200;
				}
				else
				{
                	BloomBuffSlots[1] = string.Empty;
				}
            }

            //automatically set the string to empty if the player doesnt have the additional 3rd slot unlocked
            if (Duration3 > 0 && UnlockedSlot3)
            {
                Duration3--;
            }
            else
            {
                if (DaylightSavings && Main.rand.NextBool(5))
				{
					Duration3 = 7200;
				}
				else
				{
                	BloomBuffSlots[2] = string.Empty;
				}
            }

            //automatically set the string to empty if the player doesnt have the additional 4th slot unlocked
            if (Duration4 > 0 && UnlockedSlot4)
            {
                Duration4--;
            }
            else
            {
                if (DaylightSavings && Main.rand.NextBool(5))
				{
					Duration4 = 7200;
				}
				else
				{
                	BloomBuffSlots[3] = string.Empty;
				}
            }

            //set the duration of each buff slot to 0 if the slot is empty
			if (BloomBuffSlots[0] == string.Empty)
			{
				Duration1 = 0;
			}
			if (BloomBuffSlots[1] == string.Empty)
			{
				Duration2 = 0;
			}
			if (BloomBuffSlots[2] == string.Empty)
			{
				Duration3 = 0;
			}
			if (BloomBuffSlots[3] == string.Empty)
			{
				Duration4 = 0;
			}
        }

		public override void OnHurt(Player.HurtInfo info)
		{
			//fire off venom spines when hit with the sea urchin
			if (SeaUrchin)
			{
				ModContent.GetInstance<HeadUrchin>().stretchRecoil = Main.rand.NextFloat(0.5f, 0.75f);

				int MinDamage = Main.raining ? 50 : 30; //minimum damage, increases while raining
				float DamageMultiplier = Main.raining ? 1f : 0.75f; //damage multiplier, while raining it does full damage

				float Damage = info.Damage < MinDamage ? MinDamage * DamageMultiplier : info.Damage * DamageMultiplier;

				for (int numProjectiles = -2; numProjectiles <= 2; numProjectiles++)
				{
					Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), Player.Top, 12f * Player.DirectionTo(Player.Top).RotatedBy(MathHelper.ToRadians(12) * numProjectiles),
					ModContent.ProjectileType<HeadUrchinSpike>(), (int)Damage, 4.5f, Player.whoAmI);
				}
			}

			//cemetery rose spawns a circle of withered petals on hit
			if (CemeteryRose)
			{
				SoundEngine.PlaySound(SoundID.Zombie21 with { Volume = 0.5f, Pitch = 1.05f }, Player.Center);

				int MinDamage = 30; //minimum damage
				float DamageMultiplier = 0.5f; //damage multiplier, each petal should do half damage

				float Damage = info.Damage < MinDamage ? MinDamage * DamageMultiplier : info.Damage * DamageMultiplier;

				float maxAmount = 5;
				int currentAmount = 0;
				while (currentAmount <= maxAmount)
				{
					Vector2 velocity = new Vector2(5f, 5f);
					Vector2 Bounds = new Vector2(3f, 3f);
					float intensity = 5f;

					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (5f / maxAmount)), default) * Bounds;
					vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
					Vector2 ShootVelocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

					Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), Player.Center, ShootVelocity, ModContent.ProjectileType<CemeteryRosePetal>(), (int)Damage, 4.5f, Player.whoAmI);

					currentAmount++;
				}
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool ShouldPlayerDie = true;

			//spring lily revive ability
			if (Player.statLife <= 0)
			{
				if (CemeteryLily)
				{
					Player.statLife = Player.statLifeMax2;

					if (CemeteryLilyRevives > 0)
					{
						SoundEngine.PlaySound(SoundID.Item103, Player.Center);
						Player.AddBuff(ModContent.BuffType<CemeteryLilyBuff>(), 900);
						Player.immuneTime += 60;
						Player.statLife = Player.statLifeMax2;

						CemeteryLilyRevives--;
						ShouldPlayerDie = false;
					}
				}
			}

			return ShouldPlayerDie;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			//reset every single slot and duration when the player dies
			BloomBuffSlots[0] = string.Empty;
			BloomBuffSlots[1] = string.Empty;
			BloomBuffSlots[2] = string.Empty;
			BloomBuffSlots[3] = string.Empty;

			Duration1 = 0;
			Duration2 = 0;
			Duration3 = 0;
			Duration4 = 0;
		}

		public override void PreUpdate()
        {
			GivePlayerBloomBonus();
            HandleBloomBuffDuration();

			//open the bloom buff UI if you have any bloom buff at all, if not then close it
			//instead of just appearing, make the UI fade in for a cool effect if the player eats a bloom
			if (BloomBuffSlots[0] == string.Empty && BloomBuffSlots[1] == string.Empty && BloomBuffSlots[2] == string.Empty && BloomBuffSlots[3] == string.Empty)
			{
				if (UITransparency > 0f)
				{
					UITransparency -= 0.05f;
				}
			}
			else
			{
				//draw the bloom UI fully when the players inventory is not open
				if (!Main.playerInventory)
				{
					if (UITransparency < 1f)
					{
						UITransparency += 0.05f;
					}
				}
				//fade out a little if the players inventory is open
				else
				{
					if (UITransparency > 0.5f)
					{
						UITransparency -= 0.05f;
					}
					if (UITransparency < 0.5f)
					{
						UITransparency += 0.05f;
					}
				}
			}

            //decrease lemon shoot delay time
            if (SummerLemonDelay > 0)
            {
                SummerLemonDelay--;
            }

            //spawn soul pumpkins around the player somewhat randomly 
            if (FallSoulPumpkin && Player.ownedProjectileCounts[ModContent.ProjectileType<GhastlyPumpkin>()] < 1)
            {
                FallSoulPumpkinTimer++;

				bool HoldingWeapon = ItemGlobal.ActiveItem(Player).damage > 0 && ItemGlobal.ActiveItem(Player).pick <= 0 && ItemGlobal.ActiveItem(Player).hammer <= 0 &&
				ItemGlobal.ActiveItem(Player).axe <= 0 && ItemGlobal.ActiveItem(Player).mountType <= 0;

                if (Main.rand.NextBool(25) && FallSoulPumpkinTimer > 300 && HoldingWeapon)
                {
					//minimum of 30 damage, otherwise scale with the weapon the player is holding
					int Damage = ItemGlobal.ActiveItem(Player).damage >= 30 ? ItemGlobal.ActiveItem(Player).damage : 30;

					Projectile.NewProjectile(null, new Vector2(Player.Center.X + Main.rand.Next(-30, 30), Player.Center.Y + Main.rand.Next(-50, -30)), 
                    Vector2.Zero, ModContent.ProjectileType<GhastlyPumpkin>(), Damage, 0, Player.whoAmI);

					FallSoulPumpkinTimer = 0;
                }
            }
            else
			{
				FallSoulPumpkinTimer = 0;
			}

            //create lightning projectiles with the zucchini
			if (FallZucchini)
			{
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];

                    if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 500f)
                    {
                        FallZucchiniTimer++;

                        if (FallZucchiniTimer == 300 || FallZucchiniTimer == 320 || FallZucchiniTimer == 340)
                        {
                            Vector2 ShootSpeed = NPC.Center - Player.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 8;

							int Damage = (int)(NPC.damage);

							Projectile.NewProjectile(null, Player.Center, ShootSpeed, ModContent.ProjectileType<ZucchiniLightning>(), Damage, 3, Player.whoAmI, ShootSpeed.ToRotation());
                        }

                        if (FallZucchiniTimer >= 360)
                        {
                            FallZucchiniTimer = 0;
                        }

                        break;
                    }
                }
			}
			else
			{
				FallZucchiniTimer = 0;
			}

            //spawn multiple strawberry items with the winter strawberry
			if (WinterStrawberry)
			{
				WinterStrawberryTimer++;

				if (WinterStrawberryTimer >= 2400)
				{
                    SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.2f }, Player.Center);

					for (int numBerries = 0; numBerries < 3; numBerries++)
					{
                        Projectile.NewProjectile(null, Player.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), ModContent.ProjectileType<StrawberryBoost>(), 0, 0, Player.whoAmI);
					}

					WinterStrawberryTimer = 0;
				}
			}
			else
			{
				WinterStrawberryTimer = 0;
			}

            //spawn the iris eye lock on and petal projectile every minute 
            if (SpringIris && Player.ownedProjectileCounts[ModContent.ProjectileType<IrisPetalLockOn>()] < 1)
            {
                SpringIrisTimer++;

                if (SpringIrisTimer >= 3600)
                {
                    Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<IrisPetalLockOn>(), 50, 0, Player.whoAmI);

                    SpringIrisTimer = 0;
                }
            }

            //spawn rose thorn projectile on the player
			if (SpringRose && Player.ownedProjectileCounts[ModContent.ProjectileType<RoseThornRing>()] < 1)
			{
				Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<RoseThornRing>(), 40, 0, Player.whoAmI);
			}

            //spawn an orbiting orange
			if (SummerOrange && Player.ownedProjectileCounts[ModContent.ProjectileType<OrangeOrbiter>()] < 1)
			{
                SoundEngine.PlaySound(SoundID.DD2_BetsySummon with { Pitch = 0.75f, Volume = 0.1f }, Player.Center);

                Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<OrangeOrbiter>(), 150, 0, Player.whoAmI, Main.rand.Next(0, 360));
			}

			//spawn dandelion herd clusters while the player is flying, the timer itself is handled in ItemGlobal to account for the player using wings
			if (DandelionHerd)
			{
				int[] Types = { ModContent.ProjectileType<DandelionFloater1>(), ModContent.ProjectileType<DandelionFloater2>(), ModContent.ProjectileType<DandelionFloater3>() };

				if (DandelionHerdTimer % 30 == 0 && DandelionHerdTimer > 0)
				{
					Projectile.NewProjectile(null, Player.Center, new Vector2(0, Main.rand.Next(1, 3)), Main.rand.Next(Types), 20 + (Player.wingTimeMax / 10), 0, Player.whoAmI);

                    DandelionHerdTimer = 0;
				}
			}
			else
			{
				DandelionHerdTimer = 0;
			}

			//spawn maple seed boosters while the player is flying, the timer itself is handled in ItemGlobal to account for the player using wings
			if (DandelionMapleSeed)
			{
				if (DandelionMapleSeedTimer % 80 == 0 && DandelionMapleSeedTimer > 0)
				{
					Projectile.NewProjectile(null, Player.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)), ModContent.ProjectileType<MapleSeedBooster>(), 0, 0, Player.whoAmI);

                    DandelionMapleSeedTimer = 0;
				}
			}
			else
			{
				DandelionMapleSeedTimer = 0;
			}

			//spawn orbiting dragon fruits around the player and spawn more with each stack the player has
			if (Dragonfruit && Player.ownedProjectileCounts[ModContent.ProjectileType<DragonfruitOrbiter>()] < DragonfruitStacks)
			{
				DragonFruitTimer++;

				if (DragonFruitTimer >= 120)
				{
                    SoundEngine.PlaySound(SoundID.NPCDeath42 with { Pitch = 0.75f, Volume = 0.1f }, Player.Center);

                    int numOrbiters = Player.ownedProjectileCounts[ModContent.ProjectileType<DragonfruitOrbiter>()];

					int DistanceFromPlayer = 20 * (numOrbiters + 1);

					Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<DragonfruitOrbiter>(), 60, 3, Player.whoAmI, Main.rand.Next(0, 2), Main.rand.Next(0, 360), DistanceFromPlayer);

					DragonFruitTimer = 0;
				}
			}
			else
			{
				DragonFruitTimer = 0;
			}
			//automatically remove all dragonfruit stacks and reset the timer if the player doesnt have the buff active
			if (!Dragonfruit)
			{
				DragonfruitStacks = 0;
				DragonFruitTimer = 0;
			}

			//spawn cucumber stomach when a valid enemy is nearby
			if (SeaCucumber)
			{
				NPC Target = null;

				//prioritize bosses over normal enemies
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC NPC = Main.npc[i];

					if (Target != null)
					{
						break;
					}

					if (NPC.active && NPC.CanBeChasedBy(Player) && !NPC.friendly && !NPC.dontTakeDamage && (NPC.boss || NPC.IsTechnicallyBoss()) && Vector2.Distance(Player.Center, NPC.Center) <= 400f)
					{
						Target = NPC;
						break;
					}
				}

				//target an enemy
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC NPC = Main.npc[i];

					if (Target != null)
					{
						break;
					}

					//if no boss is found, target other enemies normally
					if (NPC.active && NPC.CanBeChasedBy(Player) && !NPC.friendly && !NPC.dontTakeDamage && !NPC.boss && !NPC.IsTechnicallyBoss() && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 400f)
					{
						Target = NPC;
						break;
					}
				}
				
				if (Target != null)
				{
					if (Player.ownedProjectileCounts[ModContent.ProjectileType<CucumberStomach>()] < 1)
					{
						SoundEngine.PlaySound(SoundID.Item111 with { Pitch = -1f }, Player.Center);

						Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<CucumberStomach>(), 0, 0, Player.whoAmI, 0, 0, Target.whoAmI);
					}
				}
			}

			//sea sponge gives you a chance to absorb hostile projectiles
			if (SeaSponge)
			{
				foreach (var Proj in Main.ActiveProjectiles)
				{
					if (Player.Distance(Proj.Center) < 100f && Proj.hostile && Proj.damage > 0 && Proj.CanHitWithOwnBody(Player) && !Proj.GetGlobalProjectile<ProjectileGlobal>().SpongeAbsorbAttempt)
					{
						int ChanceToAbsorb = Main.raining ? 18 : 25;

						if (Main.rand.NextBool(ChanceToAbsorb))
						{
							SoundEngine.PlaySound(SpongeSound, Proj.Center);

							Player.AddBuff(ModContent.BuffType<SeaSpongeBuff>(), 600);

							Projectile.NewProjectile(null, Proj.Center, Vector2.Zero, ModContent.ProjectileType<SeaSpongeProj>(), 0, 0, Player.whoAmI);

							Proj.Kill();
						}

						Proj.GetGlobalProjectile<ProjectileGlobal>().SpongeAbsorbAttempt = true;
					}
				}
			}

			//spawn multiple strawberry items with the winter strawberry
			if (FossilBlackPepper)
			{
				if (FossilBlackPepperStacks < 10)
				{
					FossilBlackPepperTimer++;

					int TimeToSpawnPebble = 900 + Player.statDefense;

					if (FossilBlackPepperTimer >= TimeToSpawnPebble)
					{
						Projectile.NewProjectile(null, Player.Center, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -2)), ModContent.ProjectileType<FossilBlackPepperPebble>(), 0, 0, Player.whoAmI, ai2: Main.rand.Next(0, 2));

						FossilBlackPepperTimer = 0;
					}
				}
			}
			else
			{
				FossilBlackPepperTimer = 0;
				FossilBlackPepperStacks = 0;
			}

			//dutchman pipe gives nearby enemies in the aura a debuff so they take more damage
			if (FossilDutchmanPipe)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC NPC = Main.npc[i];

					if (NPC.active && NPC.CanBeChasedBy(Player) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 240f)
					{
						NPC.AddBuff(ModContent.BuffType<DutchmanPipeDebuff>(), 2);
					}
				}
			}

			//fossil protea slam shakes the screen and flings diamond projectiles everywhere
			if (FossilProtea)
			{
				if (Collision.SolidCollision(Player.BottomLeft, Player.width, 30, true) && !FossilProteaSlammed && Player.velocity.Y >= 10)
				{
					SoundEngine.PlaySound(SoundID.NPCDeath43 with { Pitch = -1.2f, Volume = 0.25f }, Player.Center);

					Screenshake.ShakeScreenWithIntensity(Player.Center, 6f, 100f);

					for (int numProjs = 0; numProjs < Player.velocity.Y / 2; numProjs++)
					{
						Vector2 velocity = new Vector2(0, -Player.velocity.Y / 2.75f).RotatedByRandom(MathHelper.ToRadians(55));

						Projectile.NewProjectile(null, Player.Bottom + new Vector2(Main.rand.Next(-20, 21), 0), velocity + new Vector2(0, Main.rand.Next(-3, 6)), 
						ModContent.ProjectileType<FossilProteaDiamond>(), (int)Player.velocity.Y * 2, 0, Player.whoAmI);
					}

					FossilProteaSlammed = true;
				}

				if (FossilProteaSlammed && Player.velocity.Y < 0)
				{
					FossilProteaSlammed = false;
				}
			}
			else
			{
				FossilProteaSlammed = false;
			}

			//spawn growing marigolds with the cemetery marigold
			if (CemeteryMarigold)
			{
				//do not shoot under 10mph
				if (SpookyPlayer.PlayerSpeedToMPH(Player) >= 10)
				{
					CemeteryMarigoldTimer++;

					if (CemeteryMarigoldTimer >= 75)
					{
						SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { Volume = 0.5f, Pitch = 1.2f }, Player.Center);

						//scale the damage based on the player's current speed
						int damage = 40 + ((int)SpookyPlayer.PlayerSpeedToMPH(Player) / 3);

						int Marigold = Projectile.NewProjectile(null, Player.Bottom, new Vector2(0, -6), ModContent.ProjectileType<CemeteryMarigoldProj>(), damage, 0f, Player.whoAmI);
						Main.projectile[Marigold].scale = 0f;

						CemeteryMarigoldTimer = 0;
					}
				}
			}
			else
			{
				CemeteryMarigoldTimer = 0;
			}
		}

		public override void PostUpdateEquips()
		{
			//fall gourd increases damage if you are falling
			if (FallGourd && Player.velocity.Y > 0f)
			{
				Player.GetDamage(DamageClass.Generic) += 0.15f;
			}

			//vegetable class specific damage increases
			if (VegetablePepper)
			{
				Player.GetDamage(DamageClass.Melee) += 0.1f;
			}
			if (VegetableEggplantPaint)
			{
				Player.GetDamage(DamageClass.Ranged) += 0.1f;
			}
			if (VegetableRomanesco)
			{
				Player.GetDamage(DamageClass.Magic) += 0.1f;
			}
			if (VegetableCauliflower)
			{
				Player.GetDamage(DamageClass.Summon) += 0.1f;
			}

			//magnolia increases player damage reduction based on players current health remaining
			if (FossilMagnolia)
			{
				if (Player.statLife < Player.statLifeMax2)
				{
					float DefenseAmount = 0.45f * (1f - ((float)Player.statLife / Player.statLifeMax2));
					Player.endurance += DefenseAmount;
				}
			}

			//fossil black pepper gives defense based on the stack count, 2 defense for each stack
			if (FossilBlackPepper && FossilBlackPepperStacks > 0)
			{
				Player.statDefense += FossilBlackPepperStacks * 2;
			}

			//fossil protea gives flat defense increase
			if (FossilProtea)
			{
				Player.noFallDmg = true;
				Player.statDefense += 5;
				Player.maxFallSpeed = 22f;
			}

			//lily bloom cuts the players health down to 1/3rd
			if (CemeteryLily)
			{
				Player.statLifeMax2 /= 3;
				if (Player.statLife > Player.statLifeMax2)
				{
					Player.statLife = Player.statLifeMax2;
				}

				if (CemeteryLilyRevives <= 0)
				{
					for (int i = 0; i < BloomBuffSlots.Length; i++) 
					{
						if (BloomBuffSlots[i] == "CemeteryLily")
						{
							BloomBuffSlots[i] = string.Empty;
						}
					}
				}
			}

			//player takes 15% more damage with the cemetery rose
			if (CemeteryRose)
			{
				Player.endurance -= 0.15f;
			}

			//farmer glove grants attack speed for each occupied bloom buff slot
			if (FarmerGlove)
			{
				foreach (string var in BloomBuffSlots)
				{
					if (var != string.Empty)
					{
						Player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
					}
				}
			}

			//mask grants defense for each occupied bloom buff slot
			if (TheMask)
			{
				foreach (string var in BloomBuffSlots)
				{
					if (var != string.Empty)
					{
						Player.statDefense += 5;
					}
				}
			}
		}

		public override void UpdateLifeRegen()
		{
			//give the player additional life regeneration for each bloom slot in use
			if (SpringHeartFlower)
			{
				foreach (string var in BloomBuffSlots)
				{
					if (var != string.Empty)
					{
						Player.lifeRegen += 5;
					}
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			//increase all crit damage with the poker pineapple
			if (SummerPineapple)
			{
				modifiers.CritDamage += 1.35f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			//if the player lands a crit with the poker pineapple, then heal them and produce some pineapple dusts for a cool effect
			if (SummerPineapple && hit.Crit)
			{
				int randomHealAmount = Main.rand.Next(5, 11);

				Player.statLife += randomHealAmount;
				Player.HealEffect(randomHealAmount, true);

				for (int numDusts = 0; numDusts < 6; numDusts++)
				{
					Vector2 vel = Main.rand.NextVector2Circular(2, 4);
					vel.Y = MathF.Abs(vel.Y) * -1;
					Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.Next(-24, 24), 0), ModContent.DustType<PineappleDust>(), vel, 0, default, 1f);
				}
			}

			//create frost explosion and inflict enemies with blueberry frost with the blueberry
			if (WinterBlueberry && Main.rand.NextBool(18) && !target.HasBuff(ModContent.BuffType<BlueberryFrost>()))
			{
				SoundEngine.PlaySound(SoundID.Item101, target.Center);

				target.AddBuff(ModContent.BuffType<BlueberryFrost>(), 480);

				Projectile.NewProjectile(target.GetSource_OnHurt(Player), target.Center, Vector2.Zero, ModContent.ProjectileType<BlueberryExplosion>(), 0, 0, Player.whoAmI);
			}

			//spawn gooseberries on every third enemy hit with a 33% chance
			if (WinterGooseberry)
			{
				WinterGooseberryHits++;

				if (WinterGooseberryHits > 2)
				{
					if (Main.rand.NextBool(3))
					{
						Projectile.NewProjectile(target.GetSource_OnHurt(Player), target.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-5, -2)), ModContent.ProjectileType<GooseberryBoost>(), 0, 0, Player.whoAmI);
					}

					WinterGooseberryHits = 0;
				}
			}

			//spawn tumbleweeds randomly on hit while flying
			if (DandelionTumbleweed && Player.wingTime > 0 && Player.velocity.Y < 0 && Main.rand.NextBool(10))
			{
				int RandomPosX = (int)target.Center.X + (Main.rand.NextBool() ? -1200 : 1200);
				int RandomPosY = (int)target.Center.Y + Main.rand.Next(-150, 150);

				Vector2 ShootSpeed =  (target.Center + target.velocity * 22) - new Vector2(RandomPosX, RandomPosY);
				ShootSpeed.Normalize();
				ShootSpeed *= 55f;

				Projectile.NewProjectile(target.GetSource_OnHurt(Player), new Vector2(RandomPosX, RandomPosY), ShootSpeed, ModContent.ProjectileType<Tumbleweed>(), hit.Damage + 30, hit.Knockback, Player.whoAmI);
			}

			//pepper inflicts enemies with pepper spice on melee hits
			if (VegetablePepper && Main.rand.NextBool(15) && hit.DamageType == DamageClass.Melee)
			{
				target.AddBuff(ModContent.BuffType<PepperSpice>(), 600);
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			//spawn cauliflower on hit with whips with the vegetable cauliflower
			//had to do it in OnHitNPCWithProj so that the projectile can use the whips base damage since they have damage falloff when they hit mutliple enemies
			if (VegetableCauliflower && Main.rand.NextBool(10) && hit.DamageType == DamageClass.SummonMeleeSpeed)
			{
				Projectile.NewProjectile(target.GetSource_OnHurt(Player), target.Center, new Vector2(0, -5), ModContent.ProjectileType<Cauliflower>(), proj.damage, proj.knockBack, Player.whoAmI, Main.rand.Next(0, 3));
			}

			//sea barnacle creates little damaging barnacles on enemies
			if (SeaBarnacle && Main.rand.NextBool(15) && proj.type != ModContent.ProjectileType<Barnacle>() && Player.ownedProjectileCounts[ModContent.ProjectileType<Barnacle>()] < 10)
			{
				int randomX = Main.rand.Next(-target.width / 3, target.width / 3);
				int randomY = Main.rand.Next(-target.height / 3, target.height / 3);

				Projectile.NewProjectile(target.GetSource_OnHurt(Player), target.Center + new Vector2(randomX, randomY), Vector2.Zero,
				ModContent.ProjectileType<Barnacle>(), proj.damage / 2, hit.Knockback, Player.whoAmI, ai0: target.whoAmI, ai1: randomX, ai2: randomY);
			}
		}
	}
}