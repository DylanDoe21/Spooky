using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
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
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.Boss.Moco.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Moco
{
    [AutoloadBossHead]
    public class Moco : ModNPC
    {
        int CurrentFrameX = 0;
        int SaveDirection;

        float RandomGoToX;
        float RandomGoToY;

        bool Phase2 = false;
        bool Transition = false;
        bool Sneezing = false;
        bool SwitchedSides = false;
        bool AfterImages = false;

        Vector2 SaveNPCPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound);
        public static readonly SoundStyle SneezeSound1 = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);
        public static readonly SoundStyle SneezeSound2 = new("Spooky/Content/Sounds/Moco/MocoSneeze2", SoundType.Sound);
        public static readonly SoundStyle SneezeSound3 = new("Spooky/Content/Sounds/Moco/MocoSneeze3", SoundType.Sound);
        public static readonly SoundStyle AngrySound = new("Spooky/Content/Sounds/Moco/MocoAngry", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(CurrentFrameX);
            writer.Write(SaveDirection);

            //bools
            writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(Sneezing);
            writer.Write(SwitchedSides);
            writer.Write(AfterImages);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            CurrentFrameX = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            Sneezing = reader.ReadBoolean();
            SwitchedSides = reader.ReadBoolean();
            AfterImages = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4200;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 78;
            NPC.height = 120;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit22;
			NPC.DeathSound = SoundID.NPCDeath60;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Moco");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Moco"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 10)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            
            return false;
        }

        public override void AI()
        {   
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            int Damage = Main.masterMode ? 50 / 3 : Main.expertMode ? 40 / 2 : 30;

            NPC.rotation = NPC.velocity.X * 0.01f;
            NPC.spriteDirection = NPC.direction;

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.ai[0] = -2;
            }

            //set to transition
            if (NPC.life < (NPC.lifeMax / 2) && !Phase2 && NPC.ai[0] != -1)
            {
                NPC.ai[0] = -1;
                NPC.localAI[0] = 0;
            }

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

			switch ((int)NPC.ai[0])
			{
				//despawning
				case -2:
				{
					AfterImages = true;
					NPC.velocity.Y = -25;
					NPC.EncourageDespawn(10);

					break;
				}

				//phase transition
				case -1:
				{
					break;
				}

                //chase the player (for now)
                case 0:
				{
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(FlyingSound, NPC.Center);

                        RandomGoToX = Main.rand.NextFloat(-150f, 150f);
                        RandomGoToY = Main.rand.NextFloat(-170f, -100f);
                    }

                    if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 70)
                    {
                        MoveToPlayer(player, RandomGoToX, RandomGoToY);
                    }
                    else
                    {
                        NPC.velocity = NPC.velocity * 0.92f;
                    }

                    if (NPC.localAI[0] >= 160)
                    {
                        NPC.localAI[0] = 58;
                    }

                    break;
                }
			}
		}

        //super fast movement to create a fly-like movement effect
        //TODO: pair this with afterimages to make it look super cool
        public void MoveToPlayer(Player target, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = target.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }

        public void MoveToParent(NPC parent, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = parent.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagMoco>()));

            //master relic and pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MocoRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MocoTissue>(), 4));

            //weapon drops
            int[] MainItem = new int[]
            { 
                ModContent.ItemType<BoogerFlail>(), 
                ModContent.ItemType<BoogerBlaster>(), 
                ModContent.ItemType<BoogerBook>(),
                ModContent.ItemType<BoogerStaff>()
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MocoMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MocoTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore" + numGores).Type);
                    }
                }
            }
        }

        public override void OnKill()
        {
            //drop a sentient heart for each active player in the world
            if (!Flags.downedMoco)
            {
                for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
                {
                    if (Main.player[numPlayer].active)
                    {
                        int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<SentientHeart>());

                        if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                        }
                    }
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedMoco, -1);

            if (!MenuSaveSystem.hasDefeatedMoco)
            {
                MenuSaveSystem.hasDefeatedMoco = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
    }
}