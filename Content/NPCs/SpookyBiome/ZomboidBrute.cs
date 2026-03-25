using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidBrute : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 2.5f
			};
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 450;
            NPC.damage = 55;
            NPC.defense = 15;
            NPC.width = 36;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.15f;
            NPC.value = Item.buyPrice(0, 0, 1, 50);
            NPC.HitSound = SoundID.NPCHit21 with { Pitch = 0.5f };
			NPC.DeathSound = SoundID.NPCDeath2 with { Volume = 0.5f, Pitch = -0.5f };
            NPC.aiStyle = 3;
			AIType = NPCID.GiantWalkingAntlion;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidBrute"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeNight_Background", Color.White)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            if (NPC.velocity != Vector2.Zero)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 6 * frameHeight;
            }
        }
        
        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[2]++;
            if (NPC.localAI[2] >= 300)
			{
				NPC.localAI[0]++;

				//jumping velocity
				Vector2 JumpTo = new Vector2(NPC.Center.X + (NPC.Center.X > player.Center.X ? -300 : 300), NPC.Center.Y - Main.rand.Next(350, 451));

				Vector2 velocity = JumpTo - NPC.Center;

				//actual jumping
				if (NPC.localAI[0] == 60)
				{
					NPC.aiStyle = -1;

					SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -0.75f }, NPC.Center);

					float speed = MathHelper.Clamp(velocity.Length() / 36, 10, 25);
					velocity.Normalize();
					velocity.Y -= 0.22f;
					velocity.X *= 1.05f;
					NPC.velocity = velocity * speed * 1.1f;

					NPC.netUpdate = true;
				}

				//fall down a bit before slamming
				if (NPC.localAI[0] > 60 && NPC.localAI[0] < 115)
				{
					NPC.velocity.Y += 0.5f;
				}

				//lower velocity before and while slaming down
				if (NPC.localAI[0] > 90)
				{
					NPC.velocity.Y += 0.25f;
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0f, 20f);

					NPC.velocity.X *= 0.95f;

					if (NPC.localAI[0] < 115 && NPC.Center.X <= player.Center.X + 3 && NPC.Center.X >= player.Center.X - 3)
					{
						NPC.localAI[0] = 115;
					}
				}

				//slam down
				if (NPC.localAI[0] == 115)
				{
					NPC.noGravity = true;
				}

				//set tile collide to true once it gets to the players level to prevent cheesing
				if (NPC.localAI[0] >= 115)
				{
					NPC.velocity.Y += 0.5f;
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0f, 20f);
				}

				//slam the ground
				if (NPC.localAI[0] >= 65 && NPC.localAI[1] == 0 && NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
				{
					NPC.noGravity = false;

					NPC.velocity = Vector2.Zero;

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 250f);

                    SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound with { Pitch = 1.5f }, NPC.Center);
					SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = -1.2f }, NPC.Center);

					for (int i = -3; i <= 3; i++)
					{
						Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
						center.X += 45 * i; //45 is the distance between each one
						int numtries = 0;
						int x = (int)(center.X / 16);
						int y = (int)(center.Y / 16);
						while (y < NPC.Center.Y + 20 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
						Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
						{
							y++;
							center.Y = y * 16;
						}

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(center.X, center.Y - 10), Vector2.Zero, ModContent.ProjectileType<ZomboidBruteSlam>(), NPC.damage, 4.5f);
					}
					
					//complete the slam attack
					NPC.localAI[1] = NPC.localAI[0];
				}

				//only loop attack if the jump has been completed
				if (NPC.localAI[0] >= NPC.localAI[1] + 20 && NPC.localAI[1] > 0)
				{
                    NPC.aiStyle = 3;
                    NPC.noGravity = false;

					NPC.localAI[0] = 30;
					NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;

					NPC.netUpdate = true;
				}
			}
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SharkToothNecklace, 150));
            npcLoot.Add(ItemDropRule.Common(ItemID.MoneyTrough, 200));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidBruteGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class ZomboidBruteTomato : ZomboidBrute  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidBruteTomato"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y - 10), new Vector2(0, Main.rand.Next(-15, -9)), 
                ModContent.ProjectileType<ZomboidTomatoHead>(), NPC.damage, 4.5f, ai0: 0);

                for (int numGores = 2; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidBruteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}