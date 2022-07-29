using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Creepypasta
{
    public class SirenHead : ModItem
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
        }

        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Siren Head");
            Tooltip.SetDefault("When using items, you have a small chance to shoot out gore shards"
            + "\nYou also have a rare chance to fire out a high damaging sound wave that bounces off tiles"
            + "\nThe damage of the gore shards and sound wave scale based on your weapons damage"
            + "\nAlso turns your head into siren head");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 38;
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().SirenHead = true;
            player.GetModPlayer<SirenHeadPlayer>().ForceSirenHead = !hideVisual;
            player.GetModPlayer<SirenHeadPlayer>().HideSirenHead = hideVisual;
        }

        public override bool IsVanitySet(int head, int body, int legs) => true;
    }

    public class SirenHeadPlayer : ModPlayer
    {
        // These 5 relate to ExampleCostume.
        public bool HideSirenHead;
        public bool ForceSirenHead;

        public override void ResetEffects()
        {
            HideSirenHead = false;
            ForceSirenHead = false;
        }

        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<SirenHead>())
                {
                    HideSirenHead = false;
                    ForceSirenHead = true;
                }
            }
        }

        public override void FrameEffects()
        {
            // TODO: Need new hook, FrameEffects doesn't run while paused.
            if (ForceSirenHead && !HideSirenHead)
            {
                var exampleCostume = ModContent.GetInstance<SirenHead>();
                Player.head = EquipLoader.GetEquipSlot(Mod, exampleCostume.Name, EquipType.Head);
            }
        }
    }
}