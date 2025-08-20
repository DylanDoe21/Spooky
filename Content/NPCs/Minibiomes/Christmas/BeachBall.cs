using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class BeachBall : ModNPC  
    {
		public static readonly SoundStyle BounceSound = new("Spooky/Content/Sounds/BeachBallBounce", SoundType.Sound);

        public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 44;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath63;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BeachBall"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void AI()
		{
            if (NPC.ai[0] == 0)
            {
                NPC.velocity = new Vector2(Main.rand.Next(-7, 8), Main.rand.Next(-8, -3));
                NPC.ai[0] = 1;
            }

            NPC.rotation += (NPC.velocity.X / 40);

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10, 10);

			if (NPC.collideX || NPC.collideY)
			{
                if (Main.LocalPlayer.Distance(NPC.Center) <= 600f)
                {
                    SoundEngine.PlaySound(BounceSound, NPC.Center);
                }

				if (NPC.velocity.X != NPC.oldVelocity.X)
				{
					NPC.position.X = NPC.position.X + NPC.velocity.X;
					NPC.velocity.X = -NPC.oldVelocity.X * 1.05f;
				}
				if (NPC.velocity.Y != NPC.oldVelocity.Y)
				{
					NPC.position.Y = NPC.position.Y + NPC.velocity.Y;
					NPC.velocity.Y = -NPC.oldVelocity.Y * 1.05f;
				}

                if (NPC.velocity.Y > -3 && NPC.velocity.Y < 0)
                {
                    NPC.velocity.Y = -5;
                }
                if (NPC.velocity.Y < 3 && NPC.velocity.Y > 0)
                {
                    NPC.velocity.Y = 5;
                }
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-8, 9), Main.rand.Next(-8, 9)), ModContent.Find<ModGore>("Spooky/BeachBallGore" + numGores).Type);
                    }
                }
            }
        }
    }
}