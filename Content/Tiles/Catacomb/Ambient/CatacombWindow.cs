/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.DataStructures;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
    //small window
    class CatacombWindowSmall : DummyTile
    {
        public override int DummyType => ProjectileType<CatacombWindowSmallDummy>();

        public override void SetStaticDefaults() => Quick.QuickFurniture(this, 4, 6, DustID.Stone, 500000, SoundID.Tink, false, new Color(255, 220, 0));

        public override bool SpawnConditions(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }
    }

    class CatacombWindowSmallItem : QuickTileItem
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickMossItem";

        public CatacombWindowSmallItem() : base("Catacomb Window Small", "Cheat Item", "CatacombWindowSmall", 1) { }
    }

    class CatacombWindowSmallDummy : Dummy
    {
        public CatacombWindowSmallDummy() : base(TileType<CatacombWindowSmall>(), 4 * 16, 6 * 16) { }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Spooky/Content/Backgrounds/CatacombWindowBG").Value;
            Texture2D tex2 = Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Ambient/CatacombWindowSmall").Value;

            Rectangle target = new Rectangle((int)(Projectile.position.X - Main.screenPosition.X), (int)(Projectile.position.Y - Main.screenPosition.Y), 4 * 16, 6 * 16);

            float offX = (Main.screenPosition.X + Main.screenWidth / 2 - Projectile.Center.X) * -0.14f;
            float offY = (Main.screenPosition.Y + Main.screenHeight / 2 - Projectile.Center.Y) * -0.14f;
            Rectangle source = new Rectangle((int)(Projectile.position.X % tex.Width) + (int)offX, (int)(Projectile.position.Y % tex.Height) + (int)offY, 4 * 16, 6 * 16);

            Main.spriteBatch.Draw(tex, target, source, lightColor);
            Main.spriteBatch.Draw(tex2, target, tex2.Frame(), lightColor);

            return true;
        }
    }

    //long window
    class CatacombWindowLong : DummyTile
    {
        public override int DummyType => ProjectileType<CatacombWindowLongDummy>();

        public override void SetStaticDefaults() => Quick.QuickFurniture(this, 20, 7, DustID.Stone, 500000, SoundID.Tink, false, new Color(255, 220, 0));

        public override bool SpawnConditions(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }
    }

    class CatacombWindowLongItem : QuickTileItem
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickMossItem";

        public CatacombWindowLongItem() : base("Catacomb Window Long", "Cheat Item", "CatacombWindowLong", 1) { }
    }

    class CatacombWindowLongDummy : Dummy
    {
        public CatacombWindowLongDummy() : base(TileType<CatacombWindowLong>(), 20 * 16, 7 * 16) { }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Spooky/Content/Backgrounds/CatacombWindowBG").Value;
            Texture2D tex2 = Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Ambient/CatacombWindowLong").Value;

            Rectangle target = new Rectangle((int)(Projectile.position.X - Main.screenPosition.X), (int)(Projectile.position.Y - Main.screenPosition.Y), 20 * 16, 7 * 16);
            
            float offX = (Main.screenPosition.X + Main.screenWidth / 2 - Projectile.Center.X) * -0.14f;
            float offY = (Main.screenPosition.Y + Main.screenHeight / 2 - Projectile.Center.Y) * -0.14f;
            Rectangle source = new Rectangle((int)(Projectile.position.X % tex.Width) + (int)offX - 600, (int)(Projectile.position.Y % tex.Height) + (int)offY, 20 * 16, 7 * 16);

            Main.spriteBatch.Draw(tex, target, source, lightColor);
            Main.spriteBatch.Draw(tex2, target, tex2.Frame(), lightColor);

            return true;
        }
    }

    public abstract class Dummy : ModProjectile
    {
        protected int ValidType;
        private int Width;
        private int Height;

        public Tile Parent => Main.tile[ParentX, ParentY];

        public virtual int ParentX => (int)Projectile.Center.X / 16;
        public virtual int ParentY => (int)Projectile.Center.Y / 16;

        public Dummy(int validType, int width, int height)
        {
            ValidType = validType;
            Width = width;
            Height = height;
        }

        public virtual bool ValidTile(Tile tile) => (tile.TileType == ValidType && tile.HasTile); //the tile is null only where tiles are unloaded in multiPlayer. We don't want to kill off dummies on unloaded tiles until tile is known because Projectile is recieved MUCH farther than the tiles.

        public override bool PreDraw(ref Color lightColor) => false;

        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public virtual void Update() { }

        public virtual void Collision(Player Player) { }

        public virtual void SafeSetDefaults() { }


        public sealed override void SetDefaults()
        {
            SafeSetDefaults();

            Projectile.width = Width;
            Projectile.height = Height;
            Projectile.hostile = true;
            Projectile.damage = 1;
            Projectile.timeLeft = 2;
            Projectile.netImportant = true;
        }

        public sealed override void AI()
        {
            if (ValidTile(Parent))
                Projectile.timeLeft = 2;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //in single Player we can use the CanHitPlayer, but in MP that is only run by the server so we need to check Players manually for clients
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player Player = Main.player[i];
                    if (Player.Hitbox.Intersects(Projectile.Hitbox))
                        Collision(Player);
                }
            }

            Update();
        }

        public sealed override bool CanHitPlayer(Player target)
        {
            Collision(target);
            return false;
        }
    }

    public abstract class DummyTile : ModTile
    {
        public static Dictionary<Point16, Projectile> dummies = new Dictionary<Point16, Projectile>();

        public virtual int DummyType { get; }

        public Projectile Dummy(int i, int j) => GetDummy(i, j, DummyType);

        public static Projectile GetDummy(int i, int j, int type)
        {
            Point16 key = new Point16(i, j);

            if (dummies.TryGetValue(key, out Projectile dummy))
            {
                if (dummy.type == type && dummy.active)
                    return dummy;
            }

            return null;
        }

        public static Projectile GetDummy<T>(int i, int j) where T : Dummy
        {
            Point16 key = new Point16(i, j);

            if (dummies.TryGetValue(key, out Projectile dummy))
            {
                if (dummy.ModProjectile is T && dummy.active)
                    return dummy;
            }

            return null;
        }

        public static bool DummyExists(int i, int j, int type)
        {
            if (GetDummy(i, j, type) != null)
                return true;

            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                var proj = Main.projectile[k];
                if (proj.active && proj.type == type && (proj.position / 16).ToPoint16() == new Point16(i, j))
                    return true;
            }

            return false;
        }

        public static bool DummyExists<T>(int i, int j) where T : Dummy
        {
            if (GetDummy<T>(i, j) != null)
                return true;

            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                var proj = Main.projectile[k];
                if (proj.active && proj.ModProjectile is T && (proj.position / 16).ToPoint16() == new Point16(i, j))
                    return true;
            }

            return false;
        }

        public virtual void PostSpawnDummy(Projectile dummy) { }

        public virtual void SafeNearbyEffects(int i, int j, bool closer) { }

        public virtual bool SpawnConditions(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public sealed override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.tileFrameImportant[Type] || SpawnConditions(i, j))
            {
                int type = DummyType;//cache type here so you dont grab the it from a dict every single iteration
                var dummy = Dummy(i, j);

                if (dummy is null || !dummy.active)
                {

                    Projectile p = new Projectile();
                    p.SetDefaults(type);

                    var spawnPos = new Vector2(i, j) * 16 + p.Size / 2;
                    int n = Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPos, Vector2.Zero, type, 1, 0);

                    Point16 key = new Point16(i, j);
                    dummies[key] = Main.projectile[n];

                    PostSpawnDummy(Main.projectile[n]);
                }
            }

            SafeNearbyEffects(i, j, closer);
        }
    }

    public abstract class QuickTileItem : ModItem
    {
        public string InternalName = "";
        public string Itemname;
        public string Itemtooltip;
        //private readonly int Tiletype;
        private readonly string Tilename;
        private readonly int Rare;
        private readonly string TexturePath;
        private readonly bool PathHasName;
        private readonly int ItemValue;

        public QuickTileItem() { }

        public QuickTileItem(string name, string tooltip, string placetype, int rare = ItemRarityID.White, string texturePath = null, bool pathHasName = false, int ItemValue = 0)
        {
            Itemname = name;
            Itemtooltip = tooltip;
            Tilename = placetype;
            Rare = rare;
            TexturePath = texturePath;
            PathHasName = pathHasName;
            this.ItemValue = ItemValue;
        }

        public QuickTileItem(string internalName, string name, string tooltip, string placetype, int rare = ItemRarityID.White, string texturePath = null, bool pathHasName = false, int ItemValue = 0)
        {
            InternalName = internalName;
            Itemname = name;
            Itemtooltip = tooltip;
            Tilename = placetype;
            Rare = rare;
            TexturePath = texturePath;
            PathHasName = pathHasName;
            this.ItemValue = ItemValue;
        }

        public override string Name => InternalName != "" ? InternalName : base.Name;


        public virtual void SafeSetDefaults() { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(Itemname ?? "ERROR");
            Tooltip.SetDefault(Itemtooltip ?? "Report me please!");
        }

        public override void SetDefaults()
        {
            if (Tilename is null)
                return;

            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = Mod.Find<ModTile>(Tilename).Type;
            Item.rare = Rare;
            Item.value = ItemValue;
            SafeSetDefaults();
        }
    }

    internal static class Quick
    {
        public static void QuickFurniture(this ModTile tile, int width, int height, int dustType, int minPick, SoundStyle? hitSound, bool tallBottom, Color mapColor, bool solidTop = false, bool solid = false, string mapName = "", AnchorData bottomAnchor = default, AnchorData topAnchor = default, int[] anchorTiles = null, bool faceDirection = false, bool wallAnchor = false, Point16 Origin = default)
        {
            Main.tileLavaDeath[tile.Type] = false;
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileSolidTop[tile.Type] = solidTop;
            Main.tileSolid[tile.Type] = solid;

            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;


            TileObjectData.newTile.CoordinateHeights = new int[height];

            for (int k = 0; k < height; k++)
                TileObjectData.newTile.CoordinateHeights[k] = 16;

            if (tallBottom) //this breaks for some tiles: the two leads are multitiles and tiles with random styles
                TileObjectData.newTile.CoordinateHeights[height - 1] = 18;

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = Origin == default(Point16) ? new Point16(width / 2, height - 1) : Origin;

            if (bottomAnchor != default)
                TileObjectData.newTile.AnchorBottom = bottomAnchor;

            if (topAnchor != default)
                TileObjectData.newTile.AnchorTop = topAnchor;

            if (anchorTiles != null)
                TileObjectData.newTile.AnchorAlternateTiles = anchorTiles;

            if (wallAnchor)
                TileObjectData.newTile.AnchorWall = true;

            if (faceDirection)
            {
                TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
                TileObjectData.newTile.StyleHorizontal = true;
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
                TileObjectData.addAlternate(1);
            }


            TileObjectData.addTile(tile.Type);
            tile.MinPick = minPick;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
            tile.DustType = dustType;
            tile.HitSound = hitSound;
        }
        public static void QuickSetFurniture(this ModTile tile, int width, int height, int dustType, SoundStyle? hitSound, bool tallBottom, Color mapColor, bool solidTop = false, bool solid = false, string mapName = "", AnchorData bottomAnchor = default, AnchorData topAnchor = default, int[] anchorTiles = null, bool faceDirection = false, bool wallAnchor = false, Point16 Origin = default)
        {
            Main.tileLavaDeath[tile.Type] = false;
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileSolidTop[tile.Type] = solidTop;
            Main.tileSolid[tile.Type] = solid;

            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;


            TileObjectData.newTile.CoordinateHeights = new int[height];

            for (int k = 0; k < height; k++)
                TileObjectData.newTile.CoordinateHeights[k] = 16;

            if (tallBottom) //this breaks for some tiles: the two leads are multitiles and tiles with random styles
                TileObjectData.newTile.CoordinateHeights[height - 1] = 18;

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = Origin == default(Point16) ? new Point16(width / 2, height - 1) : Origin;

            if (bottomAnchor != default)
                TileObjectData.newTile.AnchorBottom = bottomAnchor;

            if (topAnchor != default)
                TileObjectData.newTile.AnchorTop = topAnchor;

            if (anchorTiles != null)
                TileObjectData.newTile.AnchorAlternateTiles = anchorTiles;

            if (wallAnchor)
                TileObjectData.newTile.AnchorWall = true;

            if (faceDirection)
            {
                TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
                TileObjectData.newTile.StyleHorizontal = true;
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
                TileObjectData.addAlternate(1);
            }


            TileObjectData.addTile(tile.Type);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
            tile.DustType = dustType;
            tile.HitSound = hitSound;
        }

    }
}
*/