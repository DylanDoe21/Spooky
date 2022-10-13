using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Spooky.Core
{
    public class CustomTreeUtil
    {
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static void Set(ModTile tile, int minPick, int dustType, SoundStyle soundType, Color mapColor, int drop, string mapName = "")
        {
            tile.MinPick = minPick;
            tile.DustType = dustType;
            tile.HitSound = soundType;
            tile.ItemDrop = drop;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void SetProperties(ModTile tile, bool solid, bool mergeDirt, bool lighted, bool blockLight)
        {
            Main.tileMergeDirt[tile.Type] = mergeDirt;
            Main.tileSolid[tile.Type] = solid;
            Main.tileLighted[tile.Type] = lighted;
            Main.tileBlockLight[tile.Type] = blockLight;
        }

        public static void SetAll(ModTile tile, int minPick, int dust, SoundStyle sound, Color mapColour, int drop = 0, 
        string mapName = "", bool solid = true, bool mergeDirt = true, bool lighted = true, bool blockLight = true)
        {
            Set(tile, minPick, dust, sound, mapColour, drop, mapName);
            SetProperties(tile, solid, mergeDirt, lighted, blockLight);
        }

        //this is literally just for trees, so edit this offset to edit the trees global offset
        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(-2, -2));
        }

        //this is literally just for trees, so edit this offset to edit the trees global offset
        internal static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(-2, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public static bool SolidTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        }
        
        public static bool SolidTopTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }
        
        public static bool ActiveType(int i, int j, int t) 
        {
            return Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == t;
        }
    }
}