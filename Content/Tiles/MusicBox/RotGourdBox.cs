using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.MusicBox
{
    [LegacyName("PumpkinBossBox")]
    public class RotGourdBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RotGourd"), 
            ModContent.ItemType<RotGourdBox>(), ModContent.TileType<RotGourdBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.hasVanityEffects = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.accessory = true;
            Item.width = 24;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
            Item.createTile = ModContent.TileType<RotGourdBoxTile>();
        }
    }
}