using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilEye : ModNPC
    {
        public bool SpawnedHands = false;

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Daffodil/DaffodilBC",
                Position = new Vector2(1f, 0f),
                PortraitPositionXOverride = 2f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 0;
            NPC.defense = 35;
            NPC.width = 58;
            NPC.height = 56;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Daffodil"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilEyePupil");

            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = new Vector2(NPC.Center.X, NPC.Center.Y + 2) - screenPos;

            float lookX = (player.Center.X - NPC.Center.X) * 0.015f;
            float lookY = (player.Center.Y - NPC.Center.Y) * 0.01f;

            drawPos.X += lookX;
            drawPos.Y += lookY;

            spriteBatch.Draw(texture, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            Lighting.AddLight(NPC.Center, 0.5f, 0.45f, 0f);

            if (!SpawnedHands)
            {
                NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DaffodilHandLeft>(), ai2: NPC.whoAmI);
                NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DaffodilHandRight>(), ai3: NPC.whoAmI);
                
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                }

                SpawnedHands = true;
            }

            switch ((int)NPC.ai[0])
            {
                //solar laser barrage
                //spawns a telegraph on the player, then shoot a barrage of beams in that area for a second
                case 0:
                {
                    break;
                }

                //chlorophyll blasts
                //move hands to the sides, shoot chlorophyll blasts from each hand (ai for this attack will be mostly handled in the hand's ai)
                case 1:
                {
                    break;
                }

                //thorn bulbs
                //shoot out thorn pods that each spawn a thorn when they hit the ground, thorns will linger for a bit, and when hit the thorn will die
                //thorns will inflict bleeding and poison on contact
                case 2:
                {
                    break;
                }

                //seed drop
                //drop seeds randomly around the arena that turn into short lived thorn pillars that form upward upon hitting the floor
                //based on that one kirby boss attack
                case 3:
                {
                    break;
                }

                //fly swarm
                //boss roars or makes a sound, then flies begin flying around the arena in a straight line in a random bullet hell fashion (like supreme calamitas)
                //omega flowey attack reference, but also works because the catacombs is filled with flies due to being a burial for the dead
                case 4:
                {
                    break;
                }

                //hand attack
                //either swipe at the player or grab the player and pull them up, can be avoided by going under the outposts in the arena
                case 5:
                {
                    break;
                }

                //solar beam
                //shoot sweeping solar beams across the arena
                //the only way to avoid this attack is by hiding under the outposts in the arena
                case 6:
                {
                    break;
                }
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedDaffodil, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}