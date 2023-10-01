using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Spooky.Core;

namespace Spooky.Content.NPCs.Friendly
{
    public class PartySkeleton1 : ModNPC  
    {
        int shirtStyle = 0;
        int shirtLogoStyle = 0;
        int pantsStyle = 0;
        int dialogueStyle = 0;

        bool hasShirt = false;
        bool shirtLogo = false;
        bool hasPants = false;
        bool hasSunGlasses = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
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
            NPC.lifeMax = 200;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 46;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
            TownNPCStayingHomeless = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (hasPants)
            {
                Texture2D pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants1").Value;

                switch (pantsStyle)
                {
                    case 0:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants1").Value;
                        break;
                    }
                    case 1:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants2").Value;
                        break;
                    }
                    case 2:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants3").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(pantsTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasShirt)
            {
                Texture2D shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt1").Value;

                switch (shirtStyle)
                {
                    case 0:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt1").Value;
                        break;
                    }
                    case 1:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt2").Value;
                        break;
                    }
                    case 2:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt3").Value;
                        break;
                    }
                    case 3:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt4").Value;
                        break;
                    }
                    case 4:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt5").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(shirtTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (shirtLogo)
            {
                Texture2D shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo1").Value;

                switch (shirtLogoStyle)
                {
                    case 0:
                    {
                        shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo1").Value;
                        break;
                    }
                    case 1:
                    {
                        shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo2").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(shirtLogoTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasSunGlasses)
            {
                Texture2D sunglassTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/SunGlasses").Value;

                Main.EntitySpriteDraw(sunglassTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;

            //walking  animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 9)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //jumping/falling frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0 || NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 2 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue" + dialogueStyle.ToString());
		}

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SkeletonBouncer>()))
            {
                player.GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SkeletonBouncer>()))
            {
                Main.player[projectile.owner].GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
            }
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                //select the dialogue this npc should use
                dialogueStyle = Main.rand.Next(1, 30);

                //select a random name for the skeleton when it spawns
                string[] names = { "Boney", "Tony", "Jeff", "McRib", "Clemmence", "Hans Flabberghast", "Carlcium", "Ribert", "Nigel",
                "Morton", "Jeremy", "Gustavo", "Notorious B.O.N.E", "Patrice", "Patrique", "Wanda", "Señor Hernandez", "John Jr",
                "Fortunado", "Quandale", "Normal Skeleton", "Bongo", "Ongo", "Pete Griffith", "Tario", "Clavicle" };
                NPC.GivenName = Main.rand.Next(names);

                //choose pants
                if (Main.rand.NextBool(3))
                {
                    hasPants = true;
                    pantsStyle = Main.rand.Next(0, 3);
                }

                //choose shirt
                if (Main.rand.NextBool(3))
                {
                    hasShirt = true;
                    shirtStyle = Main.rand.Next(0, 5);

                    if (Main.rand.NextBool())
                    {
                        shirtLogo = true;
                        shirtLogoStyle = Main.rand.Next(0, 2);
                    }
                }

                //choose if it will have sunglasses
                if (Main.rand.NextBool(10))
                {
                    hasSunGlasses = true;
                }
            }

            if (Main.rand.NextBool(1500))
            {
                switch (Main.rand.Next(4))
                {
                    case 0:
                    {
                        EmoteBubble.NewBubble(EmoteID.PartyBalloons, new WorldUIAnchor(NPC), 200);
                        break;
                    }
                    case 1:
                    {
                        EmoteBubble.NewBubble(EmoteID.PartyCake, new WorldUIAnchor(NPC), 200);
                        break;
                    }
                    case 2:
                    {
                        EmoteBubble.NewBubble(EmoteID.PartyHats, new WorldUIAnchor(NPC), 200);
                        break;
                    }
                    case 3:
                    {
                        EmoteBubble.NewBubble(EmoteID.PartyPresent, new WorldUIAnchor(NPC), 200);
                        break;
                    }
                }
            }

            if (!Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<Biomes.RaveyardBiome>()))
            {
                NPC.alpha += 5;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
            }
        }
    }

    public class PartySkeleton2 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton3 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton3"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton4 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton4"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton5 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton5"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton6 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton6"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton7 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton7"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }

    public class PartySkeleton8 : PartySkeleton1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton8"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }
}