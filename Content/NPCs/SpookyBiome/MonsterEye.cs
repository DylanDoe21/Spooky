using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Pets;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class MonsterEye1 : ModNPC
	{
        bool HasJumped = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/MonsterEyeBestiary1",
                Rotation = MathHelper.PiOver2 + 12,
                PortraitPositionYOverride = -20f
            };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 20;
			NPC.defense = 0;
			NPC.width = 42;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 1.2f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MonsterEye1"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			float stretch = NPC.velocity.Y * 0.025f;

			stretch = Math.Abs(stretch);

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			
			if (NPC.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}
			if (NPC.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, scaleStretch, effects, 0);

			return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //face towards the player
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 250, 50, 0);
        }

        public void JumpToTarget(Player target, int JumpHeight, int TimeBeforeNextJump, int DelayBeforeNextJump)
        {
            NPC.ai[0]++;

            //set where the it should be jumping towards
            Vector2 JumpTo = new(target.Center.X, NPC.Center.Y - JumpHeight);

            //set velocity and speed
            Vector2 velocity = JumpTo - NPC.Center;
            velocity.Normalize();

            int JumpSpeed = Main.rand.Next(13, 18);

            float speed = MathHelper.Clamp(velocity.Length() / 36, 10, JumpSpeed);

            NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

            //actual jumping
            if (NPC.ai[0] >= TimeBeforeNextJump)
            {
                NPC.ai[1]++;

                if (NPC.velocity == Vector2.Zero && !HasJumped)
                {
                    if (NPC.ai[1] == 10)
                    {
                        if (target.Distance(NPC.Center) <= 450f)
                        {
                            SoundEngine.PlaySound(SoundID.GlommerBounce, NPC.Center);
                        }
                        
                        velocity.Y -= 0.25f;
                        
                        HasJumped = true;
                    }
                }
                
                if (NPC.ai[1] < 15 && HasJumped)
                {
                    NPC.velocity = velocity * speed;
                }
            }

            //loop ai
            if (NPC.ai[0] >= TimeBeforeNextJump + 100)
            {
                HasJumped = false;

                NPC.ai[0] = DelayBeforeNextJump;
                NPC.ai[1] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Lens, 3, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StickyEye>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 12; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, (NPC.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunk").Type);
                    }
                }
            }
        }
	}

    public class MonsterEye2 : MonsterEye1
	{
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/MonsterEyeBestiary2",
                PortraitPositionYOverride = -20f
            };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MonsterEye2"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            //face towards the player
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 375, 50, Main.rand.Next(15, 30));
        }
    }

    public class MonsterEye3 : MonsterEye1
	{
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/MonsterEyeBestiary3",
                PortraitPositionYOverride = -20f
            };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MonsterEye3"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            //face towards the player
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 180, 10, 0);
        }
    }

    public class MonsterEye4 : MonsterEye1
	{
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/MonsterEyeBestiary4",
                PortraitPositionYOverride = -20f
            };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MonsterEye4"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            //face towards the player
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 800, 75, Main.rand.Next(0, 40));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 12; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, (NPC.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunkFaded").Type);
                    }
                }
            }
        }
    }
}