using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.NPCs.EggEvent.Projectiles;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.NPCs.EggEvent
{
	public class FleshBolster : ModNPC
	{
        float addedStretch = 0f;
		float stretchRecoil = 0f;
        
        private static Asset<Texture2D> NPCTexture;

		public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode2", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/FleshBolsterBestiary",
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
        }

		public override void SetDefaults()
		{
			NPC.lifeMax = 1500;
            NPC.damage = 45;
			NPC.defense = 20;
			NPC.width = 76;
			NPC.height = 104;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = DeathSound;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FleshBolster"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

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

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw npc manually for stretching
			spriteBatch.Draw(NPCTexture.Value, new Vector2(NPC.Center.X, NPC.Center.Y + 56) - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            bool PlayerLineOfSight = Collision.CanHitLine(player.Center - new Vector2(10, 10), 20, 20, NPC.position, NPC.width, NPC.height);
            if (player.Distance(NPC.Center) <= 450f && PlayerLineOfSight)
            {
                NPC.ai[0]++;
                if (NPC.ai[0] % 60 == 0)
                {
					stretchRecoil = 0.35f;

                    int Pos1 = Main.rand.Next(1, 11);
                    int Pos2 = (Main.rand.NextBool() ? -1 : 1);
                    int RandomPosition = Pos1 * 45 * Pos2;

                    Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y - 100);
                    center.X += RandomPosition; //45 is the distance between each one
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

                    if (numtries <= 10)
                    {
                        Vector2 lineDirection = new Vector2(-(Pos1 * Pos2) * 2, 16);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(center.X, center.Y + 30), Vector2.Zero, ModContent.ProjectileType<FleshPillar>(), NPC.damage, 4.5f, 
                        ai0: lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                    }
                }
            }
        }
		
		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn gores
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/FleshBolsterGore" + numGores).Type);
                    }
                }

				//spawn splatter
				int NumProjectiles = Main.rand.Next(15, 25);
				for (int i = 0; i < NumProjectiles; i++)
				{
					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 0)), ModContent.ProjectileType<RedSplatter>(), 0, 0f);
				}
			}
		}
    }
}