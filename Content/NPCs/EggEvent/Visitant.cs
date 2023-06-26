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
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Visitant : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(25f, -5f),
                PortraitPositionXOverride = 8f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 55;
            NPC.defense = 15;
            NPC.width = 94;
            NPC.height = 74;
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Visitant"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/VisitantGlow").Value;

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

            //spitting frame
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
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -35) 
                {
                    MoveSpeedX -= 2;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 35)
                {
                    MoveSpeedX += 2;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -50)
                {
                    MoveSpeedY -= 2;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 50)
                {
                    MoveSpeedY += 2;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.localAI[0] >= 390 && NPC.localAI[0] < 420)
            {
                Vector2 GoTo = player.Center;
                GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                GoTo.Y -= 20;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }

            //shoot biomass
            if (NPC.localAI[0] == 420 || NPC.localAI[0] == 440 || NPC.localAI[0] == 460)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                NPC.velocity *= 0f;

                Vector2 ShootSpeed = player.Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed.X *= 4.5f;
                ShootSpeed.Y *= 4.5f;
                
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, 
                ShootSpeed.Y, ModContent.ProjectileType<Biomass>(), NPC.damage / 2, 1, NPC.target, 0, 0);
            }

            if (NPC.localAI[0] >= 500)
            {
                NPC.localAI[0] = 0;
            }
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
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/VisitantGore" + numGores).Type);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
    }
}