using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReLogic.Content;

namespace Spooky.Content.Backgrounds.Cemetery
{
    public class RaveyardSky : CustomSky
    {
        private struct LightPillar
        {
            public Vector2 Position;

            public float Depth;
        }

        private struct Star
        {
            public Vector2 Position;

            public float Depth;

            public int TextureIndex;

            public float SinOffset;

            public float AlphaFrequency;

            public float AlphaAmplitude;
        }

        private LightPillar[] _pillars;
        private Star[] _stars;
        private UnifiedRandom _random = new UnifiedRandom();

        private bool skyActive;

        private float opacity;

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.1f;
            }
        }
        public override Color OnTileColor(Color inColor)
        {
            return Main.DiscoColor * 0.5f * opacity;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            float fade = Main.GameUpdateCount % 60 / 60f;
            int index = (int)(Main.GameUpdateCount / 60 % 3);

            int num11 = -1;
            int num10 = 0;
            for (int j = 0; j < _pillars.Length; j++)
            {
                float depth = _pillars[j].Depth;
                if (num11 == -1 && depth < maxDepth)
                {
                    num11 = j;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                num10 = j;
            }
            
            //draw giant beams being the background
            if (num11 != -1)
            {
                Vector2 value4 = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
                Rectangle rectangle2 = new Rectangle(-1000, -1000, 4000, 4000);
                float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
                for (int i = num11; i < num10; i++)
                {
                    Vector2 value3 = new Vector2(1f / _pillars[i].Depth, 0.8f / _pillars[i].Depth);
                    Vector2 vector2 = _pillars[i].Position;
                    vector2 = (vector2 - value4) * value3 + value4 - Main.screenPosition;
                    if (rectangle2.Contains((int)vector2.X, (int)vector2.Y))
                    {
                        float num9 = value3.X * 500f;

                        float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 1.5f;

                        Color[] BeamColors = new Color[]
                        {
                            new Color(148, 80, 0),
                            new Color(80, 0, 148),
                            new Color(18, 148, 0)
                        };

                        Texture2D BeamTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/RaveyardSkyBeam").Value;

                        spriteBatch.Draw(BeamTexture, vector2 + new Vector2(0, 600), null, Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.75f * scale * opacity, 
                        MathF.Sin(-sin), new Vector2(0, BeamTexture.Height), new Vector2(num9 / 70f, num9 / 45f), SpriteEffects.None, 0f);

                        spriteBatch.Draw(BeamTexture, vector2 + new Vector2(0, 600), null, Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.75f * scale * opacity, 
                        MathF.Sin(sin), new Vector2(0, BeamTexture.Height), new Vector2(num9 / 70f, num9 / 45f), SpriteEffects.None, 0f);
                    }
                }
            }

            int num = -1;
            int num2 = 0;
            for (int i = 1; i < _stars.Length; i++)
            {
                float depth = _stars[i].Depth;
                if (num == -1 && depth < maxDepth && depth > minDepth)
                {
                    num = i;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                num2 = i;
            }
            if (num == -1)
            {
                return;
            }

            //draw giant beams in the background
            Vector2 vector3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
            for (int j = num; j < num2; j++)
            {
                Vector2 vector4 = new Vector2(1f / _stars[j].Depth, 1.1f / _stars[j].Depth);
                Vector2 position = (_stars[j].Position - vector3) * vector4 + vector3 - Main.screenPosition;
                if (rectangle.Contains((int)position.X, (int)position.Y))
                {
                    float value = (float)Math.Sin(_stars[j].AlphaFrequency * Main.GlobalTimeWrappedHourly + _stars[j].SinOffset) * _stars[j].AlphaAmplitude + _stars[j].AlphaAmplitude;
                    value = MathHelper.Clamp(value, 0f, 1f);

                    Color[] BeamColors = new Color[]
                    {
                        new Color(18, 148, 0),
                        new Color(148, 80, 0),
                        new Color(80, 0, 148)
                    };

                    Color[] BeamColors2 = new Color[]
                    {
                        new Color(80, 0, 148),
                        new Color(18, 148, 0),
                        new Color(148, 80, 0)
                    };

                    float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 5f;

                    Texture2D BeamTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/RaveyardSkyBeam").Value;
                    Texture2D BeamTexture2 = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/RaveyardSkyBeam2").Value;

                    spriteBatch.Draw(BeamTexture, position + new Vector2(0, 1700), null, 
                    j == num + 1 ? Color.Lerp(BeamColors2[index], BeamColors2[(index + 1) % 3], fade) * 0.3f * opacity : Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.3f * opacity,
                    j == num + 1 ? MathF.Sin(sin) : MathF.Sin(-sin), new Vector2(BeamTexture.Width / 2, BeamTexture.Height), (vector4.X * 0.5f + 0.5f) * value, SpriteEffects.None, 0f);

                    spriteBatch.Draw(BeamTexture2, position + new Vector2(0, 1700), null, 
                    j == num + 1 ? Color.Lerp(BeamColors2[index], BeamColors2[(index + 1) % 3], fade) * 0.3f * opacity : Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.3f * opacity,
                    j == num + 1 ? MathF.Sin(sin) : MathF.Sin(-sin), new Vector2(BeamTexture.Width / 2, 0), (vector4.X * 0.5f + 0.5f) * value, SpriteEffects.None, 0f);
                }
            }

            //draw the sky itself
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Color[] SkyColors = new Color[]
                {
                    new Color(35, 42, 217),
                    new Color(96, 47, 135),
                    new Color(158, 0, 145),
                    new Color(186, 0, 85),
                    new Color(158, 0, 145),
                    new Color(96, 47, 135)
                };

                Texture2D SkyTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/RaveyardSky").Value;

                spriteBatch.Draw(SkyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 2000) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight),
                Color.Lerp(SkyColors[index], SkyColors[(index + 1) % 3], fade) * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * opacity));
                Vector2 value = new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
                Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.3f + 0.7f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            opacity = 0.002f;
            skyActive = true;
            _pillars = new LightPillar[40];
            for (int i = 0; i < _pillars.Length; i++)
            {
                _pillars[i].Position.X = (float)i / _pillars.Length * (Main.maxTilesX * 16f + 20000f) + _random.NextFloat() * 40f - 20f - 20000f;
                _pillars[i].Position.Y = _random.NextFloat() * 200f - 800f;
                _pillars[i].Depth = _random.NextFloat() * 8f + 7f;
            }

            Array.Sort(_pillars, SortPillarMethod);

            int num = 120;
            int num2 = 10;
            _stars = new Star[num * num2];
            int num3 = 0;
            for (int i = 0; i < num; i++)
            {
                float num4 = (float)i / (float)num;
                for (int j = 0; j < num2; j++)
                {
                    float num5 = (float)j / (float)num2;
                    _stars[num3].Position.X = num4 * (float)Main.maxTilesX * 16f;
                    _stars[num3].Position.Y = num5 * ((float)Main.worldSurface * 16f + 3000f) - 1000f;
                    _stars[num3].Depth = _random.NextFloat() * 8f + 2f;
                    _stars[num3].SinOffset = _random.NextFloat() * 6.28f;
                    _stars[num3].AlphaAmplitude = _random.NextFloat() * 5f;
                    _stars[num3].AlphaFrequency = _random.NextFloat() + 1f;
                    num3++;
                }
            }

            Array.Sort(_stars, SortStarMethod);
        }

        private int SortStarMethod(Star meteor1, Star meteor2)
        {
            return meteor2.Depth.CompareTo(meteor1.Depth);
        }

        private int SortPillarMethod(LightPillar pillar1, LightPillar pillar2)
        {
            return pillar2.Depth.CompareTo(pillar1.Depth);
        }

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return (skyActive || opacity > 0.001f) && !Main.gameMenu;
        }
    }
}