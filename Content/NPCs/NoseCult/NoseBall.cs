using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseBallPurple : ModNPC  
    {
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Volume = 0.35f };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.width = 52;
			NPC.height = 52;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 2f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.65f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseBallPurple"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation += (NPC.velocity.X / 40);

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -12, 12);

			if (NPC.collideX || NPC.collideY)
			{
				if (NPC.velocity.X != NPC.oldVelocity.X)
				{
					NPC.position.X = NPC.position.X + NPC.velocity.X;
					NPC.velocity.X = -NPC.oldVelocity.X * 1.05f;
				}
				if (NPC.velocity.Y != NPC.oldVelocity.Y)
				{
					NPC.position.Y = NPC.position.Y + NPC.velocity.Y;
					NPC.velocity.Y = -NPC.oldVelocity.Y * Main.rand.NextFloat(0.8f, 1.5f);
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/NoseBallPurpleGore" + numGores).Type);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			var parameters = new DropOneByOne.Parameters() 
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 2,
				MaximumItemDropsCount = 4,
			};

			npcLoot.Add(new DropOneByOne(ItemID.Heart, parameters));
        }
    }

    public class NoseBallRed : NoseBallPurple  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseBallRed"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/NoseBallRedGore" + numGores).Type);
                    }
                }
            }
        }
    }
}