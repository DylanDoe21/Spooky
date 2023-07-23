using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ValleyFish : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 600;
            NPC.damage = 40;
            NPC.defense = 18;
            NPC.width = 38;
            NPC.height = 34;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit8;
			NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ValleyFish"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (NPC.ai[0] == 1) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ValleyFishGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;
            
            NPC.rotation = NPC.velocity.ToRotation();

            if (NPC.spriteDirection == 1)
            {
                NPC.rotation += MathHelper.Pi;
            }

            /*
            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            */

            switch ((int)NPC.ai[0])
            {
                //fly to the player for a short time
                case 0:
                {
                    NPC.localAI[0]++;

                    //flies to players X position
                    if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -65) 
                    {
                        MoveSpeedX--;
                    }
                    else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 65)
                    {
                        MoveSpeedX++;
                    }

                    NPC.velocity.X = MoveSpeedX * 0.1f;
                    
                    //flies to players Y position
                    if (NPC.Center.Y >= player.Center.Y && MoveSpeedY >= -30)
                    {
                        MoveSpeedY -= 2;
                    }
                    else if (NPC.Center.Y <= player.Center.Y && MoveSpeedY <= 30)
                    {
                        MoveSpeedY += 2;
                    }

                    NPC.velocity.Y = MoveSpeedY * 0.1f;

                    if (NPC.localAI[0] >= 300)
                    {
                        MoveSpeedX = 0;
                        MoveSpeedY = 0;

                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //go to a location and then charge at the player
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -280 : 280;
                        GoTo.Y -= 50;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 3, 8);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        NPC.velocity *= 0.2f;
                    }

                    if (NPC.localAI[0] == 70)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection *= 35;
                        NPC.velocity = ChargeDirection;
                    }

                    if (NPC.localAI[0] >= 80)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    if (NPC.localAI[0] >= 100)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                        
                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //vampire frog staff
            npcLoot.Add(ItemDropRule.Common(ItemID.VampireFrogStaff, 5));

            //blood rain bow
            npcLoot.Add(ItemDropRule.Common(ItemID.BloodRainBow, 5));

            //chum buckets
            npcLoot.Add(ItemDropRule.Common(ItemID.ChumBucket, 1, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleyFishGore" + numGores).Type);
                }
            }
        }
    }
}