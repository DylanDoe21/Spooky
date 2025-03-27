using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigBone : ModNPC
    {
        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4 };

		public static int PhaseOneIconIndex;
		public static int PhaseTwoIconIndex;

		int ScaleTimerLimit = 12;
		int SaveDirection;
        
		float SaveRotation;
        float ScaleAmount = 0f;
        float RealScaleAmount = 0f;
		float stretch;

		bool SpawnIntro = false;
		bool BloomIntro = false;
		bool Phase2 = false;
        bool Transition = false;
        bool FlowersSpawned = false;
        bool ActuallyDead = false;

        Vector2 SavePlayerPosition;
        Vector2 SaveNPCPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> NeckTexture;
        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle BulbGrowlSound1 = new("Spooky/Content/Sounds/BigBone/BigBoneBulbGrowl1", SoundType.Sound);
        public static readonly SoundStyle BulbGrowlSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneBulbGrowl2", SoundType.Sound);
        public static readonly SoundStyle BulbGrowlSound3 = new("Spooky/Content/Sounds/BigBone/BigBoneBulbGrowl3", SoundType.Sound);
        public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl1", SoundType.Sound);
        public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl2", SoundType.Sound);
        public static readonly SoundStyle GrowlSound3 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl3", SoundType.Sound);
        public static readonly SoundStyle LaughSound = new("Spooky/Content/Sounds/BigBone/BigBoneLaugh", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle MagicCastSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/BigBone/BigBoneDeath", SoundType.Sound);
        public static readonly SoundStyle DeathSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneDeath2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BigBoneBestiary",
                Position = new Vector2(48f, -12f),
                PortraitPositionXOverride = 12f,
                PortraitPositionYOverride = -12f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //attack pattern
            writer.Write(AttackPattern[0]);
            writer.Write(AttackPattern[1]);
            writer.Write(AttackPattern[2]);
            writer.Write(AttackPattern[3]);
            writer.Write(AttackPattern[4]);

            //vector2
            writer.WriteVector2(SavePlayerPosition);
            writer.WriteVector2(SaveNPCPosition);

            //ints
            writer.Write(ScaleTimerLimit);
            writer.Write(SaveDirection);

			//bools
			writer.Write(SpawnIntro);
			writer.Write(BloomIntro);
			writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(FlowersSpawned);
            writer.Write(ActuallyDead);

            //floats
            writer.Write(SaveRotation);
            writer.Write(ScaleAmount);
            writer.Write(RealScaleAmount);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //attack pattern
            AttackPattern[0] = reader.ReadInt32();
            AttackPattern[1] = reader.ReadInt32();
            AttackPattern[2] = reader.ReadInt32();
            AttackPattern[3] = reader.ReadInt32();
            AttackPattern[4] = reader.ReadInt32();

            //vector2
            SavePlayerPosition = reader.ReadVector2();
            SaveNPCPosition = reader.ReadVector2();

            //ints
            ScaleTimerLimit = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();

            //bools
			SpawnIntro = reader.ReadBoolean();
			BloomIntro = reader.ReadBoolean();
			Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            FlowersSpawned = reader.ReadBoolean();
            ActuallyDead = reader.ReadBoolean();

            //floats
            SaveRotation = reader.ReadSingle();
            ScaleAmount = reader.ReadSingle();
            RealScaleAmount = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100000;
            NPC.damage = 70;
            NPC.defense = 50;
            NPC.width = 134;
            NPC.height = 134;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 30, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = DeathSound2;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/BigBone");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BigBone"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		internal static void LoadHeadIcons()
		{
			string PhaseOneIcon = "Spooky/Content/NPCs/Boss/BigBone/BigBoneIcon1";
			string PhaseTwoIcon = "Spooky/Content/NPCs/Boss/BigBone/BigBoneIcon2";

			Spooky.Instance.AddBossHeadTexture(PhaseOneIcon, -1);
			PhaseOneIconIndex = ModContent.GetModBossHeadSlot(PhaseOneIcon);

			Spooky.Instance.AddBossHeadTexture(PhaseTwoIcon, -1);
			PhaseTwoIconIndex = ModContent.GetModBossHeadSlot(PhaseTwoIcon);
		}

		public override void BossHeadSlot(ref int index)
		{
			index = Phase2 ? PhaseTwoIconIndex : PhaseOneIconIndex;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			DrawNeck(Phase2 ? 50 : 0, false);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			stretch = Math.Abs(stretch);

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), 
			NPC.rotation, NPC.frame.Size() / 2, scaleStretch, Phase2 ? effects : SpriteEffects.None, 0);

            return false;
        }

		public void DrawNeck(int NPCXDistance, bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[3]];

			if (Parent.active && Parent.type == ModContent.NPCType<BigFlowerPot>() && !SpawnGore)
			{
				NeckTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneNeck");

				bool flip = false;
				if (NPC.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, NeckTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(NPCXDistance * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = drawPosNext - drawPos2;
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(NeckTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)NeckTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				bool flip = false;
				if (NPC.direction == -1)
				{
					flip = true;
				}

				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);

					if (Main.netMode != NetmodeID.Server)
					{
						//Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/EarWormSegmentGore").Type);
					}
				}
			}
		}

        //add glowmask stuff here later
		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        }

        public override void FindFrame(int frameHeight)
        {
			if (!Phase2)
			{
				NPC.frame.Y = frameHeight * 0;
			}
            else
            {
                if (NPC.direction == -1)
                {
                    NPC.frame.Y = frameHeight * 1;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
		}

        public override bool CheckDead()
        {
            //death animation stuff
            if (!ActuallyDead)
            {
                //SoundEngine.PlaySound(DeathSound, NPC.Center);
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.life = 1;

                NPC.netUpdate = true;

                return false;
            }

            return true;
        }

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			if (!Phase2)
			{
				stretch = 0.06f;
			}
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (!Phase2)
			{
				stretch = 0.06f;
			}
		}

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = NPC.direction;

            /*
            //despawn if all players are dead
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome2>()))
            {
                NPC.EncourageDespawn(10);
            }
            */

            //do not exist if not attached to its flower pot parent
            if (!Parent.active || Parent.type != ModContent.NPCType<BigFlowerPot>())
            {
                NPC.active = false;
            }

			if (SpawnIntro)
			{
				//do spawn intro stuff here
			}

			if (BloomIntro)
			{
				//do big bone intro stuff here
				Phase2 = true;
				BloomIntro = false;
			}

			if (NPC.life < (NPC.lifeMax * 0.7f) && !Phase2)
			{
				BloomIntro = true;
			}

			//do not do any other AI if big bone is in his spawn or phase 2 transition intros
			if (!SpawnIntro && !BloomIntro)
			{
				//during big bones first phase, simply just idly move up and down, and growl/shake when using attacks
				if (!Phase2)
				{
					//change display name and hit sound
					NPC.GivenName = Language.GetTextValue("Mods.Spooky.NPCs.BigBone.DisplayName2");
					NPC.HitSound = SoundID.Grass;

					//urgh i need to find a better way to do this
					if (NPC.localAI[2] == 0)
					{
						NPC.ai[0] = NPC.position.Y;
						NPC.localAI[2]++;
					}

					NPC.ai[1]++;
					NPC.position.Y = NPC.ai[0] + (float)Math.Sin(NPC.ai[1] / 120) * 15;

					//when the flower pot is about to attack, big bone bulb should shake and play a muffled growl
					//probably not the best approach since this means big bone always assumes the parent localAI is 20 before using an attack, but that will always be the case anyway
					if (Parent.localAI[0] == 2)
					{
						SoundStyle[] Sounds = new SoundStyle[] { BulbGrowlSound1, BulbGrowlSound2, BulbGrowlSound3 };

						SoundEngine.PlaySound(Main.rand.Next(Sounds), NPC.Center);

						NPC.localAI[0] = 1;
					}

					//increase/decrease value to make big bone shake
					//localAI[0] = times to repeat, localAI[1] = timer for stretching
					int Repeats = 4;
					if (NPC.localAI[0] > 0 && NPC.localAI[0] < Repeats)
					{
						NPC.localAI[1]++;

						if (NPC.localAI[1] < 10 && stretch < 0.12f)
						{
							stretch += 0.02f;
						}
						if (NPC.localAI[1] >= 10 && stretch > -0.12f)
						{
							stretch -= 0.02f;
						}

						if (NPC.localAI[1] >= 20)
						{
							NPC.localAI[1] = 0;

							if (NPC.localAI[0] < Repeats - 1)
							{
								NPC.localAI[0]++;
							}
							else
							{
								NPC.localAI[0] = 0;
							}
						}
					}
					else
					{
						//if big bone isnt manually shaking before using an attack, manually increase/decrease stretch value to go back to big bones normal scale
						//this is because big bone has a very subtle stretching effect whenever he is damaged
						if (stretch > 0)
						{
							stretch -= 0.005f;
						}
						if (stretch < 0)
						{
							stretch += 0.005f;
						}
					}
				}
				else
				{
					NPC.GivenName = Language.GetTextValue("Mods.Spooky.NPCs.BigBone.DisplayName");
					NPC.HitSound = SoundID.DD2_SkeletonHurt;

					//EoC rotation
					Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
					float RotateX = player.Center.X - vector.X;
					float RotateY = player.Center.Y - vector.Y;
					NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

					stretch = 0f;

					switch ((int)NPC.ai[0])
					{
						//death animation
						case -1:
						{
							break;
						}

						//orange bouncy homing flower (similar to white flowers from phase 1)
						case 0:
						{
							break;
						}

						//charge up and shoot 3 homing fireballs (similar to before)
						case 1:
						{
							break;
						}

						//circle of thorns with gap (can be copy-pasted from old code and visually touched up)
						case 2:
						{
							break;
						}

						//slam head against the roof, creating falling brick debris from the roof
						case 3:
						{
							break;
						}

						//charge at the player with player velocity prediction (can be copy-pasted from old code)
						case 4:
						{
							break;
						}
					}
				}
			}
		}

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagBigBone>()));
            
            //relic and master pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BigBoneRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SkullSeed>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<BigBoneHammer>(), 
                ModContent.ItemType<BigBoneBow>(), 
                ModContent.ItemType<BigBoneStaff>(), 
                ModContent.ItemType<BigBoneScepter>() 
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BigBoneMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FlowerPotHead>(), 20));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BigBoneTrophyItem>(), 10));

            //sunflower bloom seed, drop directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SummerSeed>()));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
            {
                if (Main.player[numPlayer].active && !Main.player[numPlayer].GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4)
                {
                    int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<Slot4Unlocker>());

                    if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }
            }

            if (!Flags.downedBigBone)
            {
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.BigBoneDefeat");

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }

                Flags.GuaranteedRaveyard = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedBigBone, -1);

            if (!MenuSaveSystem.hasDefeatedBigBone)
            {
                MenuSaveSystem.hasDefeatedBigBone = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CranberryJuice>();
		}
    }
}