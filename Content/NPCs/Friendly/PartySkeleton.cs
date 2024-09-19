using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using ReLogic.Content;
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

        private static Asset<Texture2D> ShirtTexture;
        private static Asset<Texture2D> ShirtLogoTexture;
        private static Asset<Texture2D> PantsTexture;
        private static Asset<Texture2D> SunglassesTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(shirtStyle);
            writer.Write(shirtLogoStyle);
            writer.Write(pantsStyle);
            writer.Write(dialogueStyle);

            //bools
            writer.Write(hasShirt);
            writer.Write(shirtLogo);
            writer.Write(hasPants);
            writer.Write(hasSunGlasses);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            shirtStyle = reader.ReadInt32();
            shirtLogoStyle = reader.ReadInt32();
            pantsStyle = reader.ReadInt32();
            dialogueStyle = reader.ReadInt32();

            //bools
            hasShirt = reader.ReadBoolean();
            shirtLogo = reader.ReadBoolean();
            hasPants = reader.ReadBoolean();
            hasSunGlasses = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
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
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.RaveyardBiome>().Type };
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
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (hasShirt)
            {
                switch (shirtStyle)
                {
                    case 0:
                    {
                        ShirtTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt1");
                        break;
                    }
                    case 1:
                    {
                        ShirtTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt2");
                        break;
                    }
                    case 2:
                    {
                        ShirtTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt3");
                        break;
                    }
                    case 3:
                    {
                        ShirtTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt4");
                        break;
                    }
                    case 4:
                    {
                        ShirtTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt5");
                        break;
                    }
                }

                Main.EntitySpriteDraw(ShirtTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (shirtLogo)
            {
                switch (shirtLogoStyle)
                {
                    case 0:
                    {
                        ShirtLogoTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo1");
                        break;
                    }
                    case 1:
                    {
                        ShirtLogoTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo2");
                        break;
                    }
                }

                Main.EntitySpriteDraw(ShirtLogoTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasPants)
            {
                switch (pantsStyle)
                {
                    case 0:
                    {
                        PantsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants1");
                        break;
                    }
                    case 1:
                    {
                        PantsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants2");
                        break;
                    }
                    case 2:
                    {
                        PantsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants3");
                        break;
                    }
                }

                Main.EntitySpriteDraw(PantsTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasSunGlasses)
            {
                SunglassesTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/SunGlasses");

                Main.EntitySpriteDraw(SunglassesTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

        public override void FindFrame(int frameHeight)
        {   
            //walking animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
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
            if (NPC.AnyNPCs(ModContent.NPCType<SkeletonBouncer>()) && projectile.type == ProjectileID.RottenEgg)
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
                dialogueStyle = Main.rand.Next(1, 32);

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

                NPC.netUpdate = true;
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

            if (!Flags.RaveyardHappening)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
            });
        }
    }
}