using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky
{
    internal class SpookyMenu : ModMenu
    {
        private bool HasClicked;

        private float LogoSquishIntensity;

        private Vector2 logoCenter = Vector2.Zero;

        public override string DisplayName => "Spooky Mod";

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyMenu");

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Spooky/MenuAssets/SpookyMenuLogo");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");
        
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => null;

        private static Asset<Texture2D> MenuTexture;
        private static Asset<Texture2D> RotGourdTexture;
        private static Asset<Texture2D> SpookySpiritTexture;
        private static Asset<Texture2D> MocoTexture;
        private static Asset<Texture2D> DaffodilTexture;
        private static Asset<Texture2D> OldHunterTexture;
        private static Asset<Texture2D> OrroboroTexture;
        private static Asset<Texture2D> BigBoneTexture;
        private static Asset<Texture2D> OutlineTexture;
        private static Asset<Texture2D> SpookFishronTexture;
        private static Asset<Texture2D> SpookFishronOutlineTexture;

        public static readonly SoundStyle LogoClickSound1 = new("Spooky/Content/Sounds/MenuLogoClick1", SoundType.Sound);
        public static readonly SoundStyle LogoClickSound2 = new("Spooky/Content/Sounds/MenuLogoClick2", SoundType.Sound);

        public override void OnDeselected()
        {
            //un-hide the sun when this menu is switched
            Main.sunModY = 0;
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Color.White;

            //hides the sun offscreen so you cant click it
            Main.sunModY = -300;

            //set daytime to true 
            Main.time = 27000;
            Main.dayTime = true;

            //draw the menu background
            MenuTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/SpookyMenu");

            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / MenuTexture.Width();
            float yScale = (float)Main.screenHeight / MenuTexture.Height();
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (MenuTexture.Width() * scale - Main.screenWidth - 10) * 0.5f;
                }
                else
                {
                    drawOffset.Y -= (MenuTexture.Height() * scale - Main.screenHeight) * 0.5f;
                }
            }

            spriteBatch.Draw(MenuTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //draw the defeated bosses
            if (MenuSaveSystem.hasDefeatedRotGourd)
            {
                RotGourdTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockRotGourd");
                spriteBatch.Draw(RotGourdTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedSpookySpirit)
            {
                SpookySpiritTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockSpookySpirit");
                spriteBatch.Draw(SpookySpiritTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedMoco)
            {
                MocoTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockMoco");
                spriteBatch.Draw(MocoTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedDaffodil)
            {
                DaffodilTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockDaffodil");
                spriteBatch.Draw(DaffodilTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedOldHunter)
            {
                OldHunterTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockOldHunter");
                spriteBatch.Draw(OldHunterTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedOrroboro)
            {
                OrroboroTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockOrroboro");
                spriteBatch.Draw(OrroboroTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedBigBone)
            {
                BigBoneTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockBigBone");
                spriteBatch.Draw(BigBoneTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (MenuSaveSystem.hasDefeatedSpookFishron)
            {
                SpookFishronTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockSpookFishron");
                spriteBatch.Draw(SpookFishronTexture.Value, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            //outlines for each boss
            OutlineTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/SpookyMenuOutlines");
            spriteBatch.Draw(OutlineTexture.Value, drawOffset, null, Color.White * LogoSquishIntensity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spook fishron
            SpookFishronOutlineTexture ??= ModContent.Request<Texture2D>("Spooky/MenuAssets/UnlockSpookFishronOutline");
            spriteBatch.Draw(SpookFishronOutlineTexture.Value, drawOffset, null, Color.White * LogoSquishIntensity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //draw the actual menu logo
            logoCenter = logoDrawCenter;
            Main.EntitySpriteDraw(Logo.Value, logoDrawCenter, new Rectangle(0, 0, Utils.Width(Logo), Utils.Height(Logo)), Color.White, 
            logoRotation, Utils.Size(Logo) / 2f, new Vector2(0.85f + LogoSquishIntensity, 0.85f - LogoSquishIntensity), SpriteEffects.None, 0);

            return false;
        }

        public override void Update(bool isOnTitleScreen)
        {
            LogoSquishIntensity *= 0.9f;
            if (Main.mouseLeft && !HasClicked && Math.Abs(Main.MouseScreen.X - logoCenter.X) < 300f && Math.Abs(Main.MouseScreen.Y - logoCenter.Y) < 70f)
            {
                LogoSquishIntensity = 1f;

                if (Math.Abs(LogoSquishIntensity) < 0.1f)
                {
                    LogoSquishIntensity = Math.Sign(LogoSquishIntensity) * 0.1f;
                }

                SoundEngine.PlaySound(LogoClickSound1, null);
                SoundEngine.PlaySound(LogoClickSound2, null);
            }

            HasClicked = Main.mouseLeft;
        }
    }
}
