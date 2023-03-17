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

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Boss;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Vigilante : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public bool AfterImages = false;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vigilante");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 100;
            NPC.height = 84;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.EggEventBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Flying creatures who's purpose is to monitor and attack any threat to the egg. Their fast speed should not be taken lightly."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (AfterImages) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/VigilanteGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()))
            {
                return 15f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            //flying
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //charging frame
            if (NPC.localAI[0] >= 420)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            if (NPC.localAI[0] < 420)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
            }
            else
            {
                NPC.rotation = 0f;
            }

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 390)
            {
                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -45) 
                {
                    MoveSpeedX -= 2;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 45)
                {
                    MoveSpeedX += 2;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -45)
                {
                    MoveSpeedY -= 2;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 45)
                {
                    MoveSpeedY += 2;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.localAI[1] <= 1)
            {
                if (NPC.localAI[0] >= 390 && NPC.localAI[0] < 420)
                {
                    Vector2 GoTo = player.Center;
                    GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                    GoTo.Y -= 20;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                }

                if (NPC.localAI[0] == 420)
                {
                    AfterImages = true;

                    SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                    int ChargeSpeed = 20;

                    Vector2 ChargeDirection = player.Center - NPC.Center;
                    ChargeDirection.Normalize();
                            
                    ChargeDirection.X *= ChargeSpeed;
                    ChargeDirection.Y *= ChargeSpeed / 1.5f;
                    NPC.velocity.X = ChargeDirection.X;
                    NPC.velocity.Y = ChargeDirection.Y;
                }

                if (NPC.localAI[0] == 460)
                {
                    NPC.localAI[0] = 390;
                    NPC.localAI[1]++;
                }
            }
            else
            {
                AfterImages = false;

                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/VigilanteGore" + numGores).Type);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
    }
}