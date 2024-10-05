using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;
using Spooky.Content.Projectiles.Blooms;
using Spooky.Content.UserInterfaces;
using Mono.Cecil;
using Spooky.Content.Projectiles.Sentient;
using Terraria.GameContent.ItemDropRules;

namespace Spooky.Core
{
    //separate modplayer for all of the bloom buffs since there is a ton of them and i dont feel like cluttering SpookyPlayer
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
        public bool DandelionHerd = false;
        public bool DandelionMapleSeed = false;
        public bool DandelionTumbleweed = false;
        public bool Dragonfruit = false;

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

        //UI default position
        public Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

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
                //if every single buff slot is filled and both slot 3 and slot 4 are locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && !UnlockedSlot3 && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled and slot 4 is locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled, dont allow the player to consume a bloom
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
            DandelionHerd = BloomBuffSlots.Contains("DandelionHerd");
            DandelionMapleSeed = BloomBuffSlots.Contains("DandelionMapleSeed");
            DandelionTumbleweed = BloomBuffSlots.Contains("DandelionTumbleweed");
            Dragonfruit = BloomBuffSlots.Contains("Dragonfruit");
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
                BloomBuffSlots[0] = string.Empty;
            }

            if (Duration2 > 0)
            {
                Duration2--;
            }
            else
            {
                BloomBuffSlots[1] = string.Empty;
            }

            //automatically set the string to empty if the player doesnt have the additional 3rd slot unlocked
            if (Duration3 > 0 && UnlockedSlot3)
            {
                Duration3--;
            }
            else
            {
                BloomBuffSlots[2] = string.Empty;
            }

            //automatically set the string to empty if the player doesnt have the additional 4th slot unlocked
            if (Duration4 > 0 && UnlockedSlot4)
            {
                Duration4--;
            }
            else
            {
                BloomBuffSlots[3] = string.Empty;
            }

            //set the duration of a buff slot to 0 if the slot is empty
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

        //save/load the position of the players UI position so it doesnt reset when leaving the world and save/load unlocked slots so they are actually saved per player
        public override void SaveData(TagCompound tag)
        {
            tag["UITopLeft"] = UITopLeft;

            tag["BloomBuffSlot1"] = BloomBuffSlots[0];
            tag["BloomBuffSlot2"] = BloomBuffSlots[1];
            tag["BloomBuffSlot3"] = BloomBuffSlots[2];
            tag["BloomBuffSlot4"] = BloomBuffSlots[3];

			tag["Duration1"] = Duration1;
			tag["Duration2"] = Duration2;
			tag["Duration3"] = Duration3;
			tag["Duration4"] = Duration4;

			if (UnlockedSlot3) tag["UnlockedSlot3"] = true;
            if (UnlockedSlot4) tag["UnlockedSlot4"] = true;
        }
        public override void LoadData(TagCompound tag)
        {
            UITopLeft = tag.Get<Vector2>("UITopLeft");

            BloomBuffSlots[0] = tag.Get<string>("BloomBuffSlot1");
			BloomBuffSlots[1] = tag.Get<string>("BloomBuffSlot2");
			BloomBuffSlots[2] = tag.Get<string>("BloomBuffSlot3");
			BloomBuffSlots[3] = tag.Get<string>("BloomBuffSlot4");

			Duration1 = tag.Get<int>("Duration1");
			Duration2 = tag.Get<int>("Duration2");
			Duration3 = tag.Get<int>("Duration3");
			Duration4 = tag.Get<int>("Duration4");

			UnlockedSlot3 = tag.ContainsKey("UnlockedSlot3");
            UnlockedSlot4 = tag.ContainsKey("UnlockedSlot4");
        }

        public override void PreUpdate()
        {
			GivePlayerBloomBonus();
            HandleBloomBuffDuration();

			//open the bloom buff UI if you have any bloom buff at all, if not then close it
			//instead of just appearing, make the UI fade in for a cool effect if the player eats a bloom
			if (BloomBuffSlots[0] == string.Empty && BloomBuffSlots[1] == string.Empty && BloomBuffSlots[2] == string.Empty && BloomBuffSlots[3] == string.Empty)
			{
				if (BloomBuffUI.Transparency > 0f)
				{
					BloomBuffUI.Transparency -= 0.05f;
				}
			}
			else
			{
				//draw the bloom UI fully when the players inventory is not open
				if (!Main.playerInventory)
				{
					if (BloomBuffUI.Transparency < 1f)
					{
						BloomBuffUI.Transparency += 0.05f;
					}
				}
				//fade out a little if the players inventory is open
				else
				{
					if (BloomBuffUI.Transparency > 0.5f)
					{
						BloomBuffUI.Transparency -= 0.05f;
					}
					if (BloomBuffUI.Transparency < 0.5f)
					{
						BloomBuffUI.Transparency += 0.05f;
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

                if (FallSoulPumpkinTimer > 360 && Main.rand.NextBool(25))
                {
                    Projectile.NewProjectile(null, new Vector2(Player.Center.X + Main.rand.Next(-30, 30), Player.Center.Y + Main.rand.Next(-50, -30)), 
                    Vector2.Zero, ModContent.ProjectileType<GhastlyPumpkin>(), 30, 0, Player.whoAmI);

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

                    if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 450f)
                    {
                        FallZucchiniTimer++;

                        if (FallZucchiniTimer == 300 || FallZucchiniTimer == 320 || FallZucchiniTimer == 340)
                        {
                            Vector2 ShootSpeed = NPC.Center - Player.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 8;

                            Projectile.NewProjectile(null, Player.Center, ShootSpeed, ModContent.ProjectileType<ZucchiniLightning>(), 20, 3, Player.whoAmI, ShootSpeed.ToRotation());
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
					Projectile.NewProjectile(null, Player.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)), ModContent.ProjectileType<MapleSeedBoost>(), 0, 0, Player.whoAmI);

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
        }

        public override void PostUpdate()
        {
            //fall gourd increases damage if you are falling
			if (FallGourd && Player.velocity.Y > 0f)
			{
				Player.GetDamage(DamageClass.Generic) += 0.15f;
			}
        }

		public override void UpdateLifeRegen()
		{
			//give the player additional life regeneration for each bloom slot in use
			if (SpringHeartFlower)
			{
				if (BloomBuffSlots[0] != string.Empty)
				{
					Player.lifeRegen += 5;
				}
				if (BloomBuffSlots[1] != string.Empty)
				{
					Player.lifeRegen += 5;
				}
				if (BloomBuffSlots[2] != string.Empty)
				{
					Player.lifeRegen += 5;
				}
				if (BloomBuffSlots[3] != string.Empty)
				{
					Player.lifeRegen += 5;
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

				Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<BlueberryExplosion>(), 0, 0, Player.whoAmI);
			}

			//spawn gooseberries on every third enemy hit with a 33% chance
			if (WinterGooseberry)
			{
				WinterGooseberryHits++;

				if (WinterGooseberryHits > 2)
				{
					if (Main.rand.NextBool(3))
					{
						Projectile.NewProjectile(null, target.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-5, -2)), ModContent.ProjectileType<GooseberryBoost>(), 0, 0, Player.whoAmI);
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

				Projectile.NewProjectile(null, new Vector2(RandomPosX, RandomPosY), ShootSpeed, ModContent.ProjectileType<Tumbleweed>(), hit.Damage + 30, 0, Player.whoAmI);
			}

			base.OnHitNPC(target, hit, damageDone);
		}
	}
}