using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class BoneMask : ModItem
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

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Generic;
            Item.width = 34;
            Item.height = 22;
            Item.expert = true;
            Item.accessory = true;
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().BoneMask = true;
            player.GetModPlayer<BoneMaskPlayer>().ForceBoneMask = !hideVisual;
            player.GetModPlayer<BoneMaskPlayer>().HideBoneMask = hideVisual;
        }
    }

    public class BoneMaskPlayer : ModPlayer
    {
        public bool ForceBoneMask;
        public bool HideBoneMask;

        public override void ResetEffects()
        {
            ForceBoneMask = false;
            HideBoneMask = false;
        }

        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<BoneMask>())
                {
                    ForceBoneMask = true;
                    HideBoneMask = false;
                }
            }
        }

        public override void FrameEffects()
        {
            // TODO: Need new hook, FrameEffects doesn't run while paused.
            if (ForceBoneMask && !HideBoneMask)
            {
                var boneMaskHead = ModContent.GetInstance<BoneMask>();
                Player.head = EquipLoader.GetEquipSlot(Mod, boneMaskHead.Name, EquipType.Head);
            }
        }
    }
}