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
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class TomatoRoman : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Pitch = -0.65f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 25;
			NPC.defense = 5;
			NPC.width = 54;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoRoman"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (NPC.IsABestiaryIconDummy ? 1 : (int)NPC.ai[0]) * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.Distance(player.Center) <= 300f && NPC.ai[0] == 0)
            {
                Vector2 JumpTo = player.Center;

                int JumpHeight = 250;

                NPC.velocity = ArcVelocityHelper.GetArcVelocity(NPC, JumpTo, 0.35f, JumpHeight, JumpHeight + 1, maxXvel: 10);

                for (int numDusts = 0; numDusts <= 10; numDusts++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.BottomLeft, NPC.width, 1, DustID.Dirt, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, Color.White, 2.5f);
                    dust.color = Color.White;
                    dust.noGravity = true;
                }

                NPC.ai[0]++;
                NPC.netUpdate = true;
            }

            if (NPC.ai[0] == 1)
            {   
                NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.02f * (NPC.velocity.X < 0 ? -1 : 1);

                NPC.aiStyle = -1;

                NPC.noGravity = true;

                NPC.velocity.Y += 0.25f;
                if (NPC.velocity.Y > 2f)
                {
                    NPC.velocity.Y += 0.25f;
                }

                NPC.ai[1]++;
                if (NPC.ai[1] >= 10 && NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
                {
                    SoundEngine.PlaySound(SplatSound, NPC.Center);

                    for (int numDusts = 0; numDusts <= 10; numDusts++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.BottomLeft, NPC.width, 1, DustID.TintableDust, Main.rand.NextFloat(-4f, 4f), 0f, 100, Color.Red, 2.5f);
                        dust.noGravity = true;
                    }

                    NPC.ai[0]++;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.ai[0] == 2)
            {
                NPC.rotation = 0;
                NPC.aiStyle = 0;

                NPC.noGravity = false;

                NPC.velocity.X = 0;
            }
        }
        
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TomatoSeed>(), 120));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
            }
        }
    }
}